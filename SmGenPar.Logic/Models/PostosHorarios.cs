using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record PostosHorarios
{
    public Posto? Domingo { get; set; }
    public Posto? Segunda { get; set; }
    public Posto? Terca   { get; set; }
    public Posto? Quarta  { get; set; }
    public Posto? Quinta  { get; set; }
    public Posto? Sexta   { get; set; }
    public Posto? Sabado  { get; set; }
    public Posto? Feriado { get; set; }
}

[PublicAPI]
public record Posto
{
    public TimeOnly? Ponta { get; set; }

    [Display(Name = "Fora Ponta")]
    public TimeOnly? ForaPonta { get; set; }

    public TimeOnly? Reservado { get; set; }

    public TimeOnly? Feriado { get; set; }
}