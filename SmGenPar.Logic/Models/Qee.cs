﻿using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMResultTypes;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class Qee : IXElementParsable<Qee>
{
    [Display(Name = "Tenção Nominal")]

    public float TensaoNominal { get; set; }

    [Display(Name = "Tipo de Ligação")]
    public QeeTipoLigacao TipoLigacao { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
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

    public static Either<ParseResult, Qee> FromXElement(XElement? element)
    {
        var xTensaoNominal = element?.Element("TensaoNominal");
        var xTipoLigacao = element?.Element("TipoLigacao");

        var tensaoNominal = XParser.XParse<float>(xTensaoNominal, CultureInfo.InvariantCulture);
        var tipoLigacao = XParser.XParseEnum<QeeTipoLigacao>(xTipoLigacao);

        var qee = new Qee {
            TensaoNominal = tensaoNominal.GetValueOrDefault(),
            TipoLigacao   = tipoLigacao.GetValueOrDefault()
        };

        return qee;
    }
}