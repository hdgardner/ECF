using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using System.Collections.Specialized;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;

namespace Mediachase.Commerce.Manager.Content.Template
{
	public partial class TemplateEdit : BaseUserControl
    {
		private const string _TemplateDtoString = "TemplateDto";

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
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();

            if (!this.IsPostBack)
                DataBind();
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (TemplateId > 0)
            {
                TemplateDto templates = DictionaryManager.GetTemplateDto(TemplateId);

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
				dic.Add(_TemplateDtoString, templates);

                // Call tabs load context
                ViewControl.LoadContext(dic);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void EditSaveControl_SaveChanges(object sender, EventArgs e)
        {
            TemplateDto dto = null;

            if (TemplateId > 0)
                dto = DictionaryManager.GetTemplateDto(TemplateId);
            else
                dto = new TemplateDto();

            // Put a dictionary key that can be used by other tabs
            IDictionary dic = new ListDictionary();
			dic.Add(_TemplateDtoString, dto);

            ViewControl.SaveChanges(dic);

            // Save modifications
            if (dto.HasChanges())
                DictionaryManager.SaveTemplateDto(dto);

            // Call commit changes
            ViewControl.CommitChanges(dic);
        }
     }
}