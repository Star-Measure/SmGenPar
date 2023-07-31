using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SmGenPar.Logic.Models;
using SmGenPar.Logic.Cache;

namespace SmGenPar.Wasm.Components;

public delegate void FormsRenderer(
    RenderTreeBuilder       builder,
    string                  id,
    string                  name,
    Action<ChangeEventArgs> callback,
    params Attribute[]      attributes);

[PublicAPI]
public class FormRender
{
    public readonly static Dictionary<string, object> CachedModels = new()
    {
        { "Extend", new Extend() },
    };
    public readonly Dictionary<string, object>      ElementValue              = new();
    readonly        Dictionary<Type, FormsRenderer> _customTypeRenderFragment = new();
    public RenderFragment RenderModel(object model) => builder =>
    {
        var modelType = model.GetType();

        foreach (var propertyInfo in modelType.GetPropertiesCached()) {
            builder.AddLabelElement(propertyInfo.Name, propertyInfo.Name);

            var attributes = propertyInfo.GetCustomAttributes().ToArray();
            RenderElementForType(builder, propertyInfo.Name, propertyInfo.PropertyType, args =>
            {
                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                var value = converter.CanConvertFrom(args.Value!.GetType())
                    ? converter.ConvertFrom(args.Value)
                    : args.Value;

                propertyInfo.SetValue(model, value);
            }, attributes);
        }
    };
    public Dictionary<string, RenderFragment>? RenderModelProperties(object model)
    {
        var modelType = model.GetType();

        var props = modelType.GetPropertiesCached();

        var span = props.AsSpan();

        var fragments = new Dictionary<string, RenderFragment>(span.Length);

        foreach (var propertyInfo in span) {
            var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var fragment = new RenderFragment(builder =>
            {
                builder.AddLabelElement(propertyInfo.Name, propertyInfo.Name);
                var attributes = propertyInfo.GetCustomAttributes().ToArray();
                RenderElementForType(
                    builder,
                    $"{modelType.Name}.{propertyInfo.Name}",
                    propertyInfo.PropertyType,
                    args =>
                    {
                        var converter = TypeDescriptor.GetConverter(
                            propertyInfo.PropertyType
                        );

                        var value = converter.CanConvertFrom(args.Value!.GetType())
                            ? converter.ConvertFrom(args.Value)
                            : args.Value;

                        propertyInfo.SetValue(model, value);
                    }, attributes);
            });
            var name = displayAttribute?.Name ?? propertyInfo.Name;
            fragments.Add(propertyInfo.Name, fragment);
        }

        return fragments;
    }

    public void AddRenderForType<TType>(FormsRenderer fragment)
    {
        _customTypeRenderFragment.Add(typeof(TType), fragment);
    }

    void OpenInputElement(
        RenderTreeBuilder        builder,
        string?                  id,
        string                   type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var sequence = 0;
        var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();

        builder.OpenElement(sequence  += 1, "input");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "type", type);
        builder.AddAttribute(sequence += 1, "class", "form-control");
        builder.AddAttribute(sequence += 1, "value", ElementValue.GetValueOrDefault(id!));

        if (requiredAttribute is not null) {
            builder.AddAttribute(sequence += 1, "required", true);
        }

        builder.AddAttribute(sequence + 1, "onchange", (ChangeEventArgs args) =>
        {
            ElementValue[id!] = args.Value as string ?? $"{args.Value}";
            callback?.Invoke(args);
            return callback;
        });
    }

    void OpenCheckBoxElement(
        RenderTreeBuilder        builder,
        string?                  id,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var sequence = 0;

        builder.OpenElement(sequence  += 1, "input");
        builder.AddAttribute(sequence += 1, "id", id);
        builder.AddAttribute(sequence += 1, "type", "checkbox");

        builder.AddAttribute(sequence + 1, "onchange", (ChangeEventArgs args) =>
        {
            ElementValue[id!] = args.Value as string ?? $"{args.Value}";
            callback?.Invoke(args);
            return callback;
        });
    }

    public void RenderElementForType(
        RenderTreeBuilder       builder,
        string                  id,
        Type                    type,
        Action<ChangeEventArgs> callback,
        params Attribute[]      attributes)

    {
        if (Nullable.GetUnderlyingType(type) is {} underlyingType) {
            type = underlyingType;
        }
        var typeCode = Type.GetTypeCode(type);
        var name = type.Name;
        if (_customTypeRenderFragment.TryGetValue(type, out var customRenderer)) {
            customRenderer(builder, id, name, callback, attributes);
        }
        else if (type.IsEnum) {
            RenderEnum(builder, id, name, type, callback, attributes);
        }
        else if (type.IsArray) {
            RenderArray(builder, id, type, callback, attributes);
        }
        else if (type == typeof(DateTime)) {
            OpenInputElement(builder, id, "datetime-local", callback, attributes);
            builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
            builder.CloseElement();
        }
        else if (type == typeof(DateOnly)) {
            OpenInputElement(builder, id, "date", callback, attributes);
            builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
            builder.CloseElement();
        }
        else if (type == typeof(TimeOnly)) {
            OpenInputElement(builder, id, "time", callback, attributes);
            builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
            builder.CloseElement();
        }
        else {
            switch (typeCode) {
            case >= TypeCode.SByte and <= TypeCode.UInt64:
                OpenInputElement(builder, id, "number", callback, attributes);
                builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
                builder.CloseElement();
                break;
            case >= TypeCode.Single and <= TypeCode.Decimal:
                OpenInputElement(builder, id, "number", callback, attributes);
                builder.AddAttribute(09, "step", "any");
                builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
                builder.CloseElement();
                break;
            case TypeCode.Boolean:
                OpenCheckBoxElement(builder, id, callback, attributes);
                builder.CloseElement();
                break;
            case TypeCode.String:
                OpenInputElement(builder, id, "text", callback, attributes);
                builder.AddAttribute(10, "value", ElementValue.GetValueOrDefault(id));
                builder.CloseElement();

                break;

            case TypeCode.Object:
                RenderObject(builder, type, id, name, callback, attributes);

                break;
            }
        }
    }

    void RenderEnum(
        RenderTreeBuilder        builder,
        string                   id,
        string                   name,
        Type                     type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var flagsAttribute = type.GetCustomAttribute<FlagsAttribute>();

        builder.OpenSelectElement(id, name, callback, attributes);

        foreach (var enumName in Enum.GetNames(type)) {
            builder.AddOptionElement($"{id}.{enumName}", enumName, attributes);
        }

        builder.CloseElement();
    }

    void RenderArray(
        RenderTreeBuilder        builder,
        string                   id,
        Type                     type,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var array = ElementValue.GetValueOrDefault(id) as IList;
        var elementType = type.GetElementType() ?? typeof(object);
        var lenghtAttribute = attributes.OfType<LengthAttribute>().FirstOrDefault();

        for (var i = 0; i < lenghtAttribute?.Lenght; ++i) {
            var index = i;
            RenderElementForType(builder, $"{id}[{i}]", elementType, args =>
            {
                Array ArrayFactory() => Array.CreateInstance(elementType, lenghtAttribute.Lenght);
                array ??= (IList)ElementValue.GetValueRefOrCreate(id, ArrayFactory)!;
                var converter = TypeDescriptor.GetConverter(elementType);

                var argValue = args.Value;
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

                callback?.Invoke(new ChangeEventArgs
                {
                    Value = array
                });
            });
        }
    }

    void RenderObject(
        RenderTreeBuilder        builder,
        Type                     type,
        string                   id,
        string                   name,
        Action<ChangeEventArgs>? callback = null,
        params Attribute[]       attributes)
    {
        var instance = ElementValue.GetValueOrDefault(id);
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "form-control");
        foreach (var propertyInfo in type.GetPropertiesCached()) {
            var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.Name ?? propertyInfo.Name;
            builder.OpenElement(0, "div");
            builder.AddLabelElement(propertyInfo.Name, displayName);
            var newAttributes = propertyInfo.GetCustomAttributes().ToArray();
            RenderElementForType(
                builder,
                $"{id}.{propertyInfo.Name}",
                propertyInfo.PropertyType,
                args =>
                {
                    object? ObjectFactory() => Activator.CreateInstance(type);
                    instance ??= ElementValue!.GetValueRefOrCreate(id, ObjectFactory);
                    var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                    var argValue = args.Value!;
                    var argType = argValue!.GetType();
                    if (converter.CanConvertFrom(argType)) {
                        try {
                            var convertedValue = converter.ConvertFromInvariantString((string)argValue);
                            propertyInfo.SetValue(instance, convertedValue);
                        }
                        catch (FormatException) {
                            return;
                        }
                    }
                    else {
                        propertyInfo.SetValue(instance, args.Value);
                    }
                    callback?.Invoke(new ChangeEventArgs
                    {
                        Value = instance
                    });
                }, newAttributes);
            builder.CloseElement();
        }
        builder.CloseElement();
    }
}