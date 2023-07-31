using System.Globalization;
using System.Xml.Linq;
using JetBrains.Annotations;
using SMResultTypes;

namespace SmGenPar.Logic.Parser;

[PublicAPI] public static class XParser
{
    public static CultureInfo BrazilianCultureInfo { get; } = new("pt-BR");
    
    public static Either<ParseResult, TParsable?> XParseNullable<TParsable>(XElement? xElement)
        where TParsable : struct, IParsable<TParsable>
    {
        if (xElement is { IsEmpty: false } ponta) {
            if (TParsable.TryParse(ponta.Value, null, out var value)) {
                return value;
            }
            return ParseResult.ParseError;
        }
        return ParseResult.None;
    }
    public static Either<ParseResult, TParsable> XParse<TParsable>(
        XElement? xElement,
        IFormatProvider? formatProvider)
        where TParsable : IParsable<TParsable>
    {
        if (xElement is { IsEmpty: false } ponta) {
            if (TParsable.TryParse(ponta.Value, formatProvider, out var value)) {
                return value;
            }
            return ParseResult.ParseError;
        }
        return ParseResult.None;
    }
    public static Either<ParseResult, TParsable> XParse<TParsable>(XElement? xElement)
        where TParsable : IParsable<TParsable> => XParse<TParsable>(xElement, null);
    public static Either<ParseResult, TParsable> XParseEnum<TParsable>(
        XElement? xElement)
        where TParsable : struct, Enum
    {
        if (xElement is { IsEmpty: false } ponta) {
            if (Enum.TryParse<TParsable>(ponta.Value, out var value)) {
                return value;
            }
            return ParseResult.ParseError;
        }
        return ParseResult.None;
    }

    public static Either<ParseResult, Unit> XParse<TParsable>(
        Span<XElement> xElements,
        Span<TParsable> values)
        where TParsable : IParsable<TParsable>
    {
        for (var i = 0; i < xElements.Length; ++i) {
            var c = xElements[i];
            if (TParsable.TryParse(c.Value, null, out var value)) {
                values[i] = value;
            }
            else {
                return ParseResult.ParseError;
            }
        }
        return Unit.Value;
    }
    public static Either<ParseResult, Unit> XParseNullable<TParsable>(
        Span<XElement> xElements,
        Span<TParsable?> values)
        where TParsable : struct, IParsable<TParsable>
    {
        for (var i = 0; i < xElements.Length; ++i) {
            var c = xElements[i];
            if (TParsable.TryParse(c.Value, null, out var value)) {
                values[i] = value;
            }
            else {
                return ParseResult.ParseError;
            }
        }
        return Unit.Value;
    }
}