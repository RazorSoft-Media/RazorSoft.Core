// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Collections;
using System.Collections.Generic;


namespace RazorSoft.Core.Data {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logEntry"></param>
    public delegate void Log(string logEntry);

    /// <summary>
    /// 
    /// </summary>
    public interface IObjectContext : IDisposable {
        /// <summary>
        /// 
        /// </summary>
        string DataPath { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable All();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        object Add(object item);
        /// <summary>
        /// 
        /// </summary>
        void Commit();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove(object item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectContext<T> : IDisposable where T : class, new() {
        /// <summary>
        /// 
        /// </summary>
        OnValidateAdd<T> ValidateAdd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> All();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        T Add(T item);
        /// <summary>
        /// 
        /// </summary>
        void Commit();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove(T item);
        /// <summary>
        /// Update replaces the item in the cache. If the item is not found, it is added.
        /// </summary>
        bool Update(T item);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        bool Update(IEnumerable<T> itemList);
    }
}
