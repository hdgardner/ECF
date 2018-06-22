using System;




namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Summary description for IPageDocumentStorageProvider
    /// </summary>
    public interface IPageDocumentStorageProvider
    {


        /// <summary>
        /// PageDocument Load
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
        /// <returns></returns>
    	PageDocument Load(int PageVersionId,Guid UserUID);

        /// <summary>
        /// PageDocument Save
        /// </summary>
        /// <param name="pageDocument">The page document.</param>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
        void Save(PageDocument pageDocument, int PageVersionId, Guid UserUID);



        /// <summary>
        /// Deletes the specified page version id.
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
        void Delete(int PageVersionId, Guid UserUID);

    }
}
