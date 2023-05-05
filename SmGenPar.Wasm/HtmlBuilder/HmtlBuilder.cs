using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.Rendering;

namespace SmGenPar.Wasm.HtmlBuilder;

public static class HtmlBuilder
{
    public static Element OpenElement(this RenderTreeBuilder builder, string name, int sequence = 0)
    {
        return new Element(sequence, name, builder);
    }

    public static Element AddElement(this Element element, string name)
    {
        element.Sequence += 1;
        return new Element(element.Sequence, name, element.Builder);
    }

    public static void AddAttribute(this Element element, string name, object? value)
    {
        element.Sequence += 1;
        element.Builder.AddAttribute(element.Sequence, name, value);
    }

    public static void AddContent(this Element element, string content)
    {
        element.Sequence += 1;
        element.Builder.AddContent(element.Sequence, content);
    }

    [PublicAPI] public static readonly Dictionary<Type, string[]> CachedEnumNames = new();

    public static void AddInputType(this Element element, Type type)
    {
        element.Builder.AddAttribute(element.Sequence, "type", MapInputType(type));
    }
    static string MapInputType(Type type) => Type.GetTypeCode(type) switch
    {
        _ when type.IsEnum                        => "select",
        >= TypeCode.SByte and <= TypeCode.Decimal => "number",
        TypeCode.String                           => "text",
        TypeCode.DateTime                         => "datetime-local",
        TypeCode.Boolean                          => "checkbox",
        TypeCode.Object => type.Name switch
        {
            "DateTimeOffset" => "datetime-local",
            "TimeOnly"       => "time",
            "DateOnly"       => "date",
            _                => "text"
        },
        _ => "text"
    };
}

[PublicAPI]
public record Element : IDisposable
{
    internal          int               Sequence;
    internal          string            Name;
    internal readonly RenderTreeBuilder Builder;

    internal Element(int sequence, string name, RenderTreeBuilder builder)
    {
        Sequence = sequence;
        Name     = name;
        Builder  = builder;
        builder.OpenElement(sequence, name);
    }

    public void Dispose()
    {
        Sequence = 0;
        Builder.CloseElement();
    }
}