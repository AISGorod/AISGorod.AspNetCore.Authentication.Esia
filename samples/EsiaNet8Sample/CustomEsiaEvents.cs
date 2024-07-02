using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EsiaNet8Sample;

/// <summary>
/// Собственная реализация обработки событий от поставщика данных.
/// </summary>
public class CustomEsiaEvents(
    EsiaOptions esiaOptions,
    IEsiaEnvironment esiaEnvironment,
    IServiceProvider serviceProvider,
    ITempDataDictionaryFactory tempDataDictionaryFactory)
    : EsiaEvents(esiaOptions, esiaEnvironment, serviceProvider)
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
