using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Cms.Controls
{
	public static class McXmlSerializer
	{
		#region GetObject
		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static object GetObject(string typeName, string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			XmlSerializer xmlsz = new XmlSerializer(AssemblyUtil.LoadType(typeName));

			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return xmlsz.Deserialize(ms);
			}
		}

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static object GetObject(Type type, string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			XmlSerializer xmlsz = new XmlSerializer(type);

			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return xmlsz.Deserialize(ms);
			}
		}

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="extraTypes">The extra types.</param>
		/// <returns></returns>
		public static object GetObject(Type type, string value, params Type[] extraTypes)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			XmlSerializer xmlsz = new XmlSerializer(type, extraTypes);

			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return xmlsz.Deserialize(ms);
			}
		}


		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static T GetObject<T>(string value)
		{
			if (string.IsNullOrEmpty(value))
				return default(T);
			XmlSerializer xmlsz = new XmlSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return (T)xmlsz.Deserialize(ms);
			}
		}

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static T GetObject<S, T>(string value) where S : XmlSerializer, new()
		{
			if (string.IsNullOrEmpty(value))
				return default(T);

			S xmlsz = new S();

			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return (T)xmlsz.Deserialize(ms);
			}
		}

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="extraTypes">The extra types.</param>
		/// <returns></returns>
		public static T GetObject<T>(string value, params Type[] extraTypes)
		{
			if (string.IsNullOrEmpty(value))
				return default(T);
			XmlSerializer xmlsz = new XmlSerializer(typeof(T), extraTypes);
			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				return (T)xmlsz.Deserialize(ms);
			}
		}

		#endregion
		#region GetObjectFromFile
		/// <summary>
		/// Gets the object from file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static T GetObjectFromFile<T>(string path)
		{
			if (string.IsNullOrEmpty(path))
				return default(T);

			XmlSerializer xmlsz = new XmlSerializer(typeof(T));
			using (FileStream ms = File.OpenRead(path))
			{
				return (T)xmlsz.Deserialize(ms);
			}
		}

        /// <summary>
        /// Gets the object from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns></returns>
		public static T GetObjectFromFile<T>(string path, params Type[] extraTypes)
		{
			if (string.IsNullOrEmpty(path))
				return default(T);

			XmlSerializer xmlsz = new XmlSerializer(typeof(T), extraTypes);
			using (FileStream ms = File.OpenRead(path))
			{
				return (T)xmlsz.Deserialize(ms);
			}
		}
		#endregion

		#region GetString
		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string GetString(Type type, object value)
		{
			if (value == null)
				return string.Empty;
			XmlSerializer xmlsz = new XmlSerializer(type);
			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, value);
				return System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}
		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string GetString<T>(T value)
		{
			if (value == null)
				return string.Empty;
			XmlSerializer xmlsz = new XmlSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, value);
				return System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="extraTypes">The extra types.</param>
		/// <returns></returns>
		public static string GetString<T>(T value, params Type[] extraTypes)
		{
			if (value == null)
				return string.Empty;
			XmlSerializer xmlsz = new XmlSerializer(typeof(T), extraTypes);
			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, value);
				return System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public static string GetString<S, T>(T value) where S : XmlSerializer, new()
		{
			if (value == null)
				return string.Empty;

			S xmlsz = new S();

			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, value);
				return System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}
		#endregion

		#region SaveObjectToFile
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
		public static void SaveObjectToFile<T>(string path, T value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			XmlSerializer xmlsz = new XmlSerializer(typeof(T));

			using (FileStream ms = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				xmlsz.Serialize(ms, value);
				ms.Flush();
			}
		}

		/// <summary>
		/// Saves the object to file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="value">The value.</param>
		/// <param name="extraTypes">The extra types.</param>
		public static void SaveObjectToFile<T>(string path, T value, params Type[] extraTypes)
		{
			if (value == null)
				throw new ArgumentNullException("Value");

			XmlSerializer xmlsz = new XmlSerializer(typeof(T), extraTypes);

			using (FileStream ms = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				xmlsz.Serialize(ms, value);
				ms.Flush();
			}
		}

		#endregion
	}
}
