//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
//
using RazorSoft.Core.Events;
using RazorSoft.Core.Messaging;


namespace RazorSoft.Core.Extensions {

    public static class MessageExtensions {
		public static void Publish<TEventMessage>(this TEventMessage eventMessage) where TEventMessage : IEventMessage {
            EventAggregator.Default.Publish(eventMessage);
        }

        public static void Publish<TEventMessage>(this TEventMessage eventMessage, params Action<TEventMessage>[] actions) where TEventMessage : IEventMessage {
            foreach(var act in actions) {
                act(eventMessage);
            }

            EventAggregator.Default.Publish(eventMessage);
        }
		public static void Publish<TEventMessage>(this Func<TEventMessage> messageFunc) where TEventMessage : IEventMessage {
            messageFunc().Publish();
        }

        public static void Publish<TEventMessage>(this Func<TEventMessage> messageFunc, params Action<TEventMessage>[] actions) where TEventMessage : IEventMessage {
            var eventMessage = messageFunc();

            foreach(var act in actions) {
                act(eventMessage);
            }

            eventMessage.Publish();
        }

        public static TResult Execute<TTarget, TResult>(this ITransaction transaction, Func<TTarget, TResult> func) {
            return CommandRouter.Default.Execute(transaction, func);
        }

        public static TResult Execute<TTarget, TResult>(this Command<TTarget> target, Func<TTarget, TResult> func) {
            return CommandRouter.Default.Execute(func);
        }
    }
}
