using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace MVC.ModelBinders;

public sealed class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
        var value = valueResult.FirstValue;
        if (string.IsNullOrWhiteSpace(value))
            return Task.CompletedTask;

        // Acepta decimales tanto con ',' como con '.' para evitar fallos por cultura del navegador
        if (TryParse(value, out var parsed))
        {
            bindingContext.Result = ModelBindingResult.Success(parsed);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"El valor '{value}' no es un número válido.");
        return Task.CompletedTask;
    }

    private static bool TryParse(string input, out decimal result)
    {
        var styles = NumberStyles.Number;

        // 1) Cultura actual
        if (decimal.TryParse(input, styles, CultureInfo.CurrentCulture, out result))
            return true;

        // 2) Invariant
        if (decimal.TryParse(input, styles, CultureInfo.InvariantCulture, out result))
            return true;

        // 3) Normalización simple
        var normalized = input.Trim();
        if (normalized.Contains(',') && !normalized.Contains('.'))
            normalized = normalized.Replace(',', '.');

        return decimal.TryParse(normalized, styles, CultureInfo.InvariantCulture, out result);
    }
}
