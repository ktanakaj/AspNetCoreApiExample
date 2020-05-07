# アプリ用の設定とプラグインを取り込んだFluentd実行イメージを作成
FROM fluent/fluentd AS runtime
RUN gem install fluent-plugin-rewrite-tag-filter
COPY Docker/fluentd/fluent.conf /fluentd/etc/fluent.conf
