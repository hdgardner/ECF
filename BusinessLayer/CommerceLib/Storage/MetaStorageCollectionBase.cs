using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Collection of MetaStorageBase objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MetaStorageCollectionBase<T> : StorageCollectionBase where T : MetaStorageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageCollectionBase&lt;T&gt;"/> class.
        /// </summary>
        public MetaStorageCollectionBase()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public new T this[int index]
        {
            get { return (T)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual int Add(T value)
        {
            // associate collection with item
            value.StorageCollection = this;

            return List.Add(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual int IndexOf(T value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void Remove(T value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public virtual void AcceptChanges()
        {
            List<MetaStorageBase> newList = new List<MetaStorageBase>(this.Count);
            foreach (MetaStorageBase item in this.InnerList)
            {
                newList.Add(item);
            }

            List<MetaStorageBase> deletedList = new List<MetaStorageBase>(this.DeletedList.Count);
            foreach (MetaStorageBase item in this.DeletedList)
            {
                deletedList.Add(item);
            }

            // Remove items that were deleted
            foreach (MetaStorageBase item in deletedList)
            {
                item.AcceptChanges();
            }
            
            foreach (MetaStorageBase item in newList)
            {
                item.AcceptChanges();
            }
        }

        /// <summary>
        /// Refreshes the collection.
        /// </summary>
        public virtual void RefreshCollection()
        {

        }

        /// <summary>
        /// Marks all instances in collection as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal virtual void MarkNew()
        {
            foreach (MetaStorageBase item in this.InnerList)
            {
                item.MarkNew();
            }
        }
        /*
        /// <summary>
        /// Duplicates this instance. Creates a copy of collection with all items marked for addition.
        /// </summary>
        /// <returns></returns>
        public virtual MetaStorageCollectionBase<T> Duplicate()
        {

        }
         * */

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        public virtual T[] ToArray()
        {
            List<T> list = new List<T>();
            foreach (object item in List)
            {
                list.Add((T)item);
            }

            return list.ToArray();
        }
    }
}
