using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileId;

namespace ServiceTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void T00_Util()
        {
            Assert.IsTrue(Util.IsXmlSafe(""), "empty string");
            Assert.IsTrue(Util.IsXmlSafe("a"));
            Assert.IsTrue(Util.IsXmlSafe("a:b"));
            Assert.IsFalse(Util.IsXmlSafe("<"));
            Assert.IsFalse(Util.IsXmlSafe("a:<b"));
            Assert.IsFalse(Util.IsXmlSafe(">"));
            Assert.IsFalse(Util.IsXmlSafe("a:>b"));
            Assert.IsFalse(Util.IsXmlSafe("\""));
        }

        [TestMethod]
        public void T01_AuthRequestDto()
        {
            AuthRequestDto dto = new AuthRequestDto();
            Assert.IsFalse(dto.IsComplete(), "empty dto");

            dto.ApId = "http://changeme.swisscom.ch";
            Assert.AreEqual("http://changeme.swisscom.ch", dto.ApId, "ApId");
            Assert.IsFalse(dto.IsComplete(), "dto(ApId) incomplete");

            try
            {
                dto.PhoneNumber = "";
                Assert.Fail("PhoneNumber('')");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(ArgumentNullException), "Expect ArgumentNullException on empty PhoneNumber");
            }

            try
            {
                dto.PhoneNumber = "garbarge";
                Assert.Fail("PhoneNumber('garbarge')");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(ArgumentException), "Expect ArgumentException on garbarge PhoneNumber");
                Assert.AreEqual("PhoneNumberIsIllFormed", e.Message, "Expect PhoneNumberIsIllFormed on garbarge PhoneNumber");
            }

            dto.PhoneNumber = "+41791234567";
            Assert.AreEqual("+41791234567", dto.PhoneNumber, "dto.PhoneNumber");
            Assert.IsFalse(dto.IsComplete(), "dto(ApId,PhoneNumber) incomplete");

        }

        [TestMethod]
        public void T02_AuthRequestDto_ToString()
        {
            AuthRequestDto dto = new AuthRequestDto();
            Assert.AreEqual("ApId:null, Instant:null, MsgToBeSigned:null, TimeOut:80, TransId:null, TransIdPrefix:\"AP.TEST.\", SrvSideValidation:False, UserLanguage:en", dto.ToString(), "ToString(empty_dto)");

            dto.ApId = "http://changeme.swisscom.ch";
            dto.PhoneNumber = "+41791234567";
            dto.DataToBeSigned = "Hello, Mobile ID"; 
#if DEBUG
            Console.WriteLine("dto.Length=" + dto.ToString().Length);
#endif
            Assert.AreEqual("ApId:\"http://changeme.swisscom.ch\", Instant:null, MsgToBeSigned:\"Hello, Mobile ID\", TimeOut:80, TransId:null, TransIdPrefix:\"AP.TEST.\", SrvSideValidation:False, UserLanguage:en", dto.ToString(), "ToString(minimal_valid_dto)");
        }

    }
}
