using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMResultTypes;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public record HorariosDeVerao
{
    [Display(Name = "")]
    [XmlArrayItem("Periodo")]
    [XmlArray("Periodo")]
    [Length(15)]
    public Periodo[]? Periodos { get; set; }

    public static Either<ParseResult, HorariosDeVerao> FromXElement(XElement? element)
    {
        var horariosDeVerao = new HorariosDeVerao
        {
            Periodos = element?.Elements("Periodo")
                .Select(Periodo.FromXElement)
                .Where(periodo => periodo.HasValue)
                .Select(periodo => periodo.GetValueOrDefault())
                .ToArray()
        };
        return horariosDeVerao;
    }
}
[PublicAPI]
public record struct Periodo
{
    public DateOnly? Inicio { get; set; }
    public DateOnly? Fim { get; set; }

    public static Option<Periodo> FromXElement(XElement element)
    {
        var xInicio = element.Element("Inicio");
        var xFim = element.Element("Fim");

        bool success = true;

        success &= DateOnly.TryParse(xInicio?.Value, out var inicio);
        success &= DateOnly.TryParse(xFim?.Value, out var fim);

        if (!success)
        {
            return Option.None;
        }

        var periodo = new Periodo
        {
            Inicio = inicio,
            Fim = fim
        };
        return periodo;
    }
}