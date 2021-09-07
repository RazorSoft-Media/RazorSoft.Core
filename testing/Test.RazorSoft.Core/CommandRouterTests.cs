// Copyright: ©2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


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
        public void ExecuteCommandTask() {
            var expResult = 6;

            commandRouter.AddRoute<ITaskInterface>(new CommandAPI());

            var actResult = new AddCommand().Execute((CommandAPI api) => api.Add(2, 4));

            Assert.AreEqual(expResult, actResult);
        }

        [TestMethod]
        public void ExecuteCommandDelegate() {
            var expAddResult = 6;
            var expSubResult = 23;

            var api = commandRouter.AddRoute<IDelegateInterface>(new CommandAPI());

            int Add(int x, int y) {
                return api.Execute((api) => api.Add(2, 4));
            }

            var actAddResult = Add(4, 2);
            var actSubResult = api.Execute((api) => api.Subtract(48, 25));

            Assert.AreEqual(expAddResult, actAddResult);
            Assert.AreEqual(expSubResult, actSubResult);
        }

        public class AddCommand : ICommandTask {
            public Type Type => typeof(ITaskInterface);
        }

        public interface ITaskInterface {
            int Add(int x, int y);
        }

        public interface IDelegateInterface {
            int Add(int x, int y);
            int Subtract(int x, int y);
        }

        private class CommandAPI : ITaskInterface, IDelegateInterface {

            public int Add(int x, int y) {
                return x + y;
            }

            public int Subtract(int x, int y) {
                return x - y;
            }
        }
    }
}
