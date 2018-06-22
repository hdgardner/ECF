using System;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The SecureWebPageDirectoryCollection class houses a collection of SecureWebPageDirectory instances.
	/// </summary>
	public class SecureWebPageDirectoryCollection : SecureWebPageItemCollection
	{
		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public SecureWebPageDirectoryCollection() : base()
		{
		}

		/// <summary>
		/// Indexer for the collection.
		/// </summary>
		public SecureWebPageDirectory this [int index]
		{
			get { return (SecureWebPageDirectory) List[index]; }
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(SecureWebPageDirectory item)
		{
			return List.Add(item);
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The index to insert the item at.</param>
		/// <param name="item">The item to insert.</param>
		public void Insert(int index, SecureWebPageDirectory item)
		{
			List.Insert(index, item);
		}

		/// <summary>
		/// Removes an item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SecureWebPageDirectory item)
		{
			List.Remove(item);
		}
	}
}
