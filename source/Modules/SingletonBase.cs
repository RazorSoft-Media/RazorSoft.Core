// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Reflection;


namespace RazorSoft.Core {

    /// <summary>
    /// Base singleton implementation to provide consistency and standardize singleton behavior.
    /// </summary>
    /// <typeparam name="TSingleton">singleton type</typeparam>
    public abstract class SingletonBase<TSingleton> where TSingleton : class {
        private static readonly Lazy<TSingleton> instance = new Lazy<TSingleton>(() => GetConstructor());


        /// <summary>
        /// Protected Instance
        /// </summary>
        protected static TSingleton Singleton {
            get { return instance.Value; }
        }


        /// <summary>
        /// Protected ctor
        /// </summary>
        protected SingletonBase() { }
        

        /// <summary>
        /// Constructs the singleton class. Requires a non-public, parameterless constructor.
        /// </summary>
        /// <returns>Singleton</returns>
        private static TSingleton GetConstructor( ) {
            var type = typeof(TSingleton);
            var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (ctor == null) {
                throw new InvalidOperationException("Singleton must have a non-public parameterless constructor");
            }

            return (ctor.Invoke(null) as TSingleton);
        }
    }
}
