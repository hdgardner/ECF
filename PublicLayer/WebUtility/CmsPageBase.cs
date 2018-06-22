using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using Mediachase.Cms;
using Mediachase.Commerce;
using Mediachase.Commerce.Profile;
using Mediachase.Licensing;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Web.UI.Controls;

namespace Mediachase.Cms
{
    /// <summary>
    /// Base for CMS Pages.
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {
        #region LanguageId
        /// <summary>
        /// Gets the language id.
        /// </summary>
        /// <value>The language id.</value>
        public int LanguageId
        {
            get
            {
                return CMSContext.Current.LanguageId;
            }
        }
        #endregion

        #region LanguageName
        /// <summary>
        /// Gets the name of the language.
        /// </summary>
        /// <value>The name of the language.</value>
        public string LanguageName
        {
            get
            {
                return CMSContext.Current.LanguageName;
            }
        }
        #endregion

        #region Render
        /// <summary>
        /// We override Render to swap out the default HtmlTextWriter for our own. Our own Writer's sole purpose is to
        /// change the the action attribute on the form tag to the vanity Url. This way all postbacks occur on the vanity url
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            HtmlMeta generatorMeta = new HtmlMeta();
            generatorMeta.Name = "GENERATOR";
            generatorMeta.Content = FrameworkContext.ProductName;
            this.Header.Controls.Add(generatorMeta);

            HtmlMeta versionMeta = new HtmlMeta();
            versionMeta.Name = "VERSION";
            versionMeta.Content = FrameworkContext.ProductVersionDesc;
            this.Header.Controls.Add(versionMeta);

            HtmlMeta licenseMeta = new HtmlMeta();
            licenseMeta.Name = "LICENSE";

            CommerceLicenseInfo[] licenseInfo = CommerceLicensing.Current;
            if (licenseInfo == null || licenseInfo.Length == 0)
            {
                licenseMeta.Content = "unlicensed version";
            }
            else
            {
                licenseMeta.Content = String.Format("{0} ({1})", licenseInfo[0].Edition, licenseInfo[0].Company);
            }

            this.Header.Controls.Add(licenseMeta); 

            if (CMSContext.Current.IsUrlReWritten)
            {
                if (writer is System.Web.UI.Html32TextWriter)
                {
                    writer = new FormFixerHtml32TextWriter(writer.InnerWriter);
                }
                else
                {
                    writer = new FormFixerHtmlTextWriter(writer.InnerWriter);
                }
            }

            base.Render(writer);

            /*
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            base.Render(hw);
            string html = sw.ToString();

            // Hose the writers we don't need anymore.
            hw.Close();
            sw.Close();

            int start = html.IndexOf("<input type=\"hidden\" name=\"__VIEWSTATE\"");
            // If we find it, then move it.
            if (start > -1)
            {
                int end = html.IndexOf("/>", start) + 2;

                // Strip out the viewstate.
                string viewstate = html.Substring(start, end - start);
                html = html.Remove(start, end - start);

                // Find the end of the form and insert it there, since we can't put it any lower in the response stream.
                int formend = html.IndexOf("</form>");
                html = html.Insert(formend, viewstate);
            }

            // Send the results back into the writer provided.
            writer.Write(html);
             * */
        }
        #endregion

        #region FormFixers

        #region FormFixerHtml32TextWriter
        internal class FormFixerHtml32TextWriter : System.Web.UI.Html32TextWriter
        {
            private string _url;

            /// <summary>
            /// Initializes a new instance of the <see cref="FormFixerHtml32TextWriter"/> class.
            /// </summary>
            /// <param name="writer">The writer.</param>
            internal FormFixerHtml32TextWriter(TextWriter writer)
                : base(writer)
            {
                _url = CMSContext.Current.CurrentUrl;
            }

            /// <summary>
            /// Writes the attribute.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            /// <param name="encode">if set to <c>true</c> [encode].</param>
            public override void WriteAttribute(string name, string value, bool encode)
            {
                if (_url != null && string.Compare(name, "action", true) == 0)
                {
                    value = _url;
                }
                base.WriteAttribute(name, value, encode);
            }
        }
        #endregion

        #region FormFixerHtmlTextWriter
        internal class FormFixerHtmlTextWriter : System.Web.UI.HtmlTextWriter
        {
            private string _url;
            /// <summary>
            /// Initializes a new instance of the <see cref="FormFixerHtmlTextWriter"/> class.
            /// </summary>
            /// <param name="writer">The writer.</param>
            internal FormFixerHtmlTextWriter(TextWriter writer)
                : base(writer)
            {

                _url = CMSContext.Current.CurrentUrl;
            }

            /// <summary>
            /// Writes the attribute.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            /// <param name="encode">if set to <c>true</c> [encode].</param>
            public override void WriteAttribute(string name, string value, bool encode)
            {
                if (_url != null && string.Compare(name, "action", true) == 0)
                {
                    value = _url;
                }

                base.WriteAttribute(name, value, encode);
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Converts a URL into one that is usable on the requesting client.
        /// </summary>
        /// <param name="relativeUrl">The URL associated with the <see cref="P:System.Web.UI.Control.TemplateSourceDirectory"/> property.</param>
        /// <returns>The converted URL.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Occurs if the <paramref name="relativeUrl"/> parameter contains null.
        /// </exception>
        public new string ResolveUrl(string relativeUrl)
        {
            return CMSContext.Current.ResolveUrl(relativeUrl);
        }

        /*
        protected override void CreateChildControls()
        {
            if (CMSContext.Current.IsDesignMode)
            {
                // Wrap static controls in design view
                if (PageDocument.Current.StaticNode != null)
                {
                    foreach (Control control in PageDocument.Current.StaticNode.Controls)
                    {
                        BaseStaticWrapper wrapper = new BaseStaticWrapper();

                        // Remove it from hierarchy of the parent control
                        Control parent = control.Parent;
                        parent.Controls.Remove(control);
                        wrapper.Controls.Add(control);
                        parent.Controls.Add(wrapper);
                    }
                }
            }

            base.CreateChildControls();
        }
         * */
    }
}