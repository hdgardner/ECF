using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Ibn.Web.UI.Layout
{
	#region class : ColumnInfo
	public class ColumnInfo : IEqualityComparer<ColumnInfo>
	{
		public string Id;
		public int Width;

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj == this)
				return true;

			if (obj is ColumnInfo)
			{
				return String.Equals(((ColumnInfo)obj).Id, this.Id, StringComparison.InvariantCultureIgnoreCase);
			}

			return base.Equals(obj);
			
		}


        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>
        /// Compiler complains that there is no override of the GetHashCode() method. 
        /// Maybe a separate one can be implemented in the future? 
        /// For now, just the default definition .NET provides.
        /// </remarks>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		#region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
		public ColumnInfo()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
		public ColumnInfo(string id)
			: this()
		{
			this.Id = id;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="width">The width.</param>
		public ColumnInfo(string id, int width)
			: this(id)
		{
			this.Width = width;
		}
		#endregion

		#region IEqualityComparer<ColumnInfo> Members

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
		public bool Equals(ColumnInfo x, ColumnInfo y)
		{
			return String.Equals(x.Id, y.Id);
		}

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public int GetHashCode(ColumnInfo obj)
		{
			return base.GetHashCode();
		}

		#endregion		
	} 
	#endregion

	#region class : CpInfoItem
	public class CpInfoItem
	{
		public string Id;
		public string Collapsed;
		public string InstanceUid;
	}
	#endregion

	#region class : CpInfo
	public class CpInfo
	{
		public string Id;
		public List<CpInfoItem> Items;
	} 
	#endregion

	#region class: LayoutContextKey
	public class LayoutContextKey
	{
		#region prop: PageUid
		private string pageUid;

		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public string PageUid
		{
			get { return pageUid; }
			set { pageUid = value; }
		}
		#endregion

		#region .ctor
		public LayoutContextKey()
		{
		}

		public LayoutContextKey(string PageUid)
			: this()
		{
			this.PageUid = PageUid;
		}
		#endregion
	}
	#endregion
	
	public class IbnControlPlaceManager : CompositeDataBoundControl
	{
		#region GetCurrent
        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="_page">The _page.</param>
        /// <returns></returns>
		public static IbnControlPlaceManager GetCurrent(Page _page)
		{
			IbnControlPlaceManager retVal = null;
			retVal = GetActionManagerFromCollection(_page.Controls);
			return retVal;
		}

        /// <summary>
        /// Gets the action manager from collection.
        /// </summary>
        /// <param name="coll">The coll.</param>
        /// <returns></returns>
		private static IbnControlPlaceManager GetActionManagerFromCollection(ControlCollection coll)
		{
			IbnControlPlaceManager retVal = null;
			foreach (Control c in coll)
			{
				if (c is IbnControlPlaceManager)
				{
					retVal = (IbnControlPlaceManager)c;
					break;

				}
				else
				{
					retVal = GetActionManagerFromCollection(c.Controls);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IbnControlPlaceManager"/> class.
        /// </summary>
		public IbnControlPlaceManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public string PageUid
		{
			get
			{
				if (ViewState["_PageUid"] != null)
					return ViewState["_PageUid"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["_PageUid"] = value;
			}
		}
		#endregion

		#region prop: ControlPlaces
		private ArrayList controlPlaces;
        /// <summary>
        /// Gets the control places.
        /// </summary>
        /// <value>The control places.</value>
		public ArrayList ControlPlaces
		{
			get
			{
				if (controlPlaces == null)
					controlPlaces = new ArrayList();

				return controlPlaces;
			}
		}
		#endregion

		#region prop: JsonItems
		private string jsonItems = string.Empty;
        /// <summary>
        /// Gets or sets the json items.
        /// </summary>
        /// <value>The json items.</value>
		public string JsonItems
		{
			get
			{
				return jsonItems;
			}
			set
			{
				jsonItems = value;
			}
		}
		#endregion

		#region FillJsonItems
        /// <summary>
        /// Fills the json items.
        /// </summary>
		private void FillJsonItems()
		{
			this.JsonItems = string.Empty;

			foreach (Control ctrl in this.ControlPlaces)
			{
				if (ctrl is IbnControlPlace)
				{
					this.JsonItems += ((IbnControlPlace)ctrl).GetItemsJson() + ",";
				}
			}

			if (this.JsonItems.Length > 0)
				this.JsonItems = this.JsonItems.Remove(this.JsonItems.Length - 1);
		} 
		#endregion

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.EnsureChildControls();
			this.EnsureDataBound();
		}

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			FillJsonItems();
		}

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
			writer.RenderBeginTag("div");

			base.RenderChildren(writer);

			writer.RenderEndTag();

			writer.AddAttribute("id", "clearLayout");
			writer.AddAttribute("clear", "both");
			writer.RenderBeginTag("div");
			writer.RenderEndTag();
		}

		#region GetIdsForControlPlace
        /// <summary>
        /// Gets the ids for control place.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
		String GetIdsForControlPlace(string id)
		{
			String retVal = String.Empty;
			String pageKey = this.PageUid;

			// get this string from the database
			string userSettings = String.Empty;
			CustomerProfile profile = ProfileContext.Current.Profile;
			if (profile != null && profile.PageSettings != null)
				userSettings = profile.PageSettings.GetSettingString(pageKey);

			if (String.IsNullOrEmpty(userSettings))
				userSettings = Mediachase.Commerce.Manager.Dashboard.Home._DefaultControls;

			// TODO: add security check here

			// deserialize user settings
			System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<CpInfo> list = null;

			try
			{
				list = jss.Deserialize<List<CpInfo>>(userSettings);
			}
			catch
			{
				// something's wrong with user settings, reset it
				userSettings = null;
			}

			// get default control info, if it has not been customized and saved yet
			if (String.IsNullOrEmpty(userSettings))
			{
				int counter = 0;
				foreach (DynamicControlInfo dci in DynamicControlFactory.GetControlInfos())
				{
					if (id == "Column1" && (counter % 2) == 0)
					{
						retVal += String.Format("{0}^false^{1}:", dci.Uid, Guid.NewGuid().ToString("N")); //dci.Uid + ":";
					}

					if (id == "Column2" && (counter % 2) != 0)
					{
						retVal += String.Format("{0}^false^{1}:", dci.Uid, Guid.NewGuid().ToString("N"));  //dci.Uid + ":";
					}

					counter++;
				}

				if (retVal.Length > 0)
					retVal = retVal.TrimEnd(':');

				return retVal;
			}

			foreach (CpInfo cpInfo in list)
			{
				if (cpInfo.Id == id)
				{
					for (int i = 0; i < cpInfo.Items.Count; i++)
					{
						retVal += String.Format("{0}^{1}^{2}:", cpInfo.Items[i].Id, cpInfo.Items[i].Collapsed, cpInfo.Items[i].InstanceUid);
					}
				}
			}

			if (retVal.Length > 0)
				retVal = retVal.TrimEnd(':');

			return retVal;
		}
		#endregion

		#region CreateChildControls
        /// <summary>
        /// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
        /// </summary>
        /// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"></see> that contains the values to bind to the control.</param>
        /// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see> is called during data binding; otherwise, false.</param>
        /// <returns>
        /// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see>.
        /// </returns>
		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			if (dataBinding)
			{
				ViewState["__TemplateUid"] = dataSource.ToString();

				this.BindData(dataSource.ToString());
			}
			else
			{
				if (ViewState["__TemplateUid"] == null)
					throw new ArgumentNullException("__TemplateUid @ IbnControlPlaceManager");

				string _uid = ViewState["__TemplateUid"].ToString();

				this.BindData(_uid);
			}

			//2. Init DataSource foreach PlaceHolderWrapper
			foreach (Control ctrl in this.ControlPlaces)
			{
			    if (ctrl is IbnControlPlace)
			    {
			        ((IbnControlPlace)ctrl).DataSource = GetIdsForControlPlace(((IbnControlPlace)ctrl).ControlPlaceId).Split(':');
			        ((IbnControlPlace)ctrl).DataBind();
			    }
			}

			return 0;
		} 
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="uid">The uid.</param>
		private void BindData(string uid)
		{
			if (uid == null)
				throw new ArgumentNullException("uid");

			WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(uid);

			if (wti == null)
				throw new ArgumentException(string.Format("Can't find Template with uid: {0}", uid));

			System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<ColumnInfo> list = jss.Deserialize<List<ColumnInfo>>(wti.ColumnInfo);

			foreach (ColumnInfo ci in list)
			{
				IbnControlPlace icp = new IbnControlPlace();
				icp.WidthPercentage = ci.Width;
				icp.ID = ci.Id;
				icp.ControlPlaceId = ci.Id;
				this.Controls.Add(icp);
			}
		}
		#endregion
	}
}
