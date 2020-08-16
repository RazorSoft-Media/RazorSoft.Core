//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.Linq;
using System.Collections.Generic;


namespace RazorSoft.Core.Messaging {
    public delegate void SendMessage<TEventMessage>(TEventMessage eventMessage);

    /// <summary>
    /// EventPublisher: singleton
    /// Publishes events to subscribers.
    /// </summary>
    public class EventPublisher : SingletonBase<EventPublisher> {
        private readonly Dictionary<Type, IList<Delegate>> subsDistribution;


        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventPublisher Default => Singleton;


        /// <summary>
        /// Enumerated subscription names
        /// </summary>
        public IEnumerable<string> Subscriptions => subsDistribution.Keys.Select(k => k.Name);
        /// <summary>
        /// Get a collection of all distribution delegates
        /// </summary>
        public IDistribution Distribution => new SubsDistribution(subsDistribution);


        /// <summary>
        /// ctor
        /// </summary>
        private EventPublisher() {
            subsDistribution = new Dictionary<Type, IList<Delegate>>();
        }


        /// <summary>
        /// Create an event message publication
        /// </summary>
        /// <typeparam name="TEventMessage">event message type</typeparam>
        public void CreatePublication<TEventMessage>() where TEventMessage : IEventMessage {
            var type = typeof(TEventMessage);

            if (!subsDistribution.ContainsKey(type)) {
                subsDistribution.Add(type, new List<Delegate>());
            }
        }
        /// <summary>
        /// Subscribe to an event message publication
        /// </summary>
        /// <typeparam name="TEventMessage">event message type</typeparam>
        /// <param name="messageHandler">event message handler</param>
        public void Subscribe<TEventMessage>(SendMessage<TEventMessage> messageHandler) where TEventMessage : IEventMessage {
            var type = typeof(TEventMessage);
            IList<Delegate> distribution;

            if (!subsDistribution.TryGetValue(type, out distribution)) {
                CreatePublication<TEventMessage>();
                distribution = subsDistribution[type];
            }

            distribution.Add(messageHandler);
        }
        /// <summary>
        /// Unsubscribe from an event message publication
        /// </summary>
        /// <typeparam name="TEventMessage">event message type</typeparam>
        /// <param name="messageHandler">event message handler</param>
        public void Unsubscribe<TEventMessage>(SendMessage<TEventMessage> messageHandler) where TEventMessage : IEventMessage {
            var type = typeof(TEventMessage);
            IList<Delegate> distribution;

            if (!subsDistribution.TryGetValue(type, out distribution)) {
                CreatePublication<TEventMessage>();
                distribution = subsDistribution[type];
            }

            distribution.Remove(messageHandler);
        }


        /// <summary>
        /// Internal
        /// Publish an event message to all subscribers
        /// </summary>
        /// <typeparam name="TEventMessage">event message type</typeparam>
        /// <param name="eventMessage">event message to be published</param>
        internal void Publish<TEventMessage>(TEventMessage eventMessage) where TEventMessage : IEventMessage {
            if(subsDistribution.TryGetValue(typeof(TEventMessage), out IList<Delegate> subDistro)) {
                var distribution = new List<Delegate>(subDistro);

                foreach(var route in distribution) {
                    var send = (SendMessage<TEventMessage>)route;

                    send.Invoke(eventMessage);
                }
            }
        }


        /// <summary>
        /// Subscription distribution collection
        /// </summary>
        private class SubsDistribution : IDistribution {
            private readonly Dictionary<Type, IList<Delegate>> distribution;


            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="subsDistribution"></param>
            internal SubsDistribution(Dictionary<Type, IList<Delegate>> subsDistribution) {
                distribution = subsDistribution;
            }


            /// <summary>
            /// Get a read-only collection of delegates
            /// </summary>
            /// <param name="type">event message type</param>
            /// <returns>Delegate</returns>
            public IReadOnlyList<Delegate> this [Type type] => distribution[type].ToList();

        }
    }

    public interface IDistribution {
        /// <summary>
        /// Get a read-only collection of delegates
        /// </summary>
        /// <param name="type">event message type</param>
        /// <returns>Delegate</returns>
        IReadOnlyList<Delegate> this [Type type] { get; }
    }
}
