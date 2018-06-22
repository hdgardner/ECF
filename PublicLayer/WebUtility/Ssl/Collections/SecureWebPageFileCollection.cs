using System;
using System.Collections;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The SecureWebPageFileCollection class houses a collection of SecureWebPageFile instances.
	/// </summary>
	public class SecureWebPageFileCollection : SecureWebPageItemCollection
	{
		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public SecureWebPageFileCollection() : base()
		{
		}

		/// <summary>
		/// Indexer for the collection.
		/// </summary>
		public SecureWebPageFile this [int index]
		{
			get { return (SecureWebPageFile) List[index]; }
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(SecureWebPageFile item)
		{
			return List.Add(item);
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The index to insert the item at.</param>
		/// <param name="item">The item to insert.</param>
		public void Insert(int index, SecureWebPageFile item)
		{
			List.Insert(index, item);
		}

		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SecureWebPageFile item)
		{
			List.Remove(item);
		}
	}
}
