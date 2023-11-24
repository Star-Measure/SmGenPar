using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMResultTypes;
using SMStructs.Enums;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record ParametrosMedicaoIndireta : IXElementParsable<ParametrosMedicaoIndireta>
{
    public TipoSaidaUsuario SaidaDeUsuario { get; set; }
    
    [Length(2)]
    public int?[]? Kp { get; set; }

    public int NmrCasasDecimaisEnergia { get; set; }
    public int NmrCasasDecimaisDemanda { get; set; }

    public static Either<ParseResult, ParametrosMedicaoIndireta> FromXElement(XElement? element)
    {
        var xKp = element?.Elements("Kp").ToArray();
        var xSaidaDeUsuario = element?.Element("SaidaDeUsuario");
        var xNmrCasasDecimaisEnergia = element?.Element("NmrCasasDecimaisEnergia");
        var xNmrCasasDecimaisDemanda = element?.Element("NmrCasasDecimaisDemanda");

        var kp = xKp?.Length > 0 ? new int?[xKp.Length] : Array.Empty<int?>();

        XParser.XParseNullable(xKp.AsSpan(), kp.AsSpan());
        var saidaDeUsuario = XParser.XParseEnum<TipoSaidaUsuario>(xSaidaDeUsuario);

        var nmrCasasDecimaisEnergia = XParser.XParse<int>(xNmrCasasDecimaisEnergia);
        var nmrCasasDecimaisDemanda = XParser.XParse<int>(xNmrCasasDecimaisDemanda);
        
        var parametrosMedicaoIndireta = new ParametrosMedicaoIndireta {
            Kp                      = kp,
            SaidaDeUsuario          = saidaDeUsuario.GetValueOrDefault(),
            NmrCasasDecimaisEnergia = nmrCasasDecimaisEnergia.GetValueOrDefault(),
            NmrCasasDecimaisDemanda = nmrCasasDecimaisDemanda.GetValueOrDefault()
        };
        
        return parametrosMedicaoIndireta;
    }
}