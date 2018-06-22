using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


/// <summary>
/// Summary description for Helper
/// </summary>
/// 
namespace Mediachase.Cms.Pages
{
	public static class Helper
	{
        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		public static byte[] Serialize(object value)
		{
			if (value == null)
				return null;

			IFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(2048))
			{
				formatter.Serialize((Stream)stream, value);
				stream.Capacity = (int)stream.Length;
				return stream.GetBuffer();
			}
		}

		/// <summary>
		/// Deserializes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static object Deserialize(byte[] data)
		{
			if (data == null)
				return null;

			IFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(data))
			{
				return formatter.Deserialize((Stream)stream);
			}
		}
	}
}