# アプリ用の設定を取り込んだNGINX実行イメージを作成
FROM nginx AS runtime
COPY Docker/nginx/mysite.template /etc/nginx/conf.d/
# NGINX起動時に動的に環境変数を展開する
CMD envsubst '$$NGINX_PORT $$BACKEND_SERVER'< /etc/nginx/conf.d/mysite.template > /etc/nginx/conf.d/default.conf && exec nginx -g 'daemon off;'
