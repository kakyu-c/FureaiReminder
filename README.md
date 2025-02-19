# FureaiReminder

## これは何？
ブルアカのカフェタッチ（ふれあい）用のリマインダーです。

## 動作環境
- 通常版は64bitのWindows10以上、.NET 8.0以上が入った環境なら動きます。  
.NET 8.0は[こちらのデスクトップランタイム(x64)](https://dotnet.microsoft.com/ja-jp/download/dotnet/8.0)が入っていれば多分動きます。

- フル版は64bitのWindows10以上なら基本的に動くと思います。その代わりサイズがデカいです。

## ダウンロード
このページの右の方のReleasesに置いてあります。

## 使い方
- FureaiReminder.exeを起動して時刻をセットするだけです。  
3時間後、もしくは4 or 16時になるとデスクトップ通知でお知らせします。  
![デスクトップ通知のスクリーンショット](/Assets/ToastNotification.png)

- 今！ボタンを押すと現在時刻がセットされ自動でリマインダーが設定されます。  
手動で時刻入力した場合はセットボタンを押すまで動きません。

- 通知上の「ふれあった！」ボタンを押すとその時点での時刻が自動で入力されてリマインダーが再び設定されます。便利だね。

- プログラム終了時に時刻をファイルに保存しているので次回起動時に読み込みます。  
まだ時間が来ていなければそのままリマインダー起動、過ぎている場合は無視されます。

- トレイアイコンにマウスカーソルを乗せると次回までの時間が表示されます。  
![トレイアイコンのスクリーンショット](/Assets/Tray.png)

## 連絡先
不具合や要望など何かあれば以下まで。

Bluesky: [かきゅー](https://bsky.app/profile/kakyu.bsky.social)

## Lisence

This project is licensed under the MIT License, see the LICENSE.txt file for details