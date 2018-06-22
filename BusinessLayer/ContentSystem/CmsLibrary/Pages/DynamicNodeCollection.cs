using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Pages
{
    [Serializable]
    public class DynamicNodeCollection : System.Collections.CollectionBase
    {
        private PageDocument _ownerDocument = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicNodeCollection"/> class.
        /// </summary>
        /// <param name="ownerDocument">The owner document.</param>
        public DynamicNodeCollection(PageDocument ownerDocument)
        {
            _ownerDocument = ownerDocument;
        }
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Cms.Pages.DynamicNode"/> at the specified index.
        /// </summary>
        /// <value></value>
        public DynamicNode this[int index]
        {
            get
            {
                return ((DynamicNode)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int Add(DynamicNode value)
        {
            //value.IsModified = true;
            return (List.Add(value));
    
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(DynamicNode value)
        {
            return (List.IndexOf(value));
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, DynamicNode value)
        {
            List.Insert(index, value);
            value.IsModified = true;
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(DynamicNode value)
        {
            List.Remove(value);
            foreach (DynamicNode dn in this.List)
            {
                if ((dn.ControlPlaceId == value.ControlPlaceId)&&((Convert.ToInt32(dn.ControlPlaceIndex)>Convert.ToInt32(value.ControlPlaceIndex))))
                {
                    dn.ControlPlaceIndex = dn.ControlPlaceIndex-1;
                    dn.IsModified = true;
                }
            
            }
            

            _isModified = true;
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(DynamicNode value)
        {
            // If value is not of type Int16, this will return false.
            return (List.Contains(value));
        }

		//DV: 2007-09-12
        /// <summary>
        /// Loads the by UID.
        /// </summary>
        /// <param name="UID">The UID.</param>
        /// <returns></returns>
		public DynamicNode LoadByUID(string UID)
		{
			for (int index = 0; index < this.Count; index++)
			{
				if (this[index].NodeUID == UID)
					return this[index];
			}

			return null;
		}

        /// <summary>
        /// Performs additional custom processes when clearing the contents of the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        protected override void OnClear()
        {
            // Remove all Owner from Node
            for (int index = 0; index < this.Count; index++)
            {
                this[index].SetOwnerDocument(null);
            }
        }

        /// <summary>
        /// Performs additional custom processes before inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnInsert(int index, object value)
        {
            ((Node)value).SetOwnerDocument(_ownerDocument);
        }

        /// <summary>
        /// Performs additional custom processes when removing an element from the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> can be found.</param>
        /// <param name="value">The value of the element to remove from <paramref name="index"/>.</param>
        protected override void OnRemove(int index, object value)
        {
            ((Node)value).SetOwnerDocument(null);
        }

        /// <summary>
        /// Performs additional custom processes before setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSet(int index, object oldValue, object newValue)
        {
            ((Node)oldValue).SetOwnerDocument(null);
            ((Node)newValue).SetOwnerDocument(_ownerDocument);
        }

        /// <summary>
        /// Performs additional custom processes when validating a value.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="value"/> is null.</exception>
        protected override void OnValidate(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

			Node node = value as Node;

			if (node == null)
				throw new ArgumentException("The value should be Node.", "value");

			if (node.OwnerDocument != null && node.OwnerDocument != _ownerDocument)
				throw new Exception("The node is currently assigned to another GanttView control.");
        }

        private bool _isModified = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsModified
        {
            get
            {
                foreach (DynamicNode dn in this)
                {
                    if (dn.IsModified)
                        return true;
                }
                return _isModified;
            }
            set
            {
                _isModified = value;
            
            }
        }


    }


}

