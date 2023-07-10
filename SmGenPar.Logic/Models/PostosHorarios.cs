using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMIO.Buffers.AB;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record PostosHorarios : IXElementParsable<PostosHorarios>
{
    public DateOnly? DataDeInicio { get; set; }
    public Posto?    Domingo      { get; set; }
    public Posto?    Segunda      { get; set; }
    public Posto?    Terca        { get; set; }
    public Posto?    Quarta       { get; set; }
    public Posto?    Quinta       { get; set; }
    public Posto?    Sexta        { get; set; }
    public Posto?    Sabado       { get; set; }
    public Posto?    Feriado      { get; set; }

    public static Either<ParseResult, PostosHorarios> FromXElement(XElement? element)
    {
        var xDomingo = element?.Element("Domingo");
        var xSegunda = element?.Element("Segunda");
        var xTerca = element?.Element("Terca");
        var xQuarta = element?.Element("Quarta");
        var xQuinta = element?.Element("Quinta");
        var xSexta = element?.Element("Sexta");
        var xSabado = element?.Element("Sabado");
        var xFeriado = element?.Element("Feriado");

        var domingo = Posto.FromXElement(xDomingo);
        var segunda = Posto.FromXElement(xSegunda);
        var terca = Posto.FromXElement(xTerca);
        var quarta = Posto.FromXElement(xQuarta);
        var quinta = Posto.FromXElement(xQuinta);
        var sexta = Posto.FromXElement(xSexta);
        var sabado = Posto.FromXElement(xSabado);
        var feriado = Posto.FromXElement(xFeriado);

        var postosHorarios = new PostosHorarios {
            Domingo = domingo.GetValueOrDefault(),
            Segunda = segunda.GetValueOrDefault(),
            Terca   = terca.GetValueOrDefault(),
            Quarta  = quarta.GetValueOrDefault(),
            Quinta  = quinta.GetValueOrDefault(),
            Sexta   = sexta.GetValueOrDefault(),
            Sabado  = sabado.GetValueOrDefault(),
            Feriado = feriado.GetValueOrDefault()
        };

        return postosHorarios;
    }
}
[PublicAPI]
public record Posto : IXElementParsable<Posto>
{
    [Length(4)] public TimeOnly?[]? Ponta { get; set; }

    [Display(Name = "Fora Ponta")]
    [Length(4)] public TimeOnly?[]? ForaPonta { get; set; }

    [Length(4)] public TimeOnly?[]? Reservado { get; set; }

    [Length(4)] public TimeOnly?[]? Intermediario { get; set; }

    public static Either<ParseResult, Posto> FromXElement(XElement? element)
    {
        var xPonta = element?.Elements("Ponta").ToArray();
        var xForaPonta = element?.Elements("ForaPonta").ToArray();
        var xReservado = element?.Elements("Reservado").ToArray();
        var xIntermediario = element?.Elements("Intermediario").ToArray();

        var arrPonta = new TimeOnly?[4];
        var arrForaPonta = new TimeOnly?[4];
        var arrReservado = new TimeOnly?[4];
        var arrIntermediario = new TimeOnly?[4];

        XParser.XParseNullable<TimeOnly>(xPonta, arrPonta);
        XParser.XParseNullable<TimeOnly>(xForaPonta, arrForaPonta);
        XParser.XParseNullable<TimeOnly>(xReservado, arrReservado);
        XParser.XParseNullable<TimeOnly>(xIntermediario, arrIntermediario);

        var posto = new Posto {
            Ponta         = arrPonta,
            ForaPonta     = arrForaPonta,
            Reservado     = arrReservado,
            Intermediario = arrIntermediario
        };

        return posto;
    }
}