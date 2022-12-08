using System;
using System.Threading.Tasks;
using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EsiaSample;

/// <summary>
/// Собственная реализация обработки событий от поставщика данных.
/// </summary>
public class CustomEsiaEvents : AISGorod.AspNetCore.Authentication.Esia.EsiaEvents
{
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

    public CustomEsiaEvents(
        EsiaOptions esiaOptions,
        IEsiaEnvironment esiaEnvironment,
        IServiceProvider serviceProvider,
        // Собственные зависимости.
        ITempDataDictionaryFactory tempDataDictionaryFactory) : base(esiaOptions, esiaEnvironment, serviceProvider)
    {
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
    }

    public override Task RemoteFailure(RemoteFailureContext context)
    {
        var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
        if (!tempData.ContainsKey("ErrorMessage"))
        {
            tempData.Add("ErrorMessage", "Ошибка взаимодействия с ЕСИА. Пожалуйста, попробуйте ещё раз.");
            tempData.Save();
        }
        context.Response.Redirect(context.Properties.RedirectUri ?? "/");
        context.HandleResponse();
        return Task.CompletedTask;
    }
}
