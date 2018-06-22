using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Web.Console.Config
{
	public enum ColumnType
	{
		None,
		Text,
		CheckBox,
		DropDown,
        HyperLink,
		CustomTemplate,
		Action,
        DateTime
    }

    public class ViewColumn
    {
		public static readonly string _HeadingTextAttributeName = "HeadingText";

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

		private ColumnType _ColumnType = ColumnType.None;

		/// <summary>
		/// Gets or sets ColumnType.
		/// </summary>
		public ColumnType ColumnType
		{
			get
			{
				return _ColumnType;
                /*
				ColumnType retVal = ColumnType.None;
				if (Attributes.ContainsKey("ColumnType"))
				{
					try
					{
						retVal = (ColumnType)Enum.Parse(typeof(ColumnType), (string)Attributes["ColumnType"], true);
					}
					catch { }
				}
				return retVal;
                 */
			}
			set
			{
				_ColumnType = value;
			}
		}

		private List<ViewColumnAction> _Actions = new List<ViewColumnAction>();
		public IList<ViewColumnAction> Actions
		{
			get
			{
				return _Actions.AsReadOnly();
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is Visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is Visible; otherwise, <c>false</c>.
        /// </value>
		public bool Visible
		{
			get
			{
				if (!Attributes.ContainsKey("Visible"))
					return true;
				else
					return Boolean.Parse(Attributes["Visible"].ToString());
			}
			set
			{
				Attributes["Visible"] = value;
			}
		}

        private ViewColumnTemplate _Template = null;

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public ViewColumnTemplate Template
        {
            get { return _Template; }
            set { _Template = value; }
        }

        /// <summary>
        /// Gets the Width.
        /// </summary>
        /// <value>The Width.</value>
		public string Width
		{
			get
			{
				string widthKey = "Width";
				if (Attributes.ContainsKey(widthKey))
					return Attributes[widthKey].ToString();
				else
					return "100";
			}
			set
			{
				Attributes["Width"] = value;
			}
		}

        /// <summary>
        /// Gets the AllowSorting.
        /// </summary>
        /// <value>The AllowSorting.</value>
		public bool AllowSorting
		{
			get
			{
				object sortObj = Attributes["AllowSorting"];
				bool sort = false;
				if (sortObj != null)
					Boolean.TryParse(sortObj.ToString(), out sort);
				return sort;
			}
			set
			{
				Attributes["AllowSorting"] = value;
			}
		}

		/// <summary>
		/// Gets the Resizable. Returns true if value is not set.
		/// </summary>
		/// <value>The Resizable.</value>
		public bool Resizable
		{
			get
			{
				object obj = Attributes["Resizable"];
				bool resizable = true; // column should be resizable by default
				if (obj != null)
					Boolean.TryParse(obj.ToString(), out resizable);
				return resizable;
			}
			set
			{
				Attributes["Resizable"] = value;
			}
		}

		/// <summary>
		/// Gets the HeadingText.
		/// </summary>
		/// <value>The HeadingText.</value>
		public string HeadingText
		{
			get
			{
				object ht = Attributes[_HeadingTextAttributeName];
				if (ht != null)
					return ht.ToString();
				else
					return String.Empty;
			}
			set
			{
				Attributes[_HeadingTextAttributeName] = value;
			}
		}

        /// <summary>
        /// Gets the data field.
        /// </summary>
        /// <value>The data field.</value>
        public string DataField
        {
            get
            {
                return Attributes["DataField"].ToString();
            }
            set
            {
                Attributes["DataField"] = value;
            }
        }

		/// <summary>
		/// Gets the string for formatting column's value. Used only for columns of type Text/None.
		/// </summary>
		/// <value>The FormatString.</value>
		public string FormatString
		{
			get
			{
				string formatStringKey = "FormatString";
				if (Attributes.ContainsKey(formatStringKey))
					return Attributes[formatStringKey].ToString();
				else
					return String.Empty;
			}
			set
			{
				Attributes["Width"] = value;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ListColumn"/> class.
        /// </summary>
        /// <param name="dataField">The data field.</param>
        internal ViewColumn(string dataField)
        {
            Attributes["DataField"] = dataField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewColumn"/> class.
        /// </summary>
        internal ViewColumn()
        {
            Attributes["DataField"] = String.Empty;
        }

		private int _ColumnIndex;
		public int ColumnIndex
		{
			get
			{
				return _ColumnIndex;
			}
			internal set
			{
				_ColumnIndex = value;
			}
		}

		private int _ColumnVisibleIndex;
		public int ColumnVisibleIndex
		{
			get
			{
				return _ColumnVisibleIndex;
			}
			set
			{
				_ColumnVisibleIndex = value;
			}
		}

        /// <summary>
        /// Returns string specified in SortExpression attribute. If attribute is not set, DataField attribute's value is returned.
        /// </summary>
        /// <returns></returns>
		public string GetSortExpression()
		{
			string sortKey = "SortExpression";
			if (Attributes.ContainsKey(sortKey))
				return Attributes[sortKey].ToString();
			else
				return DataField;
		}

		public string GetColumnUniqueId()
		{
			return ColumnIndex + "_" + DataField;
		}

		public string GetLocalizedHeadingString()
		{
			return UtilHelper.GetResFileString(this.HeadingText);
		}

        /// <summary>
        /// Adds the column action.
        /// </summary>
        /// <param name="action">The action.</param>
		internal void AddColumnAction(ViewColumnAction action)
		{
			if (action != null)
				_Actions.Add(action);
		}
    }
}
