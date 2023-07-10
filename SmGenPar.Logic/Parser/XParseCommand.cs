using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using JetBrains.Annotations;
using SmGenPar.Logic.Models;
using SMIO;
using SMIO.Buffers.AB;
using SMIO.Buffers.EB;
using SMIO.Collections;
using SMIO.ValueTypes;
using SMResultTypes;
using PostosHorarios = SMIO.Collections.PostosHorarios;

namespace SmGenPar.Logic.Parser;

[PublicAPI] public static class XParseCommand
{
    public static Either<ParseResult, AB92> XParsePostoHorario(XElement element)
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
            Modo                         = default,
            AtuaisOuFuturos              = default,
            ComportamentoHorarioVerao    = default,
            Data                         = default,
            CondicaoAtivacaoPostoHorario = condicaoDeAtivacao,
            ConjuntoPonta = new PostosHorarios {
                [0] = BcdTimeShort.FromDateTime(pontas[0].GetValueOrDefault()),
                [1] = BcdTimeShort.FromDateTime(pontas[1].GetValueOrDefault()),
                [2] = BcdTimeShort.FromDateTime(pontas[2].GetValueOrDefault()),
                [3] = BcdTimeShort.FromDateTime(pontas[3].GetValueOrDefault())
            },
            ConjuntoForaPonta = new PostosHorarios {
                [0] = BcdTimeShort.FromDateTime(foraPonta[0].GetValueOrDefault()),
                [1] = BcdTimeShort.FromDateTime(foraPonta[1].GetValueOrDefault()),
                [2] = BcdTimeShort.FromDateTime(foraPonta[2].GetValueOrDefault()),
                [3] = BcdTimeShort.FromDateTime(foraPonta[3].GetValueOrDefault())
            },
            ConjuntoReservado = new PostosHorarios {
                [0] = BcdTimeShort.FromDateTime(reservado[0].GetValueOrDefault()),
                [1] = BcdTimeShort.FromDateTime(reservado[1].GetValueOrDefault()),
                [2] = BcdTimeShort.FromDateTime(reservado[2].GetValueOrDefault()),
                [3] = BcdTimeShort.FromDateTime(reservado[3].GetValueOrDefault())
            },
            ConjuntoIntermediario = new PostosHorarios {
                [0] = BcdTimeShort.FromDateTime(intermediario[0].GetValueOrDefault()),
                [1] = BcdTimeShort.FromDateTime(intermediario[1].GetValueOrDefault()),
                [2] = BcdTimeShort.FromDateTime(intermediario[2].GetValueOrDefault()),
                [3] = BcdTimeShort.FromDateTime(intermediario[3].GetValueOrDefault())
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
    public static Either<ParseResult, IReadOnlyCollection<AB92>> XParsePostosHorarios(XElement element)
    {
        var xDataInicio = element.Element(nameof(Models.PostosHorarios.DataDeInicio));
        var xDomingo    = element.Element(nameof(Models.PostosHorarios.Domingo));
        var xSegunda    = element.Element(nameof(Models.PostosHorarios.Segunda));
        var xTerca      = element.Element(nameof(Models.PostosHorarios.Terca));
        var xQuarta     = element.Element(nameof(Models.PostosHorarios.Quarta));
        var xQuinta     = element.Element(nameof(Models.PostosHorarios.Quinta));
        var xSexta      = element.Element(nameof(Models.PostosHorarios.Sexta));
        var xSabado     = element.Element(nameof(Models.PostosHorarios.Sabado));
        var xFeriado    = element.Element(nameof(Models.PostosHorarios.Feriado));


        if (xDataInicio is null || xDomingo is null || xSegunda is null || xTerca is null  ||
            xQuarta is null     || xQuinta is null  || xSexta is null   || xSabado is null ||
            xFeriado is null) {
            return ParseResult.ParseError;
        }

        var (state, err, data) = XParser.XParse<DateOnly>(xDataInicio);

        if (state is State.Err) {
            return err;
        }

        var cmdDomingo = XParsePostoHorario(xDomingo)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Domingo);

        var cmdSegunda = XParsePostoHorario(xSegunda)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Segunda);

        var cmdTerca = XParsePostoHorario(xTerca)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Terca);

        var cmdQuarta = XParsePostoHorario(xQuarta)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Quarta);

        var cmdQuinta = XParsePostoHorario(xQuinta)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Quinta);

        var cmdSexta = XParsePostoHorario(xSexta)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Sexta);

        var cmdSabado = XParsePostoHorario(xSabado)
            .OnValue(command => command.CondicaoAtivacaoPostoHorario |= CondicaoAtivacaoPostoHorario.Sabado);

        var cmdFeriado = XParsePostoHorario(xFeriado)
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

        var commands = TupleUtils<AB92>.Flatten(results.Unwrap());

        CollectionsMarshal.AsSpan(commands).Reverse();

        for (var index = 0; index < commands.Count; ++index) {
            AB92 command = commands[index];
            command.AtuaisOuFuturos           = Postos.Futuros;
            command.Data                      = BcdDate.FromDateTime(data);
            command.Modo                      = ModoComando.Escrita;
            command.ComportamentoHorarioVerao = ComportamentoHorarioVerao.Ignora;

            if (command.CondicaoAtivacaoPostoHorario.DiaDaSemana == DiaDaSemana.Feriado) {}
        }
        return new Either<ParseResult, IReadOnlyCollection<AB92>>(commands);
    }
    public static Either<ParseResult, (EB03 First, EB03? Second)> XParseSelfRead(XElement element)
    {
        var arrSelfRead = element.Elements(nameof(Extend.SelfRead))
            .Select(XParser.XParse<DateOnly>)
            .Where(x => x.HasValue)
            .Select(x => x.Unwrap())
            .Select(BcdDate.FromDateTime)
            .ToArray();

        var first  = arrSelfRead.AsSpan(0, 8);
        var second = arrSelfRead.AsSpan(8, 16);

        var eb03First = new EB03 {
            Modo         = ModoComando.Escrita,
            ModoInsercao = ModoInsercao.Sobrescreve,
            SelfRead     = ReinterpretCast<ConjuntoSelfRead>.GetRef(first),
        };
        var eb03Second = new EB03 {
            Modo         = ModoComando.Escrita,
            ModoInsercao = ModoInsercao.Sobrescreve,
            SelfRead     = ReinterpretCast<ConjuntoSelfRead>.GetRef(second),
        };
        
        return new Either<ParseResult, (EB03 First, EB03? Second)>((eb03First, eb03Second));
    }
}