using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Blob;
using Mediachase.Ibn.Library.Configuration;
using System.Configuration;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Data;
using System.IO;
using Mediachase.Ibn.Blob.ContentTypeResolver;
using Mediachase.Ibn.Blob.BlobProfileDownload.Configuration;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using Mediachase.Ibn.Data.Services;
using System.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.FileUploader.Web;

namespace Mediachase.Ibn.Library
{
    partial class FolderElement
    {
        public static String folderElementExtensionCfgName = "mediachase.ibn.library/folderElementExtension";
        public static String folderElementLocationsCfgName = "mediachase.ibn.library/folderElementLocations";
        public static String wildCardChar = "*";

        public static String filterParam = "filter";

        private BlobInfo _cacheBlobInfo;

        /// <summary>
        /// Gets the BLOB info.
        /// </summary>
        /// <returns></returns>
        public BlobInfo GetBlobInfo()
        {
            if (!this.BlobUid.HasValue)
                return null;

            if (_cacheBlobInfo != null && _cacheBlobInfo.Uid == this.BlobUid)
                return _cacheBlobInfo;

            _cacheBlobInfo = BlobStorage.Providers[this.BlobStorageProvider].GetInfo(this.BlobUid.Value);
            return _cacheBlobInfo;
        }

        /// <summary>
        /// Finds the last folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="path">The path.</param>
        /// <param name="delim">The delim.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        private static Folder FindFolderByPattern(Folder folder, string path, char delim,
                                                  string pattern)
        {
            Folder retVal = null;
            TreeService tree = folder.GetTreeService();

            //recursion end point
            if (pattern.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                return folder;

            foreach (TreeNode node in tree.GetChildNodes())
            {
                String folderPath = path + delim + ((Folder)node.InnerObject).Name;
                if (retVal != null)
                    break;

                if (pattern.StartsWith(folderPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    retVal = FindFolderByPattern((Folder)node.InnerObject, folderPath,
                                                delim, pattern);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the element by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static FolderElement[] GetElementsByPath(string path)
        {
            FolderElement[] retVal = null;
            char delim = '/';

            string[] folderParts = path.Split(new char[] { delim });

            //Incorrect path
            if (folderParts.Length == 0)
                return retVal;

            string rootFolderName = folderParts[0];

            FilterElement filter =
                        new FilterElement("Name", FilterElementType.Equal, rootFolderName);

            Folder[] folders = Folder.List(filter);
            if (folders.Length != 0)
            {
                Folder rootFolder = folders[0];
                rootFolder = FindFolderByPattern(rootFolder, rootFolder.Name, delim,
                                                String.Join(delim.ToString(), folderParts));
                if (rootFolder != null)
                {
                    FilterElement filterElem =
                                    new FilterElement("ParentId", FilterElementType.Equal,
                                                      rootFolder.PrimaryKeyId.Value);
                    retVal = FolderElement.List(filterElem);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the hierarhy.
        /// </summary>
        /// <param name="delim">The delim.</param>
        /// <returns></returns>
        public string GetElementPath(string delim)
        {
            StringBuilder retVal = new StringBuilder(delim);
            if (this.ParentId != null)
            {
                Folder folder = new Folder((int)this.ParentId);
                TreeService tree = folder.GetTreeService();
                if (tree != null)
                {
                    TreeNode[] nodes = tree.GetPathToCurrentNode();
                    foreach (TreeNode node in nodes)
                    {
                        retVal.Append(((Folder)node.InnerObject).Name + delim);
                    }

                    retVal.Append(folder.Name + delim);

                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Moves the specified folder element.
        /// </summary>
        /// <param name="folderElement">The folder element.</param>
        /// <param name="newParentId">The new parent id.</param>
        public static void Move(FolderElement folderElement, int newParentId)
        {
            folderElement.ParentId = newParentId;
            folderElement.Save();
        }

        /// <summary>
        /// Moves the specified folder element id.
        /// </summary>
        /// <param name="folderElementId">The folder element id.</param>
        /// <param name="newParentId">The new parent id.</param>
        public static void Move(int folderElementId, int newParentId)
        {
            FolderElement folderElement = new FolderElement(folderElementId);
            Move(folderElement, newParentId);
        }

        /// <summary>
        /// Copies the specified folder element.
        /// </summary>
        /// <param name="folderElement">The folder element.</param>
        /// <param name="newParentId">The parent id.</param>
        public static void Copy(FolderElement folderElement, int parentId)
        {
            FolderElement newElement = null;

            BlobInfo blobInfo = folderElement.GetBlobInfo();
            BlobInfo blobInfoNew = (BlobInfo)blobInfo.Clone();
            blobInfoNew.Uid = Guid.NewGuid();

            BlobStorageProvider provider = BlobStorage.Providers[folderElement.BlobStorageProvider];

            using (Stream srcStream = provider.ReadStream(blobInfo))
            {
                using (Stream dstStream = provider.CreateStream(blobInfoNew))
                {
                    BlobStreamHelper.WriteToStream(dstStream, srcStream, 0, srcStream.Length);
                    provider.CommitStream(blobInfoNew);
                }
            }

            newElement = (FolderElement)folderElement.Clone();
            newElement.BlobUid = blobInfoNew.Uid;
            newElement.Save();

            Move(newElement, parentId);
        }

        /// <summary>
        /// Copies the specified folder element id.
        /// </summary>
        /// <param name="folderElementId">The folder element id.</param>
        /// <param name="parentId">The parent id.</param>
        public static void Copy(int folderElementId, int parentId)
        {
            FolderElement folderElement = new FolderElement(folderElementId);
            Copy(folderElement, parentId);
        }

        /// <summary>
        /// Deletes the specified folder element.
        /// </summary>
        /// <param name="folderElement">The folder element.</param>
        public static void Delete(FolderElement folderElement)
        {
            folderElement.Delete();
        }

        /// <summary>
        /// Deletes the specified folder element id.
        /// </summary>
        /// <param name="folderElementId">The folder element id.</param>
        public static void Delete(int folderElementId)
        {
            FolderElement folderElement = new FolderElement(folderElementId);

            Delete(folderElement);
        }

        /// <summary>
        /// Called when [deleting].
        /// </summary>
        protected override void OnDeleting()
        {
            base.OnDeleting();

            CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(PrimaryKeyId.ToString());
            if (catalogRelationDto.CatalogItemAsset.Count > 0)
            {
                for (int i = 0; i < catalogRelationDto.CatalogItemAsset.Count; i++)
                {
                    catalogRelationDto.CatalogItemAsset[i].Delete();
                }

                if (catalogRelationDto.HasChanges())
                    CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
            }

            // Clean Up BlobStorage
            BlobStorageProvider provider = BlobStorage.Providers[BlobStorageProvider];
            if (provider != null)
            {
                BlobInfo blobInfo = provider.GetInfo((Guid)BlobUid);
                if (blobInfo != null)
                    provider.ReleaseStream(blobInfo);
            }
        }

        /// <summary>
        /// Creates the specified parent id.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="srcStream">The SRC stream.</param>
        /// <returns></returns>
        public static FolderElement Create(int parentId, string fileName,
                                           Stream srcStream, Guid progressUid)
        {
            FolderElement retVal = new FolderElement();
            BlobInfo blobInfo = new BlobInfo(Guid.NewGuid());

            retVal.ParentId = parentId;

            //Try recognize provider from config setings
            BlobStorageProvider provider = BlobStorage.Provider;
            String providerName;
            String contentType = Mediachase.Ibn.Blob.ContentTypeResolver.ContentType.ResolveByPath(fileName);
            long fileSize = srcStream.Length;

            if (TryRecognizeStorageProvider(retVal, fileName, contentType,
                                            fileSize, out providerName))
            {
                provider = BlobStorage.Providers[providerName];
            }

            blobInfo.FileName = fileName;
            blobInfo.Provider = provider.Name;
            blobInfo.Created = DateTime.UtcNow;
            blobInfo.ContentSize = fileSize;
            blobInfo.ContentType = contentType;
            //Content type assigned by provider

            //Save blob info to storage
            using (Stream dstStream = provider.CreateStream(blobInfo))
            {
                //BlobStreamHelper.WriteToStream(dstStream, srcStream, 0, fileSize);
                WriteToStream(dstStream, srcStream, 0, fileSize, progressUid);
            }

            provider.CommitStream(blobInfo);

            retVal.BlobStorageProvider = provider.Name;
            retVal.BlobUid = blobInfo.Uid;
            retVal.Created = DateTime.UtcNow;
            retVal.Name = blobInfo.FileName;
            if(retVal.Properties.Contains("ContentSize"))
                retVal.Properties["ContentSize"].Value = (Int32)blobInfo.ContentSize;
            if (retVal.Properties.Contains("ContentType"))
                retVal.Properties["ContentType"].Value = blobInfo.ContentType;

            //try
            IFolderElementExtension folderEl = GetCfgFolderElementExtension(blobInfo);

            if (folderEl != null)
                folderEl.Process(retVal);

            retVal.Save();
            return retVal;
        }

        public static FolderElement Create(int parentId, string fileName, Stream srcStream)
        {
            return Create(parentId, fileName, srcStream, Guid.Empty);
        }

        /// <summary>
        /// Writes to stream.
        /// </summary>
        /// <param name="dstStream">The DST stream.</param>
        /// <param name="srcStream">The SRC stream.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="length">The length.</param>
        public static void WriteToStream(Stream dstStream, Stream srcStream, long startOffset,
                                          long length, Guid progressUid)
        {
            long maxBufferSize = 65443;

            if (dstStream == null || srcStream == null)
                throw new ArgumentNullException("stream");

            int bufferSize = maxBufferSize < length ? (int)maxBufferSize : (int)length;
            byte[] tmpBuffer = new byte[bufferSize];
            int readCount = 0;
            long allBytes = 0;
            srcStream.Seek(startOffset, SeekOrigin.Begin);
            do
            {
                readCount = srcStream.Read(tmpBuffer, 0, bufferSize);
                allBytes += readCount;
                readCount = allBytes > length ? readCount - (int)(allBytes - length)
                                              : readCount;
                dstStream.Write(tmpBuffer, 0, readCount);

                if (progressUid != Guid.Empty)
                    UploadProgress.Provider.UpdateBytesReceived(progressUid, (Int32)allBytes); 
            }
            while ((readCount != 0) && (allBytes < length));

            dstStream.Flush();
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="downloadProfile">The download profile.</param>
        /// <returns></returns>
        public String GetUrl()
        {
            StringBuilder retVal = new StringBuilder();
            BlobStorageProvider provider =
                        BlobStorage.Providers[this.BlobStorageProvider];

            if ((provider != null) && (this.BlobUid != null))
            {
                // eliminate calls to blobinfo
                //BlobInfo blobInfo = provider.GetInfo((Guid)this.BlobUid);
                //if (blobInfo != null)
                {
                    retVal.Append(GetElementPath("/"));
                    retVal.Append(this.Name);
                    //Read from config add or not HandlerExtension
                    String addHandlerExt = ConfigurationManager.AppSettings["LibraryHandlerExtension"];
                    if (!String.IsNullOrEmpty(addHandlerExt))
                    {
                        retVal.Append(addHandlerExt);
                    }
                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Tries the recognize storage provider.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static bool TryRecognizeStorageProvider(FolderElement element, string fileName,
                                                        String contentType, long fileSize,
                                                        out String providerName)
        {
            string profileName = String.Empty;
            return TryRecognizeStorageProvider(element, fileName, contentType, fileSize, out providerName, out profileName);
        }

        /// <summary>
        /// Tries the recognize provider.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="profileName">Name of the profile.</param>
        /// <returns></returns>
        public static bool TryRecognizeStorageProvider(FolderElement element, string fileName,
                                                        String contentType, long fileSize,
                                                        out String providerName, out string profileName)
        {
            providerName = String.Empty;
            profileName = String.Empty;
            FileLibraryLocationSection section = (FileLibraryLocationSection)
                                                          ConfigurationManager.GetSection(folderElementLocationsCfgName);

            if (section != null)
            {
                FileLibraryLocationCollection locations = section.Locations;
                Dictionary<String, LocationRuleElement> matchList =
                                 new Dictionary<string, LocationRuleElement>();

                string hierarhy = element.GetElementPath("\\");
                foreach (FileLibraryLocationElement location in locations)
                {
                    bool wildcard = location.Path.EndsWith(wildCardChar);
                    string locationPath = wildcard == true ?
                            location.Path.Substring(0, location.Path.IndexOf(wildCardChar))
                            : location.Path;

                    if (hierarhy.Equals(locationPath,
                                       StringComparison.InvariantCultureIgnoreCase)
                        || ((wildcard == true) && (hierarhy.StartsWith(locationPath, StringComparison.InvariantCultureIgnoreCase))))
                    {

                        LocationRuleCollections rules = location.LocationRules;

                        foreach (LocationRuleElement rule in rules)
                        {
                            bool found = true;

                            if (!String.IsNullOrEmpty(rule.Extension))
                            {
                                found = fileName.EndsWith(rule.Extension,
                                    StringComparison.InvariantCultureIgnoreCase) ? found : false;
                            }

                            if (!String.IsNullOrEmpty(rule.MimeType))
                            {
                                found = contentType.Equals(rule.MimeType,
                                    StringComparison.InvariantCultureIgnoreCase) ? found : false;
                            }

                            if (!String.IsNullOrEmpty(rule.MaxSize))
                            {
                                String regExpPat = @"(?<size>[0-9]+)\s*(?<unit>Kb|Mb|Gb)?";
                                Regex regExp = new Regex(regExpPat, RegexOptions.IgnoreCase);
                                Match sizeMatch = regExp.Match(rule.MaxSize);
                                if (sizeMatch.Success)
                                {
                                    long maxSize = Convert.ToInt64(sizeMatch.Groups["size"].Value);
                                    string unit = sizeMatch.Groups["unit"].Value;
                                    if (!String.IsNullOrEmpty(unit))
                                    {
                                        if (unit.Equals("Kb", StringComparison.InvariantCultureIgnoreCase))
                                            maxSize *= 1024;
                                        else if (unit.Equals("Mb", StringComparison.InvariantCultureIgnoreCase))
                                            maxSize *= 1048576;
                                        else if (unit.Equals("Gb", StringComparison.InvariantCultureIgnoreCase))
                                            maxSize *= 1073741824;
                                    }

                                    found = fileSize >= maxSize ? found : false;
                                }
                            }

                            if ((found == true) || (String.IsNullOrEmpty(rule.MaxSize)
                                                      && String.IsNullOrEmpty(rule.MimeType)
                                                      && String.IsNullOrEmpty(rule.MaxSize)))
                            {
                                matchList.Add(location.Path, rule);
                                break;
                            }
                        }
                    }
                }

                if (matchList.Count != 0)
                {
                    //get location element with max depth
                    int maxDepth = -1;
                    String maxKey = String.Empty;
                    foreach (String key in matchList.Keys)
                    {
                        int depth = key.Split(new Char[] { '\\' }).Length;
                        if (depth > maxDepth)
                        {
                            maxDepth = depth;
                            maxKey = key;
                        }
                    }
                    providerName = matchList[maxKey].StorageProvider;
                    profileName = matchList[maxKey].DownloadProfile;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the folder element type from CFG.
        /// </summary>
        /// <param name="blobInfo">The BLOB info.</param>
        /// <returns></returns>
        private static IFolderElementExtension GetCfgFolderElementExtension(BlobInfo blobInfo)
        {
            FolderElementExtensionSection section = (FolderElementExtensionSection)
                                                          ConfigurationManager.GetSection(folderElementExtensionCfgName);

            if (section != null)
            {
                foreach (FolderElementExtensionElement element in section.ElementTypes)
                {
                    if (element.MimeType.Equals(blobInfo.ContentType,
                                               StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Try to create profile instance
                        Type folderType = AssemblyUtil.LoadType(element.Type);

                        IFolderElementExtension retVal = (IFolderElementExtension)
                                                    Activator.CreateInstance(folderType);
                        //Initialize config parameters
                        retVal.Initialize(element);

                        return retVal;
                    }
                }
            }

            return null;
        }
    }
}
