@echo off
net stop ImagePrefetchService
TaskKill /f /im ImagePrefetchService
sc.exe config "ImagePrefetchService" obj= ".\Administrator" password= "Pa$$word"
timeout /t  5
net start ImagePrefetchService
timeout /t  10