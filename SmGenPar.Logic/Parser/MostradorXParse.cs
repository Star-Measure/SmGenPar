using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using SMIO.ValueTypes;
using SMResultTypes;

namespace SmGenPar.Logic.Parser;

public static class MostradorXParse
{
    public static Either<ParseResult, Mostradores> FromXElement(XElement? xElement)
    {
        var xMostrador = xElement?.Element("Codigo");

        if (BigInteger.TryParse(xMostrador?.Value[2..], NumberStyles.HexNumber, null, out var bitField)) {
            return new Mostradores(bitField);
        }

        return ParseResult.None;
    }
}