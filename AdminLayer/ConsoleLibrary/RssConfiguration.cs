using System;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace Mediachase.Web.Console
{
	public sealed class RssConfiguration : ConfigurationSection
	{
		//these two variables are the basis of the singleton implementation (uses double-checking)
		private static volatile RssConfiguration _instance;
		private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RssConfiguration"/> class.
        /// </summary>
		private RssConfiguration() { }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
		public static RssConfiguration Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lockObject)
					{
						if (_instance == null)
						{
							_instance = (RssConfiguration)ConfigurationManager.GetSection("Mediachase.RssNews");
						}
					}
				}

				return _instance;
			}
		}

        /// <summary>
        /// Gets the RSS items.
        /// </summary>
        /// <value>The RSS items.</value>
		[ConfigurationProperty("Items", IsRequired = false)]
		public RssItemsCollection RssItems
		{
			get
			{
				return (RssItemsCollection)Instance["Items"];
			}
		}
	}

	public class RssItemsCollection : ConfigurationElementCollection
	{
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Web.Console.RssItem"/> at the specified index.
        /// </summary>
        /// <value></value>
		public RssItem this[int index]
		{
			get
			{
				return base.BaseGet(index) as RssItem;
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				base.BaseAdd(index, value);
			}
		}

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new RssItem();
		}

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			RssItem item = element as RssItem;
			string key = String.Empty;
			if (item != null)
				key = String.Concat(item.UrlPath, "^", item.Culture);
			return key;
		}
	}

	public class RssItem : ConfigurationElement
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="RssItem"/> class.
        /// </summary>
		public RssItem() { }

        /// <summary>
        /// Retrieves url used to get rss feed
        /// </summary>
        /// <value>The URL path.</value>
		[ConfigurationProperty("urlPath", IsRequired = true)]
		public string UrlPath
		{
			get
			{
				return (string)this["urlPath"];
			}
			set
			{
				this["urlPath"] = value;
			}
		}

        /// <summary>
        /// Gets culture property
        /// </summary>
        /// <value>The culture.</value>
		[ConfigurationProperty("culture", IsRequired = false)]
		public string Culture
		{
			get
			{
				return (string)this["culture"];
			}
			set
			{
				this["culture"] = value;
			}
		}

        /// <summary>
        /// Gets amount of news to display
        /// </summary>
        /// <value>The news count.</value>
		[ConfigurationProperty("newsCount", IsRequired = true)]
		public string NewsCount
		{
			get
			{
				return (string)this["newsCount"];
			}
			set
			{
				this["newsCount"] = value;
			}
		}

		//make the configuration section editable by overriding this ConfigurationElement property
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only; otherwise, false.
        /// </returns>
		public override bool IsReadOnly()
		{
			return false;
		}
	}
}