using AISGorod.AspNetCore.Authentication.Esia;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EsiaNet8Sample;

/// <summary>
/// Собственная реализация обработки событий от поставщика данных.
/// </summary>
public class CustomEsiaEvents(
    EsiaOptions esiaOptions,
    IEsiaEnvironment esiaEnvironment,
    IEsiaSigner esiaSigner,
    ITempDataDictionaryFactory tempDataDictionaryFactory)
    : EsiaEvents(esiaOptions, esiaEnvironment, esiaSigner)
{
    // Собственные зависимости.

    public override Task RemoteFailure(RemoteFailureContext context)
    {
        var tempData = tempDataDictionaryFactory.GetTempData(context.HttpContext);
        if (tempData.TryAdd("ErrorMessage", "Ошибка взаимодействия с ЕСИА. Пожалуйста, попробуйте ещё раз."))
        {
            tempData.Save();
        }

        context.Response.Redirect(context.Properties?.RedirectUri ?? "/");
        context.HandleResponse();
        return Task.CompletedTask;
    }
}