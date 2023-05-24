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

[PublicAPI]
public record ReposicaoDemanda
{
    public byte Janeiro   { get; set; }
    public byte Fevereiro { get; set; }
    public byte Marco     { get; set; }
    public byte Abril     { get; set; }
    public byte Maio      { get; set; }
    public byte Junho     { get; set; }
    public byte Julho     { get; set; }
    public byte Agosto    { get; set; }
    public byte Setembro  { get; set; }
    public byte Outubro   { get; set; }
    public byte Novembro  { get; set; }
    public byte Dezembro  { get; set; }
}