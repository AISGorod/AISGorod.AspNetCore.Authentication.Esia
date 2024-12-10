#!/bin/bash

# Создаем папку nupkg, если её нет
mkdir -p nupkg

# Базовый URL и список пакетов
BASE_URL="https://github.com/CryptoPro/libcore/releases/download/v2024.11.19/"
PACKAGES=(
    "cryptopro.net.security.2024.11.19.nupkg"
    "cryptopro.security.cryptography.2024.11.19.nupkg"
    "cryptopro.security.cryptography.pkcs.2024.11.19.nupkg"
    "cryptopro.security.cryptography.xml.2024.11.19.nupkg"
)

# Скачиваем каждый пакет
for PACKAGE in "${PACKAGES[@]}"; do
    echo "Downloading $PACKAGE..."
    wget -P nupkg/ "${BASE_URL}${PACKAGE}"
done

echo "All packages downloaded successfully!"