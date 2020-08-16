//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RazorSoft.Core;
using RazorSoft.Core.Events;
using RazorSoft.Core.Extensions;


namespace UnitTest.RazorSoft.Core {

    [TestClass]
    public class AggregatorTests {
        private static readonly EventAggregator eventAggregator = EventAggregator.Default;

        private bool hasEvent;
        private int loadId;
        private string action;

        #region test harness configuration
        [TestInitialize]
        public void InitializeTest() {
            hasEvent = false;
            loadId = -1;
            action = string.Empty;
        }
        #endregion

        [TestMethod]
        public void AddEventSubscription() {
            var type = typeof(LoadEventMessage);
            var typeName = type.Name;

            eventAggregator.CreatePublication<LoadEventMessage>();

            Assert.IsTrue(eventAggregator.Subscriptions.Any(s => s == typeName));
            Assert.IsNotNull(eventAggregator.Distribution[type]);
            Assert.AreEqual(0, eventAggregator.Distribution[type].Count);
        }

        [TestMethod]
        public void SubscribeToEventSubscription() {
            var type = typeof(LoadEventMessage);
            var typeName = type.Name;

            eventAggregator.CreatePublication<LoadEventMessage>();

            var expHandler = new SendMessage<LoadEventMessage>(ProcessLoadEvent);
            eventAggregator.Subscribe(expHandler);

            var actHandler = eventAggregator.Distribution[type]
                .FirstOrDefault(h => ((SendMessage<LoadEventMessage>)h).Equals(expHandler));

            Assert.IsNotNull(actHandler);
        }

        [TestMethod]
        public void PublishEventMessage() {
            var type = typeof(LoadEventMessage);
            var typeName = type.Name;

            eventAggregator.CreatePublication<LoadEventMessage>();

            var expHandler = new SendMessage<LoadEventMessage>(ProcessLoadEvent);
            eventAggregator.Subscribe(expHandler);

            var expLoadInfo = new LoadEventMessage(1, "NEW");
            expLoadInfo.Publish();

            Assert.AreEqual(true, hasEvent);
            Assert.AreEqual(expLoadInfo.LoadId, loadId);
            Assert.AreEqual(expLoadInfo.Action, action);

            //  now test multiple subs
            var hasNewLoadEvent = false;
            expLoadInfo = new LoadEventMessage(2, "ASSIGNED");

            eventAggregator.Subscribe((LoadEventMessage m) => {
                hasNewLoadEvent = true;
                loadId = m.LoadId;
                action = m.Action;

                Debug.WriteLine($"Load[{m.LoadId}]: {m.Action}");
            });

            expLoadInfo.Publish();

            Assert.IsTrue(hasNewLoadEvent);
            Assert.AreEqual(expLoadInfo.LoadId, loadId);
            Assert.AreEqual(expLoadInfo.Action, action);
        }

        [TestMethod]
        public void EventMessageAsInterface() {
            var expLoadId = 5;
            var expLoadAction = "DROPPED";

            eventAggregator.CreatePublication<ILoadEventMessage>();
            eventAggregator.Subscribe<ILoadEventMessage>(ProcessLoadEvent);

            ((ILoadEventMessage)new LoadEventMessage(expLoadId, expLoadAction)).Publish();

            Assert.IsTrue(hasEvent);
            Assert.AreEqual(expLoadId, loadId);
            Assert.AreEqual(expLoadAction, action);
        }

        private void ProcessLoadEvent(LoadEventMessage eventMessage) {
            hasEvent = true;
            loadId = eventMessage.LoadId;
            action = eventMessage.Action;
        }

        private void ProcessLoadEvent(ILoadEventMessage eventMessage) {
            hasEvent = true;
            loadId = eventMessage.LoadId;
            action = eventMessage.Action;
        }

        private class LoadEventMessage : ILoadEventMessage {
            public int LoadId { get; }

            public string Action { get; }

            internal LoadEventMessage(int loadId, string action) {
                LoadId = loadId;
                Action = action;
            }

        }

        public interface ILoadEventMessage : IEventMessage {
            int LoadId { get; }
            string Action { get; }
        }
    }

}
