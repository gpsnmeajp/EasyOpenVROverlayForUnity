# EasyOpenVROverlayForUnity
OepnVRを用いたオーバーレイ表示の支援スクリプトです。

CC0ライセンスです。  
バグ報告などは、メール、もしくはDiscordまで  


# EasyOpenVROverlayForUnity.cs
![](https://sabowl.sakura.ne.jp/gpsnmeajp/unity/EasyOpenVROverlayForUnity/shot1.png)  
  
OepnVRを用いたオーバーレイ表示の支援スクリプトです。  
Unity2018.2.5f1 Personalで動作確認済です。  
  
任意のSteamVRゲームのVR空間内に割り込んで自分のアプリケーションのRenderTextureを出せます。  
機能の仕組み上、表示は2Dです。  
  
1スクリプトというシンプルな構成で、なるべく簡単に使えることを目指しました。  
OVRDropのような入力機能は、うまく動作しないので省いています。  
  
注意: 実行ファイル生成時のopenvr_api.dllの同梱し忘れにはご注意ください  
  
v0.24 OpenGL環境で正常に動作しない問題修正  
　showDevicesでエラーが発生することがあるためコメントアウト  
v0.23 Side-By-Side 3D対応  
v0.22 デバイスアップデート  
・デバイス詳細情報をログやInspectorに表示するように(シリアル番号など)  
・デバイスを8まで選択可能に(ベースステーションやトラッカーが使用できるように) v0.21 微修正  
v0.2 大規模更新 2018-09-23  
・デバッグタグの方法を変更  
・uGUIのクリックに対応  
・コントローラーを選択できるように  
・外部からのエラーチェック、表示状態管理関数を追加。  
・各処理を関数化  
・終了時に開放する処理を追加  
・エラー時に開放する処理を追加  
・マウススケールの処理を追加  
・終了イベントのキャッチを追加  
・デモアプリケーションを追加  
v0.1 公開  

## こちらのQiita記事もどうぞ  
[VRゲームにオーバーレイ表示したい人向けサンプル(OpenVR Overlay)](https://qiita.com/gpsnmeajp/items/421e3853465df3b1520b)  
[VaNiiMenuみたいに空間タッチできるオーバーレイアプリケーションの作り方](https://qiita.com/gpsnmeajp/items/3b67223f7f11bb6d93c3)  
  
## 使い方  
## Unity2020.3.3f1の場合
1. [OpenVR SDK](https://github.com/ValveSoftware/openvr)をダウンロード
2. Unityを立ち上げ、3Dプロジェクトを新規作成
3. Pluginsフォルダを作り、そこに「openvr-1.16.8\bin\win64\openvr_api.dll」「openvr-1.16.8\headers\openvr_api.cs」を入れる
4. OpenXRが有効なら切る
5. 
6. EasyOpenVROverlayForUnity.csをインポートします。  
7. Assetsを右クリックし、Create→Render Texture。「New Render Texture」ができる。  
8. HierarchyのMain Cameraの「Target Texture」に、「New Render Texture」をドラッグアンドドロップしてセット  
9. Hierarchyを右クリックしてCreate Empty  
10. 出来上がったGameObjectにEasyOpenVROverlayForUnityをドラッグアンドドロップ  
11. GameObjectをクリック  
12. EasyOpenVROverlayForUnityのRender Textureに「New Render Texture」をドラッグアンドドロップしてセット  


以下古い手順  
  
### Unityプロジェクトの準備(Unity2018.2.5f1 Personal)  
1. Unityを立ち上げ、3Dプロジェクトを新規作成します  
2. Asset StoreからSteam VR Pluginをインポートします  
3. SteamVR_Settingsが立ち上がるので、Accept All  
4. Edit→Project Settings→Playerを開き、Inspectorの下のXR SettingsからVirtual Reality Supportedをオフに  
5. Edit→Preferencesを開き、SteamVRのAutomativally Enable VRをオフに  
6. EasyOpenVROverlayForUnity.csをインポートします。  
7. Assetsを右クリックし、Create→Render Texture。「New Render Texture」ができる。  
8. HierarchyのMain Cameraの「Target Texture」に、「New Render Texture」をドラッグアンドドロップしてセット  
9. Hierarchyを右クリックしてCreate Empty  
10. 出来上がったGameObjectにEasyOpenVROverlayForUnityをドラッグアンドドロップ  
11. GameObjectをクリック  
12. EasyOpenVROverlayForUnityのRender Textureに「New Render Texture」をドラッグアンドドロップしてセット  
  
動作確認は、SteamVRを起動後、Unityの再生ボタンをクリック。  
Consoleに「[EasyOpenVROverlayForUnity]初期化完了しました」と表示され、HMD内に先ほどまでのUnityの画面みたいなのが中央に出ていればOK。  
この場合MainCameraはディスプレイ出力をしなくなるため、「Display 1 No cameras rendering」と出るが問題ない  
  
  
### uGUIのクリックについて  
使い方  
1. LaycastRootObjectには、操作したいCanvas(シーン直下に配置)を設定  
2. Buttonのクリックだけ対応(コントローラーの先端でOverlayを叩くとクリック)  
3. ButtonのRaycast TargetはONに。ButtonのTextにあるRaycast TargetはOFFに  
4. CanvasのRender Modeは"Screen Space - Camera"に。  
5. CanvasのRender Cameraは、RenderTextureを設定したCameraと同じものにすること  
なお、LaycastRootObjectをnull(None)にするとGUI機能は無効化される  

設定  
![](https://sabowl.sakura.ne.jp/gpsnmeajp/unity/EasyOpenVROverlayForUnity/set2.png)  
  
設定項目について解説します。  
🔍がついているものはInspector上での表示あるいはスクリプトからの取得用です。  
操作しても意味がありません  
🔧がついているものはInspector上でのデバッグ操作用です。  
  
🔍Error エラー状態を示します。チェックが入っていると何らかのエラーで動作を停止しています。  
🔧Event Log Open VRの出力する大量のイベント情報をログに出力します。  
  
**Render Texture**  
**Render Texture** VR空間に描画するRenderTextureを指定します。  
  
**Transform**  
**Position** VR空間内での位置を指定します。  
**Rotation** VR空間内での回転を指定します。Quadなので+-90度以上(裏面)は見えなくなります。  
**Scale** VR空間内での拡大率を指定します。(非推奨。大きさ調整はwidthを推奨)  
**Mirror** X X方向の鏡像反転  
**Mirror** Y Y方向の鏡像反転  
  
**Setting**  
**Width** 幅の大きさをm単位で指定します。高さはTextureの比から自動で計算されます。  
**Alpha** 全体の透明度を指定します。0が完全に透明、1が不透明  
**Show** 表示非表示を切り替えます。  
  
**Name**  
**Overlay Friendly Name** ユーザーに表示する用の名前を設定します。  
**Overlay Key Name OpenVR**システムが識別する用の名前を設定します。  
　(この名前が同じオーバーレイは同時に表示できないため、必ず変更してください)  
  
**DeviceTracking**  
**Device Tracking** デバイスからの相対位置か、ルーム内絶対位置にするか設定します。  
**Device Index** デバイス番号を指定します。Left/Right Controllerは変動する値に自動追従します  
  
**Absolute space** (この項目はDevice TrackingがOFFのときのみ有効です。)  
**Seated** ルームスケールか着座スケールかを選択します  
**🔧ResetSeatedCamera** 着座スケールの際、カメラ位置をリセットします  
　(リセット完了後自動でOFFになります)  
  
**Device Info**  
**🔧Put Log Devices Info** 現在接続されているデバイス一覧をログに出力します。  
　(出力完了後自動でOFFになります)  
**🔍Connected Devices Device** Indexが切り替えられたタイミングでの接続デバイス数  
**🔍Selected Device Index Device** Indexで選択されているデバイスのIndex  
**🔍Device Serial Number Device** Indexで選択されているデバイスのシリアル番号  
**🔍Device Render Model Name Device** Indexで選択されているデバイスの機種情報  
  
**GUI Tap**  
**Laycast Root Object** 操作対象のCanvas(シーン直下に配置してください)  
**🔍tappedLeft** 左コントローラーでタップされている間ON  
**🔍tappedRight** 右コントローラーでタップされている間ON  
**TapOnDistance** タップされていると判定する距離  
**TapOffDistance** タップが終わったと判定する距離  
**🔍LeftHandU** 左コントローラーでタップされているU座標  
**🔍LeftHandV** 左コントローラーでタップされているV座標  
**🔍LeftHandDistance** 左コントローラーの距離  
**🔍RightHandU** 右コントローラーでタップされているU座標  
**🔍RightHandV** 右コントローラーでタップされているV座標  
**🔍RightHandDistance** 右コントローラーの距離  
