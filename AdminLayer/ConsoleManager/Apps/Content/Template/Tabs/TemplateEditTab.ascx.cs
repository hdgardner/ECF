using System;
using System.Collections;
using System.Data;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using mc = Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Template.Tabs
{
	public partial class TemplateEditTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the template id.
        /// </summary>
        /// <value>The template id.</value>
        public int TemplateId
        {
            get
            {
                if (Parameters["TemplateId"] != null)
                    return Int32.Parse(Parameters["TemplateId"].ToString());
                else
                    return -1;
            }
        }

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
                if (Parameters["lang"] != null)
                    return Parameters["lang"];
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (TemplateId < 0)
                return;

            TemplateDto templates = DictionaryManager.GetTemplateDto(TemplateId);
            if (templates.main_Templates.Count > 0)
            {
                Name.Text = templates.main_Templates[0].Name;
                FriendlyName.Text = templates.main_Templates[0].FriendlyName;
                Path.Text = templates.main_Templates[0].Path;
                TemplateType.Text = templates.main_Templates[0].TemplateType;

            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }

        #region IAdminTabControl Members

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            TemplateDto dto = (TemplateDto)context["TemplateDto"];
            TemplateDto.main_TemplatesRow row = null;

            if(dto.main_Templates.Count > 0)
            {
                row = dto.main_Templates[0];
            }
            else
            {
                row = dto.main_Templates.Newmain_TemplatesRow();
                row.ApplicationId = mc.CmsConfiguration.Instance.ApplicationId;
            }

            row.Name = Name.Text;
            row.FriendlyName = FriendlyName.Text;
            row.Path = Path.Text;
            row.TemplateType = TemplateType.Text;
            row.LanguageCode = LanguageCode;

            if (row.RowState == DataRowState.Detached)
            {
                dto.main_Templates.Rows.Add(row);
            }

        }
        #endregion
    }
}