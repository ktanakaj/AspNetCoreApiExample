# アプリを動作させるイメージを構築
# ※ このステージは、VS から高速モードで実行するときに使用されます（デバッグ構成の既定値）
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# アプリをビルドするためのイメージを構築
# ※ このステージは、サービス プロジェクトのビルドに使用されます
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ローカルファイルをコンテナ内にコピー&依存関係解決。
# Dockerキャッシュ用に、変更されることが少ないプロジェクトファイルだけコピーして、NuGet依存関係を先に解決。
COPY ["AspNetCoreApiExample/AspNetCoreApiExample.csproj", "AspNetCoreApiExample/"]
RUN dotnet restore "AspNetCoreApiExample/AspNetCoreApiExample.csproj"
# 残りをコピー
COPY . .

# アプリをビルド
WORKDIR "/src/AspNetCoreApiExample"
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./AspNetCoreApiExample.csproj" -c $BUILD_CONFIGURATION -o /app/build

# アプリを公開用に生成
# ※ このステージは、最終ステージにコピーするサービス プロジェクトを公開するために使用されます
FROM build AS publish
RUN dotnet publish "./AspNetCoreApiExample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ビルドしたアプリを実行用イメージに展開、起動する
# ※ このステージは、運用環境または VS から通常モードで実行している場合に使用されます（デバッグ構成を使用しない場合の既定）
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreApiExample.dll"]