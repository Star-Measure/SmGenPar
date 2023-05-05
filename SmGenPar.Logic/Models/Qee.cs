using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class Qee
{
    [SerializeProperty(Name = "Tenção Nominal")]
    
    public float          TensaoNominal { get; set; }
    
    [SerializeProperty(Name = "Tipo de Ligação")]
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