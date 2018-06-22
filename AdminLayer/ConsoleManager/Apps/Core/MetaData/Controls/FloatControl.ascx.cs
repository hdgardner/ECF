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

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
	/// <summary>
    ///		Summary description for FloatControl.
	/// </summary>
    public partial class FloatControl : CoreBaseUserControl, IMetaControl
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
            if (MetaField != null && !MetaField.AllowNulls)
                RequiredFieldValidator1.Enabled = true;
            else
                RequiredFieldValidator1.Enabled = false;

            MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);

            MetaValueCtrl.Text = MetaObject == null ? "" : MetaObject[MetaField].ToString();

            MetaDescriptionCtrl.Text = MetaField.Description;
            RequiredFieldValidator1.ErrorMessage = String.Format("the {0} field is required", MetaField.FriendlyName);
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

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
			if(String.IsNullOrEmpty(MetaValueCtrl.Text) && MetaField.AllowNulls)
                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { null });
			else
                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { Double.Parse(MetaValueCtrl.Text) });
        }
        #endregion
	}
}
