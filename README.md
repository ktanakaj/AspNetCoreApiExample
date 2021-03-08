# ASP.NET Core Web APIサンプルアプリ
[ASP.NET Core](https://docs.microsoft.com/ja-jp/aspnet/core/?view=aspnetcore-3.1)のAPI開発勉強用に作成したサンプルアプリです。

ごくごく簡単なブログを想定したAPI。アプリ構成や開発環境的な部分がメインです。

## 環境
* CentOS 7
* MariaDB 5.x
* nginx 1.12.x

### 開発環境
* Visual Studio 2019 Community - 統合開発環境
* Vagrant 2.2.x - 仮想環境管理
    * VirtualBox 6.x - 仮想環境
    * vagrant-vbguest - Vagrantプラグイン

※ VirtualBoxの他、Hyper-Vでも動作します。

## 開発環境構築
### VM環境での開発
実環境に近いVM環境で動かす場合は、`vagrant up` で環境を構築してください。  
初回起動時に、Ansibleにより自動的にアプリが動作する環境が作成されます。

VM環境では、`dotnet publish -c Release` で作成したビルドが `aspnetcore-example.service` として動作します。  
動作中のサービスには http://[DHCPで振られたIP]/swagger/ または http://localhost/swagger/ でアクセス可能です。

### Visual Studioでの開発
サンプルアプリは、Visual Studio上でも実行可能です。  
その場合は普通にIDE上からデバッグ実行なりで実行してください。  
（DBはテスト用のインメモリDBになります。）

## その他
各種ログは `/var/log/local/aspnetcoreapi-example` 下に出力されます。  
（Visual Studio実行時は `%TEMP%\AspNetCoreApiExample` 下に出力。）  
システムログ、アクセスログ、SQLログを出力します。

VMのDBを参照する場合は、MySQL Workbench等でMySQLの標準ポートに接続してください（接続情報は `appsettings.json` 参照）。

## ライセンス
[MIT](https://github.com/ktanakaj/AspNetCoreApiExample/blob/master/LICENSE)
