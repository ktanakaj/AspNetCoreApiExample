# MySQLクライアントの入ったSQL実行用イメージを作成
FROM debian:11 AS runtime
RUN apt-get update -yq && \
    apt -y install wget lsb-release gnupg && \
    wget -O /tmp/mysql-apt-config.deb https://dev.mysql.com/get/mysql-apt-config_0.8.23-1_all.deb && \
    ( yes 4 | dpkg -i /tmp/mysql-apt-config.deb ) && \
    apt-get update -yq && \
    apt -y install mysql-client

# DB起動チェック用のスクリプトもインストール
ADD https://github.com/vishnubob/wait-for-it/raw/master/wait-for-it.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/wait-for-it.sh

# 環境変数で指定されたDBにマイグレーションSQLを実行する
COPY AspNetCoreApiExample/Migrations/idempotent.sql /sql/
ENTRYPOINT wait-for-it.sh $DB_HOST:${DB_PORT:-3306} -- mysql -h $DB_HOST -P ${DB_PORT:-3306} -u $DB_USER -p$DB_PASSWORD $DB_NAME < /sql/idempotent.sql
