using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
	/// <summary>
	///		Summary description for IntegerControl.
	/// </summary>
    public partial class PlanCycleMode_IntegerControl : CoreBaseUserControl, IMetaControl
	{

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			//if(!this.IsPostBack)
				BindData();

			base.DataBind ();
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
            MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);

            ManagementHelper.SelectListItem(MetaValueCtrl, MetaObject == null ? "" : MetaObject.GetInt32(MetaField).ToString());

            MetaDescriptionCtrl.Text = MetaField.Description;
            //RequiredFieldValidator1.ErrorMessage = String.Format("the {0} field is required", MetaField.FriendlyName);
		}

        #region IMetaControl Members
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return String.Empty;
            }
            set
            {
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

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
    		MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { Int32.Parse(MetaValueCtrl.SelectedValue) });
        }
        #endregion
	}
}
