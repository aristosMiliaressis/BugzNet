#!/usr/bin/python3

from selenium import webdriver
from selenium.webdriver import ActionChains
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
import time

opts = webdriver.FirefoxOptions()
opts.add_argument("--headless")
driver = webdriver.Firefox(firefox_options=opts)
driver.get("http://localhost/Identity/Login")

time.sleep(1)
u_input = driver.find_element_by_name("Data.Email")
p_input = driver.find_element_by_name("Data.Password")
u_input.send_keys("admin@bugznet.com")
p_input.send_keys("!bugzn3t@admin")
time.sleep(1)
p_input.send_keys(Keys.RETURN)
time.sleep(1)

driver.get("http://localhost/Bugs")
print("Admin user clicking around the ui")

links = driver.find_elements(By.CSS_SELECTOR, 'a[href*="/Bugs/"]')
link_count=len(links)
for i in range(link_count):
    links = driver.find_elements(By.CSS_SELECTOR, 'a[href*="/Bugs/"]')
    driver.execute_script("arguments[0].scrollIntoView(true);", links[i])
    time.sleep(1)
    links[i].click()
    time.sleep(1)
    form = driver.find_element_by_tag_name('form')
    action = ActionChains(driver)
    action.move_to_element_with_offset(form,50,50).click().perform()
    time.sleep(1)
    driver.back()
    time.sleep(1)

time.sleep(2)
driver.close()