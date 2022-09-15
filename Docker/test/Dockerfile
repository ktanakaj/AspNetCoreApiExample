# ユニットテストを実行するイメージを構築
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# ローカルファイルをコンテナ内にコピー&依存関係解決
WORKDIR /src
# Dockerキャッシュ用に、変更されることが少ないプロジェクトファイルだけコピーして、NuGet依存関係を先に解決
COPY AspNetCoreApiExample.sln .
COPY docker-compose.dcproj .
COPY ["AspNetCoreApiExample/AspNetCoreApiExample.csproj", "AspNetCoreApiExample/"]
COPY ["AspNetCoreApiExample.Tests/AspNetCoreApiExample.Tests.csproj", "AspNetCoreApiExample.Tests/"]
RUN dotnet restore "AspNetCoreApiExample.sln"
# 残りをコピー
COPY . .

# ユニットテストを実行する
CMD dotnet test ./AspNetCoreApiExample.sln --no-restore
