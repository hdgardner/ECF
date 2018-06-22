using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
	[Serializable]
    public class ViewCollectionBase<T> : CollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCollectionBase&lt;T&gt;"/> class.
        /// </summary>
        public ViewCollectionBase()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
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
            return List.Add(value);
        }
    }
}
