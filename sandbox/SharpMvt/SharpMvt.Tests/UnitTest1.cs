using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMvt;
using SharpMvt.Production;

namespace SharpMvt.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var typeCollector = new TypeCollector();

            var dllsDir = AppDomain.CurrentDomain.BaseDirectory;

            var types = typeCollector.GetTypes(dllsDir);
        }
    }
}
