# マイグレーションを実行するイメージを構築
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# dotnet-efをインストール、パスを通す
RUN dotnet tool install -g dotnet-ef --version 3.1.0
ENV PATH $PATH:/root/.dotnet/tools
# 起動チェック用コマンドをインストール
RUN apt-get update -yq && apt-get install -yq wait-for-it

# ローカルファイルをコンテナ内にコピー&依存関係解決
WORKDIR /src
COPY ["AspNetCoreApiExample/AspNetCoreApiExample.csproj", "AspNetCoreApiExample/"]
RUN dotnet restore "AspNetCoreApiExample/AspNetCoreApiExample.csproj"
# 残りをコピー
COPY . .

# アプリをビルド
WORKDIR "/src/AspNetCoreApiExample"
RUN dotnet build

# マイグレーションを実行する
FROM build AS runtime
WORKDIR /src
ENTRYPOINT ["./Docker/migration/migration.sh"]
