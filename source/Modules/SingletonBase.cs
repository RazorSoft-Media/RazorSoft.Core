//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Reflection;


namespace RazorSoft.Core {

    public abstract class SingletonBase<TSingleton> where TSingleton : class {
        private static readonly Lazy<TSingleton> instance = new Lazy<TSingleton>(() => GetConstructor());

        protected static TSingleton Singleton {
            get { return instance.Value; }
        }
        
        private static TSingleton GetConstructor( ) {
            var type = typeof(TSingleton);
            var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (ctor == null) {
                throw new InvalidOperationException("Singleton must have a non-punlic parameterless constructor");
            }

            return (ctor.Invoke(null) as TSingleton);
        }
    }
}
