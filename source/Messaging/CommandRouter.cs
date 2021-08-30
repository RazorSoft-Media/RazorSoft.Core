// Copyright: ©2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Messaging {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns></returns>
    public delegate TTarget Command<TTarget>();

    /// <summary>
    /// CommandRouter: singleton
    /// Routes commands to targets
    /// </summary>
    public class CommandRouter : SingletonBase<CommandRouter> {
        private readonly Dictionary<Type, object> commandTargets;


        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CommandRouter Default => Singleton;


        /// <summary>
        /// ctor
        /// </summary>
        private CommandRouter() {
            commandTargets = new Dictionary<Type, object>();
        }


        /// <summary>
        /// Add command target routing
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        /// <param name="target">specified target</param>
        public Command<TTarget> AddCommandTarget<TTarget>(TTarget target) {
            var type = typeof(TTarget);

            if (!commandTargets.ContainsKey(type)) {
                commandTargets.Add(type, target);
            }

            return () => target;
        }
        /// <summary>
        /// Remove command target routing
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        public void RemoveCommandTarget<TTarget>() {
            var type = typeof(TTarget);

            if (commandTargets.ContainsKey(type)) {
                commandTargets.Remove(type);
            }
        }


        /// <summary>
        /// Internal
        /// Executes a command on the target
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="commandTask">command interface</param>
        /// <param name="task">target command function</param>
        /// <returns>task return value</returns>
        internal TResult Execute<TTarget, TResult>(ICommandTask commandTask, Func<TTarget, TResult> task) {
            var result = default(TResult);

            if(commandTargets.TryGetValue(commandTask.Type, out object target)) {
                result = task((TTarget)target);
            }

            return result;
        }
        /// <summary>
        /// Internal
        /// Executes a command on the target
        /// </summary>
        /// <typeparam name="TTarget">target type</typeparam>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="task">target command function</param>
        /// <returns>task return value</returns>
        internal TResult Execute<TTarget, TResult>(Func<TTarget, TResult> task) {
            var result = default(TResult);

            if (commandTargets.TryGetValue(typeof(TTarget), out object target)) {
                result = task((TTarget)target);
            }

            return result;
        }
    }
}
