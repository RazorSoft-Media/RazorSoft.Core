/* ***********************************************
 *  © 2019 Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using InsightP2P.Domain;


namespace UnitTest.IP2P.Domain {
    [TestClass]
    public class SettingTests {

        [TestMethod]
        public void IntSetting() {
            var value = 1;
            var expName = "Setting";
            var expValue = BitConverter.GetBytes(value);

            Setting setting = (Name: expName, Value: value);

            Assert.AreEqual(expName, setting.Name);
            CollectionAssert.AreEqual(expValue, setting.Value);
        }

        [TestMethod]
        public void ChangeSetting() {
            var origValue = 1;
            var newValue1 = 2;
            var newValue2 = "help_me";
            var newValue3 = (decimal)1234.5678923;

            var expName = "Setting";
            var expValue1 = BitConverter.GetBytes(newValue1);
            var expValue2 = Encoding.UTF8.GetBytes(newValue2);
            byte[] expValue3 = GetDecimalBytes(newValue3);

            Setting setting = (Name: expName, Value: origValue);
            setting.SetValue(newValue1);

            CollectionAssert.AreEqual(expValue1, setting.Value);

            setting.SetValue(newValue2);

            CollectionAssert.AreEqual(expValue2, setting.Value);

            setting.SetValue(newValue3);

            CollectionAssert.AreEqual(expValue3, setting.Value);
        }

        private byte[] GetDecimalBytes(decimal value) {
            var ints = decimal.GetBits(value);
            return ints
                .Select(i => (byte)i)
                .ToArray();
        }
    }
}
