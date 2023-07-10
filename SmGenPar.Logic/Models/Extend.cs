using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMIO.ValueTypes;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Extend : IXElementParsable<Extend>
{
    [Display(Name = "Postos Horarios")]
    public PostosHorarios? PostosHorarios { get; set; }

    [Length(12)]
    public DateOnly?[]? SelfRead { get; set; }

    [Display(Name = "Codigo do Consumidor")]
    public NumeroTrafo? NumeroTrafo { get; set; }

    public Mostrador? Mostradores { get; set; }

    [Length(72)]
    public DateOnly?[]? DataFeriados { get; set; }

    [Display(Name = "Horario de Verao")]
    public HorariosDeVerao? HorarioDeVerao { get; set; }

    public ReposicaoDemanda? Reposicao { get; set; }

    [Display(Name = "QEE")]
    public Qee? Qee { get; set; }

    public TarifaReativos? TarifaReativos { get; set; }

    [Display(Name = "ModoII")]
    public ParametrosMedicaoIndireta? Modo2 { get; set; }

    public static Either<ParseResult, Extend> FromXElement(XElement? element)
    {
        var xPostosHorarios = element?.Element("PostosHorarios");
        var xSelfRead = element?.Elements("SelfRead").ToArray();
        var xNumeroTrafo = element?.Element("NumeroTrafo");
        var xMostradores = element?.Element("Mostradores");
        var xDataFeriados = element?.Elements("DataFeriados");
        var xHorarioDeVerao = element?.Element("HorarioDeVerao");
        var xReposicao = element?.Element("Reposicao");
        var xQee = element?.Element("Qee");
        var xTarifaReativos = element?.Element("TarifaReativos");
        var xModo2 = element?.Element("Modo2");

        var arrSelfRead = xSelfRead?
            .Select(XParser.XParseNullable<DateOnly>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .ToArray();
        
        var arrFeriados = xDataFeriados?
            .Select(XParser.XParseNullable<DateOnly>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .ToArray();
        
        var postosHorarios = PostosHorarios.FromXElement(xPostosHorarios);
        var numeroTrafo = NumeroTrafo.FromXElement(xNumeroTrafo);
        var mostradores = MostradorXParse.FromXElement(xMostradores);
        var horarioDeVerao = HorariosDeVerao.FromXElement(xHorarioDeVerao);
        var reposicao = ReposicaoDemanda.FromXElement(xReposicao);
        var qee = Qee.FromXElement(xQee);
        var tarifaReativos = TarifaReativos.FromXElement(xTarifaReativos);
        var modo2 = ParametrosMedicaoIndireta.FromXElement(xModo2);

        var extend = new Extend {
            PostosHorarios = postosHorarios.GetValueOrDefault(),
            SelfRead       = arrSelfRead,
            NumeroTrafo    = numeroTrafo.GetValueOrDefault(),
            Mostradores    = mostradores.GetValueOrDefault(),
            DataFeriados   = arrFeriados,
            HorarioDeVerao = horarioDeVerao.GetValueOrDefault(),
            Reposicao      = reposicao.GetValueOrDefault(),
            Qee            = qee.GetValueOrDefault(),
            TarifaReativos = tarifaReativos.GetValueOrDefault(),
            Modo2          = modo2.GetValueOrDefault()
        };
        return extend;
    }
}