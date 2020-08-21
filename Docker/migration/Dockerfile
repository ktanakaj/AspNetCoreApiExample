# MySQLクライアントの入ったSQL実行用イメージを作成
FROM centos:8 AS runtime
RUN yum install mysql -y
ADD https://github.com/vishnubob/wait-for-it/raw/master/wait-for-it.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/wait-for-it.sh

# 環境変数で指定されたDBにマイグレーションSQLを実行する
COPY AspNetCoreApiExample/Migrations/idempotent.sql /sql/
ENTRYPOINT wait-for-it.sh $DB_HOST:${DB_PORT:-3306} -- mysql -h $DB_HOST -P ${DB_PORT:-3306} -u $DB_USER -p$DB_PASSWORD $DB_NAME < /sql/idempotent.sql
