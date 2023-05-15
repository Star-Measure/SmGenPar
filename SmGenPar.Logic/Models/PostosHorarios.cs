using System.Collections;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class PostosHorarios
{
    [DataType(DataType.Time)]
    public string Ponta { get; set; }

    [Display(Name = "Fora Ponta")]
    [DataType(DataType.Time)]
    public string ForaPonta { get; set; }
    
    [DataType(DataType.Time)]
    public string Reservado { get; set; }

    [DataType(DataType.Time)]
    public string Feriado { get; set; }
    
    public PostosHorarios()
    {
        Ponta       = string.Empty;
        ForaPonta   = string.Empty;
        Reservado   = string.Empty;
        Feriado     = string.Empty;
    }
}

[PublicAPI]
public class ReposicaoDemanda
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