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
public class ObjectFormRender
{
    RenderTreeBuilder _builder;

    public ObjectFormRender(RenderTreeBuilder builder)
    {
        this._builder = builder;
    }

    public void RenderModel(object model)
    {
        var modelType = model.GetType();
        foreach (var propertyInfo in modelType.GetPropertiesCached()) {
            var attributes = propertyInfo.GetCustomAttributes().ToArray();
            VisitType(propertyInfo.PropertyType, (ChangeEventArgs args) =>
            {
                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                propertyInfo.SetValue(model,
                    converter.CanConvertFrom(args.Value!.GetType())
                        ? converter.ConvertFrom(args.Value)
                        : args.Value);
            }, attributes);
        }
    }

    static void RenderInput(
        string?                  id,
        string                   type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int sequence          = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();

        //_builder.OpenElement(sequence  += 1, "input");
        //_builder.AddAttribute(sequence += 1, "id", id);
        //_builder.AddAttribute(sequence += 1, "type", type);
        if (requiredAttribute is { }) {
            //_builder.AddAttribute(sequence += 1, "required", true);
        }

        //_builder.AddAttribute(sequence += 1, "onchange", callback);
        //_builder.CloseElement();
    }

    public Action? OnVisitEnum; 
    void VisitType(
        Type                     type,
        Action<ChangeEventArgs> callback,
        params Attribute[]       attributes)
    {
        var    typeCode = Type.GetTypeCode(type);
        string name     = type.Name;

        if (Nullable.GetUnderlyingType(type) is { } underlyingType) {
            type = underlyingType;
        }

        if (type.IsEnum) {
            OnVisitEnum?.Invoke();
            //RenderHelper.OpenSelectElement(_builder, name, name, callback, attributes);
            foreach (string? enumValue in Enum.GetNames(type)) {
                //RenderHelper.AddOptionElement(_builder, null, enumValue, attributes);
            }

            //.CloseElement();
        }
        else if (type == typeof(DateTime)) {
            RenderInput(name, "datetime-local", callback, attributes);
        }
        else if (type == typeof(DateOnly)) {
            RenderInput(name, "date", callback, attributes);
        }
        else if (type == typeof(TimeOnly)) {
            RenderInput(name, "time", callback, attributes);
        }
        else if (type.IsArray) {
            RenderArray(type, callback, attributes);
        }
        else {
            switch (typeCode) {
            case >= TypeCode.SByte and <= TypeCode.Decimal:
                    RenderInput(name, "number", callback, attributes);
                break;
            case TypeCode.Boolean:
                    RenderInput(name, "checkbox", callback, attributes);
                break;
            case TypeCode.String:
                    RenderInput(name, "text", callback, attributes);
                break;
            case TypeCode.Object:
                RenderObject(type, name, name, callback, attributes);
                break;
            }
        }
    }

    void RenderArray(
        Type                     type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        IList? array = null;
        //_builder.OpenElement(0, "div");
        //_builder.AddAttribute(1, "style", "margin: 1em;");
        var elementType     = type.GetElementType() ?? typeof(object);
        var lenghtAttribute = attributes.OfType<LengthAttribute>().FirstOrDefault();

        for (int i = 0; i < lenghtAttribute?.Lenght; ++i) {
            //_builder.OpenElement(2, "div");
            int index = i;
            VisitType(elementType, args =>
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
            //_builder.CloseElement();
        }

        //_builder.CloseElement();
    }
    void RenderObject(
        Type                     type,
        string                   id,
        string                   name,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        int     sequence = 0;
        object? instance = null;
        //_builder.OpenElement(sequence  += 1, "div");
        //_builder.AddAttribute(sequence += 1, "style", "margin: 1em;");

        foreach (var propertyInfo in type.GetPropertiesCached()) {
            //_builder.OpenElement(sequence += 1, "div");
            //RenderHelper.AddLabelElement(_builder, propertyInfo.Name, propertyInfo.Name);
            var newAttributes = propertyInfo.GetCustomAttributes().ToArray();
            VisitType(propertyInfo.PropertyType, args =>
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
            //_builder.CloseElement();
        }

        //_builder.CloseElement();
    }
}