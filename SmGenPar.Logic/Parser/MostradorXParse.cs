using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using SmGenPar.Logic.Parser;
using SMIO.ValueTypes;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

public static class MostradorXParse
{
    public static Either<ParseResult, Mostrador> FromXElement(XElement? xElement)
    {
        var xMostrador = xElement?.Element("Codigo");

        if (BigInteger.TryParse(xMostrador?.Value[2..], NumberStyles.HexNumber, null, out var bitField)) {
            return new Mostrador(bitField);
        }

        return ParseResult.None;
    }
}