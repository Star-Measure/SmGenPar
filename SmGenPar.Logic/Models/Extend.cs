using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
namespace SmGenPar.Logic.Models;

[PublicAPI]
public class Extend
{
    [SerializeProperty]
    [Required]
    public DateTime       DataHora       { get; set; }
    public PostosHorarios PostosHorarios { get; set; }
    public SelfRead       SelfRead       { get; set; }
    
    [SerializeProperty(Name = "Codigo do Consumidor")]
    public int           NumeroTrafo    { get; set; }
    public Qee            Qee            { get; set; }
    
    public Extend()
    {
        DataHora       = default;
        PostosHorarios = new();
        SelfRead       = new();
        Qee            = new();
    }
}

public class SerializePropertyAttribute : Attribute
{
    public string Name { get; init; }
    
    public SerializePropertyAttribute(string name)
    {
        Name = name;
    }
    
    public SerializePropertyAttribute()
    {
        Name = string.Empty;
    }
}

// public string Name = "Extend";
// public static readonly string[] ParameterNames = {
//     "Data Hora",
//     "Postos Horarios",
//     "Self Read",
//     "Numero Trafo",
//     "Mostrador",
//     "Feriados",
//     "Horario De Verao",
//     "Reposicao",
//     "QEE",
//     "Tarifa Reativos",
//     "Modo II",
// };
// public enum Parametros
// {
//     DataHora,
//     PostosHorarios,
//     SelfRead,
//     NumeroTrafo,
//     Mostrador,
//     Feriados,
//     HorarioDeVerao,
//     Reposicao,
//     Qee,
//     TarifaReativos,
//     Modo2,
// }