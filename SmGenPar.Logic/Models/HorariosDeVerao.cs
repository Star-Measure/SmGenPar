using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;


[PublicAPI]
public record HorariosDeVerao
{
    [Length(15)]
    [Display(Name = "")]
    public HorariosDeVarao[]? Periodo { get; set; }
}

[PublicAPI]
public record HorariosDeVarao
{
    public DateOnly? Inicio { get; set; }
    public DateOnly? Fim { get; set; }
}