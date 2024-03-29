﻿// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace RazorSoft.Core.Data {

    /// <summary>
    /// Common generic repository interface
    /// </summary>
    /// <typeparam name="T">TYPE T</typeparam>
    public interface IRepository<T> : IDisposable where T : class, new() {
        /// <summary>
        /// Get the data source name
        /// </summary>
        string DataSource { get; }
        /// <summary>
        /// Retrieves all records of the specified Type
        /// </summary>
        IEnumerable<T> All();
        /// <summary>
        /// Finds all records matching specified query criteria
        /// </summary>
        IEnumerable<T> Get(Expression<Func<T, bool>> query);
        /// <summary>
        /// Add a record of the specified Type
        /// </summary>
        T Add(T item);
        /// <summary>
        /// Commit data changes to the underlying data store
        /// </summary>
        void Commit();
        /// <summary>
        /// Update data object
        /// </summary>
        bool Update(T item);
        /// <summary>
        /// Update data object batch
        /// </summary>
        bool Update(IEnumerable<T> itemList);
        /// <summary>
        /// Delete given record
        /// </summary>
        bool Delete(T item);
    }
}
