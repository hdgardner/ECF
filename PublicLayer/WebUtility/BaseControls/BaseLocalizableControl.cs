using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

using Mediachase.Cms;

namespace Mediachase.Web.UI
{
    /// <summary>
    /// Summary description for BaseLocalizableControl
    /// </summary>
    public class BaseLocalizableControl : System.Web.UI.UserControl
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
    }
}