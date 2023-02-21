#!/bin/bash

if [[ $# -eq 0 ]]
then
    echo "USAGE: $0 <url>"
fi

echo "Make sure to use either your primary or docker0 interface ip for exfiltration"

container_hash=$(docker ps | grep bugznet_bugznet_web | awk '{print $1}')

docker exec -it $container_hash /app/emulate_user.py $1