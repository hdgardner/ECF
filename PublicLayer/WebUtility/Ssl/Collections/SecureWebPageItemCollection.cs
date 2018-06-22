using System;
using System.Collections;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The SecureWebPageItemCollection class houses a collection of SecureWebPageItem instances.
	/// </summary>
	public class SecureWebPageItemCollection : CollectionBase
	{
		/// <summary>
		/// Initialize an instance of this collection.
		/// </summary>
		public SecureWebPageItemCollection()
		{
		}

		/// <summary>
		/// Returns the index of a specified item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns the index of the item.</returns>
		public int IndexOf(SecureWebPageItem item)
		{
			return List.IndexOf(item);
		}

		/// <summary>
		/// Returns the index of an item with the specified path in the collection.
		/// </summary>
		/// <param name="Path">The path of the item to find.</param>
		/// <returns>Returns the index of the item with the path.</returns>
		public int IndexOf(string path)
		{
			// Create a comparer for sorting and searching
			SecureWebPageItemComparer Comparer = new SecureWebPageItemComparer();
			InnerList.Sort(Comparer);
			return InnerList.BinarySearch(path, Comparer);
		}

	}
}
