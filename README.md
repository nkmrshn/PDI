# PDI - Physical Disk Indicator

## 概要

物理ディスクの読み込み速度と書き込み速度を取得し、通知領域に物理ディスクの状態をアイコンで表示します。

ノートパソコンを買ったらHDDのインジケータが省略されており、自動起動やNLogについても勉強したかった為、作ってみました。

以前は、スタートアップフォルダやレジストリ（SOFTWARE\Microsoft\Windows\CurrentVersion\Run）で起動させていました。現在は、Windows標準のタスクスケジューラに手動で登録しています。

%LOCALAPPDATA%\PDI\PDI.logというログファイルが作成され、起動・終了が記録されます。

## 注意事項

* NLogを用いている為、ビルドするにはNuGet等でインストールする必要があります。
* GitHubで公開している他のレポジトリと同様、勉強やお遊びで作ったものでメンテナンスなどはしていません。

## 開発環境

* Microsoft Windows 10 (64bit)
* Microsoft Visual Studio 2013, 2019
* Microsoft .NET Framework 4.7.2
* NLog 4.6.2

## ライセンス
LICENSEファイルをご覧ください。

以上