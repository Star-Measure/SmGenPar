using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public sealed record TarifaReativos
{
    public DateTime DataVigencia { get; set; }

    [Length(2)]
    public TimeOnly?[]? Indutivo { get; set; }

    [Length(2)]
    public TimeOnly?[]? Capacitivo { get; set; }

    public TarifaReativosFlag DiasUteis { get; set; }
    public TarifaReativosFlag Sabados   { get; set; }
    public TarifaReativosFlag Domingos  { get; set; }
    public TarifaReativosFlag Feriados  { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TarifaReativosFlag
    {
        Indefinido,
        IndutivoNoHorario,
        CapacitivoNoHorario,
        IndutivoECapacitivoNoHorario,
        Nenhum,
        IndutivoDiaTodo,
        CapacitivoDiaTodo,
        IndutivoECapacitivoDiaTodo
    }
    [Display(Name = "FP de Referencia %")]
    public float FpDeReferencia { get; set; }
}