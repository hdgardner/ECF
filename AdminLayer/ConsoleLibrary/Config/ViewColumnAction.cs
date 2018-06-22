using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
	public class ViewColumnAction
	{
		public string ImageUrl
		{
			get { return (string)Attributes["ImageUrl"]; }
			set { Attributes["ImageUrl"] = value; }
		}

		public string CommandName
		{
			get { return (string)Attributes["CommandName"]; }
			set { Attributes["CommandName"] = value; }
		}

		public string CommandParameters
		{
			get
			{
				string paramsKey = "CommandParameters";
				if (Attributes.ContainsKey(paramsKey))
					return Attributes[paramsKey].ToString();
				else
					return String.Empty;
			}
			set { Attributes["CommandParameters"] = value; }
		}

		public string ToolTip
		{
			get
			{
				string key = "ToolTip";
				if (Attributes.ContainsKey(key))
					return Attributes[key].ToString();
				else
					return String.Empty;
			}
			set { Attributes["ToolTip"] = value; }
		}

		private Hashtable _Attributes = new Hashtable();

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public Hashtable Attributes
		{
			get { return _Attributes; }
			set { _Attributes = value; }
		}

		public ViewColumnAction()
		{
		}
	}
}