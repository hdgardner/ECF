using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class StorageCollectionBase : IList, ICollection
    {
        // Fields
        private ArrayList list;
        private ArrayList _deletedList;

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollectionBase"/> class.
        /// </summary>
        protected StorageCollectionBase()
        {
            this.list = new ArrayList();
            this._deletedList = new ArrayList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollectionBase"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        protected StorageCollectionBase(int capacity)
        {
            this.list = new ArrayList(capacity);
            this._deletedList = new ArrayList();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {
            this.InnerList.Clear();
            this._deletedList.Clear();
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "Index out of range");
            }
            object obj2 = this.InnerList[index];
            this.InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">array is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        /// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        int IList.Add(object value)
        {
            int index = this.InnerList.Add(value);
            return index;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return this.InnerList.Contains(value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>.</param>
        /// <returns>
        /// The index of value if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return this.InnerList.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception>
        void IList.Insert(int index, object value)
        {
            if ((index < 0) || (index > this.InnerList.Count))
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange");
            }
            this.InnerList.Insert(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            int index = this.InnerList.IndexOf(value);
            if (index < 0)
            {
                throw new ArgumentException("Arg_RemoveArgNotFound");
            }
            this.InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get { return List[index]; }
            set { List[index] = value; }
        }

        // Properties
        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        [ComVisible(false)]
        public int Capacity
        {
            get
            {
                return this.InnerList.Capacity;
            }
            set
            {
                this.InnerList.Capacity = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.</returns>
        public int Count
        {
            get
            {
                if (this.list != null)
                {
                    return this.list.Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the inner list.
        /// </summary>
        /// <value>The inner list.</value>
        protected ArrayList InnerList
        {
            get
            {
                if (this.list == null)
                {
                    this.list = new ArrayList();
                }
                return this.list;
            }
        }

        /// <summary>
        /// Gets the deleted list.
        /// </summary>
        /// <value>The deleted list.</value>
        protected ArrayList DeletedList
        {
            get
            {
                if (this._deletedList == null)
                {
                    this._deletedList = new ArrayList();
                }
                return this._deletedList;
            }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        protected IList List
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        bool ICollection.IsSynchronized
        {
            get
            {
                return this.InnerList.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
        object ICollection.SyncRoot
        {
            get
            {
                return this.InnerList.SyncRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get
            {
                return this.InnerList.IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.</returns>
        bool IList.IsReadOnly
        {
            get
            {
                return this.InnerList.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        object IList.this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
                }
                return this.InnerList[index];
            }
            set
            {
                if ((index < 0) || (index >= this.InnerList.Count))
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
                }
                object oldValue = this.InnerList[index];
                this.InnerList[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public virtual void Remove(object value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void DeleteItem(object value)
        {
            _deletedList.Add(value);
            InnerList.Remove(value);
        }

        /// <summary>
        /// Removeds the deleted item.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void RemovedDeletedItem(object value)
        {
            _deletedList.Remove(value);
        }

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new StorageCollectionEnumerator(InnerList);
        }

        #endregion

    }

    /// <summary>
    /// Implements operations for the storage collection enumerator. (Inherits <see cref="IDisposable"/>, <see cref="IEnumerator"/>.)
    /// </summary>
    [Serializable]
    public sealed class StorageCollectionEnumerator : IDisposable, IEnumerator
    {
        // Fields
        private ArrayList collection;
        private int index;

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollectionEnumerator"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        internal StorageCollectionEnumerator(ArrayList list)
        {

            this.index = -1;
            collection = new ArrayList();
            foreach (object o in list)
            {
                collection.Add(o);
            }

//            ((IEnumerator)this).Reset();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        bool IEnumerator.MoveNext()
        {
            //if(this.index == -1)
            //    ((IEnumerator)this).Reset();

            this.index++;
            if (this.index >= this.collection.Count)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        void IEnumerator.Reset()
        {
            //this.collection = new ArrayList();
            this.index = -1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <value></value>
        /// <returns>The current element in the collection.</returns>
        /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
        object IEnumerator.Current
        {
            get
            {
                if ((this.index < 0) || (this.index >= this.collection.Count))
                {
                    throw new InvalidOperationException("EnumeratorPositionBad");
                }
                return this.collection[this.index];
            }
        }
    }
}
