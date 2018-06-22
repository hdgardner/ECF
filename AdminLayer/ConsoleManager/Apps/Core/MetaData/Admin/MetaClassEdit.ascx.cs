using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	///		Used for creating new Meta Class.
	/// </summary>
    public partial class MetaClassEdit : CoreBaseUserControl
	{
        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace
        {
            get
            {
                object id = Request.QueryString["Namespace"];
                if (id == null)
                    return String.Empty;
                else
                    return id.ToString();
            }
        }

        /// <summary>
        /// Gets the save control.
        /// </summary>
        /// <value>The save control.</value>
        public SaveControl SaveControl
        {
            get
            {
                return EditSaveControl;
            }
        }


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if(!this.IsPostBack)
				DataBind();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			BindData();
			base.DataBind();
		}


        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			//SaveButton.Text = RM.GetString("GENERAL_SAVEBUTTON");
            NameRequired.ErrorMessage = RM.GetString("ATTRIBUTECLASSEDIT_ERROR_EMPTY_NAME");
			RequiredFieldValidatorFriendlyName.ErrorMessage = RM.GetString("ATTRIBUTECLASSEDIT_ERROR_EMPTY_FRIENDLYNAME");

            /*
			ItemsGrid.Columns[0].HeaderText = RM.GetString("ATTRIBUTECLASSEDIT_SELECT");
			ItemsGrid.Columns[1].HeaderText = RM.GetString("ATTRIBUTECLASSEDIT_SORT");
			ItemsGrid.Columns[2].HeaderText = RM.GetString("ATTRIBUTECLASSEDIT_NAME");
             * */
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			BindElement();
			//NewAttribute.Visible = false;
		}

        /// <summary>
        /// Binds the element.
        /// </summary>
		private void BindElement()
		{
			DdlObjectType.Items.Clear();
            MetaClassCollection coll = MetaClass.GetList(MDContext, Namespace, true);
			foreach (MetaClass mc in coll)
			{
				if(mc.Parent==null)
				/*if(String.Compare(mc.TableName, "category", true)==0 ||
					String.Compare(mc.TableName, "product", true)==0 ||
					String.Compare(mc.TableName, "sku", true)==0)*/
					DdlObjectType.Items.Add(new ListItem(mc.FriendlyName, mc.Id.ToString()));
			}
            /*
			if(DdlObjectType.Items.Count>0)
				Util.CommonHelper.SelectListItem(DdlObjectType, DdlObjectType.Items[0].Value);
             * */
		}

        /// <summary>
        /// Binds the available fields.
        /// </summary>
		private void BindAvailableFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("MetaFieldId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));
			
			// Fields
			MetaFieldCollection mfc = MetaField.GetList(MDContext);

			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					DataRow row = dt.NewRow();
					row["MetaFieldId"] = field.Id;
					row["FriendlyName"] = field.FriendlyName;
					dt.Rows.Add(row);
				}
			}
			
            /*
			ItemsGrid.DataSource = new DataView(dt);
			ItemsGrid.DataBind();
             * */
		}		

		/*private ObjectType GetSelectedObjectType()
		{
			if(DdlObjectType.Items.Count>0)
			{
				MetaClass mc = MetaClass.Load(int.Parse(DdlObjectType.SelectedValue));
				if(String.Compare(mc.TableName, "category", true)==0)
					return ObjectType.Category;
				else if(String.Compare(mc.TableName, "product", true)==0)
					return ObjectType.Product;
				else if(String.Compare(mc.TableName, "sku", true)==0)
					return ObjectType.Sku;
			}
			return ObjectType.Category;
		}*/

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(SaveButton_Click);
		}
		#endregion

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveButton_Click(object sender, System.EventArgs e)
		{
            // Validate form
            if (!this.Page.IsValid)
            {
                ((Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs)e).RunScript = false;
                return;
            }

            string name = Name.Text;
            string friendlyName = FriendlyName.Text;
            string description = Description.Text;
            int parentId = int.Parse(DdlObjectType.SelectedValue);

            MetaClass mcParent = MetaClass.Load(MDContext, parentId);
            if (mcParent != null)
            {
                MetaClass mc = MetaClass.Create(MDContext, Namespace + ".User", name, friendlyName,
                    String.Format("{0}{1}{2}", mcParent.Name, "Ex_", name), parentId, false, description);
            }
            else
            {
                // throw some exception here
            }
        }

        /// <summary>
        /// Checks if entered meta class name is unique.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void MetaClassNameCheck(object sender, ServerValidateEventArgs args)
        {
            // load meta class by name
            MetaClass mcByName = MetaClass.Load(MDContext, args.Value);

            // check if meta class with specified name is loaded
            if(mcByName != null)
            {
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
        }

        /// <summary>
        /// Checks if entered meta class friendly name is unique for parent meta class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void MetaClassFriendlyNameCheck(object sender, ServerValidateEventArgs args)
        {
            int parentId = int.Parse(DdlObjectType.SelectedValue);

            // load parent meta class
            MetaClass mclass = MetaClass.Load(MDContext, parentId);

            // check if children meta classes with specified friendly name is loaded
            if (mclass.ChildClasses != null && mclass.ChildClasses.Count > 0)
            {
                MetaClassCollection children = mclass.ChildClasses;
                foreach (MetaClass child in children)
                {
                    if (String.Compare(child.FriendlyName, args.Value) == 0)
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }

            args.IsValid = true;
        }
	}
}
