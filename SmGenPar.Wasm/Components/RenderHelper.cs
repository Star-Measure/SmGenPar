using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace SmGenPar.Wasm.Components;

public static class RenderHelper
{
    public static void AddOptionElement(
        RenderTreeBuilder  builder,
        string?            id,
        string             enumValue,
        params Attribute[] attributes)
    {
        int sequence = 0;
        builder.OpenElement(sequence  += 1, "option");
        builder.AddAttribute(sequence += 1, "forId", id);
        builder.AddAttribute(sequence += 1, "value", enumValue);
        builder.AddContent(sequence + 1, enumValue);
        builder.CloseElement();
    }

    public static void OpenSelectElement(
        RenderTreeBuilder        builder,
        string?                  id,
        string                   text,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int sequence          = 0;
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
        RenderTreeBuilder  builder,
        string?            forId,
        string             contentText,
        params Attribute[] attributes)
    {
        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
        contentText = displayAttribute?.Name ?? contentText;

        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "for", forId);
        builder.AddContent(2, contentText);
        builder.CloseElement();
    }

    public static void AddInputElement(
        RenderTreeBuilder        builder,
        string?                  id,
        string                   type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int sequence          = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();

        builder.OpenElement(sequence  += 1, "input");
        builder.AddAttribute(sequence += 1, "forId", id);
        builder.AddAttribute(sequence += 1, "type", type);
        if (requiredAttribute is not null) {
            builder.AddAttribute(sequence += 1, "required", true);
        }

        builder.AddAttribute(sequence, "onchange", callback);
        builder.CloseElement();
    }
}