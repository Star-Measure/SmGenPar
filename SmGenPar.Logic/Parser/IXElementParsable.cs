using System.Xml.Linq;
using JetBrains.Annotations;
using SMResultTypes;

namespace SmGenPar.Logic.Parser;

[PublicAPI] public interface IXElementParsable<TModel> where TModel : IXElementParsable<TModel>
{
    public abstract static Either<ParseResult, TModel> FromXElement(XElement? element);
}
[Flags] public enum ParseResult
{
    None,
    ParseError,
}