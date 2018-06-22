using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Web.UI;
using System.Collections;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.WebUtility
{
    /// <summary>
    /// Base class for all user controls in the CMS System. Adds additional properties which are common
    /// for website project but not available for web application projects.
    /// </summary>
    public class BaseStoreUserControl : UserControl, IContextUserControl
    {
        /// <summary>
        /// Gets the RM.
        /// </summary>
        /// <value>The RM.</value>
        public StoreResourceManager RM
        {
            get
            {
                return new StoreResourceManager();
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public CustomerProfile Profile
        {
            get
            {
                return ProfileContext.Current.Profile;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UserControlBase"/> class.
        /// </summary>
        public BaseStoreUserControl()
        {
        }

        #region IContextUserControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void LoadContext(IDictionary context)
        {
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

    /// <summary>
    /// 
    /// </summary>
    public interface IContextUserControl
    {
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        void LoadContext(IDictionary context);
    }

    public class StoreResourceManager
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetString(string name)
        {
            string str = "";
            try
            {
                str = System.Web.HttpContext.GetGlobalResourceObject("StoreResources", name, System.Threading.Thread.CurrentThread.CurrentUICulture).ToString();
            }
            catch
            {
                try
                {
                    object obj = System.Web.HttpContext.GetGlobalResourceObject("StoreResources", name);
                    if (obj == null)
                        throw new MissingManifestResourceException(String.Format("Resource for key {0} was not found.", name), null);

                    str = obj.ToString();
                }
                catch (System.Resources.MissingManifestResourceException ex)
                {

                    throw new MissingManifestResourceException(String.Format("Resource for key {0} was not found.", name), ex);
                }
            }
            return str;
        }
    }
}
