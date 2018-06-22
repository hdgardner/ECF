using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Mediachase.Cms.Pages
{
	/// <summary>
	/// Summary description for NodeControlSettingsCollection
	/// </summary>
	[Serializable]
	public class NodeControlSettingsCollection : System.Collections.Specialized.NameObjectCollectionBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
        /// </summary>
		public NodeControlSettingsCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="col">The col.</param>
		public NodeControlSettingsCollection(NodeControlSettingsCollection col)
		{
			this.Add(col);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		public NodeControlSettingsCollection(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="equalityComparer">The equality comparer.</param>
		public NodeControlSettingsCollection(IEqualityComparer equalityComparer)
			: base(equalityComparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		/// <param name="col">The col.</param>
		public NodeControlSettingsCollection(int capacity, NodeControlSettingsCollection col)
		{
			this.Add(col);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" qualify="true"/> object that contains the information required to serialize the new <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" qualify="true"/> object that contains the source and destination of the serialized stream associated with the new <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.</param>
		protected NodeControlSettingsCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NodeControlSettingsCollection"/> class.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		/// <param name="equalityComparer">The equality comparer.</param>
		public NodeControlSettingsCollection(int capacity, IEqualityComparer equalityComparer)
			: base(capacity, equalityComparer)
		{
		}

		/// <summary>
		/// Adds the specified c.
		/// </summary>
		/// <param name="c">The c.</param>
		public void Add(NodeControlSettingsCollection c)
		{
			int MaxCount = c.Count;
			for (int Index = 0; Index < MaxCount; Index++)
			{
				string text = c.GetKey(Index);
				ControlSettings data = c.GetValue(Index);

				this.Add(text, data);
				// this[text].IsModified = true;
			}
		}
		
		/// <summary>
		/// Adds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public virtual void Add(string name, ControlSettings value)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is ReadOnly");
			}

			this.BaseAdd(name, value);
			//  this[name].IsModified = true;
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is ReadOnly");
			}
			this.BaseClear();
		}

		/// <summary>
		/// Gets the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public virtual ControlSettings Get(int index)
		{
			return (ControlSettings)base.BaseGet(index);
		}

		/// <summary>
		/// Gets the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public virtual ControlSettings Get(string name)
		{
			return (ControlSettings)base.BaseGet(name);
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public virtual string GetKey(int index)
		{
			if (this.Count > 0)
				return this.BaseGetKey(index);
			else
				return string.Empty;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public virtual ControlSettings GetValue(int index)
		{
			return (ControlSettings)this.BaseGet(index);
		}

		/// <summary>
		/// Removes the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public virtual void Remove(string name)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is ReadOnly");
			}

			this.BaseRemove(name);
			_isModified = true;
		}

		/// <summary>
		/// Sets the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public virtual void Set(string name, object value)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is ReadOnly");
			}

			this.BaseSet(name, value);
		}

		// Properties
		/// <summary>
		/// Gets all keys.
		/// </summary>
		/// <value>All keys.</value>
		public virtual string[] AllKeys
		{
			get
			{
				return this.BaseGetAllKeys();
			}
		}

        /// <summary>
        /// Gets all values.
        /// </summary>
        /// <value>All values.</value>
		public virtual ControlSettings[] AllValues
		{
			get
			{
				ControlSettings cs = new ControlSettings();
				return (ControlSettings[])this.BaseGetAllValues(cs.GetType());
			}

		}
		/// <summary>
		/// Gets the <see cref="ControlSettings"/> at the specified index.
		/// </summary>
		/// <value></value>
		public ControlSettings this[int index]
		{
			get
			{
				return (ControlSettings)this.BaseGet(index);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ControlSettings"/> with the specified name.
		/// </summary>
		/// <value></value>
		public ControlSettings this[string name]
		{
			get
			{
				return (ControlSettings)this.BaseGet(name);
			}
			set
			{
				if (base.IsReadOnly)
				{
					throw new NotSupportedException("Collection is ReadOnly");
				}

				this.BaseSet(name, value);
			}
		}

		private bool _isModified = false;

        /// <summary>
        /// Gets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
		public bool IsModified
		{
			get
			{
				foreach (ControlSettings cs in this.AllValues)
				{
					if (cs.IsModified)
						return true;
				}
				return _isModified;
			}
		}
	}
}