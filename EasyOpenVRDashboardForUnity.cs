/*
 * EasyOpenVRDashboardForUnity by gpsnmeajp v0.1
 * 2018/09/02
 * 
 * v0.1 公開
 * These codes are licensed under CC0.
 * http://creativecommons.org/publicdomain/zero/1.0/deed.ja
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR; //Steam VR

public class EasyOpenVRDashboardForUnity : MonoBehaviour
{
    //エラーメッセージの名前
    const string Tag = "[EasyOpenVRDashboardForUnity]";

    //--------------------------------------------------------------------------
    [Header("thumbnail (Please check [Advanced] -> [Read/Write Enabled])")]
    //サムネイルテクスチャ
    public Texture2D thumbnailTextureInput;

    [Header("RenderTexture")]
    //取得元のRenderTexture
    public RenderTexture renderTexture;

    [Header("Setting")]
    //オーバーレイの大きさ設定(幅のみ。高さはテクスチャの比から自動計算される)
    [Range(0, 100)]
    public float width = 1.5f;

    //オーバーレイの透明度を設定
    [Range(0, 1)]
    public float alpha = 0.2f;

    //表示するか否か
    public bool show = true;

    [Header("Name")]
    //ユーザーが確認するためのオーバーレイの名前
    public string OverlayFriendlyName = "SampleOverlay";

    //グローバルキー(システムのオーバーレイ同士の識別名)。
    //ユニークでなければならない。乱数やUUIDなどを勧める
    public string OverlayKeyName = "SampleOverlay";

    [Header("Error")]
    public bool error = false;
    public bool eventlog = true;

    [Header("Mouse (Read only)")]
    public float MouseX = 0.0f;
    public float MouseY = 0.0f;
    public bool MouseClick=false;


    //--------------------------------------------------------------------------

    //オーバーレイのハンドル(整数)
    ulong overlayHandle = 0;
    ulong thumbnailHandle = 0;

    //OpenVRシステムインスタンス
#pragma warning disable 0414
    CVRSystem openvr;
#pragma warning restore 0414

    //Overlayインスタンス
    CVROverlay overlay;

    //オーバーレイに渡すネイティブテクスチャ
    Texture_t overlayTexture;
    Texture_t thumbnailTexture;

    //サムネイルの加工用テクスチャ領域
    Texture2D thumbnailTexture2D;

    //サムネイルが利用可能か
    bool thumbnailAvailable = false;

    //入力イベント取得用イベント
    VREvent_t Event;

    //テスクチャ領域設定
    VRTextureBounds_t OverlayTextureBounds;

    HmdVector2_t vecMouseScale;

    void Start()
    {
        var openVRError = EVRInitError.None;
        var overlayError = EVROverlayError.None;

        //OpenVRの初期化
        openvr = OpenVR.Init(ref openVRError, EVRApplicationType.VRApplication_Overlay);
        if (openVRError != EVRInitError.None)
        {
            Debug.LogError(Tag + "OpenVRの初期化に失敗." + openVRError.ToString());
            error = true;
            return;
        }

        //オーバーレイ機能の初期化
        overlay = OpenVR.Overlay;
        overlayError = overlay.CreateDashboardOverlay(OverlayKeyName, OverlayFriendlyName, ref overlayHandle, ref thumbnailHandle);
        if (overlayError != EVROverlayError.None)
        {
            Debug.LogError(Tag + "Overlayの初期化に失敗. " + overlayError.ToString());
            error = true;
            return;
        }

        //マウス(Dashboardのときのみ有効)
        overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);

        //オーバーレイに渡すテクスチャ種類の設定
        OverlayTextureBounds = new VRTextureBounds_t();
        var isOpenGL = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
        if (isOpenGL)
        {
            //pGLuintTexture
            overlayTexture.eType = ETextureType.OpenGL;
            thumbnailTexture.eType = ETextureType.OpenGL;
            //上下反転しない
            OverlayTextureBounds.uMin = 1;
            OverlayTextureBounds.vMin = 0;
            OverlayTextureBounds.uMax = 1;
            OverlayTextureBounds.vMax = 0;
            overlay.SetOverlayTextureBounds(thumbnailHandle, ref OverlayTextureBounds);
        }
        else
        {
            //pTexture
            overlayTexture.eType = ETextureType.DirectX;
            thumbnailTexture.eType = ETextureType.DirectX;
            //上下反転する
            OverlayTextureBounds.uMin = 0;
            OverlayTextureBounds.vMin = 1;
            OverlayTextureBounds.uMax = 1;
            OverlayTextureBounds.vMax = 0;
            overlay.SetOverlayTextureBounds(overlayHandle, ref OverlayTextureBounds);
        }

        //サムネイルテクスチャが存在するなら
        if (thumbnailTextureInput != null)
        {
            //サムネイルテクスチャの情報を取得
            var thumWidth = thumbnailTextureInput.width;
            var thumHeight = thumbnailTextureInput.height;
            //作業用テクスチャの領域を確保
            thumbnailTexture2D = new Texture2D(thumWidth, thumHeight, TextureFormat.RGBA32, false);

            //力技でコピーして圧縮を解除
            if (isOpenGL)
            {
                for (int y = 0; y < thumHeight; y++)
                {
                    for (int x = 0; x < thumWidth; x++)
                    {
                        thumbnailTexture2D.SetPixel(x, y, thumbnailTextureInput.GetPixel(x, y));
                    }
                }
            }
            else {
                //DirectXは上下反転
                for (int y = 0; y < thumHeight; y++)
                {
                    for (int x = 0; x < thumWidth; x++)
                    {
                        thumbnailTexture2D.SetPixel(x, thumHeight-y-1, thumbnailTextureInput.GetPixel(x, y));
                    }
                }
            }

            //操作を反映
            thumbnailTexture2D.Apply();
            //ネイティブテクスチャを取得して設定
            thumbnailTexture.handle = thumbnailTexture2D.GetNativeTexturePtr();
            //サムネイル利用可能フラグを立てる
            thumbnailAvailable = true;
        }

        Debug.Log(Tag + "初期化完了しました");
    }

    void Update()
    {
        //初期化失敗するなどoverlayが無効な場合は実行しない
        if (overlay == null || error)
        {
            return;
        }

        if (show)
        {
            //オーバーレイを表示する
            overlay.ShowOverlay(overlayHandle);
        }
        else
        {
            //オーバーレイを非表示にする
            overlay.HideOverlay(overlayHandle);
        }
        
        //イベントを処理する
        uint uncbVREvent = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));
        while (overlay.PollNextOverlayEvent(overlayHandle, ref Event, uncbVREvent))
        {
            if (eventlog)
            {
                Debug.Log(Tag + "Event:" + ((EVREventType)Event.eventType).ToString());
            }
            //Debug.Log(Tag +"Event:"+ (Event.eventType).ToString());
            switch ((EVREventType)Event.eventType)
            {
                case EVREventType.VREvent_MouseMove:
                    MouseX = Event.data.mouse.x;
                    MouseY = Event.data.mouse.y;
                    break;
                case EVREventType.VREvent_MouseButtonDown:
                    MouseClick = true;
                    break;
                case EVREventType.VREvent_MouseButtonUp:
                    MouseClick = false;
                    break;
                case EVREventType.VREvent_DashboardActivated:
                    break;
                case EVREventType.VREvent_DashboardDeactivated:
                    break;
                case EVREventType.VREvent_DashboardRequested:
                    break;
                case EVREventType.VREvent_DashboardThumbSelected:
                    break;
                case EVREventType.VREvent_EnterStandbyMode:
                    break;
                case EVREventType.VREvent_LeaveStandbyMode:
                    break;
                case EVREventType.VREvent_KeyboardCharInput:
                    break;
                case EVREventType.VREvent_KeyboardClosed:
                    break;
                case EVREventType.VREvent_KeyboardDone:
                    break;
                case EVREventType.VREvent_ResetDashboard:
                    break;
                case EVREventType.VREvent_ScreenshotTriggered:
                    break;
                case EVREventType.VREvent_WirelessDisconnect:
                    break;
                case EVREventType.VREvent_WirelessReconnect:
                    break;
                case EVREventType.VREvent_Quit:
                    Debug.Log(Tag + "Quit");
                    ApplicationQuit();
                    break;
                default:
                    break;
            }
        }

        if (overlay.IsDashboardVisible())
        {
            //サムネイルにテクスチャを設定
            if (thumbnailAvailable)
            {
                var overlayError = EVROverlayError.None;
                overlayError = overlay.SetOverlayTexture(thumbnailHandle, ref thumbnailTexture);
                if (overlayError != EVROverlayError.None)
                {
                    Debug.LogError(Tag + "Overlayにサムネイルをセットできませんでした. " + overlayError.ToString());
                    error = true;
                    return;
                }
            }
        }

        //オーバーレイが表示されている時
        if (overlay.IsOverlayVisible(overlayHandle) && overlay.IsActiveDashboardOverlay(overlayHandle))
        {
            //オーバーレイの大きさ設定(幅のみ。高さはSetOverlayMouseScaleの比から自動計算される)
            overlay.SetOverlayWidthInMeters(overlayHandle, width);
            //オーバーレイの透明度を設定
            overlay.SetOverlayAlpha(overlayHandle, alpha);

            //RenderTextureが生成されているかチェック
            if (!renderTexture.IsCreated())
            {
                Debug.Log(Tag + "RenderTextureがまだ生成されていない");
                return;
            }

            try
            {
                //マウスカーソルスケールを設定する(これにより表示領域のサイズも決定される)
                vecMouseScale.v0 = renderTexture.width;
                vecMouseScale.v1 = renderTexture.height;
                overlay.SetOverlayMouseScale(overlayHandle, ref vecMouseScale);
                //RenderTextureからネイティブテクスチャのハンドルを取得
                overlayTexture.handle = renderTexture.GetNativeTexturePtr();
            }
            catch (UnassignedReferenceException e)
            {
                Debug.LogError(Tag + "RenderTextureがセットされていません "+e.ToString());
                error = true;
                return;
            }

            //オーバーレイにテクスチャを設定
            var overlayError = EVROverlayError.None;
            overlayError = overlay.SetOverlayTexture(overlayHandle, ref overlayTexture);
            if (overlayError != EVROverlayError.None)
            {
                Debug.LogError(Tag + "Overlayにテクスチャをセットできませんでした. " + overlayError.ToString());
                error = true;
                return;
            }
        }       
    }

    private void OnDestroy()
    {
        //アプリケーション終了時にOverlayハンドルを破棄する
        if (overlay != null)
        {
            overlay.DestroyOverlay(overlayHandle);
        }
    }

    //アプリケーションを終了させる
    void ApplicationQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}