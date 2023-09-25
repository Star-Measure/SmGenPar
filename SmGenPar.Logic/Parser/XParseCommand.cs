using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Models;
using SMIO.Buffers;
using SMIO.Buffers.AB;
using SMIO.Buffers.Abnt;
using SMIO.Buffers.EB;
using SMIO.ValueTypes;
using SMResultTypes;
using static SMIO.SpanExtensions;
using Postos = SMIO.ValueTypes.Postos;

namespace SmGenPar.Logic.Parser;

[PublicAPI] public static class XParseCommand
{
    [PublicAPI] public static IEnumerable<MeterCommand> XParseParameterPage(XElement parameterPage)
    {
        var postosHorarios    = XParsePostosHorarios(parameterPage);
        var mostradoresAtivos = XParseMostradoresAtivos(parameterPage);
        var selRead           = XParseSelfRead(parameterPage);
        var feriados          = XParseFeriados(parameterPage);
        var horarioVerao      = XParseHorarioVerao(parameterPage);
        var qee               = XParseQEE(parameterPage);
        var tarifaReativos    = XParseTarifaReativos(parameterPage);
        var modo2             = XParseModo2(parameterPage);

        var collect = new List<MeterCommand>();
        if (postosHorarios.HasValue) {
            collect.AddRange(postosHorarios.Unwrap());
        }
        if (mostradoresAtivos.HasValue) {
            collect.Add(mostradoresAtivos.Unwrap());
        }
        if (selRead.HasValue) {
            collect.AddRange(selRead.Unwrap());
        }
        if (feriados.HasValue) {
            collect.AddRange(feriados.Unwrap());
        }
        if (horarioVerao.HasValue) {
            collect.AddRange(horarioVerao.Unwrap());
        }
        if (qee.HasValue) {
            collect.AddRange(qee.Unwrap());
        }
        if (tarifaReativos.HasValue) {
            collect.Add(tarifaReativos.Unwrap());
        }
        if (modo2.HasValue) {
            collect.AddRange(modo2.Unwrap());
        }
        return collect;
    }

    public static Either<ParseResult, AB92> XParsePosto(XElement element)
    {
        var xPonta         = element?.Elements(nameof(Posto.Ponta)).ToArray()         ?? Array.Empty<XElement>();
        var xForaPonta     = element?.Elements(nameof(Posto.ForaPonta)).ToArray()     ?? Array.Empty<XElement>();
        var xReservado     = element?.Elements(nameof(Posto.Reservado)).ToArray()     ?? Array.Empty<XElement>();
        var xIntermediario = element?.Elements(nameof(Posto.Intermediario)).ToArray() ?? Array.Empty<XElement>();

        if (xForaPonta.Length <= 0 && xReservado.Length <= 0 && xIntermediario.Length <= 0 && xPonta.Length <= 0) {
            return ParseResult.None;
        }

        var pontas        = ParseAndCoalesce(stackalloc Either<ParseResult, TimeOnly>[4], xPonta);
        var foraPonta     = ParseAndCoalesce(stackalloc Either<ParseResult, TimeOnly>[4], xForaPonta);
        var reservado     = ParseAndCoalesce(stackalloc Either<ParseResult, TimeOnly>[4], xReservado);
        var intermediario = ParseAndCoalesce(stackalloc Either<ParseResult, TimeOnly>[4], xIntermediario);

        CondicaoAtivacaoPostoHorario condicaoDeAtivacao = default;
        if (xPonta.Length <= 0) {
            condicaoDeAtivacao |= CondicaoAtivacaoPostoHorario.PontaInvalido;
        }
        if (xForaPonta.Length <= 0) {
            condicaoDeAtivacao |= CondicaoAtivacaoPostoHorario.ForaPontaInvalido;
        }
        if (xReservado.Length <= 0) {
            condicaoDeAtivacao |= CondicaoAtivacaoPostoHorario.ReservadoInvalido;
        }
        if (xIntermediario.Length > 0) {
            condicaoDeAtivacao |= CondicaoAtivacaoPostoHorario.QuartoPostoValido;
        }

        var ab92 = new AB92(0x999990) {
            CommandMode                  = default,
            AtuaisOuFuturos              = default,
            ComportamentoHorarioVerao    = default,
            Data                         = default,
            CondicaoAtivacaoPostoHorario = condicaoDeAtivacao,
            ConjuntoPonta = new Postos {
                [0] = BcdTimeMH.FromDateTime(pontas[0].GetValueOrDefault()),
                [1] = BcdTimeMH.FromDateTime(pontas[1].GetValueOrDefault()),
                [2] = BcdTimeMH.FromDateTime(pontas[2].GetValueOrDefault()),
                [3] = BcdTimeMH.FromDateTime(pontas[3].GetValueOrDefault())
            },
            ConjuntoForaPonta = new Postos {
                [0] = BcdTimeMH.FromDateTime(foraPonta[0].GetValueOrDefault()),
                [1] = BcdTimeMH.FromDateTime(foraPonta[1].GetValueOrDefault()),
                [2] = BcdTimeMH.FromDateTime(foraPonta[2].GetValueOrDefault()),
                [3] = BcdTimeMH.FromDateTime(foraPonta[3].GetValueOrDefault())
            },
            ConjuntoReservado = new Postos {
                [0] = BcdTimeMH.FromDateTime(reservado[0].GetValueOrDefault()),
                [1] = BcdTimeMH.FromDateTime(reservado[1].GetValueOrDefault()),
                [2] = BcdTimeMH.FromDateTime(reservado[2].GetValueOrDefault()),
                [3] = BcdTimeMH.FromDateTime(reservado[3].GetValueOrDefault())
            },
            ConjuntoIntermediario = new Postos {
                [0] = BcdTimeMH.FromDateTime(intermediario[0].GetValueOrDefault()),
                [1] = BcdTimeMH.FromDateTime(intermediario[1].GetValueOrDefault()),
                [2] = BcdTimeMH.FromDateTime(intermediario[2].GetValueOrDefault()),
                [3] = BcdTimeMH.FromDateTime(intermediario[3].GetValueOrDefault())
            }
        };
        return ab92;
    }
    static Span<Either<ParseResult, TParsable>> ParseAndCoalesce<TParsable>(
        Span<Either<ParseResult, TParsable>> values,
        Span<XElement> elements) where TParsable : IParsable<TParsable>
    {
        for (var i = 0; i < elements.Length; ++i) {
            values[i] = XParser.XParse<TParsable>(elements[i]);
        }
        Either<ParseResult, TParsable> lastValid = default;
        for (var i = 0; i < values.Length; ++i) {
            if (values[i].HasValue) {
                lastValid = values[i];
            }
            else if (values[i].State == State.None) {
                values[i] = lastValid;
            }
        }
        lastValid = default;
        for (var i = values.Length - 1; i >= 0; --i) {
            if (values[i].HasValue) {
                lastValid = values[i];
            }
            else if (values[i].State == State.None) {
                values[i] = lastValid;
            }
        }
        return values;
    }
    public static Either<ParseResult, IReadOnlyCollection<AB92>> XParsePostosHorarios(XElement element, DateOnly? dateOnly = default)
    {
        var xPostosHorarios = element?.Element(nameof(PostosHorarios));
        var xDataInicio     = xPostosHorarios?.Element(nameof(PostosHorarios.DataDeInicio));
        var xDomingo        = xPostosHorarios?.Element(nameof(PostosHorarios.Domingo));
        var xSegunda        = xPostosHorarios?.Element(nameof(PostosHorarios.Segunda));
        var xTerca          = xPostosHorarios?.Element(nameof(PostosHorarios.Terca));
        var xQuarta         = xPostosHorarios?.Element(nameof(PostosHorarios.Quarta));
        var xQuinta         = xPostosHorarios?.Element(nameof(PostosHorarios.Quinta));
        var xSexta          = xPostosHorarios?.Element(nameof(PostosHorarios.Sexta));
        var xSabado         = xPostosHorarios?.Element(nameof(PostosHorarios.Sabado));
        var xFeriado        = xPostosHorarios?.Element(nameof(PostosHorarios.Feriado));


        bool hasNull = false;

        hasNull |= xDomingo is null;
        hasNull |= xSegunda is null;
        hasNull |= xTerca is null;
        hasNull |= xQuarta is null;
        hasNull |= xQuinta is null;
        hasNull |= xSexta is null;
        hasNull |= xSabado is null;
        hasNull |= xFeriado is null;

        if (hasNull) {
            return ParseResult.ParseError;
        }

        var data = dateOnly ?? XParser.XParse<DateOnly>(xDataInicio).GetValueOrDefault(DateOnly.FromDateTime(DateTime.Now));

        var cmdDomingo = XParsePosto(xDomingo!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Domingo);

        var cmdSegunda = XParsePosto(xSegunda!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Segunda);

        var cmdTerca = XParsePosto(xTerca!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Terca);

        var cmdQuarta = XParsePosto(xQuarta!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Quarta);

        var cmdQuinta = XParsePosto(xQuinta!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Quinta);

        var cmdSexta = XParsePosto(xSexta!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Sexta);

        var cmdSabado = XParsePosto(xSabado!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Sabado);

        var cmdFeriado = XParsePosto(xFeriado!)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Feriado);

        var results = cmdDomingo
            .Append(cmdSegunda)
            .Append(cmdTerca)
            .Append(cmdQuarta)
            .Append(cmdQuinta)
            .Append(cmdSexta)
            .Append(cmdSabado)
            .Append(cmdFeriado);

        if (results.HasError) {
            return results.UnwrapError();
        }

        var commands = TupleHelper<AB92>.ToList(results.Unwrap());

        CollectionsMarshal.AsSpan(commands).Reverse();

        for (var index = 0; index < commands.Count; ++index) {
            AB92 command = commands[index];
            command.AtuaisOuFuturos           = AtuaisOuFuturos.Futuros;
            command.Data                      = BcdDateDMY.FromDateTime(data);
            command.CommandMode               = CommandMode.Escrita;
            command.ComportamentoHorarioVerao = ComportamentoHorarioVerao.Ignora;
        }
        return new Either<ParseResult, IReadOnlyCollection<AB92>>(commands);
    }
    public static Either<ParseResult, IReadOnlyCollection<EB03>> XParseSelfRead(XElement element)
    {
        const int selfReadCount = 16;
        var arrSelfRead = element.Elements(nameof(Extend.SelfRead))
            .Select(XParser.XParse<DateTime>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Select(SelfRead.FromDateTime)
            .ToArray();

        if (arrSelfRead.Length == 0) {
            return ParseResult.ParseError;
        }

        if (arrSelfRead.Length > selfReadCount) {
            return ParseResult.ParseError;
        }

        Span<SelfRead> span = stackalloc SelfRead[selfReadCount];

        for (var i = 0; i < arrSelfRead.Length; i++) {
            span[i] = arrSelfRead[i];
        }

        var first  = span[..8];
        var second = span[8..];

        var lst = new List<EB03> {
            new EB03 {
                //CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Sobrescreve,
                SelfRead     = first,
            },
            new EB03 {
                //CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Adiciona,
                SelfRead     = second,
            }
        };
        return lst;
    }
    public static Either<ParseResult, AB04> XParseMostradoresAtivos(XElement element)
    {
        var xMostrador = element.Element(nameof(Mostradores))?.Element("Codigo");

        if (BigInteger.TryParse(xMostrador?.Value[2..], NumberStyles.HexNumber, null, out var bitField)) {
            var mostradoresAtivos = new Mostradores(bitField);

            return new Either<ParseResult, AB04>(new AB04 {
                CommandMode       = CommandMode.Escrita,
                MostradoresAtivos = mostradoresAtivos,
            });
        }
        return ParseResult.ParseError;
    }
    public static Either<ParseResult, IReadOnlyCollection<Abnt9832>> XParseFeriados(XElement element)
    {
        const int feriadoCount = 82;
        var arrFeriados = element.Elements(nameof(Extend.DataFeriados))
            .Select(XParser.XParse<DateTime>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Select(BcdDateDMY.FromDateTime)
            .ToArray();

        if (arrFeriados.Length == 0) {
            return ParseResult.ParseError;
        }

        if (arrFeriados.Length > feriadoCount) {
            return ParseResult.ParseError;
        }

        const int packetSize = 19;

        Span<BcdDateDMY> span = stackalloc BcdDateDMY[feriadoCount];

        for (var i = 0; i < arrFeriados.Length; i++) {
            span[i] = arrFeriados[i];
        }

        Span<BcdDateDMY> first  = stackalloc BcdDateDMY[packetSize];
        Span<BcdDateDMY> second = stackalloc BcdDateDMY[packetSize];
        Span<BcdDateDMY> third  = stackalloc BcdDateDMY[packetSize];
        Span<BcdDateDMY> fourth = stackalloc BcdDateDMY[packetSize];
        Span<BcdDateDMY> fifth  = stackalloc BcdDateDMY[feriadoCount % (packetSize * 4)];

        ReadFrom(ref span, first);
        ReadFrom(ref span, second);
        ReadFrom(ref span, third);
        ReadFrom(ref span, fourth);
        ReadFrom(ref span, fifth);

        var lst = new List<Abnt9832> {
            new Abnt9832(0x999990) {
                CommandMode  = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Sobrescreve,
                Feriados     = first,
            },
            new Abnt9832(0x999991) {
                CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Adiciona,
                Feriados     = second,
            },
            new Abnt9832(0x999991) {
                CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Adiciona,
                Feriados     = third,
            },
            new Abnt9832(0x999991) {
                CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Adiciona,
                Feriados     = fourth,
            },
            new Abnt9832(0x999991) {
                CommandMode         = CommandMode.Escrita,
                ModoInsercao = ModoInsercao.Adiciona,
                Feriados     = fifth,
            }
        };
        return lst;
    }

    public static Either<ParseResult, IReadOnlyCollection<AB64>> XParseHorarioVerao(XElement element)
    {
        const int horarioVeraoCount = 15;

        var arrHorariosDeVerao = element.Elements(nameof(Extend.HorarioDeVerao))
            .Select(Periodo.FromXElement)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Where(x => x is { Inicio: not null, Fim: not null })
            .Select(x => (Inicio: x.Inicio!.Value, Fim: x.Fim!.Value))
            .Select(x => HorarioVerao.FromDateTime(x.Inicio, x.Fim))
            .ToArray();

        if (arrHorariosDeVerao.Length == 0) {
            return ParseResult.ParseError;
        }

        if (arrHorariosDeVerao.Length > horarioVeraoCount) {
            return ParseResult.ParseError;
        }

        Span<HorarioVerao> span = stackalloc HorarioVerao[horarioVeraoCount];

        for (var i = 0; i < arrHorariosDeVerao.Length; i++) {
            span[i] = arrHorariosDeVerao[i];
        }

        const int          packetSize = 9;
        Span<HorarioVerao> first      = stackalloc HorarioVerao[packetSize];
        Span<HorarioVerao> second     = stackalloc HorarioVerao[horarioVeraoCount % packetSize];

        ReadFrom(ref span, first);
        ReadFrom(ref span, second);

        var lst = new List<AB64> {
            new AB64(0x999990) {
                CommandMode           = CommandMode.Escrita,
                ModoInsercao   = ModoInsercao.Sobrescreve,
                HorarioDeVerao = first,
            },
            new AB64(0x999991) {
                CommandMode           = CommandMode.Escrita,
                ModoInsercao   = ModoInsercao.Adiciona,
                HorarioDeVerao = second,
            },
        };
        return lst;
    }

    public static Either<ParseResult, IReadOnlyCollection<MeterCommand>> XParseQEE(XElement element)
    {
        var xQEE           = element.Element(nameof(Extend.Qee));
        var xTensaoNominal = xQEE?.Element(nameof(Qee.TensaoNominal));
        var xTipoLigacao   = xQEE?.Element(nameof(Qee.TipoLigacao));

        if (xTensaoNominal is null && xTipoLigacao is null) {
            return ParseResult.ParseError;
        }

        var lst = new List<MeterCommand>();

        if (XParser.XParse<float>(xTensaoNominal).TryGetValue(out var tensaoNominal)) {
            var abnt9895 = new Abnt9895(0x999990) {
                TensaoNominalPrimaria          = tensaoNominal,
                FlagIndicacaoAlteracaoAbnt9895 = FlagIndicacaoAlteracaoAbnt9895.TensaoNominalPrimaria
            };
            lst.Add(abnt9895);
        }
        if (xTipoLigacao?.Value is { Length: > 0 } tipoLigacao) {
            var ab95 = new Abnt95(0x999990) {
                TipoLigacaoQee = Enum.Parse<TipoLigacaoQee>(tipoLigacao),
                Alteracoes     = FlagAlteracaoConstantes.TipoLigacao
            };
            lst.Add(ab95);
        }
        return lst.Count == 0 ? ParseResult.ParseError : lst;
    }
    public static Either<ParseResult, AB67> XParseTarifaReativos(XElement element)
    {
        var xTarifaReativos = element.Element(nameof(Extend.TarifaReativos));
        var xDataVigencia   = xTarifaReativos?.Element(nameof(TarifaReativos.DataVigencia));
        var xFpReferencia   = xTarifaReativos?.Element(nameof(TarifaReativos.FpDeReferencia));
        var xSabados        = xTarifaReativos?.Element(nameof(TarifaReativos.Sabados));
        var xDiasUteis      = xTarifaReativos?.Element(nameof(TarifaReativos.DiasUteis));
        var xFeriados       = xTarifaReativos?.Element(nameof(TarifaReativos.Feriados));
        var xDomingos       = xTarifaReativos?.Element(nameof(TarifaReativos.Domingos));

        ParseResult parseResult;

        bool hasNull = false;

        hasNull |= xDataVigencia is null;
        hasNull |= xFpReferencia is null;
        hasNull |= xSabados is null;
        hasNull |= xDiasUteis is null;
        hasNull |= xFeriados is null;
        hasNull |= xDomingos is null;

        if (hasNull) {
            return ParseResult.ParseError;
        }

        if (!XParser.XParse<DateOnly>(xDataVigencia).TryGetValue(out var dataVigencia, out parseResult)) {
            return parseResult;
        }

        if (!XParser.XParse<byte>(xFpReferencia).TryGetValue(out var fpReferencia, out parseResult)) {
            return parseResult;
        }

        if (!Enum.TryParse<TarifaReativosFlag>(xSabados?.Value, out var sabados)) {
            sabados = TarifaReativosFlag.Indefinido;
        }
        if (!Enum.TryParse<TarifaReativosFlag>(xDiasUteis?.Value, out var diasUteis)) {
            diasUteis = TarifaReativosFlag.Indefinido;
        }
        if (!Enum.TryParse<TarifaReativosFlag>(xFeriados?.Value, out var feriados)) {
            feriados = TarifaReativosFlag.Indefinido;
        }
        if (!Enum.TryParse<TarifaReativosFlag>(xDomingos?.Value, out var domingos)) {
            domingos = TarifaReativosFlag.Indefinido;
        }

        var arrIndutivos = xTarifaReativos?.Elements(nameof(Extend.TarifaReativos.Indutivo))
            .Select(XParser.XParse<DateTime>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Select(BcdTimeMH.FromDateTime)
            .ToArray() ?? Array.Empty<BcdTimeMH>();

        var arrCapacitivos = xTarifaReativos?.Elements(nameof(Extend.TarifaReativos.Capacitivo))
            .Select(XParser.XParse<DateTime>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Select(BcdTimeMH.FromDateTime)
            .ToArray() ?? Array.Empty<BcdTimeMH>();

        var ab67 = new AB67(0x999990) {
            CommandMode                     = CommandMode.Escrita,
            DataInicioTarifaReativos = BcdDateDMY.FromDateTime(dataVigencia),
            FatorPotenciaReferencia  = Bcd.ToBcd(fpReferencia),
            HorarioIndutivo          = arrIndutivos,
            HorarioCapacitivo        = arrCapacitivos,
            AtivosSabado             = sabados,
            AtivosDiasUteis          = diasUteis,
            AtivosFeriados           = feriados,
            AtivosDomingo            = domingos,
        };

        return ab67;
    }

    public static Either<ParseResult, IReadOnlyCollection<MeterCommand>> XParseModo2(XElement element)
    {
        var xModo2                        = element.Element(nameof(Extend.Modo2));
        var xKp                           = xModo2?.Elements(nameof(Extend.Modo2.Kp));
        var xSaidaUsuario                 = xModo2?.Element(nameof(Extend.Modo2.SaidaDeUsuario));
        var xNmrCasasDecimaisEnergiaAtiva = xModo2?.Element(nameof(Extend.Modo2.NmrCasasDecimaisEnergia));
        var xNmrCasasDecimaisDemanda      = xModo2?.Element(nameof(Extend.Modo2.NmrCasasDecimaisDemanda));

        bool hasAbnt90 = false;
        hasAbnt90 |= xNmrCasasDecimaisEnergiaAtiva is not null;
        hasAbnt90 |= xNmrCasasDecimaisDemanda is not null;

        bool hasAbnt95 = false;
        hasAbnt95 |= xSaidaUsuario is not null;
        hasAbnt95 |= xKp is not null;


        if (!hasAbnt90 && !hasAbnt95) {
            return ParseResult.ParseError;
        }

        var lst = new List<MeterCommand>();
        if (hasAbnt90) {
            if (AddAbnt90(xNmrCasasDecimaisEnergiaAtiva, xNmrCasasDecimaisDemanda).TryGetValue(out var abnt90, out var parseResult)) {
                lst.Add(abnt90);
            }
            else {
                return parseResult;
            }
        }
        if (hasAbnt95) {
            if (AddAbnt95(xSaidaUsuario, xKp?.ToArray()).TryGetValue(out var abnt95, out var parseResult)) {
                lst.Add(abnt95);
            }
            else {
                return parseResult;
            }
        }

        static Either<ParseResult, MeterCommand> AddAbnt90(XElement? xCasasDecimaisEnergiaAtiva, XElement? xCasasDecimaisDemanda)
        {
            if (!XParser.XParse<byte>(xCasasDecimaisEnergiaAtiva).TryGetValue(out var nmrCasasDecimaisEnergia, out var parseResult)) {
                return parseResult;
            }
            if (!XParser.XParse<byte>(xCasasDecimaisDemanda).TryGetValue(out var nmrCasasDecimaisDemanda, out parseResult)) {
                return parseResult;
            }
            // ReSharper disable twice BitwiseOperatorOnEnumWithoutFlags
            var energia = (ModoApresentacao)(nmrCasasDecimaisEnergia << 4) | ModoApresentacao.K_Grandeza;
            var demanda = (ModoApresentacao)(nmrCasasDecimaisDemanda << 4) | ModoApresentacao.K_Grandeza;

            var abnt90 = new Abnt90(0x999990) {
                ModoApresentacaoCanais = new[] { (energia, demanda) }
            };
            return abnt90;
        }

        static Either<ParseResult, MeterCommand> AddAbnt95(XElement? xSaidaUsuario, IReadOnlyCollection<XElement>? xKp)
        {
            var abnt95 = new Abnt95(0x999990) {
                Alteracoes = default
            };
            if (Enum.TryParse<TipoSaidaUsuario>(xSaidaUsuario?.Value, out var saidaUsuario)) {
                abnt95.TipoSaidaUsuario =  saidaUsuario;
                abnt95.Alteracoes       |= FlagAlteracaoConstantes.SaidaUsuario;
            }
            if (xKp?.Count == 2) {
                var arrKp = xKp
                    .Select(XParser.XParse<int>)
                    .Where(x => x.HasValue)
                    .Select(x => x.Unwrap())
                    .ToArray();
                abnt95.ConstanteKp = (arrKp[0], arrKp[1]);
            }
            else {
                abnt95.ConstanteKp = (1, 1);
            }
            return abnt95;
        }

        return lst;
    }
    public static Either<ParseResult, AB63> XParseReposicao(XElement element)
    {
        var xReposicao = element.Element(nameof(Extend.Reposicao));
        var xJaneiro   = xReposicao?.Element(nameof(Extend.Reposicao.Janeiro));
        var xFevereiro = xReposicao?.Element(nameof(Extend.Reposicao.Fevereiro));
        var xMarco     = xReposicao?.Element(nameof(Extend.Reposicao.Marco));
        var xAbril     = xReposicao?.Element(nameof(Extend.Reposicao.Abril));
        var xMaio      = xReposicao?.Element(nameof(Extend.Reposicao.Maio));
        var xJunho     = xReposicao?.Element(nameof(Extend.Reposicao.Junho));
        var xJulho     = xReposicao?.Element(nameof(Extend.Reposicao.Julho));
        var xAgosto    = xReposicao?.Element(nameof(Extend.Reposicao.Agosto));
        var xSetembro  = xReposicao?.Element(nameof(Extend.Reposicao.Setembro));
        var xOutubro   = xReposicao?.Element(nameof(Extend.Reposicao.Outubro));
        var xNovembro  = xReposicao?.Element(nameof(Extend.Reposicao.Novembro));
        var xDezembro  = xReposicao?.Element(nameof(Extend.Reposicao.Dezembro));

        bool hasNull = false;
        hasNull |= xJaneiro is null;
        hasNull |= xFevereiro is null;
        hasNull |= xMarco is null;
        hasNull |= xAbril is null;
        hasNull |= xMaio is null;
        hasNull |= xJunho is null;
        hasNull |= xJulho is null;
        hasNull |= xAgosto is null;
        hasNull |= xSetembro is null;
        hasNull |= xOutubro is null;
        hasNull |= xNovembro is null;
        hasNull |= xDezembro is null;

        if (hasNull) {
            return ParseResult.ParseError;
        }

        Span<Either<ParseResult, byte>> daysOfMonth = stackalloc Either<ParseResult, byte>[] {
            XParser.XParse<byte>(xJaneiro), XParser.XParse<byte>(xFevereiro), XParser.XParse<byte>(xMarco), XParser.XParse<byte>(xAbril),
            XParser.XParse<byte>(xMaio), XParser.XParse<byte>(xJunho), XParser.XParse<byte>(xJulho), XParser.XParse<byte>(xAgosto),
            XParser.XParse<byte>(xSetembro), XParser.XParse<byte>(xOutubro), XParser.XParse<byte>(xNovembro), XParser.XParse<byte>(xDezembro)
        };

        if (!daysOfMonth.All()) {
            return ParseResult.ParseError;
        }

        var ab63 = new AB63(0x999990) {
            AtivaDesativa = true,
            CommandMode          = CommandMode.Escrita,
            DiasDeReposicao = new (Bcd, Bcd)[] {
                (Bcd.ToBcd(daysOfMonth[01].Unwrap()), 0x01), (Bcd.ToBcd(daysOfMonth[02].Unwrap()), 0x02),
                (Bcd.ToBcd(daysOfMonth[03].Unwrap()), 0x03), (Bcd.ToBcd(daysOfMonth[04].Unwrap()), 0x04),
                (Bcd.ToBcd(daysOfMonth[05].Unwrap()), 0x05), (Bcd.ToBcd(daysOfMonth[06].Unwrap()), 0x06),
                (Bcd.ToBcd(daysOfMonth[07].Unwrap()), 0x07), (Bcd.ToBcd(daysOfMonth[08].Unwrap()), 0x08),
                (Bcd.ToBcd(daysOfMonth[09].Unwrap()), 0x09), (Bcd.ToBcd(daysOfMonth[10].Unwrap()), 0x10),
                (Bcd.ToBcd(daysOfMonth[11].Unwrap()), 0x11), (Bcd.ToBcd(daysOfMonth[12].Unwrap()), 0x12),
            }
        };

        return ab63;
    }
}