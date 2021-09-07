// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
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
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        IEnumerable<TData> All<TData>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        TData Add<TData>(TData item);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove<TData>(TData item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectContext<T> : IObjectContext where T : class, new() {
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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(T item);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        bool Update(IEnumerable<T> itemList);
    }
}
