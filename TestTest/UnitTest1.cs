using System;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings;
using KimHaiQuang.TheDCIBabyIDE.Domain.Operation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TheDCIBabyIDE.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string contextFile = TestInfo.FrontLoadOperation;
            var IDESettings = new BabyIDESettings();
            IDESettings.ContextFileTypeSettings = BabyIDESettings.ContextFiletype.ContextFiletype_Injectionless;


            var dciContext = new ContextFileParsingContext(IDESettings).Parse(contextFile);
            Assert.IsTrue(dciContext.Name.EndsWith("FrontLoadContext"));
            Assert.IsTrue(dciContext.UsecaseSpan.Length > 0);
            Assert.IsTrue(dciContext.CodeSpan.Length > 0);

            Assert.IsTrue(dciContext.Roles.Count == 5);

            Assert.IsTrue(dciContext.Roles["UnPlannedActivity"].Interface.Signatures.Count > 0);
            Assert.IsTrue(dciContext.Roles["UnPlannedActivity"].Methods.Count > 0);
        }
    }
}
