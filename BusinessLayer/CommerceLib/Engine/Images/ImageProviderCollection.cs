using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace Mediachase.Commerce.Engine.Images
{
    /// <summary>
    /// Implements operations for the image provider collection. (Inherits <see cref="ProviderCollection"/>.)
    /// </summary>
    public class ImageProviderCollection : ProviderCollection
    {
        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Engine.Images.ImageProvider"/> with the specified name.
        /// </summary>
        /// <value></value>
        public new ImageProvider this[string name]
        {
            get { return (ImageProvider)base[name]; }
        }

        /// <summary>
        /// Adds a provider to the collection.
        /// </summary>
        /// <param name="provider">The provider to be added.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="provider"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is null.- or -The length of the <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is less than 1.</exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// </PermissionSet>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is ImageProvider))
                throw new ArgumentException
                    ("Invalid provider type", "provider");

            base.Add(provider);
        }
    }
}