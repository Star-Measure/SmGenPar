using System.ComponentModel.DataAnnotations;
using System.Numerics;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Extend
{
    [Display(Name = "Data e Hora")]
    public DateTime DataHora { get; set; }

    [Display(Name = "Postos Horarios")]
    public PostosHorarios? PostosHorarios { get; set; }

    [Display]
    [Length(12)]
    public DateOnly[]? SelfRead { get; set; }

    [Display(Name = "Codigo do Consumidor")]
    public NumeroTrafo? NumeroTrafo { get; set; }
    
    public MostradoresFlag Mostradores { get; set; }
    
    [Display(Name = "QEE")]
    public Qee? Qee { get; set; }
}

public class LengthAttribute : Attribute
{
    public readonly long Lenght;
    
    public LengthAttribute(long lenght)
    {
        Lenght = lenght;
    }
}