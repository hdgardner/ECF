using System;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The SecureWebPageFile class represents an file entry in the &lt;secureWebPages&gt;
	/// configuration section.
	/// </summary>
	public class SecureWebPageFile : SecureWebPageItem
	{
		/// <summary>
		/// Creates an instance with default values.
		/// </summary>
		public SecureWebPageFile() : base()
		{
		}

		/// <summary>
		/// Creates an instance with initial values.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		/// <param name="ignore">A flag to ignore security for the directory or file.</param>
		public SecureWebPageFile(string path, SecurityType secure) : base(path, secure)
		{
		}

		/// <summary>
		/// Creates an instance with an initial path value.
		/// </summary>
		/// <param name="path">The relative path to the directory or file.</param>
		public SecureWebPageFile(string path) : base(path)
		{
		}
	}
}
