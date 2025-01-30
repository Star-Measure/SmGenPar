using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMResultTypes;
using System.Xml.Linq;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record ReposicaoDemanda : IXElementParsable<ReposicaoDemanda>
{
    public byte Janeiro { get; set; }
    public byte Fevereiro { get; set; }
    public byte Marco { get; set; }
    public byte Abril { get; set; }
    public byte Maio { get; set; }
    public byte Junho { get; set; }
    public byte Julho { get; set; }
    public byte Agosto { get; set; }
    public byte Setembro { get; set; }
    public byte Outubro { get; set; }
    public byte Novembro { get; set; }
    public byte Dezembro { get; set; }

    public byte this[int index]
    {
        set
        {
            // @formatter:off
            switch (index)
            {
                case 0: Janeiro = value; break;
                case 1: Fevereiro = value; break;
                case 2: Marco = value; break;
                case 3: Abril = value; break;
                case 4: Maio = value; break;
                case 5: Junho = value; break;
                case 6: Julho = value; break;
                case 7: Agosto = value; break;
                case 8: Setembro = value; break;
                case 9: Outubro = value; break;
                case 10: Novembro = value; break;
                case 11: Dezembro = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
            // @formatter:on
        }
    }

    public static Either<ParseResult, ReposicaoDemanda> FromXElement(XElement? element)
    {
        static Option<byte> ParseByte(string s) => byte.TryParse(s, out var b) ? Option.Some(b) : Option.None;

        var xMonths = element?
            .Elements()
            .Select(e => e.Value)
            .Select(ParseByte)
            .ToArray();

        if (xMonths is null)
        {
            return ParseResult.ParseError;
        }

        var reposicaoDemanda = new ReposicaoDemanda();
        for (var i = 0; i < xMonths.Length; i++)
        {
            reposicaoDemanda[i] = xMonths[i].GetValueOrDefault();
        }
        return reposicaoDemanda;
    }
}