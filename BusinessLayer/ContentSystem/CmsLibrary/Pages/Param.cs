using System;
using System.Collections;


namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Summary description for Param
    /// </summary>
    /// 
    [Serializable]
    public class Param : System.Collections.Specialized.NameObjectCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Param"/> class.
        /// </summary>
        public Param()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Param"/> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object that contains the information required to serialize the new <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext"/> object that contains the source and destination of the serialized stream associated with the new <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.</param>
        protected Param(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        private DictionaryEntry _de = new DictionaryEntry();


        //      // Adds elements from an IDictionary into the new collection.
        //     public Param(IDictionary d, Boolean bReadOnly)
        //     {
        //   foreach ( DictionaryEntry de in d )  {
        //      this.BaseAdd( (String) de.Key, de.Value );
        //   }
        //   this.IsReadOnly = bReadOnly;
        //}

        // Gets a key-and-value pair (DictionaryEntry) using an index.
        /// <summary>
        /// Gets the <see cref="System.Collections.DictionaryEntry"/> at the specified index.
        /// </summary>
        /// <value></value>
        public DictionaryEntry this[int index]
        {
            get
            {
                _de.Key = this.BaseGetKey(index);
                _de.Value = this.BaseGet(index);
                return (_de);
            }
        }

        // Gets or sets the value associated with the specified key.
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public Object this[String key]
        {
            get
            {
                return (this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        // Gets a String array that contains all the keys in the collection.
        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <value>All keys.</value>
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        // Gets an Object array that contains all the values in the collection.
        /// <summary>
        /// Gets all values.
        /// </summary>
        /// <value>All values.</value>
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        // Gets a String array that contains all the values in the collection.
        /// <summary>
        /// Gets all string values.
        /// </summary>
        /// <value>All string values.</value>
        public String[] AllStringValues
        {
            get
            {
                return ((String[])this.BaseGetAllValues(typeof(string)));
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        /// <summary>
        /// Gets a value indicating whether this instance has keys.
        /// </summary>
        /// <value><c>true</c> if this instance has keys; otherwise, <c>false</c>.</value>
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }




    }
}