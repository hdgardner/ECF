using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebActionSet;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Web.UI.Controls
{
	/// <summary>
	/// Summary description for SnapManager
	/// </summary>
	public class PlaceHolderManager : Control
	{
		#region GetCurrent
        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
		public static PlaceHolderManager GetCurrent(Page page)
		{
			foreach (Control c in page.Form.Controls)
			{
				if (c is PlaceHolderManager)
					return (PlaceHolderManager)c;
			}

			return null;
		} 
		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceHolderManager"/> class.
        /// </summary>
		public PlaceHolderManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		#region prop: UserUID
        /// <summary>
        /// Gets the user UID.
        /// </summary>
        /// <value>The user UID.</value>
		public Guid UserUID
		{
			get
			{
				if (this.Page.User.Identity.IsAuthenticated)
                    return (Guid)ProfileContext.Current.User.ProviderUserKey;
				else
					return Guid.Empty;
			}

		}
		#endregion

		#region prop: PlaceHolders
		private ArrayList placeHolders;
        /// <summary>
        /// Gets the place holders.
        /// </summary>
        /// <value>The place holders.</value>
		public ArrayList PlaceHolders
		{
			get
			{
				if (placeHolders == null)
					placeHolders = new ArrayList();

				return placeHolders;
			}
		}
		#endregion

		#region prop: hfDeletetedId
        /// <summary>
        /// Gets the hf deleteted id.
        /// </summary>
        /// <value>The hf deleteted id.</value>
		public string hfDeletetedId
		{
			get
			{
				if (this.Parent.FindControl("hfDeletedInfo") != null)
					return this.Parent.FindControl("hfDeletedInfo").ClientID;

				return string.Empty;
			}
		}

        /// <summary>
        /// Gets the hf deleted.
        /// </summary>
        /// <value>The hf deleted.</value>
		public HiddenField hfDeleted
		{
			get
			{
				if (this.Parent.FindControl("hfDeletedInfo") != null)
					return (HiddenField)this.Parent.FindControl("hfDeletedInfo");

				return null;
			}
		}
		#endregion

		#region prop: hfEdited
        /// <summary>
        /// Gets the hf edited.
        /// </summary>
        /// <value>The hf edited.</value>
		public HiddenField hfEdited
		{
			get
			{
				if (this.Parent.FindControl("hfEditedInfo") != null)
					return (HiddenField)this.Parent.FindControl("hfEditedInfo");

				return null;
			}
		} 
		#endregion

		#region ChangePlaceHoldersDataSource
        /// <summary>
        /// Changes the place holders data source.
        /// </summary>
		private void ChangePlaceHoldersDataSource()
		{
			foreach (CmsPlaceHolder ctrl in this.PlaceHolders)
			{
				ctrl.DataSource = Mediachase.Cms.Pages.PageDocument.Current.GetDynamicNodes(ctrl.ID);
				ctrl.SetRequireDataBind();
			}
		} 
		#endregion

		#region AddToPageDocument
        /// <summary>
        /// Adds to page document.
        /// </summary>
        /// <param name="dn">The dn.</param>
		private void AddToPageDocument(DynamicNode dn)
		{
            DynamicNode[] nodes = PageDocument.Current.GetDynamicNodes(dn.ControlPlaceId);
			foreach (DynamicNode d in nodes)
			{
				if (d.ControlPlaceIndex >= dn.ControlPlaceIndex)
					d.ControlPlaceIndex++;
			}

            if (nodes == null || nodes.Length == 0)
                dn.ControlPlaceIndex = 0;

			PageDocument.Current.DynamicNodes.Add(dn);
		} 
		#endregion

		#region AddControl
        /// <summary>
        /// Adds the control.
        /// </summary>
        /// <param name="FactoryControlUID">The factory control UID.</param>
        /// <param name="PlaceHolderUID">The place holder UID.</param>
        /// <param name="Index">The index.</param>
		public void AddControl(string FactoryControlUID, string PlaceHolderUID, int Index)
		{
			DynamicNode dn = new DynamicNode();
			dn.NodeUID = "dv" + Guid.NewGuid().ToString("N");

			ControlSettings cs = new ControlSettings();
			Param param = new Param();
			cs.Params = param;
			NodeControlSettingsCollection ncsc = new NodeControlSettingsCollection();
			ncsc.Add(dn.NodeUID, cs);
			dn.Controls = ncsc;

			dn.ControlPlaceIndex = Index;
			dn.ControlPlaceId = PlaceHolderUID;
			dn.FactoryControlUID = FactoryControlUID;
			dn.FactoryUID = string.Empty;
			dn.IsModified = true;
			this.AddToPageDocument(dn);

			foreach (CmsPlaceHolder ctrl in this.PlaceHolders)
			{
				if (ctrl.ID == PlaceHolderUID)
				{
					ctrl.DataSource = Mediachase.Cms.Pages.PageDocument.Current.GetDynamicNodes(dn.ControlPlaceId);

					//EnsureDataBound();
					ctrl.SetRequireDataBind();
				}
			}
		} 
		#endregion

		#region DeleteFromPageDocument
		/// <summary>
		/// Marks as deleted from page document.
		/// </summary>
		/// <param name="dn">The dn.</param>
		private void MarkAsDeletedFromPageDocument(DynamicNode dn)
		{
			foreach (DynamicNode d in Mediachase.Cms.Pages.PageDocument.Current.GetDynamicNodes(dn.ControlPlaceId))
			{
				if (d.ControlPlaceIndex >= dn.ControlPlaceIndex)
					d.ControlPlaceIndex--;
			}

			//Mark as deleted
			dn.NodeUID = "0";
			//PageDocument.Current.DynamicNodes.Remove(dn);
		} 
		#endregion

		#region DeleteFromPageDocument
        /// <summary>
        /// Deletes from page document.
        /// </summary>
		private void DeleteFromPageDocument()
		{
			DynamicNodeCollection AllDn = Mediachase.Cms.Pages.PageDocument.Current.DynamicNodes;

			for (int i = 0; i < AllDn.Count; i++)
			{
				if (AllDn[i].NodeUID == "0")
				{
					PageDocument.Current.DynamicNodes.Remove(AllDn[i]);
					i--;
				}
			}
		} 
		#endregion

		#region DeleteControl
        /// <summary>
        /// Deletes the control.
        /// </summary>
        /// <param name="NodeUID">The node UID.</param>
		public void DeleteControl(string NodeUID)
		{
			foreach (DynamicNode dn in Mediachase.Cms.Pages.PageDocument.Current.DynamicNodes)
			{
				if (dn.NodeUID == NodeUID)
				{
					this.MarkAsDeletedFromPageDocument(dn);
				}
			}

			this.DeleteFromPageDocument();
			this.ChangePlaceHoldersDataSource();

			PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.TemporaryStorage, this.UserUID);
		} 
		#endregion

		#region HandleClientActions
        /// <summary>
        /// Handles the client actions.
        /// </summary>
		private void HandleClientActions()
		{
			HiddenField hfDel = this.hfDeleted;

			// Move Controls
			if (this.Page.IsPostBack && CMSContext.Current.IsDesignMode)
			{
				foreach (CmsPlaceHolder ctrl in this.PlaceHolders)
				{
					foreach (Control c in ctrl.Controls)
					{
						if (c is Snap)
						{
							DynamicNode d = Mediachase.Cms.Pages.PageDocument.Current.DynamicNodes.LoadByUID(c.ID);

                            // Jeff: Control has moved to a different CmsPlaceHolder.
                            if (((Snap)c).CurrentDockingContainer != d.ControlPlaceId)
                            {
                                d.ControlPlaceId = ((Snap)c).CurrentDockingContainer;

                                if (((Snap)c).CurrentDockingIndex != d.ControlPlaceIndex)
                                {
                                    // The ComponentArt library modifies the CurrentDockingIndex of the Snap on the client-side.
                                    d.ControlPlaceIndex = ((Snap)c).CurrentDockingIndex;
                                }

                                d.IsModified = true;
                            }

                            // Jeff: Control has changed position within the same CmsPlaceholder, skipped if the above block executes.
                            if (((Snap)c).CurrentDockingIndex != d.ControlPlaceIndex)
                            {
                                // The ComponentArt library modifies the CurrentDockingIndex of the Snap on the client-side.
                                d.ControlPlaceIndex = ((Snap)c).CurrentDockingIndex;
                                d.IsModified = true;
                            }
						}
					}

					ctrl.DataSource = Mediachase.Cms.Pages.PageDocument.Current.GetDynamicNodes(ctrl.ID);
					// Sasha: need to rebind here, otherwise the control won't display any data
                    //ctrl.SetRequireDataBind();
					ctrl.DataBind();
				}

                // Jeff: Uncommented to preserve dynamic node state; a save occurs in the DeleteControl() below.
				PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.TemporaryStorage, this.UserUID);

				// Alex & Nadya comment: saving should be performed only for the temporary storage mode.
				//PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.PersistentStorage, this.UserUID);
			}

			//Delete
			if (hfDel.Value != string.Empty)
			{
				hfDel.Value = hfDel.Value.Remove(hfDel.Value.Length - 1);

				foreach (string s in hfDel.Value.Split(','))
				{
					DeleteControl(s);
				}

				hfDel.Value = string.Empty;
			}


		} 
		#endregion

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            /* page document is loaded in the template.aspx.cs
            //load PageDocument on post before CreateChildControls occurs
            if (this.Page.IsPostBack)
            {
                //1. Load PageDocument
                if (CMSContext.Current.IsDesignMode)
					PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.Design, UserUID);
                else
					PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.View, UserUID);
            }
             * */
            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            /* loaded in the template.aspx.cs
            if (!this.Page.IsPostBack)
            {
                //1. Load PageDocument on get
                if (CMSContext.Current.IsDesignMode)
					PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.Design, UserUID);
                else
					PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.View, UserUID);
            }
             * */

			//2. Init DataSource foreach PlaceHolderWrapper
			foreach (Control ctrl in this.PlaceHolders)
			{
				if (ctrl is CmsPlaceHolder)
				{
					((CmsPlaceHolder)ctrl).DataSource = PageDocument.Current.GetDynamicNodes(ctrl.ID);

					if (!this.Page.IsPostBack)
						((CmsPlaceHolder)ctrl).DataBind();
				}
			}

			//3. Handle Move and Delete actions
			this.HandleClientActions();

		}
	}
}