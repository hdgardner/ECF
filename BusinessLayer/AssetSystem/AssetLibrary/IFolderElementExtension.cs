using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Mediachase.Ibn.Library.Configuration;
using System.IO;
using Mediachase.Ibn.Blob;

namespace Mediachase.Ibn.Library
{
    public interface IFolderElementExtension
    {
        /// <summary>
        /// Initializes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        void Initialize(FolderElementExtensionElement param);
        /// <summary>
        /// Processes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Process(FolderElement element);

    }
}
