// Copyright: ©2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
//
using RazorSoft.Core.Messaging;


namespace RazorSoft.Core.Extensions {

    /// <summary>
    /// 
    /// </summary>
    public static class MessageExtensions {

        /// <summary>
        /// Publish event message
        /// </summary>
        /// <typeparam name="TEventMessage">message type</typeparam>
        /// <param name="eventMessage">message</param>
		public static void Publish<TEventMessage>(this TEventMessage eventMessage) where TEventMessage : IEventMessage {
            EventPublisher.Default.Publish(eventMessage);
        }
        /// <summary>
        /// Publish event message with specified actions
        /// </summary>
        /// <typeparam name="TEventMessage">message type</typeparam>
        /// <param name="eventMessage">message</param>
        /// <param name="actions">actions to perform on message</param>
        public static void Publish<TEventMessage>(this TEventMessage eventMessage, params Action<TEventMessage>[] actions) where TEventMessage : IEventMessage {
            foreach(var task in actions) {
                task(eventMessage);
            }

            EventPublisher.Default.Publish(eventMessage);
        }
        /// <summary>
        /// Publish event message as derived from function
        /// </summary>
        /// <typeparam name="TEventMessage">message type</typeparam>
        /// <param name="messageFunc">message function</param>
        public static void Publish<TEventMessage>(this Func<TEventMessage> messageFunc) where TEventMessage : IEventMessage {
            messageFunc().Publish();
        }
        /// <summary>
        /// Publish event message as derived from function and with specified actions
        /// </summary>
        /// <typeparam name="TEventMessage">message type</typeparam>
        /// <param name="messageFunc">message function</param>
        /// <param name="actions">actions to perform on message</param>
        public static void Publish<TEventMessage>(this Func<TEventMessage> messageFunc, params Action<TEventMessage>[] actions) where TEventMessage : IEventMessage {
            var eventMessage = messageFunc();

            foreach(var task in actions) {
                task(eventMessage);
            }

            eventMessage.Publish();
        }
        /// <summary>
        /// Execute ICommandTask
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="task">task target</param>
        /// <param name="func">task</param>
        /// <returns>(TResult) result</returns>
        public static TResult Execute<TTarget, TResult>(this ICommandTask task, Func<TTarget, TResult> func) {
            return CommandRouter.Default.Execute(task, func);
        }
        /// <summary>
        /// Execute Command on target
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="target">task target</param>
        /// <param name="func">task</param>
        /// <returns>(TResult) result</returns>
        public static TResult Execute<TTarget, TResult>(this Command<TTarget> target, Func<TTarget, TResult> func) {
            return CommandRouter.Default.Execute(func);
        }
    }
}
