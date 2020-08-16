//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
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
            commandRouter.AddCommandTarget<ICommandInterface>(new CommandAPI());

            var result = new AddCommand().Execute((CommandAPI r) => r.Add(2, 4));

            Assert.AreEqual(6, result);
        }

        public class AddCommand : ICommandTask {
            public Type Type => typeof(ICommandInterface);
        }

        public interface ICommandInterface {
            int Add(int x, int y);
        }

        private class CommandAPI : ICommandInterface {

            public int Add(int x, int y) {
                return x + y;
            }
        }
    }
}
