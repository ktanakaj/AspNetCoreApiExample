# アプリを動作させるイメージを構築
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

# アプリをビルドするためのイメージを構築
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# ローカルファイルをコンテナ内にコピー&依存関係解決
WORKDIR /src
COPY ["AspNetCoreApiExample/AspNetCoreApiExample.csproj", "AspNetCoreApiExample/"]
RUN dotnet restore "AspNetCoreApiExample/AspNetCoreApiExample.csproj"
# 残りをコピー
COPY . .

# アプリを公開用にビルド
FROM build AS publish
WORKDIR "/src/AspNetCoreApiExample"
RUN dotnet publish -c Release -o /app

# ビルドしたアプリを実行用イメージに展開、起動する
FROM base AS runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreApiExample.dll"]
