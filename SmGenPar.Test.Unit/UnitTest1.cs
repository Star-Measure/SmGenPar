using System.Xml.Linq;

namespace SmGenPar.Test.Unit;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var xml = XDocument.Load(@"C:\Users\alessandro.soares\Desktop\a.txt");
    }
}