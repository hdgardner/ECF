using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains collection of item file.
    /// </summary>
    [DataContract]
    public partial class ItemFiles
    {
        /// <summary>Collection of files</summary>
        private ItemFile[] _File;

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public ItemFile[] File
        {
            get { return _File; }
            set { _File = value; }
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Catalog.Objects.ItemFile"/> with the specified name.
        /// </summary>
        /// <value></value>
        public ItemFile this[string name]
        {
            get
            {
                if (_File != null)
                    foreach (ItemFile file in _File)
                    {
                        if (file.Name.ToLower().CompareTo(name.ToLower()) == 0)
                            return file;
                    }

                return null;
            }
        }
    }
}
