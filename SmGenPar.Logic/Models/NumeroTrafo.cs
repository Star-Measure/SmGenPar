using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record NumeroTrafo : IXElementParsable<NumeroTrafo>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Toggle
    {
        SemAcao,
        Ativar,
        Desativar
    }
    [Display(Name = "")]
    public Toggle Acao { get; set; }
    public int Numero { get;  set; }

    public static Either<ParseResult, NumeroTrafo> FromXElement(XElement? element)
    {
        var xAcao = element?.Element("Acao");
        var xNumero = element?.Element("Numero");
        
        var acao = XParser.XParseEnum<Toggle>(xAcao);
        var numero = XParser.XParse<int>(xNumero);

        var numeroTrafo = new NumeroTrafo {
            Acao   = acao.GetValueOrDefault(),
            Numero = numero.GetValueOrDefault()
        };

        return numeroTrafo;
    }
}