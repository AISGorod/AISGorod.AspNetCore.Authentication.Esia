# AISGorod.AspNetCore.Authentication.Esia

[![Build Status](https://github.com/AISGorod/AISGorod.AspNetCore.Authentication.Esia/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/AISGorod/AISGorod.AspNetCore.Authentication.Esia/actions/workflows/main.yml)
[![Nuget](https://img.shields.io/nuget/v/AISGorod.AspNetCore.Authentication.Esia)](https://www.nuget.org/packages/AISGorod.AspNetCore.Authentication.Esia/)

Данная библиотека добавляет возможность авторизации через госуслуги (ЕСИА) по протоколу OpenID Connect, а также добавляет интерфейс доступа к REST-сервисам ЕСИА.

## [История изменений](CHANGELOG.md)

## Требования

1. AspNetCore не ниже 6.0.
2. Алгоритм формирования электронной подписи должен быть RS256 (указывается в настройках ИС на [технологическом портале](https://esia.gosuslugi.ru/console/tech/)).

## Подключение

1. Добавьте NuGet-пакет `AISGorod.AspNetCore.Authentication.Esia`.
2. Добавьте в _Startup.cs_ следующие строки (ниже данные для примера):

```csharp
services
    .AddAuthentication(...)
    ...
    .AddEsia("Esia", options =>
    {
        options.Environment = EsiaEnvironmentType.Test;
        //options.EnvironmentInstance = ...; // можно использовать свою реализацию.
        options.Mnemonic = "TESTSYS";
        options.Scope = new[] { "fullname", "snils", "email", "mobile" };
    });
services.AddSingleton<IEsiaSigner, OpensslEsiaSigner>(); // нужна своя реализация подписи запросов от ИС в ЕСИА
```

3. Также убедитесь, что в _Startup.cs_ есть подключение _HttpContextAccessor_:

```csharp
services.AddHttpContextAccessor();
```

> Пример кода смотрите в проекте `EsiaSample`.
> Необходимо только изменить _Startup.cs_.

## Выполнение методов API

Необходимо в контроллере (или где-нибудь ещё) запросить интерфейс `IEsiaRestService`.
В нём есть метод `CallAsync`, который и отвечает за актуализацию токенов и общение с API ЕСИА.

Пример запроса:

```csharp
var oId = User.Claims.First(i => i.Type == "sub").Value;
var contactsJson = await esiaRestService.CallAsync($"/rs/prns/{oId}/ctts?embed=(elements)", HttpMethod.Get);
ViewBag.Contacts = contactsJson.ToString(Newtonsoft.Json.Formatting.Indented);
```

Данный кусок кода получает _oId_ пользователя, запрашивает все контакты и складывает их JSON-представление в ViewBag.

## Получение настроек подключения к ЕСИА

Бывает полезно получить информацию о подключении к ЕСИА (адрес хоста, сертификат и т.д.) вне `IEsiaRestService`.

Это можно сделать путём запроса интерфейса `IEsiaEnvironment`.

Также открытыми являются классы `TestEsiaEnvironment` и `ProductionEsiaEnvironment`, от которых можно унаследоваться.

> Пример использования настроек подключения смотрите в проекте `EsiaSample` на стартовой странице.

## Как запустить пример

> Для ОС Windows 10 необходимо установить [Windows Subsystem for Linux](https://docs.microsoft.com/ru-ru/windows/wsl/install-win10) и Ubuntu 18.04 в нём.
> Действия выполняются внутри терминала этой ОС.

Данный раздел показывает, как можно запустить пример работы с ЕСИА на Ubuntu 18.04 (или Windows 10 c WSL).
Такая конфигурация выбрана из-за того, что на Linux намного удобнее включается поддержка ГОСТ для openssl.

Сперва необходимо обновить списки пакетов: `$ sudo apt update`.

Затем устанавливается пакет для поддержки ГОСТ в openssl: `$ sudo apt install libengine-gost-openssl1.1`.

После этого необходимо открыть файл с настройками openssl: `$ sudo nano /etc/ssl/openssl.cnf`.

Дописать в начало файла (например, после `oid_section = new_oids`): `openssl_conf = openssl_def`.

Дописать в конец файла:

```ini
[openssl_def]
engines = engine_section

[engine_section]
gost = gost_section

[gost_section]
engine_id = gost
dynamic_path = /usr/lib/x86_64-linux-gnu/engines-1.1/gost.so
default_algorithms = ALL
CRYPT_PARAMS = id-Gost28147-89-CryptoPro-A-ParamSet
```

Для проверки установки движка gost можно выполнить следующую команду и сравнить результат с представленным ниже:

```bash
$ openssl engine gost -c
(gost) Reference implementation of GOST engine
 [gost89, gost89-cnt, gost89-cnt-12, gost89-cbc, grasshopper-ecb, grasshopper-cbc, grasshopper-cfb, grasshopper-ofb, grasshopper-ctr, md_gost94, gost-mac, md_gost12_256, md_gost12_512, gost-mac-12, gost2001, gost-mac, gost2012_256, gost2012_512, gost-mac-12]
```

Теперь необходимо сгенерировать ключи для ЕСИА при помощи команд:

```bash
$ openssl req -x509 -newkey gost2012_256 -pkeyopt paramset:A -nodes -keyout esia.key -out esia.pem -days 3650 -engine gost
$ openssl pkcs12 -export -out esia.pfx -inkey esia.key -in esia.pem -engine gost
```

Данные о стране, городе, имени сертификата можно вбивать любые, они не играют роли для ЕСИА.

Чтобы проверить, что подпись данных в openssl работает, можете использовать следующую команду:

```bash
$ openssl cms -sign -engine gost -inkey esia.key -signer esia.pem <<< '123'
```

Должен вернуться вывод с огромным base64-текстом, разбитым на несколько строк.

> Для регистрации ключа в ЕСИА на технологический портал требуется загружать файл `.pem`.

В проекте существует 2 способа подписи:

- Bouncy castle.
- CryptoPro.

По умолчание указан Bouncy castle. CryptoPro используется аналогично.

Теперь для запуска примера потребуется:

- изменить мнемонику ИС в `~/samples/EsiaSample/Program.cs`.
- изменить путь до ключа (KeyFilePath) и сертификата (CertFilePath) в `~/samples/EsiaSample/Program.cs` метод `UseBouncyCastle(...)`.
- установить [.NET Core SDK](https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-package-manager-ubuntu-1804), если он ещё не стоит.
  При этом версия SDK должна совпадать с версией netcore в `~/samples/EsiaSample`.
  Это необходимо для Razor.

Если вы будете использовать режим подписи через CryptoPro, необходимо загрузить nuget пакеты. В проекте есть файл, который делает это автоматически:

Необходимо сделать файл скрипта исполняемым и запустить:

```bash
$ chmod +x utils/download_crypto_pro.sh
$ utils/download_crypto_pro.sh
```

Запуск примера можно проделать следующим образом:

```bash
$ dotnet build
$ dotnet run -p samples/EsiaSample/
```

Веб-сайт для демонстрации работы с ЕСИА будет доступен по адресу https://localhost:5000/.

> Кстати, замечено, что при включенном ГОСТ в openssl не всегда восстанавливаются пакеты NuGet.
> Временно выключить поддержку ГОСТ можно, закомментировав строку, написанную в настройках openssl в начале файла.

## Есть замечания / хочу внести вклад

Создавайте _issue_, предлагайте свои _pull request_-ы.

Вместе мы сможем сделать отличную библиотеку. :)
