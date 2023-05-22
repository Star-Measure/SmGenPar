using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SmGenPar.Logic.Models;
using SmGenPar.Logic.Reflection;

namespace SmGenPar.Wasm.Components;

[PublicAPI]
public static class ObjectFormRender
{
    public static void RenderModel(RenderTreeBuilder builder, object model)
    {
        var modelType = model.GetType();
        foreach (var propertyInfo in modelType.GetPropertiesCached()) {
            RenderHelper.AddLabelElement(builder, propertyInfo.Name, propertyInfo.Name);
            var attributes = propertyInfo.GetCustomAttributes().ToArray();
            VisitType(builder, propertyInfo.PropertyType, args =>
            {
                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                propertyInfo.SetValue(model,
                    converter.CanConvertFrom(args.Value!.GetType())
                        ? converter.ConvertFrom(args.Value)
                        : args.Value);
            }, attributes);
            builder.AddMarkupContent(0, "<br />");
        }
    }

    static void RenderInput(
        RenderTreeBuilder        builder,
        string?                  id,
        string                   type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int sequence          = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();

        builder.OpenElement(sequence  += 1, "input");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "type", type);
        if (requiredAttribute is { }) {
            builder.AddAttribute(sequence += 1, "required", true);
        }

        builder.AddAttribute(sequence += 1, "onchange", callback);
        builder.CloseElement();
    }

    static void VisitType(
        RenderTreeBuilder       builder,
        Type                    type,
        Action<ChangeEventArgs> callback,
        params Attribute[]      attributes)
    {
        var    typeCode = Type.GetTypeCode(type);
        string name     = type.Name;

        if (Nullable.GetUnderlyingType(type) is { } underlyingType) {
            type = underlyingType;
        }

        if (type.IsEnum) {
            RenderHelper.OpenSelectElement(builder, name, name, callback, attributes);
            foreach (string? enumValue in Enum.GetNames(type)) {
                RenderHelper.AddOptionElement(builder, null, enumValue, attributes);
            }

            builder.CloseElement();
        }
        else if (type == typeof(DateTime)) {
            RenderInput(builder, name, "datetime-local", callback, attributes);
        }
        else if (type == typeof(DateOnly)) {
            RenderInput(builder, name, "date", callback, attributes);
        }
        else if (type == typeof(TimeOnly)) {
            RenderInput(builder, name, "time", callback, attributes);
        }
        else if (type.IsArray) {
            RenderArray(builder, type, callback, attributes);
        }
        else {
            switch (typeCode) {
            case >= TypeCode.SByte and <= TypeCode.Decimal:
                RenderInput(builder, name, "number", callback, attributes);
                break;
            case TypeCode.Boolean:
                RenderInput(builder, name, "checkbox", callback, attributes);
                break;
            case TypeCode.String:
                RenderInput(builder, name, "text", callback, attributes);
                break;
            case TypeCode.Object:
                RenderObject(builder, type, name, name, callback, attributes);
                break;
            }
        }
    }

    static void RenderArray(
        RenderTreeBuilder        builder,
        Type                     type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        IList? array = null;
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "style", "margin: 1em;");
        var elementType     = type.GetElementType() ?? typeof(object);
        var lenghtAttribute = attributes.OfType<LengthAttribute>().FirstOrDefault();

        for (int i = 0; i < lenghtAttribute?.Lenght; ++i) {
            builder.OpenElement(2, "div");
            int index = i;
            VisitType(builder, elementType, args =>
            {
                array ??= Array.CreateInstance(elementType, lenghtAttribute.Lenght);
                var converter = TypeDescriptor.GetConverter(elementType);

                object? argValue = args.Value;
                var     argType  = argValue?.GetType()!;

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

    static void RenderObject(
        RenderTreeBuilder        builder,
        Type                     type,
        string                   id,
        string                   name,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int     sequence = 0;
        object? instance = null;
        builder.OpenElement(sequence  += 1, "div");
        builder.AddAttribute(sequence += 1, "style", "margin: 1em;");

        foreach (var propertyInfo in type.GetPropertiesCached()) {
            builder.OpenElement(sequence += 1, "div");
            RenderHelper.AddLabelElement(builder, propertyInfo.Name, propertyInfo.Name);
            var newAttributes = propertyInfo.GetCustomAttributes().ToArray();
            VisitType(builder, propertyInfo.PropertyType, args =>
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
}