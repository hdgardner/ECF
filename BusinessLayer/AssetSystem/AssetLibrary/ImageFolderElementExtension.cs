using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Mediachase.Ibn.Library.Configuration;
using Mediachase.Ibn.Blob;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using System.IO;

namespace Mediachase.Ibn.Library
{
    public class ImageFolderElementExtension : IFolderElementExtension
    {
        public static String cardName = "ImageFolderElement";
        #region IFolderElementType Members

        /// <summary>
        /// Initializes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        public void Initialize(FolderElementExtensionElement param)
        {
         //Nothing to do;   
        }

        /// <summary>
        /// Processes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Process(FolderElement element)
        {
            element.Card = cardName;
            BlobInfo blobInfo = element.GetBlobInfo();
            if(blobInfo != null)
            {
                BlobStorageProvider provider = BlobStorage.Providers[blobInfo.Provider];
                if(provider != null)
                {
                    using(Stream stream = provider.ReadStream(blobInfo))
                    {
                        try
                        {
                            Image img = Image.FromStream(stream);
                            element.Properties["Width"].Value = img.Width;
                            element.Properties["Height"].Value = img.Height;

                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                   
                }
               
                       
            }
            
        }

        #endregion
    }
}
