using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Web.UI.Controls;
using System.Web.UI;


namespace Mediachase.Cms.WebUtility.Controls
{
    public class BaseTemplateUserControl : BaseStaticUserControl, ICmsDataAdapter
    {
        Param _Parameters = null;
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public Param Parameters
        {
            get
            {
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        string _Template = String.Empty;
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public string Template
        {
            get
            {
                return _Template;
            }
            set
            {
                _Template = value;
            }
        }

        string _ControlFactoryUID = String.Empty;
        /// <summary>
        /// Gets or sets the type of the template.
        /// </summary>
        /// <value>The type of the template.</value>
        public string TemplateType
        {
            get
            {
                return _ControlFactoryUID;
            }
            set
            {
                _ControlFactoryUID = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            CreateChildControlsTree();
            base.OnLoad(e);
        }

        /// <summary>
        /// Creates the child controls tree.
        /// </summary>
        private void CreateChildControlsTree()
        {
            System.Web.UI.Control ctrl = null;

            string id = "Template" + this.ID;
            ctrl = this.FindControl(id);
            if (ctrl == null)
            {
                string templateUrl = GetTemplateUrl();

                if (File.Exists(Server.MapPath(templateUrl)))
                {
                    try
                    {
                        ctrl = this.LoadControl(templateUrl.ToString());

                        if (ctrl is IContextUserControl)
                        {
                            ((IContextUserControl)ctrl).LoadContext(ConvertToDictionary(this.Parameters));
                        }
                    }
                    catch (HttpException ex)
                    {
                        if (ex.GetHttpCode() == 404)
                            throw new System.IO.FileNotFoundException("Template not found", ex);
                        else
                            throw;
                    }

                    this.Controls.Add(ctrl);
                }
            }
        }

        /// <summary>
        /// Gets the template URL.
        /// </summary>
        /// <returns></returns>
        private string GetTemplateUrl()
        {
            return DictionaryManager.GetTemplatePath(String.IsNullOrEmpty(Template) ? "default" : Template, TemplateType);
        }

        /// <summary>
        /// Converts to dictionary.
        /// </summary>
        /// <param name="pars">The pars.</param>
        /// <returns></returns>
        private IDictionary ConvertToDictionary(Param pars)
        {
            IDictionary dic = new ListDictionary();

			if (pars != null)
			{
				foreach (string key in pars.AllKeys)
				{
					dic.Add(key, pars[key]);
				}
			}

            return dic;
        }

        #region ICmsDataAdapter Members
        /// <summary>
        /// Sets the param info.
        /// </summary>
        /// <param name="param">The param.</param>
        public void SetParamInfo(object param)
        {
            ControlSettings settings = (ControlSettings)param;

            if (settings != null)
            {
                if (settings.Params == null)
                    settings.Params = new Param();

                this.Parameters = settings.Params;

                if (settings.Params["DisplayTemplate"] != null)
                    this.Template = settings.Params["DisplayTemplate"].ToString();

                this.DataBind();
            }
        }
        #endregion
    }
}
