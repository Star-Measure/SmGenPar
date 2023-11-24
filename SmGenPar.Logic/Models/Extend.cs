using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMStructs;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Extend : IXElementParsable<Extend>
{
    [Display(Name = "Postos Horarios")]
    public PostosHorarios? PostosHorarios { get; set; }

    [Length(16)]
    public DateTime?[]? SelfRead { get; set; }

    [Display(Name = "Codigo do Consumidor")]
    public NumeroTrafo? NumeroTrafo { get; set; }

    public Mostradores? Mostradores { get; set; }

    [Length(82)]
    public DateOnly?[]? DataFeriados { get; set; }

    [Display(Name = "Horario de Verao")]
    [Length(15)]
    public Periodo?[]? HorarioDeVerao { get; set; }
    
    public ReposicaoDemanda? Reposicao { get; set; }

    [Display(Name = "QEE")]
    public Qee? Qee { get; set; }

    public TarifaReativos? TarifaReativos { get; set; }

    [Display(Name = "ModoII")]
    public ParametrosMedicaoIndireta? Modo2 { get; set; }
}