
# BugzNet 

An intentionally vulnerable .NET webapp

Run `docker-compose up` to start

Use `emulate.sh <url>` to pass urls to an emulated user

if you get an sql error when starting up the app try removing the data volume
`docker volume rm bugznet_data`