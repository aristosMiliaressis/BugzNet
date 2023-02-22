#!/usr/bin/python3

from selenium import webdriver
import time, sys

opts = webdriver.FirefoxOptions()
opts.add_argument("--headless")
driver = webdriver.Firefox(options=opts)
driver.get(sys.argv[1])
time.sleep(1)

##
# SPOILER !!
#
# ||  this emulates a browser auto filling creds on a site you've saved login creds for.
# ||  i couldn't properly emulate it with selenium, but you can test it on yourself here
# vv  https://github.com/davidgilbertson/xss-autofil

u_input = driver.find_element_by_name("Data.Email")
p_input = driver.find_element_by_name("Data.Password")
u_input.send_keys("user@bugznet.com")
p_input.send_keys("!bugzn3t@user")
p_input.click()

time.sleep(1)
driver.close()
