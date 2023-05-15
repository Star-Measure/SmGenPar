using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Extend
{
    [Display(Name = "Data e Hora")]
    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    [Display(Name = "Postos Horarios")]
    public PostosHorarios PostosHorarios { get; set; } = new();

    [Display]
    public SelfRead SelfRead { get; set; } = new();

    [Display(Name = "Codigo do Consumidor")]
    public NumeroTrafo NumeroTrafo { get; set; } = new();

    [Display(Name = "QEE")]
    public Qee Qee { get; set; } = new();
}

[PublicAPI]
public class NumeroTrafo
{
    public enum AcaoTrafo
    {
        SemAcao,
        Ativar,
        Desativar
    }
    [Display(Name = "")]
    public AcaoTrafo Acao { get; set; }
    public int Numero { get; set; }
}

