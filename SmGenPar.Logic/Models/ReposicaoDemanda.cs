using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

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