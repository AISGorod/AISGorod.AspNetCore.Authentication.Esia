# AISGorod.AspNetCore.Authentication.Esia

[![Build Status](https://github.com/AISGorod/AISGorod.AspNetCore.Authentication.Esia/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/AISGorod/AISGorod.AspNetCore.Authentication.Esia/actions/workflows/main.yml)
[![Nuget](https://img.shields.io/nuget/v/AISGorod.AspNetCore.Authentication.Esia)](https://www.nuget.org/packages/AISGorod.AspNetCore.Authentication.Esia/)

Данная библиотека добавляет возможность авторизации через госуслуги (ЕСИА) по протоколу OpenID Connect, а также добавляет интерфейс доступа к REST-сервисам ЕСИА.

## [История изменений](CHANGELOG.md)

## Требования

1. AspNetCore не ниже 8.0.
2. Алгоритм формирования электронной подписи должен быть RS256 (указывается в настройках ИС на [технологическом портале](https://esia.gosuslugi.ru/console/tech/)).

## Подключение

1. Добавьте NuGet-пакет `AISGorod.AspNetCore.Authentication.Esia`.
2. Добавьте NuGet-пакет с криптографией, например, `AISGorod.AspNetCore.Authentication.Esia.BouncyCastle`.
3. Добавьте в _Startup.cs_ следующие строки (ниже данные для примера):

```csharp
services
    .AddAuthentication(/*...*/)
    //...
    .AddEsia(options =>
    {
        options.Environment = EsiaEnvironmentType.Test;
        // options.EnvironmentInstance = ...; // можно использовать свою реализацию.
        options.Mnemonic = "МНЕМОНИКА_ВАШЕЙ_СИСТЕМЫ";
        options.Scope = ["fullname", "snils", "email", "mobile", "usr_org"];

        options.UseBouncyCastle(bouncyCastleOptions =>
        {
            bouncyCastleOptions.KeyFilePath = "/home/username/esia.key";
            bouncyCastleOptions.CertFilePath = "/home/username/esia.pem";
        });
    });
```

4. Если хотите вызывать API-методы с использованием access_token через `IEsiaRestService`, убедитесь, что в приложении подключен _HttpContextAccessor_:

```csharp
services.AddHttpContextAccessor();
```

[Больше примеров в репозитории](./samples/)

## Выполнение методов API через `IEsiaRestService`

Необходимо в контроллере (или где-нибудь ещё) запросить интерфейс `IEsiaRestService`.
В нём есть метод `CallAsync`, который и отвечает за актуализацию токенов и общение с API ЕСИА.

Пример запроса:

```csharp
var oId = User.Claims.First(i => i.Type == "sub").Value;
var contactsJson = await esiaRestService.CallAsync($"/rs/prns/{oId}/ctts?embed=(elements)", HttpMethod.Get);
ViewBag.Contacts = contactsJson.ToString(Newtonsoft.Json.Formatting.Indented);
```

Данный фрагмент кода получает _oId_ пользователя, запрашивает все контакты и складывает их JSON-представление в ViewBag.

## Получение настроек подключения к ЕСИА

Бывает полезно получить информацию о подключении к ЕСИА (адрес хоста, сертификат и т.д.) вне `IEsiaRestService`.

Это можно сделать путём запроса интерфейса `IEsiaEnvironment`.

Также открытыми являются классы `TestEsiaEnvironment` и `ProductionEsiaEnvironment`, от которых можно унаследоваться.

> Пример использования настроек подключения смотрите в проекте [EsiaNet8Sample](./samples/EsiaNet8Sample/) на стартовой странице.

## Получение дополнительных сведений о ФЛ в момент входа

Для упрощения процесса получения данных о физическом лице в момент его аутентификации в системе, в настройках `IEsiaOptions` объявлены флаги `GetPrns<...>OnSignIn`.
Если они выбраны, то после получения _access_token_ дополнительно в API ЕСИА отправляются запросы, связанные с этими флагами.

Примеры данных, которые можно дополнительно получить (при наличии соответствующего scope):

- `GetPrnsContactInformationOnSignIn` - сведения о контактных данных ФЛ.
- `GetPrnsAddressesOnSignIn` - сведения об адресах ФЛ.
- `GetPrnsDocumentsOnSignIn` - сведения о документах ФЛ.

## Как запустить примеры

Для того, чтобы запустить примеры, необходимо предварительно:

- Определиться с поставщиком криптографии.
  Для целей тестирования можно использовать как trial-версию КриптоПРО CSP, так и BouncyCastle.
- Затем необходимо выпустить тестовый сертификат ЭП, который понадобится для подписи запросов.
  Его выпуск целиком и полностью зависит от того поставщика криптографии, который был выбран ранее.
- Затем необходимо подать заявку на регистрацию ИС в тестовой среде ЕСИА согласно [регламенту взаимодействия с ЕСИА](https://digital.gov.ru/documents/reglament-informaczionnogo-vzaimodejstviya-uchastnikov-s-operatorom-esia-i-operatorom-ekspluataczii-infrastruktury-elektronnogo-pravitelstva).

Теперь для запуска примера потребуется:

- Изменить мнемонику ИС в `~/samples/EsiaSample/Program.cs`.
- Изменить путь до ключа (KeyFilePath) и сертификата (CertFilePath) в `~/samples/EsiaSample/Program.cs` метод `UseBouncyCastle(...)`.
  Или использовать КриптоПРО .NET.
  Там будет необходимо указать отпечаток сертификата ЭП и пароль от ключевого носителя (при наличии).

Если вы будете использовать режим подписи через CryptoPro, необходимо загрузить nuget пакеты. В проекте есть файл, который делает это автоматически:

Необходимо сделать файл скрипта исполняемым и запустить:

```bash
$ chmod +x utils/download_crypto_pro.sh
$ utils/download_crypto_pro.sh
```

Запуск примера можно проделать следующим образом:

```bash
$ dotnet build
$ dotnet run -p samples/EsiaNet8Sample/
```

Веб-сайт для демонстрации работы с ЕСИА будет доступен по адресу https://localhost:5000/.

## Есть замечания / хочу внести вклад

Создавайте _issue_, предлагайте свои _pull request_-ы.

Вместе мы сможем сделать отличную библиотеку. :)
