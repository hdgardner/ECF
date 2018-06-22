using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Simple object collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SimpleObjectCollectionBase<T> : StorageCollectionBase where T : SimpleObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleObjectCollectionBase&lt;T&gt;"/> class.
        /// </summary>
        public SimpleObjectCollectionBase()
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
            List<SimpleObject> newList = new List<SimpleObject>(this.Count);
            foreach (SimpleObject item in this.InnerList)
            {
                newList.Add(item);
            }

            List<SimpleObject> deletedList = new List<SimpleObject>(this.DeletedList.Count);
            foreach (SimpleObject item in this.DeletedList)
            {
                deletedList.Add(item);
            }

            // Remove items that were deleted
            foreach (SimpleObject item in deletedList)
            {
                item.AcceptChanges();
            }

            foreach (SimpleObject item in newList)
            {
                item.AcceptChanges();
            }
        }

        /// <summary>
        /// Marks all instances in collection as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal virtual void MarkNew()
        {
            foreach (SimpleObject item in this.InnerList)
            {
                item.MarkNew();
            }
        }
    }
}
