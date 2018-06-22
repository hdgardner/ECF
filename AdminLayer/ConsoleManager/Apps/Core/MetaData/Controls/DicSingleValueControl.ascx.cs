using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Storage;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{

    /// <summary>
	///		Summary description for DicSingleValueControl.
	/// </summary>
    public partial class DicSingleValueControl : CoreBaseUserControl, IMetaControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		public override void DataBind()
		{
			//if(!this.IsPostBack)
				BindData();

			base.DataBind ();
		}

		private void BindData()
		{
            if (MetaField != null && !MetaField.AllowNulls)
                RequiredFieldValidator1.Enabled = true;
            else
                RequiredFieldValidator1.Enabled = false;

            MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);

            MetaDescriptionCtrl.Text = MetaField.Description;
            RequiredFieldValidator1.ErrorMessage = String.Format("the {0} field is required", MetaField.FriendlyName);

            if(MetaField.AllowNulls)
            {
                DicSingleValueCtrl.Items.Add(new ListItem("["+RM.GetString("SINGLEVALUECONTROL_EMPTY_VALUE")+"]", ""));
            }

            MetaDictionary dictionary = MetaField.Dictionary;
            if (dictionary != null)
            {
                foreach (MetaDictionaryItem item in dictionary)
                    DicSingleValueCtrl.Items.Add(new ListItem(item.Value.ToString(), item.Value.ToString()));
            }

            if (DicSingleValueCtrl.Items.Count > 0)
            {
                try
                {
                    ManagementHelper.SelectListItem(DicSingleValueCtrl, MetaObject.GetDictionaryItem(MetaField).Value);
                }
                catch
                {

                }
            }
		}

		#region Web Form Designer generated code
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

		}
		#endregion		

		#region IMetaControl Members
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return RequiredFieldValidator1.ValidationGroup;
            }
            set
            {
                RequiredFieldValidator1.ValidationGroup = value;
            }
        }

        private MetaField _MetaField;
        /// <summary>
        /// Gets or sets the meta field.
        /// </summary>
        /// <value>The meta field.</value>
        public MetaField MetaField
        {
            get
            {
                return _MetaField;
            }
            set
            {
                _MetaField = value;
            }
        }


        MetaObject _MetaObject;
        /// <summary>
        /// Gets or sets the meta object.
        /// </summary>
        /// <value>The meta object.</value>
        public MetaObject MetaObject
        {
            get
            {
                return _MetaObject;
            }
            set
            {
                _MetaObject = value;
            }
        }

        private string _LanguageCode;
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
            }
        }

		public void Update()
		{
            if (String.IsNullOrEmpty(DicSingleValueCtrl.SelectedValue) && MetaField.AllowNulls)

                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { null });
            else
                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { DicSingleValueCtrl.SelectedValue });

		}
		#endregion
	}
}
