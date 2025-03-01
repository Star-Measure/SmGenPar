﻿using JetBrains.Annotations;
using SmGenPar.Logic.Parser;
using SMStructs.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public sealed record TarifaReativos : IXElementParsable<TarifaReativos>
{
    public DateOnly DataVigencia { get; set; }

    [Length(2)]
    public TimeOnly?[]? Indutivo { get; set; }

    [Length(2)]
    public TimeOnly?[]? Capacitivo { get; set; }

    public TarifaReativosFlag DiasUteis { get; set; }
    public TarifaReativosFlag Sabados { get; set; }
    public TarifaReativosFlag Domingos { get; set; }
    public TarifaReativosFlag Feriados { get; set; }
    [Display(Name = "FP de Referencia %")]
    public float FpDeReferencia { get; set; }
}