using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SMIO.Types;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Extend
{
    [Display(Name = "Data e Hora")]
    public DateTime? DataHora { get; set; }

    [Display(Name = "Postos Horarios")]
    public PostosHorarios? PostosHorarios { get; set; }

    [Display]
    [Length(12)]
    public DateOnly?[]? SelfRead { get; set; }

    [Display(Name = "Codigo do Consumidor")]
    public NumeroTrafo? NumeroTrafo { get; set; }
    
    public Mostrador Mostradores { get; set; }
    
    public Feriados? Feriados { get; set; }
    
    [Display(Name = "Horario de Verao")]
    public HorariosDeVerao? HorarioDeVerao { get; set; }
    
    public ReposicaoDemanda? Reposicao { get; set; }
    
    [Display(Name = "QEE")]
    public Qee? Qee { get; set; }
    
    public TarifaReativos? TarifaReativos { get; set; }
    
    [Display(Name = "ModoII")]
    public ParametrosMedicaoIndireta? Modo2 { get; set; }
}

public class LengthAttribute : Attribute
{
    public readonly long Lenght;
    
    public LengthAttribute(long lenght)
    {
        Lenght = lenght;
    }
}