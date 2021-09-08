// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;


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
    public abstract class JsonRepository<TEntity> : JsonRepository, IObjectContext<TEntity> where TEntity : class, new() {
        #region		fields
        private readonly InternalCache cache = new();
        #endregion	fields


        #region		properties
        /// <summary>
        /// 
        /// </summary>
        public OnValidateAdd<TEntity> ValidateAdd {
            get => cache.OnValidateAdd;
            set => cache.OnValidateAdd = value;
        }

        string IObjectContext<TEntity>.DataSource => $@"{DataPath}{DataFile}";
        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFile"></param>
        public JsonRepository(string dataFile) : base(dataFile) { }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> IObjectContext<TEntity>.All() {
            var iterator = All().GetEnumerator();
            while (iterator.MoveNext()) {
                if(iterator.Current is TEntity entity) {
                    yield return entity;
                }
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TEntity Add(TEntity item) {
            cache.Add(item);

            return item;
        }
        /// <summary>
        /// 
        /// </summary>
        void IObjectContext<TEntity>.Commit() {
            Save();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IObjectContext<TEntity>.Remove(TEntity item) {
            return cache.Remove(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IObjectContext<TEntity>.Update(TEntity item) {
            throw new NotImplementedException("Update item not implemented");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        bool IObjectContext<TEntity>.Update(IEnumerable<TEntity> itemList) {
            throw new NotImplementedException("Update range not implemented");
        }

        #endregion	public methods & functions


        #region		non-public methods & functions
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override IList Cache() {
            return cache;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"></param>
        protected override void OnDataLoaded(IEnumerable data) {
            cache.AddRange(data.OfType<TEntity>());
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnInitialized() {
            Load();
        }
        #endregion	non-public methods & functions


        #region     private class
        private class InternalCache : IEnumerable<TEntity>, IList, IList<TEntity>, ICollection<TEntity>, IQueryable<TEntity> {
            #region		fields
            private readonly object syncRoot = new();

            private readonly List<TEntity> entities = new();

            #endregion	fields


            #region		properties
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public TEntity this[int index] {
                get => entities[index];
                set => entities[index] = value;
            }
            /// <summary>
            /// 
            /// </summary>
            public int Count => entities.Count;
            /// <summary>
            /// 
            /// </summary>
            public bool IsReadOnly => ((ICollection<TEntity>)entities).IsReadOnly;
            /// <summary>
            /// 
            /// </summary>
            public Type ElementType => typeof(TEntity);
            /// <summary>
            /// 
            /// </summary>
            public Expression Expression => GetQueryable().Expression;
            /// <summary>
            /// 
            /// </summary>
            public IQueryProvider Provider => GetQueryable().Provider;
            /// <summary>
            /// 
            /// </summary>
            public bool IsFixedSize => false;
            /// <summary>
            /// 
            /// </summary>
            public bool IsSynchronized => throw new NotImplementedException("when is this property used?");
            /// <summary>
            /// 
            /// </summary>
            public object SyncRoot => syncRoot;
            /// <summary>
            /// 
            /// </summary>
            public OnValidateAdd<TEntity> OnValidateAdd { get; internal set; } = (o) => true;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            #endregion	properties


            #region		constructors & destructors
            internal InternalCache() {

            }
            #endregion	constructors & destructors


            #region		public methods & functions

            public void Add(TEntity item) {
                if (OnValidateAdd(item)) {
                    entities.Add(item);
                }
            }

            public int Add(object value) {
                var result = -1;

                if(value is TEntity item) {
                    Add(item);
                    result = IndexOf(item);
                }

                return result;
            }

            public void AddRange(IEnumerable<TEntity> items) {
                entities.AddRange(items);
            }

            public void Clear() {
                entities.Clear();
            }

            public bool Contains(TEntity item) {
                return entities.Contains(item);
            }

            public bool Contains(object value) {
                var result = false;

                if(value is TEntity item) {
                    result = entities.Contains(item);
                }

                return result;
            }

            public void CopyTo(TEntity[] array, int arrayIndex) {
                entities.CopyTo(array, arrayIndex);
            }

            public void CopyTo(Array array, int index) {
                Buffer.BlockCopy(entities.ToArray(), 0, array, index, entities.Count);
            }

            public int IndexOf(TEntity item) {
                return entities.IndexOf(item);
            }

            public int IndexOf(object value) {
                var result = -1;

                if (value is TEntity item) {
                    result = entities.IndexOf(item);
                }

                return result;
            }

            public void Insert(int index, TEntity item) {
                entities.Insert(index, item);
            }

            public void Insert(int index, object value) {
                if(value is TEntity item) {
                    entities.Insert(index, item);
                }
            }

            public bool Remove(TEntity item) {
                return entities.Remove(item);
            }

            public void Remove(object value) {
                if(value is TEntity item) {
                    entities.Remove(item);
                }
            }

            public void RemoveAt(int index) {
                entities.RemoveAt(index);
            }

            public IEnumerator<TEntity> GetEnumerator() {
                return entities.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            #endregion	public methods & functions


            #region		non-public methods & functions

            private IQueryable<TEntity> GetQueryable() {
                return new EnumerableQuery<TEntity>(this).Select(x => x).AsQueryable();
            }
            #endregion	non-public methods & functions
        }
        #endregion  private class
    }
}
