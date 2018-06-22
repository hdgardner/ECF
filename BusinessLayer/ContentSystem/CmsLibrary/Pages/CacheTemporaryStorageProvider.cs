using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Cms.Pages
{
    [Serializable]
    public class CacheTemporaryStorageProvider : IPageDocumentStorageProvider
    {
        #region IPageDocumentStorageProvider Members

        /// <summary>
        /// Loads PageDocument from temporary cache storage
        /// </summary>
        /// <param name="PageVersionId"></param>
        /// <param name="UserUID"></param>
        /// <returns></returns>

        public PageDocument Load(int PageVersionId, Guid UserUID)
        {
            if (HttpContext.Current.Cache[UserUID.ToString() + "_" + PageVersionId.ToString()] != null)
            {
                return (PageDocument)HttpContext.Current.Cache[UserUID.ToString() + "_" + PageVersionId.ToString()];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Saves PageDocument to the temporary cache storage
        /// </summary>
        /// <param name="pageDocument"></param>
        /// <param name="PageVersionId"></param>
        /// <param name="UserUID"></param>
        public void Save(PageDocument pageDocument, int PageVersionId, Guid UserUID)
        {
            if (pageDocument == null)
                return;
            HttpContext.Current.Cache[UserUID.ToString() + "_" + PageVersionId.ToString()] = pageDocument;
        }

        /// <summary>
        /// Deletes the PageDocument with UID from temporary cache storage.
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>

        public void Delete(int PageVersionId, Guid UserUID)
        {
            HttpContext.Current.Cache.Remove(UserUID.ToString() + "_" + PageVersionId.ToString()); 
        }
        #endregion
    }
}
