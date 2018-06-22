using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The exception thrown for any errors while reading the secureWebPages 
	/// section of a configuration file.
	/// </summary>
	public class SecureWebPageSectionException : System.Configuration.ConfigurationErrorsException
	{
		/// <summary>
		/// Intializes an instance of this exception.
		/// </summary>
		public SecureWebPageSectionException()
		{
		}

		/// <summary>
		/// Initializes an instance of this exception with specified parameters.
		/// </summary>
		/// <param name="message">The message to display to the client when the exception is thrown.</param>
		/// <param name="node">The XmlNode that contains the error.</param>
		public SecureWebPageSectionException(string message, XmlNode node) : base(message, node)
		{
		}
	}


	/// <summary>
	/// SecureWebPageSectionHandler reads any <secureWebPages> section from a configuration file.
	/// </summary>
	public class SecureWebPageSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		public SecureWebPageSectionHandler()
		{
		}

		/// <summary>
		/// Parses the XML configuration section and returns the settings.
		/// </summary>
		/// <param name="parent">
		///		The configuration settings in a corresponding parent 
		///		configuration section.
		/// </param>
		/// <param name="configContext">
		///		An HttpConfigurationContext when Create is called from the ASP.NET 
		///		configuration system. Otherwise, this parameter is reserved and is 
		///		a null reference (Nothing in Visual Basic).
		/// </param>
		/// <param name="section">
		///		The XmlNode that contains the configuration information from the 
		///		configuration file. Provides direct access to the XML contents of 
		///		the configuration section.
		/// </param>
		/// <returns>
		///		Returns a SecureWebPageSettings instance initialized with the 
		///		read configuration settings.
		///	</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			// Create a SecureWebPageSettings object for the settings in this section
			SecureWebPageSettings Settings = new SecureWebPageSettings();
			
			// Read the general settings
			ReadGeneralSettings(section, Settings);

			// Traverse the child nodes
			foreach (XmlNode Node in section.ChildNodes)
			{
				if (Node.NodeType == System.Xml.XmlNodeType.Comment)
					// Skip comment nodes (thanks to dcbrower on CodeProject for pointing this out)
					continue;
				else if (Node.Name.ToLower() == "directory")
					// This is a directory path node
					Settings.Directories.Add(ReadDirectoryItem(Node));
				else if (Node.Name.ToLower() == "file")
					// This is a file path node
					Settings.Files.Add(ReadFileItem(Node));
				else
					// Throw an exception for this unrecognized node
					throw new SecureWebPageSectionException(string.Format("'{0}' is not an acceptable setting.", Node.Name), Node);
			}

			// Return the settings
			return Settings;
		}

		/// <summary>
		/// Reads general settings from the secureWebPages section into the given SecureWebPageSettings instance.
		/// </summary>
		/// <param name="section">The XmlNode to read from.</param>
		/// <param name="settings">The SecureWebPageSettings instance to set.</param>
		protected void ReadGeneralSettings(XmlNode section, SecureWebPageSettings settings)
		{
			// Get the mode attribute
			if (section.Attributes["mode"] != null)
			{
				switch (section.Attributes["mode"].Value.ToLower())
				{
					case "on":
						settings.Mode = SecureWebPageMode.On;
						break;

					case "remoteonly":
						settings.Mode = SecureWebPageMode.RemoteOnly;
						break;

					case "localonly":
						settings.Mode = SecureWebPageMode.LocalOnly;
						break;

					case "off":
						settings.Mode = SecureWebPageMode.Off;
						break;

					default:
						throw new SecureWebPageSectionException("Invalid value for the 'mode' attribute.", section);
				}
			}

			// Get the encryptedUri attribute
			if (section.Attributes["encryptedUri"] != null)
				settings.EncryptedUri = section.Attributes["encryptedUri"].Value;

			// Get the unencryptedUri attribute
			if (section.Attributes["unencryptedUri"] != null)
				settings.UnencryptedUri = section.Attributes["unencryptedUri"].Value;

			// Validate that if either encryptedUri or unencryptedUri are set, both must be set
			if (
				(settings.EncryptedUri.Length > 0 && settings.UnencryptedUri.Length == 0) || 
				(settings.UnencryptedUri.Length > 0 && settings.EncryptedUri.Length == 0))
				throw new SecureWebPageSectionException("You must specify both 'encryptedUri' and 'unencryptedUri', or neither.", section);

			// Get the maintainPath attribute
			if (section.Attributes["maintainPath"] != null)
			{
				string Value = section.Attributes["maintainPath"].Value.ToLower();
				settings.MaintainPath = (Value == "true" || Value == "yes" || Value == "on");
			}

			// Get the warningBypassMode attribute
			if (section.Attributes["warningBypassMode"] != null)
			{
				switch (section.Attributes["warningBypassMode"].Value.ToLower())
				{
					case "alwaysbypass":
						settings.WarningBypassMode = SecurityWarningBypassMode.AlwaysBypass;
						break;

					case "bypasswithqueryparam":
						settings.WarningBypassMode = SecurityWarningBypassMode.BypassWithQueryParam;
						break;

					case "neverbypass":
						settings.WarningBypassMode = SecurityWarningBypassMode.NeverBypass;
						break;

					default:
						throw new SecureWebPageSectionException("Invalid value for the 'warningBypassMode' attribute.", section);
				}
			}

			// Get the bypassQueryParamName attribute
			if (section.Attributes["bypassQueryParamName"] != null)
				settings.BypassQueryParamName = section.Attributes["bypassQueryParamName"].Value;
		}

		/// <summary>
		/// Reads the typical attributes for a SecureWebPageItem from the configuration node.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <param name="item">The SecureWebPageItem to set values for.</param>
		protected void ReadChildItem(XmlNode node, SecureWebPageItem item)
		{
			// Set the item only if the node has a valid path attribute value
			if (node.Attributes["path"] != null && node.Attributes["path"].Value.Trim().Length > 0)
			{
				// Get the value of the path attribute
				item.Path = node.Attributes["path"].Value.Trim().ToLower();

				// Add leading and trailing "/" characters where needed
				if (item.Path.Length > 1)
				{
					// Leading "/"
					if (!item.Path.StartsWith("/"))
						item.Path = "/" + item.Path;

					// Trailing "/" only if this is a directory item
					if (item is SecureWebPageDirectory && !item.Path.EndsWith("/"))
						item.Path += "/";
				}

				// Check for a secure attribute
				if (node.Attributes["secure"] != null)
				{
					switch (node.Attributes["secure"].Value.Trim().ToLower())
					{
						case "true":
							item.Secure = SecurityType.Secure;
							break;

						case "false":
							item.Secure = SecurityType.Insecure;
							break;

						case "ignore":
							item.Secure = SecurityType.Ignore;
							break;

						default:
							throw new SecureWebPageSectionException("Invalid value for the 'secure' attribute.", node);
					}
				}
			}
			else
				// Throw an exception for the missing Path attribute
				throw new SecureWebPageSectionException("'path' attribute not found.", node);
		}

		/// <summary>
		/// Reads a directory item from the configuration node and returns a new instance of SecureWebPageDirectory.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <returns>A SecureWebPageDirectory initialized with values read from the node.</returns>
		protected SecureWebPageDirectory ReadDirectoryItem(XmlNode node)
		{
			// Create a SecureWebPageDirectory instance
			SecureWebPageDirectory Item = new SecureWebPageDirectory();

			// Read the typical attributes
			ReadChildItem(node, Item);

			// Check for a recurse attribute
			if (node.Attributes["recurse"] != null)
				Item.Recurse = (node.Attributes["recurse"].Value.ToLower() == "true");
			
			return Item;
		}

		/// <summary>
		/// Reads a file item from the configuration node and returns a new instance of SecureWebPageFile.
		/// </summary>
		/// <param name="node">The XmlNode to read from.</param>
		/// <returns>A SecureWebPageFile initialized with values read from the node.</returns>
		protected SecureWebPageFile ReadFileItem(XmlNode node)
		{
			// Create a SecureWebPageFile instance
			SecureWebPageFile Item = new SecureWebPageFile();

			// Read the typical attributes
			ReadChildItem(node, Item);

			return Item;
		}
	}
}
