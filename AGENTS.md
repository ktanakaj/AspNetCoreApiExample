# リポジトリガイドライン

## プロジェクト概要
- このリポジトリは **ASP.NET Core Web APIのサンプルアプリ**です。

## ディレクトリ構成
* `AspNetCoreApiExample` - ASP.NET Coreサーバーソース
* `*.Tests` - 各プロジェクトのテストコード
* `Docker` - その他Docker関連の設定やスクリプト等

## ビルド&テスト手順
- `dotnet build AspNetCoreApiExample.slnx`: ビルド
- `dotnet test AspNetCoreApiExample.slnx`: テスト
- `cd AspNetCoreApiExample`
  - `dotnet ef migrations add [migration name]`: マイグレーションスクリプト生成
  - `dotnet ef migrations script --idempotent`: マイグレーションSQL生成
- `docker compose build`: Dockerビルド

## 実装ガイド
- 既存のソースコードやAPIの設計を参考にして、造りを合わせる。※ただし既存が露骨に間違っている場合は除く。
- 一般的なコーディングのベストプラクティス（命名, DRY, SOLID, KISS, 等）を意識する。
- ASP.NET CoreやEFCore, C#のベストプラクティスを意識する。
- 共通のテストデータは `TestData.cs` に置く。テスト固有のものは各テストで個別に定義。
- ユニットテストではDBはインメモリDBにしているため、無理にモックにしない。
- 必要に応じてDTOを用いるが、Entity≒DTOとなるようなケースではEntityをそのまま使ってもよい。

## 禁止事項
- **master 直コミット禁止**：必ずブランチを切り、PR経由でマージする。
- テストが通らない差分禁止: PR作成/更新時は修正箇所のビルドとテスト、StyleCopが通ることを確認すること。