#!/bin/bash

service cron start
(crontab -l ; echo "*/2 * * * * /app/emulate_admin.py") | crontab

dotnet BugzNet.Web.dll