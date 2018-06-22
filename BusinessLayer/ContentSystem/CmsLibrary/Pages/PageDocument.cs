using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using System.Collections.Generic;
using Mediachase.Cms;

namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Summary description for PageDocument
    /// </summary>
    /// 
    [Serializable]
    public class PageDocument
    {
        #region .ctor()
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PageDocument"/> class.
        /// </summary>
        public PageDocument()
        {
            DynamicNodes = new DynamicNodeCollection(this);
            StaticNode = new Node();
        }
        #endregion

        #region Init()
        /// <summary>
        /// Inits the specified document storages.
        /// </summary>
        /// <param name="persistentDocumentStorage">The persistent document storage.</param>
        /// <param name="temporaryDocumentStorage">The temporary document storage.</param>
        public static void Init(IPageDocumentStorageProvider persistentDocumentStorage, IPageDocumentStorageProvider temporaryDocumentStorage)
        {
            PersistentDocumentStorage = persistentDocumentStorage;
            TemporaryDocumentStorage = temporaryDocumentStorage;

        }
        #endregion

        #region ResetModified()
        /// <summary>
        /// Resets the modified.
        /// </summary>
        public void ResetModified()
        {
            if ((this.StaticNode != null) && (this.StaticNode.Controls != null) && (this.StaticNode.Controls.AllValues != null))
            {
                foreach (ControlSettings cs in this.StaticNode.Controls.AllValues)
                {
                    cs.IsModified = false;
                }
            }
            foreach (DynamicNode dn in this.DynamicNodes)
            {

                foreach (ControlSettings cs in dn.Controls.AllValues)
                {
                    cs.IsModified = false;
                }

                dn.IsModified = false;
            }
            this.DynamicNodes.IsModified = false;
            this._isModified = false;
        }
        #endregion

        #region PageVersionId
        private int _pageVersionId;
        /// <summary>
        /// Gets or sets the page version id.
        /// </summary>
        /// <value>The page version id.</value>
        public int PageVersionId
        {
            get
            {
                return _pageVersionId;
            }
            set
            {
                _pageVersionId = value;
            }
        }
        #endregion

        #region IsModified
        private bool _isModified;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsModified
        {
            get
            {
                //check dynamic nodes
                if (DynamicNodes != null && DynamicNodes.IsModified)
                    return true;
                //check static nodes
                if (StaticNode != null && StaticNode.Controls.IsModified)
                    return true;
                if (_isModified)
                    return true;
                return false;

  
            }
            set
            {
                _isModified = true;
            }
        }
        #endregion

        #region Current
        //private static PageDocument _current;
        /// <summary>
        /// Gets the current page document.
        /// </summary>
        /// <value>The current page document.</value>
        public static PageDocument Current
        {
            get
            {
				if (!HttpContext.Current.Items.Contains("Mediachase.PageDocument.Current"))
					//HttpContext.Current.Items["Mediachase.PageDocument.Current"] = new PageDocument();
					return null;

				return HttpContext.Current.Items["Mediachase.PageDocument.Current"] as PageDocument;
            }
			set
			{
				HttpContext.Current.Items["Mediachase.PageDocument.Current"] = value;
			}
        }
        #endregion

        #region TemporaryDocumentStorage
        private static IPageDocumentStorageProvider _temporaryDocumentStorage;
        /// <summary>
        /// Gets or sets the temporary document storage.
        /// </summary>
        /// <value>The temporary document storage.</value>
        public static IPageDocumentStorageProvider TemporaryDocumentStorage
        {
            get { return _temporaryDocumentStorage; }
            set { _temporaryDocumentStorage = value; }
        }
        #endregion

        #region PersistDocumentStorage
        private static IPageDocumentStorageProvider _persistentDocumentStorage;
        /// <summary>
        /// Gets or sets the persistent document storage.
        /// </summary>
        /// <value>The persistent document storage.</value>
        public static IPageDocumentStorageProvider PersistentDocumentStorage
        {
            get { return _persistentDocumentStorage; }
            set { _persistentDocumentStorage = value; }
        }
        #endregion

        #region DynamicNodes
        private DynamicNodeCollection _dynamicNodes;
        /// <summary>
        /// Gets or sets the dynamic nodes.
        /// </summary>
        /// <value>The dynamic nodes.</value>
        public DynamicNodeCollection DynamicNodes
        {
            get
            {
                return _dynamicNodes;
            }
            set
            {
                _dynamicNodes = value;
            }
        }
        #endregion

        #region StaticNode
        private Node _staticNode;
        /// <summary>
        /// Gets or sets the static node.
        /// </summary>
        /// <value>The static node.</value>
        public Node StaticNode
        {
            get
            {
                return _staticNode;
            }
            set
            {
                if (_staticNode != null)
                    _staticNode.SetOwnerDocument(null);
                _staticNode = value;
                if (_staticNode != null)
                    _staticNode.SetOwnerDocument(this);
            }

        }
        #endregion

        #region CurrentNode
        private Node _currentNode;
        /// <summary>
        /// Gets or sets the current node.
        /// </summary>
        /// <value>The current node.</value>
        public Node CurrentNode
        {
            get
            {
                return _currentNode;
            }
            set
            {
                _currentNode = value;
            }
        }
        #endregion

        #region Open()
        /// <summary>
        /// Opens the specified pagedocument.
        /// </summary>
        /// <param name="pageVersionId">The page version id.</param>
        /// <param name="openMode">The open mode.</param>
        /// <param name="userUID">The user UID.</param>
        /// <returns></returns>
        public static PageDocument Open(int pageVersionId, OpenMode openMode, Guid userUID)
        {
            PageDocument pDocument = null;
            string cacheKey = CmsCache.CreateCacheKey("pagedocument", pageVersionId.ToString(), userUID.ToString());
            
            // Check cache first
            if(openMode == OpenMode.View) // only use cache in view mode
            {
                // check cache first
                object cachedObject = CmsCache.Get(cacheKey);

                if (cachedObject != null)
                {
                    pDocument = (PageDocument)cachedObject;
                    return pDocument;
                }
            }

            int pageDocumentId = -1;
            //CHECK
            using (IDataReader reader = Database.PageDocument.GetByPageVersionId(pageVersionId))
            {
                //EXIST PD
                if (reader.Read())
                {
                    pageDocumentId = (int)reader["PageId"];
                }
                //CREATE NEW PD
                else
                {
                    pageDocumentId = Database.PageDocument.Add(pageVersionId);
                    Database.Node.Add(pageDocumentId, (int)Database.NodeType.Type.StaticNode, "StaticNode", "", "", "", 0);
                }

                reader.Close();
            }

            if (pageDocumentId > 0)
            {
                switch (openMode)
                {
                    case OpenMode.Design:
                        //load from temp storage
                        pDocument = _temporaryDocumentStorage.Load(pageVersionId, userUID);
                        //create temp page document
                        if (pDocument == null && pageVersionId == -2)
                        {
                            pDocument = new PageDocument();
                            pDocument.PageVersionId = -2;
                        }
                        //load from persist storage
                        if (pDocument == null)
                            pDocument = _persistentDocumentStorage.Load(pageVersionId, Guid.Empty);
                        break;

                    case OpenMode.View:
                        pDocument = _persistentDocumentStorage.Load(pageVersionId, Guid.Empty);

                        // Insert to the cache collection
                        CmsCache.Insert(cacheKey, pDocument, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                        break;

                    default:
                        throw new ArgumentNullException();

                }
                //_current = pDocument;

				return pDocument;
            }

			return null;
        }
        #endregion

        #region Save()
        /// <summary>
        /// Saves the specified pagedocument.
        /// </summary>
        /// <param name="pageVersionId">The page version id.</param>
        /// <param name="saveMode">The save mode.</param>
        /// <param name="userUID">The user UID.</param>
        public void Save(int pageVersionId, SaveMode saveMode, Guid userUID)
        {
            switch (saveMode)
            {
                case SaveMode.TemporaryStorage:
                    if (userUID != null)
                        TemporaryDocumentStorage.Save(this, pageVersionId, userUID);
                    break;

                case SaveMode.PersistentStorage:
                    // Save new document
                    PersistentDocumentStorage.Save(this, pageVersionId, Guid.Empty);
                    // Remove old cache
                    CmsCache.RemoveByPattern(CmsCache.CreateCacheKey("pagedocument", pageVersionId.ToString()));
                    break;

                default:
                    throw new ArgumentNullException();
            }
        }
        #endregion

        #region Delete()
        /// <summary>
        /// Deletes the specified page version id.
        /// </summary>
        /// <param name="pageVersionId">The page version id.</param>
        /// <param name="userUID">The user UID.</param>
        /// <param name="deleteMode">The delete mode.</param>
        public void Delete(int pageVersionId, Guid userUID, DeleteMode deleteMode)
        {
            switch (deleteMode)
            {
                case DeleteMode.TemporaryStorage:

                    if (userUID != Guid.Empty)
                        _temporaryDocumentStorage.Delete(pageVersionId, userUID);
                    break;

                case DeleteMode.PersistantStorage:
                    //pDocument = _persistentDocumentStorage.Load(PageVersionId);
                    break;

                default:
                    throw new ArgumentNullException("DeleteMode");

            }
        }
        #endregion

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <param name="controlUID">The control UID.</param>
		/// <returns></returns>
        public ControlSettings GetSettings(string controlUID)
        {
            try
            {
                return CurrentNode.GetSettings(controlUID);
            }
            catch
            {
                throw new EvaluateException();
            }
        }

		/// <summary>
		/// Gets the dynamic nodes.
		/// </summary>
		/// <param name="controlPlaceId">The control place id.</param>
		/// <returns></returns>
		public DynamicNode[] GetDynamicNodes(string controlPlaceId)
		{
			List<DynamicNode> retVal = new List<DynamicNode>();

			foreach (DynamicNode dn in this.DynamicNodes)
			{
				if (dn.ControlPlaceId == controlPlaceId)
					retVal.Add(dn);
			}

			retVal.Sort(new Comparison<DynamicNode>(CompareDynamicNodeByIndex));

			return retVal.ToArray();
		}

		/// <summary>
		/// Compares the index of the dynamic node by.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		private static int CompareDynamicNodeByIndex(DynamicNode x, DynamicNode y)
		{
			return x.ControlPlaceIndex.CompareTo(y.ControlPlaceIndex);
		}
	}

}