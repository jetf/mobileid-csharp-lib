using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileId;

namespace ServiceTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void T10_WebClientAuthConfig()
        {
            WebClientConfig cfg = WebClientConfig.CreateConfigFromFile("WebClientAuthConfig01.xml");
            Assert.IsNotNull(cfg, "cfg defined");
            Assert.AreEqual("http://changeme.swisscom.ch", cfg.ApId);
        }
    }
}
