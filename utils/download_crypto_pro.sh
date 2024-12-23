#!/bin/bash

# Создаем папку nupkg, если её нет
mkdir -p nupkg

# Получение последней версии с помощью GitHub API
LATEST_VERSION=$(curl -s https://api.github.com/repos/CryptoPro/libcore/releases/latest | grep '"tag_name":' | sed -E 's/.*"v([^"]+)".*/\1/')

if [[ -z "$LATEST_VERSION" ]]; then
    echo "Failed to fetch the latest version."
    exit 1
fi

echo "Latest version: $LATEST_VERSION"

# Базовый URL и список пакетов
BASE_URL="https://github.com/CryptoPro/libcore/releases/download/v$LATEST_VERSION/"
PACKAGES=(
    "cryptopro.net.security.$LATEST_VERSION.nupkg"
    "cryptopro.security.cryptography.$LATEST_VERSION.nupkg"
    "cryptopro.security.cryptography.pkcs.$LATEST_VERSION.nupkg"
    "cryptopro.security.cryptography.xml.$LATEST_VERSION.nupkg"
)

# Скачиваем каждый пакет
for PACKAGE in "${PACKAGES[@]}"; do
    echo "Downloading $PACKAGE..."
    wget -P nupkg/ "${BASE_URL}${PACKAGE}"
done

echo "All packages downloaded successfully!"