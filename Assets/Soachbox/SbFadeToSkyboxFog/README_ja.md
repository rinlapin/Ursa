# Sb FadeToSkyboxFog

## 概要
フォグ濃度に応じてスカイボックスの色へとフェードしていくフォグエフェクトです。  
URP17.0以上を対象としています。

## デモ用の設定
デモシーンを開く場合は、併せてRenderPiplineAssetの設定を変更する必要があります。  
Quality設定を開き（メニューの「Edit」→「Project Settings」→「Quality」）、  
「Render Pipline Asset」の項目を「FadeToSkyboxDemoURPAsset」に変更してください。

## 基本の使い方
パッケージのインストール後、メニューの「Tools」→「FadeToSkyboxFog」→「Open MainCamera Renderer Data」を選択してください。  
インスペクターに現在のカメラで使用中のRendererの設定が表示されます。  

設定の下部にある「Add Renderer Feature」クリックし、リストから「Sb Fade To Skybox Fog」を選択します。  
これでフォグ機能が使用可能になります。

この段階で、シェーダーグラフの「Sb Fade To Skybox Sub Graph」が使用可能になります。  
全画面に適応するフォグを使用する場合は、設定内の「Enable Screen Fog」をオンにします。

あとは好みに応じて「FogType」やその他の設定を調整してください。

---
パラメータに関する詳細は **Documentation.pdf** をご参照ください。