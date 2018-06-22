using System;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Core.MetaData.Controls
{
	/// <summary>
	///		Summary description for DateTimeMetaControl.
	/// </summary>
    public partial class DateTimeMetaControl : CoreBaseUserControl, IMetaControl
	{

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            /*
			System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture(UserContext.CurrentContext.CurrentAdminSiteLanguage.ISOCode);
			DTClientControl1.Calend_Culture = ci.Name;

			if(MetaField.DataType==MetaDataType.Date)
				DTClientControl1.ShowTime = DateTimeClientControl.E_ShowTime.None;
			else
				DTClientControl1.ShowTime = DateTimeClientControl.E_ShowTime.HM;
             * */
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			//if(!this.IsPostBack)
			BindData();

			base.DataBind();
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
            MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);

            if (MetaObject != null)
            {
                object val = MetaObject[MetaField];
				if (val != null)
					DTClientControl1.Value = ManagementHelper.GetUserDateTime((DateTime)val);
            }

			MetaDescriptionCtrl.Text = MetaField.Description;
            NameRequired.Enabled = !MetaField.AllowNulls;
			DTClientControl1.ValidationEnabled = !MetaField.AllowNulls;
			//DTClientControl1.RequiredField = !MetaField.AllowNull;
		}

		#region IMetaControl Members
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
            MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { DTClientControl1.Value.ToUniversalTime() });
        }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return NameRequired.ValidationGroup;
            }
            set
            {
                NameRequired.ValidationGroup = value;
            }
        }
        #endregion
    }
}
