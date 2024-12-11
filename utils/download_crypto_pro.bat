@echo off

:: Путь к папке nupkg на уровень выше
set NUPKG_PATH=..\nupkg

:: Создаем папку nupkg, если её нет
if not exist "%NUPKG_PATH%" (
    mkdir "%NUPKG_PATH%"
)

:: Базовый URL и список пакетов
set BASE_URL=https://github.com/CryptoPro/libcore/releases/download/v2024.11.19/
set PACKAGES=cryptopro.net.security.2024.11.19.nupkg cryptopro.security.cryptography.2024.11.19.nupkg cryptopro.security.cryptography.pkcs.2024.11.19.nupkg cryptopro.security.cryptography.xml.2024.11.19.nupkg

:: Скачиваем каждый пакет
for %%P in (%PACKAGES%) do (
    echo Downloading %%P...
    curl -o "%NUPKG_PATH%/%%P" %BASE_URL%%%P
)

echo All packages downloaded successfully!
pause
