# アプリを動作させるイメージを構築
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# アプリをビルドするためのイメージを構築
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# ローカルファイルをコンテナ内にコピー&依存関係解決
WORKDIR /src
COPY ["AspNetCoreApiExample/AspNetCoreApiExample.csproj", "AspNetCoreApiExample/"]
RUN dotnet restore "AspNetCoreApiExample/AspNetCoreApiExample.csproj"
# 残りをコピー
COPY . .

# アプリをビルド
WORKDIR "/src/AspNetCoreApiExample"
RUN dotnet build "AspNetCoreApiExample.csproj" -c Release -o /app/build

# アプリを公開用に生成
FROM build AS publish
RUN dotnet publish "AspNetCoreApiExample.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ビルドしたアプリを実行用イメージに展開、起動する
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreApiExample.dll"]
