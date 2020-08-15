//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Messaging {
    public delegate TTarget Command<TTarget>(object obj);

    public class CommandRouter : SingletonBase<CommandRouter> {
        private readonly Dictionary<Type, object> commandTargets;

        public static CommandRouter Default => Singleton;

        private CommandRouter() {
            commandTargets = new Dictionary<Type, object>();
        }

        public void AddCommandTarget<TTarget>(TTarget target) {
            var type = typeof(TTarget);

            if (!commandTargets.ContainsKey(type)) {
                commandTargets.Add(type, target);
            }
        }

        public void RemoveCommandTarget<TTarget>() {
            var type = typeof(TTarget);

            if (commandTargets.ContainsKey(type)) {
                commandTargets.Remove(type);
            }
        }

        internal TResult Execute<TTarget, TResult>(ITransaction transaction, Func<TTarget, TResult> func) {
            var result = default(TResult);

            if(commandTargets.TryGetValue(transaction.Type, out object target)) {
                result = func((TTarget)target);
            }

            return result;
        }

        internal TResult Execute<TTarget, TResult>(Func<TTarget, TResult> func) {
            var result = default(TResult);

            if (commandTargets.TryGetValue(typeof(TTarget), out object target)) {
                result = func((TTarget)target);
            }

            return result;
        }
    }
}
