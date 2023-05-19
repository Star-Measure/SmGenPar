using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SmGenPar.Logic.Models;
using SmGenPar.Logic.Reflection;

namespace SmGenPar.Wasm.Components;

[PublicAPI]
public static class RenderHelper
{
    public static void AddOptionElement(
        RenderTreeBuilder builder,
        string? id,
        string enumValue,
        params Attribute[] attributes)
    {
        int sequence = 0;
        builder.OpenElement(sequence += 1, "option");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "value", enumValue);
        builder.AddContent(sequence += 1, enumValue);
        builder.CloseElement();
    }

    public static void OpenSelectElement(
        RenderTreeBuilder builder,
        string? id,
        string text,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[] attributes)
    {
        int sequence = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
        builder.OpenElement(sequence += 1, "select");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "onchange", callback);
        builder.AddAttribute(sequence += 1, "value", text);
        if (requiredAttribute is { }) {
            builder.AddAttribute(sequence += 1, "required", true);
        }
    }

    public static void AddLabelElement(
        RenderTreeBuilder builder,
        string? id,
        string text,
        params Attribute[] attributes)
    {
        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
        text = displayAttribute?.Name ?? text;

        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "for", id);
        builder.AddContent(2, text);
        builder.CloseElement();
    }

    public static void AddInputElement(
        RenderTreeBuilder builder,
        string? id,
        string type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[] attributes)
    {
        int sequence = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();

        builder.OpenElement(sequence += 1, "input");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "type", type);
        if (requiredAttribute is {}) {
            builder.AddAttribute(sequence += 1, "required", true);
        }
        builder.AddAttribute(sequence += 1, "onchange", callback);
        builder.CloseElement();
    }
    
    public static void AddInput(
        RenderTreeBuilder builder,
        Type type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[] attributes)
    {
        var typeCode = Type.GetTypeCode(type);
        string name = type.Name;

        if (Nullable.GetUnderlyingType(type) is { } underlyingType) {
            type = underlyingType;
        }

        if (type.IsEnum) {
            OpenSelectElement(builder, name, name, callback, attributes);
            foreach (string? enumValue in Enum.GetNames(type)) {
                AddOptionElement(builder, null, enumValue, attributes);
            }
            builder.CloseElement();
        }
        else if (type == typeof(DateTime)) {
            AddInputElement(builder, name, "datetime-local", callback, attributes);
        }
        else if (type == typeof(DateOnly)) {
            AddInputElement(builder, name, "date", callback, attributes);
        }
        else if (type == typeof(TimeOnly)) {
            AddInputElement(builder, name, "time", callback, attributes);
        }
        else if (type.IsArray) {
            RenderArray(builder, type, callback, attributes);
        }
        else {
            switch (typeCode) {
            case >= TypeCode.SByte and <= TypeCode.Decimal:
                AddInputElement(builder, name, "number", callback, attributes);
                break;
            case TypeCode.Boolean:
                AddInputElement(builder, name, "checkbox", callback, attributes);
                break;
            case TypeCode.String:
                AddInputElement(builder, name, "text", callback, attributes);
                break;
            case TypeCode.Object:
                RenderObject(builder, type, name, name, callback, attributes);
                break;
            }
        }
    }

    public static void RenderArray(
        RenderTreeBuilder builder,
        Type type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[] attributes)
    {
        IList? array = null;
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "style", "margin: 1em;");
        var elementType = type.GetElementType() ?? typeof(object);
        var lenghtAttribute = attributes.OfType<LengthAttribute>().FirstOrDefault();

        for (int i = 0; i < lenghtAttribute?.Lenght; ++i) {
            builder.OpenElement(2, "div");
            int index = i;
            AddInput(builder, elementType, args =>
            {
                array ??= Array.CreateInstance(elementType, lenghtAttribute.Lenght);
                var converter = TypeDescriptor.GetConverter(elementType);

                object? argValue = args.Value;
                var argType = argValue?.GetType()!;

                if (converter.CanConvertFrom(argType)) {
                    try {
                        array[index] = converter.ConvertFrom(argValue!);
                    }
                    catch (FormatException) {
                        return;
                    }
                }
                else {
                    array[index] = args.Value!;
                }
                callback?.Invoke(new ChangeEventArgs { Value = array });
            });
            builder.CloseElement();
        }
        builder.CloseElement();
    }

    public static void RenderObject(
        RenderTreeBuilder builder,
        Type type,
        string id,
        string name,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[] attributes)
    {
        int sequence = 0;
        object? instance = null;
        builder.OpenElement(sequence += 1, "div");
        builder.AddAttribute(sequence += 1, "style", "margin: 1em;");

        foreach (var propertyInfo in type.GetPropertiesCached()) {
            builder.OpenElement(sequence += 1, "div");
            AddLabelElement(builder, propertyInfo.Name, propertyInfo.Name);
            var newAttributes = propertyInfo.GetCustomAttributes().ToArray();
            AddInput(builder, propertyInfo.PropertyType, args =>
            {
                instance ??= Activator.CreateInstance(type)!;
                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                object? argValue = args.Value!;

                if (converter.CanConvertFrom(argValue!.GetType())) {
                    try {
                        propertyInfo.SetValue(instance, converter.ConvertFrom(argValue));
                    }
                    catch (FormatException) {
                        return;
                    }
                }
                else {
                    propertyInfo.SetValue(instance, args.Value);
                }
                callback?.Invoke(new ChangeEventArgs { Value = instance });
            }, newAttributes);
            builder.CloseElement();
        }
        builder.CloseElement();
}