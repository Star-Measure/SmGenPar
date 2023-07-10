using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using JetBrains.Annotations;
using Org.BouncyCastle.Asn1.Cms;
using SmGenPar.Logic.Parser;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public sealed record TarifaReativos : IXElementParsable<TarifaReativos>
{
    public DateTime DataVigencia { get; set; }

    [Length(2)]
    public TimeOnly?[]? Indutivo { get; set; }

    [Length(2)]
    public TimeOnly?[]? Capacitivo { get; set; }

    public TarifaReativosFlag DiasUteis { get; set; }
    public TarifaReativosFlag Sabados   { get; set; }
    public TarifaReativosFlag Domingos  { get; set; }
    public TarifaReativosFlag Feriados  { get; set; }
    [Display(Name = "FP de Referencia %")]
    public float FpDeReferencia { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TarifaReativosFlag
    {
        Indefinido,
        IndutivoNoHorario,
        CapacitivoNoHorario,
        IndutivoECapacitivoNoHorario,
        Nenhum,
        IndutivoDiaTodo,
        CapacitivoDiaTodo,
        IndutivoECapacitivoDiaTodo
    }

    public static Either<ParseResult, TarifaReativos> FromXElement(XElement? element)
    {
        var xDataVigencia = element?.Element("DataVigencia");
        var xIndutivo = element?.Elements("Indutivo").ToArray();
        var xCapacitivo = element?.Elements("Capacitivo").ToArray();
        var xDiasUteis = element?.Element("DiasUteis");
        var xSabados = element?.Element("Sabados");
        var xDomingos = element?.Element("Domingos");
        var xFeriados = element?.Element("Feriados");
        var xFpDeReferencia = element?.Element("FpDeReferencia");

        TimeOnly?[] arrIndutivos = new TimeOnly?[xIndutivo?.Length      ?? 0];
        TimeOnly?[] spanCapacitivos = new TimeOnly?[xCapacitivo?.Length ?? 0];
        XParser.XParseNullable(xIndutivo, arrIndutivos.AsSpan());
        XParser.XParseNullable(xCapacitivo, spanCapacitivos.AsSpan());

        var dataVigencia = XParser.XParse<DateTime>(xDataVigencia);
        var diasUteis = XParser.XParseEnum<TarifaReativosFlag>(xDiasUteis);
        var sabados = XParser.XParseEnum<TarifaReativosFlag>(xSabados);
        var domingos = XParser.XParseEnum<TarifaReativosFlag>(xDomingos);
        var feriados = XParser.XParseEnum<TarifaReativosFlag>(xFeriados);
        var fpDeReferencia = XParser.XParse<float>(xFpDeReferencia);

        var tarifaReativos = new TarifaReativos {
            DataVigencia   = dataVigencia.GetValueOrDefault(),
            Indutivo       = arrIndutivos,
            Capacitivo     = spanCapacitivos,
            DiasUteis      = diasUteis.GetValueOrDefault(),
            Sabados        = sabados.GetValueOrDefault(),
            Domingos       = domingos.GetValueOrDefault(),
            Feriados       = feriados.GetValueOrDefault(),
            FpDeReferencia = fpDeReferencia.GetValueOrDefault()
        };

        return tarifaReativos;
    }
}