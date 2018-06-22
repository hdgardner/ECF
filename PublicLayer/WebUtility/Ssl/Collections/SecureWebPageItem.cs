using System;
using System.Collections;

namespace Mediachase.Cms.WebUtility.Ssl
{

	/// <summary>
	/// Indicates the type of security for a SecureWebPageItem.
	/// </summary>
	public enum SecurityType
	{
		/// <summary>
		/// The item should be secure.
		/// </summary>
		Secure,

		/// <summary>
		/// The item should be insecure.
		/// </summary>
		Insecure,

		/// <summary>
		/// The item should be ignored.
		/// </summary>
		Ignore
	}


	/// <summary>
	/// The SecureWebPageItemComparer class implements the IComparer interface to compare.
	/// </summary>
	public class SecureWebPageItemComparer : IComparer
	{
		/// <summary>
		/// Initialize an instance of this class.
		/// </summary>
		public SecureWebPageItemComparer()
		{
		}

		/// <summary>
		/// Compares the two objects as string and SecureWebPageItem or both SecureWebPageItem 
		/// by the Path property.
		/// </summary>
		/// <param name="x">First object to compare.</param>
		/// <param name="y">Second object to compare.</param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			// Check the type of the parameters
			if (!(x is SecureWebPageItem) && !(x is string))
				// Throw an exception for the first argument
				throw new ArgumentException("Parameter must be a SecureWebPageItem or a String.", "x");
			else if (!(y is SecureWebPageItem) && !(y is string))
				// Throw an exception for the second argument
				throw new ArgumentException("Parameter must be a SecureWebPageItem or a String.", "y");

			// Initialize the path variables
			string xPath = string.Empty;
			string yPath = string.Empty;

			// Get the path for x
			if (x is SecureWebPageItem)
				xPath = ((SecureWebPageItem) x).Path;
			else
				xPath = (string) x;

			// Get the path for y
			if (y is SecureWebPageItem)
				yPath = ((SecureWebPageItem) y).Path;
			else
				yPath = (string) y;

			// Compare the paths, ignoring case
			return string.Compare(xPath, yPath, true);
		}
	}


	/// <summary>
	/// The SecureWebPageItem class is the base class that represents entries in the &lt;secureWebPages&gt;
	/// configuration section.
	/// </summary>
	public class SecureWebPageItem
	{
		// Fields
		private SecurityType secure = SecurityType.Secure;
		private string path = string.Empty;

		/// <summary>
		/// Gets or sets the type of security for this directory or file.
		/// </summary>
		public SecurityType Secure
		{
			get { return secure; }
			set { secure = value; }
		}

		/// <summary>
		/// Gets or sets the path of this directory or file.
		/// </summary>
		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// Creates an instance of this class.
		/// </summary>
		public SecureWebPageItem()
		{
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="ignore">A flag to ignore security for the directory or file.</param>
		public SecureWebPageItem(string path, SecurityType secure)
		{
			// Initialize the path and secure properties
			this.path = Path;
			this.secure = secure;
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		public SecureWebPageItem(string path) : this(path, SecurityType.Secure)
		{
		}

	}
}
