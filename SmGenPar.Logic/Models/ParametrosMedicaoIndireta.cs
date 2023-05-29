using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record ParametrosMedicaoIndireta
{
    public SaidaUsuario SaidaDeUsuario { get; set; }
    [Length(2)]
    public int[]?       Kp { get; set; }
    
    public int         NmrCasasDecimaisEnergia { get; set; }
    public int         NmrCasasDecimaisDemanda { get; set; }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SaidaUsuario
    {
        MonoDirecional = 0,
        Estendida = 1,
        Mista = 3,
        SER311 = 5,
        Serial_II_PIMA = 6,
    }
}