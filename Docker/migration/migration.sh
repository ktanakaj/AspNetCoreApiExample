#!/bin/bash -eu

#
# @(#)マイグレーションスクリプト
#

SCRIPT_DIR=$(cd $(dirname $0); pwd)
APP_DIR=${SCRIPT_DIR}/../..

echo -e "Migration as ${ASPNETCORE_ENVIRONMENT} environment."

cd ${APP_DIR}/AspNetCoreApiExample
dotnet ef database update
