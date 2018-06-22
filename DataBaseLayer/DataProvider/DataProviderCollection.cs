using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace Mediachase.Data.Provider
{
    public class DataProviderCollection : ProviderCollection
    {
        public new DataProvider this[string name]
        {
            get { return (DataProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is DataProvider))
                throw new ArgumentException
                    ("Invalid provider type", "provider");

            base.Add(provider);
        }
    }
}
