using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class Qee
{
    [Display(Name = "Tenção Nominal")]

    public float          TensaoNominal { get; set; }
    
    [Display(Name = "Tipo de Ligação")]
    public QeeTipoLigacao TipoLigacao   { get; set; }

    [PublicAPI]
    public enum QeeTipoLigacao
    {
        Indefinido,
        Estrela,
        Delta,
        Bifasico,
        Monofasico,
        SerieOuParalelo,
        DeltaAlternado,
    }
}