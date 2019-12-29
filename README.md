# AISGorod.AspNetCore.Authentication.Esia

Данная библиотека добавляет возможность авторизации через госуслуги (ЕСИА) по протоколу OpenID Connect, а также добавляет интерфейс доступа к REST-сервисам ЕСИА.

## Требования

1. AspNetCore не ниже 2.1.
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

## Немного про ГОСТ 34.10-2012 и ГОСТ 34.11-2012

> Этот раздел больше походит на шпаргалку или мини-инструкцию для генерации сертификатов через _openssl_.

Как недавно стало известно, ЕСИА с I квартала 2020 года отключает возможность подписывать запросы от ИС к ЕСИА при помощи алгоритмов, отличных от ГОСТ 34.10-2012.

Радует то, что _пока_ в планах нет использования для этих целей квалифицированных электронных подписей, то есть достаточно как и раньше самоподписанного сертификата.
Также хорошо, что ЕСИА оставили возможность формирования маркеров с их стороны при помощи RS256 (хотя бы _это_ переписывать не придётся).

Таким образом, необходимо сгенерировать сертификат с приватным ключом (сертификат генерируется на 10 лет, что небезопасно).
Также сразу создадим pfx-файл, но смысла в этом пока нет.

```
openssl req -x509 -newkey gost2012_256 -pkeyopt paramset:A -nodes -keyout esia.key -out esia.pem -days 3650
openssl pkcs12 -export -out esia.pfx -inkey esia.key -in esia.pem
```

> Стоит иметь в виду, что для работы с ГОСТ в openssl требуется подключение движка gost.

Данные о стране, городе, имени сертификата можно вбивать любые, они не играют роли для ЕСИА. 

Чтобы проверить, что openssl работает, можете использовать следующую команду:

```
openssl cms -sign -engine gost -inkey esia.key -signer esia.pem <<< '123'
```

## Есть замечания / хочу внести вклад

Создавайте _issue_, предлагайте свои _pull request_-ы.

Вместе мы сможем сделать отличную библиотеку. :)
