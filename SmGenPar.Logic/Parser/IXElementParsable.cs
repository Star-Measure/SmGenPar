﻿using JetBrains.Annotations;

namespace SmGenPar.Logic.Parser;

[PublicAPI] public interface IXElementParsable<TModel> where TModel : IXElementParsable<TModel> { }
[Flags]
public enum ParseResult
{
    None,
    ParseError,
}