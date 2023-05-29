using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record NumeroTrafo
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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