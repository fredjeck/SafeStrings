using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.github.fredjeck.SafeStrings.Test
{
    [TestClass]
    public class SafeStringsTest
    {
        [TestMethod]
        public void SunnyDayTest()
        {
            const string s = "This string should be encrypted";
            var enc = s.EncryptUsingPassword("ThisIsMySuperStringPassword");
            Assert.AreNotEqual(s, enc, "The encrypted string should not be equal to its unencrypted counterpart");
            var dec = enc.DecryptUsingPassword("ThisIsMySuperStringPassword");
            Assert.AreEqual(dec, s, "The decrypted string should be equal to its unencrypted counterpart");
        }

        [TestMethod]
        public void WrongParamtersEncryptionTest()
        {
            Assert.AreEqual(string.Empty, "".EncryptUsingPassword("password"));
            Assert.AreEqual(string.Empty, "abcdefg".EncryptUsingPassword(""));
            Assert.AreEqual(string.Empty, "abcdefg".EncryptUsingPassword(null));
        }

        [TestMethod]
        public void WrongParamtersDecryptionTest()
        {
            const string s = "Unencrypted String";
            Assert.AreEqual("", "".DecryptUsingPassword("password"));
            Assert.AreEqual(s, s.DecryptUsingPassword(""));
            Assert.AreEqual(s, s.DecryptUsingPassword(null));
            Assert.AreEqual(s, s.DecryptUsingPassword("password"));
        }

        [TestMethod]
        public void FaultyStringDecryptionTest()
        {
            // Bad password
            var s = "x-enc:fzN2O8y7UraR6zk03XSYLZE9A4rSDWHsYNFFlik8+A3sU4Fu4E7QVWgIFZohclexamyPQhX7bRQ=";
            Assert.AreEqual(s, s.DecryptUsingPassword("abcd"));
            // Truncated encoded string
            s = "x-enc:fzN2O8y7UraR6zk03XSYLZE9A4rSDWHsYNFFlik8+A3sU4Fu4E7QVWg*";
            Assert.AreEqual(s, s.DecryptUsingPassword("abcd"));
        }
    }
}
