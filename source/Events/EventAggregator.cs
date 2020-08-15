/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;
using System.Linq;
using System.Collections.Generic;


namespace RazorSoft.Core.Events {
    public delegate void SendMessage<TEventMessage>(TEventMessage eventMessage);

    public class EventAggregator : SingletonBase<EventAggregator> {
        private readonly Dictionary<Type, IList<Delegate>> subsDistribution;

        public static EventAggregator Default => Singleton;

        public IEnumerable<string> Subscriptions => subsDistribution.Keys.Select(k => k.Name);
        public IDistribution Distribution => new SubsDistribution(subsDistribution);

        private EventAggregator() {
            subsDistribution = new Dictionary<Type, IList<Delegate>>();
        }

        public void CreatePublication<TEventMessage>() {
            var type = typeof(TEventMessage);

            if (!subsDistribution.ContainsKey(type)) {
                subsDistribution.Add(type, new List<Delegate>());
            }
        }

        public void Subscribe<TEventMessage>(SendMessage<TEventMessage> messageHandler) {
            var type = typeof(TEventMessage);
            IList<Delegate> distribution;

            if (!subsDistribution.TryGetValue(type, out distribution)) {
                CreatePublication<TEventMessage>();
                distribution = subsDistribution[type];
            }

            distribution.Add(messageHandler);
        }

        public void Unsubscribe<TEventMessage>(SendMessage<TEventMessage> messageHandler) {
            var type = typeof(TEventMessage);
            IList<Delegate> distribution;

            if (!subsDistribution.TryGetValue(type, out distribution)) {
                CreatePublication<TEventMessage>();
                distribution = subsDistribution[type];
            }

            distribution.Remove(messageHandler);
        }

        internal void Publish<TEventMessage>(TEventMessage eventMessage) where TEventMessage : IEventMessage {
            if(subsDistribution.TryGetValue(typeof(TEventMessage), out IList<Delegate> subDistro)) {
                var distribution = new List<Delegate>(subDistro);

                foreach(var route in distribution) {
                    var send = (SendMessage<TEventMessage>)route;

                    send.Invoke(eventMessage);
                }
            }
        }

        private class SubsDistribution : IDistribution {
            private readonly Dictionary<Type, IList<Delegate>> distribution;

            internal SubsDistribution(Dictionary<Type, IList<Delegate>> subsDistribution) {
                distribution = subsDistribution;
            }

            public IReadOnlyList<Delegate> this [Type type] => distribution[type].ToList();

        }
    }

    public interface IDistribution {
        IReadOnlyList<Delegate> this [Type type] { get; }
    }
}
