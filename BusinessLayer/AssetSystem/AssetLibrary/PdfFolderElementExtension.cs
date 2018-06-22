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
    public class PdfFolderElementExtension : IFolderElementExtension
    {
        public static String cardName = "PdfFolderElement";
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
        }

        #endregion
    }
}
