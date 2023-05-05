using System.Collections;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class PostosHorarios : IEnumerable<TimeOnly>
{
    [SerializeProperty]
    public TimeOnly Ponta { get; set; }

    [SerializeProperty(Name = "Fora Ponta")]
    public TimeOnly ForaPonta { get; set; }

    [SerializeProperty]
    public TimeOnly Reservado { get; set; }

    [SerializeProperty]
    public TimeOnly Feriado { get; set; }

    public IEnumerator<TimeOnly> GetEnumerator()
    {
        yield return Ponta;
        yield return ForaPonta;
        yield return Reservado;
        yield return Feriado;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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