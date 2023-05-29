using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record HorariosDeVerao
{
    [Length(15)]
    [Display(Name = "")]
    public HorarioDeVarao[]? Horarios { get; set; }
}
[PublicAPI]
public record HorarioDeVarao
{
    public DateOnly? Inicio { get; set; }
    public DateOnly? Fim { get; set; }
}