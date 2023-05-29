using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record Feriados
{
    [Display(Name = "")]
    [Length(72)]
    public DateOnly?[]? DataFeriados { get; set; }
}