# ASP.NET Core Web APIサンプルアプリ
[ASP.NET Core](https://docs.microsoft.com/ja-jp/aspnet/core/?view=aspnetcore-6.0)のAPI開発勉強用に作成したサンプルアプリです。

ごくごく簡単なブログを想定したAPI。アプリ構成や開発環境的な部分がメインです。

## 環境
* Docker
  * ASP.NET Core 6.0
  * MySQL 8.x
  * nginx 1.17.x

### 開発環境
* [Docker Desktop 4.x](https://hub.docker.com/editions/community/docker-ce-desktop-windows) - 仮想環境
* [Visual Studio 2022 Community](https://docs.microsoft.com/ja-jp/visualstudio/ide/?view=vs-2022) - 統合開発環境
* [Entity Framework Core Tools](https://docs.microsoft.com/ja-jp/ef/core/miscellaneous/cli/dotnet) - DBコマンド実行用

## 開発環境構築
Windows上でVisual Studioを用いて開発する場合は、Visual StudioとDocker Desktopが動く状態にした上で、  
IDE上からdocker-composeプロジェクトを選択して、デバッグ実行なりで実行してください。  

動作中のアプリには http://localhost/swagger/ でアクセス可能です。

※ テスト用のインメモリDBで良ければ、Visual Studio上からDocker無しの単体でも実行可能です。

## その他
各種ログは標準出力に出力され、Fluentdコンテナにより集計されます。  
（Visual Studioデバッグ実行時は `%TEMP%\AspNetCoreApiExample` 下に出力。）  
システムログ、アクセスログ、SQLログを出力します。

DBを参照する場合は、MySQL Workbench等でMySQLの標準ポートに接続してください（接続情報は `docker-compose.yml` 参照）。

## ライセンス
[MIT](https://github.com/ktanakaj/AspNetCoreApiExample/blob/master/LICENSE)
