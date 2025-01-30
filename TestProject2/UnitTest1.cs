using System.Xml.Linq;
using SmGenPar.Logic;
using SmGenPar.Logic.Parser;

namespace TestProject2;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var xml = XDocument.Load(@"C:\Users\alessandro.soares\Desktop\parametrosExtend.xml");
        var command = XParseCommand.XParseParameterPage(xml.Root!);
    }
}