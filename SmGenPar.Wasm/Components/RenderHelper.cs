using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SmGenPar.Wasm.Components;

public static class RenderHelper
{
    public static void AddOptionElement(
        this RenderTreeBuilder  builder,
        string?            id,
        string             enumValue,
        params Attribute[] attributes)
    {
        var sequence = 0;
        builder.OpenElement(sequence  += 1, "option");
        builder.AddAttribute(sequence += 1, "forId", id);
        builder.AddAttribute(sequence += 1, "value", enumValue);
        builder.AddContent(sequence + 1, enumValue);
        builder.CloseElement();
    }

    public static void OpenSelectElement(
        this RenderTreeBuilder   builder,
        string?                  id,
        string                   text,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var sequence          = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
        builder.OpenElement(sequence  += 1, "select");
        builder.AddAttribute(sequence += 1, "forId", id);
        builder.AddAttribute(sequence += 1, "onchange", callback);
        builder.AddAttribute(sequence += 1, "value", text);
        if (requiredAttribute is not null) {
            builder.AddAttribute(sequence + 1, "required", true);
        }
    }

    public static void AddLabelElement(
        this RenderTreeBuilder builder,
        string?                forId,
        string                 contentText,
        params Attribute[]     attributes)
    {
        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
        contentText = displayAttribute?.Name ?? contentText;

        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "for", forId);
        builder.AddContent(2, contentText);
        builder.CloseElement();
    }
}