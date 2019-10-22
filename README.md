# AISGorod.AspNetCore.Authentication.Esia

Данная библиотека добавляет возможность авторизации через госуслуги (ЕСИА) по протоколу OpenID Connect, а также добавляет интерфейс доступа к REST-сервисам ЕСИА.

## Требования

1. AspNetCore не ниже 2.1.
2. Сертификат ИС должен быть RS256 (не ГОСТ).

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
        //options.EnvironmentInstance = ...; - можно использовать свою реализацию.
        options.Mnemonic = "TESTSYS";
        options.Certificate = () => new X509Certificate2(System.IO.File.ReadAllBytes(@"c:\cert.pfx"), "");
        options.Scope = new[] { "fullname", "snils", "email", "mobile" };
    });
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

## Генерация сертификатов (файлы *.pem, *.key и *.pfx)

> Этот раздел больше походит на шпаргалку или мини-инструкцию для генерации сертификатов через _openssl_.

Сперва необходимо сгенерировать сертификат с приватным ключом.

Воспользуемся утилитой _openssl_ (генерируется сертификат на 10 лет, что небезопасно):
```
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 3650
```

Данные о стране, городе, имени сертификата можно вбивать любые, они не играют роли для ЕСИА. 

После успешного выполнения команды файл `cert.pem` будет содержать искомый сертификат.
Именно этот файл необходимо прикладывать к заявками на подключение ИС к ЕСИА.

Файл `key.pem` будет содержать зашифрованный закрытый (приватный) ключ.

Генерация сертификата в формате PKCS#12 (или PFX, это необходимо для .NET) выполняется следующей командой:

```
openssl pkcs12 -export -out key.pfx -inkey key.pem -in cert.pem
```

## Есть замечания / хочу внести вклад

Создавайте _issue_, предлагайте свои _pull request_-ы.

Вместе мы сможем сделать отличную библиотеку. :)
