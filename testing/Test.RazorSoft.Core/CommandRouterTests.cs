/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RazorSoft.Core;
using RazorSoft.Core.Messaging;
using RazorSoft.Core.Extensions;


namespace UnitTest.RazorSoft.Core {

    [TestClass]
    public class CommandRouterTests {
        private static readonly CommandRouter commandRouter = CommandRouter.Default;

        [TestMethod]
        public void CreateExecuteCommand() {
            commandRouter.AddCommandTarget<ITransactionClass>(new TransactionClass());

            var result = new AddCommand().Execute((TransactionClass r) => r.Add(2, 4));

            Assert.AreEqual(6, result);
        }

        public class AddCommand : ITransaction {
            public Type Type => typeof(ITransactionClass);
        }

        public interface ITransactionClass {
            int Add(int x, int y);
        }

        private class TransactionClass : ITransactionClass {

            public int Add(int x, int y) {
                return x + y;
            }
        }
    }
}
