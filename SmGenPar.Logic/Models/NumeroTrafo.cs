using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record NumeroTrafo
{
    public enum Toggle
    {
        SemAcao,
        Ativar,
        Desativar
    }
    [Display(Name = "")]
    public Toggle Acao { get; set; }
    public int Numero { get;     set; }
}