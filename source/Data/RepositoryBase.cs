// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;


namespace RazorSoft.Core.Data {

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public delegate bool OnValidateAdd<TEntity>(TEntity entity);

    /// <summary>
    /// 
    /// </summary>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, new() {
		#region		fields
		private readonly IObjectContext<TEntity> context;
        #endregion	fields


        #region		properties
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string DataSource => context.DataSource;

        /// <summary>
        /// 
        /// </summary>
        protected OnValidateAdd<TEntity> OnValidate {
            get => context.ValidateAdd;
            set => context.ValidateAdd = value;
        }
		#endregion	properties


		#region		constructors & destructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectContext"></param>
		protected RepositoryBase(IObjectContext<TEntity> objectContext){
			context = objectContext;

            OnValidate = OnAdd;
		}
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TEntity Add(TEntity item) {
            return context.Add(item);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Enumerated collection of TENTITY</returns>
        public IEnumerable<TEntity> All() {
            return context.All();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Commit() {
            context.Commit();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item">TENTITY item to add</param>
        /// <returns>TRUE if success; otherwise FALSE</returns>
        public bool Delete(TEntity item) {
            return context.Remove(item);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate">query criteria</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate) {
            var query = predicate.Compile();
            return context.All()
                .Where(query);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item">item to update</param>
        public bool Update(TEntity item) {
            return context.Update(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public bool Update(IEnumerable<TEntity> itemList) {
            return context.Update(itemList);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose() {
            context.Dispose();
        }
        #endregion	public methods & functions


        #region		non-public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool OnAdd(TEntity entity) {
            return true;
        }

        #endregion	non-public methods & functions
    }
}
