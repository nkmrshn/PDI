# PDI - Physical Disk Indicator

## 概要

物理ディスクの読み込み速度と書き込み速度を取得し、タスクトレイに物理ディスクの状態をアイコンで表示します。

ノートパソコンを買ったらHDDのインジケータが省略されており、NLogや自動起動についても勉強したかった為、作ってみました。スタートアップフォルダやレジストリ（SOFTWARE\Microsoft\Windows\CurrentVersion\Run）で起動させてみましたが、現在はタスクスケジューラで起動させています。

## 注意事項

* ビルドしたバイナリは、コミットしていません。
* NLogを用いている為、ビルドするにはNuGet等でインストールする必要があります。
* GitHubで公開している他のレポジトリと同様、勉強やお遊びで作ったものでメンテナンスなどはしていません。

## 開発環境

* Microsoft Windows 10 (64bit)
* Microsoft Visual Studio 2019
* Microsoft .NET Framework 4.7.2
* NLog 4.2.3

以上
