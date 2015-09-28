テストversion: 5.1.1~

# Gesture Framework

## これなに
* Unityでスマホゲームを作成するときに煩わしいタッチ入力部分をサポートするやつ
* タッチ操作のテストがビルドしないとできないから厳しい
* 基本的なタッチ操作と（今のところ）Xcode風のAltキーによるピンチインとピンチアウトをUnityエディタ上で簡単にテスト可能
* /Assets/GestureFramework内のスクリプト郡を利用してください
* タッチ処理をGestureControllerで書いて、それをGestureManagerで管理するという形なので処理部分をコンパクトに書けるはず？
 * UIに関連する処理とかゲームのメイン処理に関連する処理に分けるとか利用シーンは結構ありそう

## 機能説明
* マルチタッチ対応
* Down, Up, Dragのタッチイベントを検知可能
 * Down: タッチ開始
 * Up; タッチ終了
 * Drag: タッチ中（動いてなくても検知される）
* タッチ処理優先度機能

## Howto
### GestureManagerの追加
* GestureManagerをAddComponentしたオブジェクトをシーン中に配置してください
* GestureManagerはタッチイベントを検知後、登録されたコントローラーを優先度の小さい数字から処理していきます
* GestureManagerはシングルトンクラスとして定義されています
### GestureControllerの追加
* GestureControllerはインターフェイスIGestureContollerを継承しているスクリプトです。
* GestureControllerはGestureManagerに登録することでタッチイベント発生時にイベント（OnGestureDownなど）が呼ばれます
* GestureManagerに登録できる数は特に制限していませんが、あまり登録しすぎると動作に影響がでるかもしれません
* GestureControllerからGestureManagerにキャンセル命令(BreakTouchEvent)を呼ぶことで、それ以降の優先度が低いGestureControllerの処理を飛ばせます
* GestureControllerのIsGestureProcessでfalseを返すと処理を飛ばすこともできます
