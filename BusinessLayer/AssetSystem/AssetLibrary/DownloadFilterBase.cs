using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using Mediachase.Ibn.Library.Configuration;
using Mediachase.Ibn.Data;


namespace Mediachase.Ibn.Library
{
    public abstract class DownloadFilterBase
    {
        public static String downloadFiltersCfgName = "mediachase.ibn.library/downloadFilters";

        /// <summary>
        /// Initializes the specified ?.
        /// </summary>
        /// <param name="?">The ?.</param>
        public abstract void Initialize(NameValueCollection parameters);


        /// <summary>
        /// Processes the filter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public abstract void ProcessFilter(object sender, DownloadFilterArgs args);


        /// <summary>
        /// Gets the name of the filter by CFG.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static DownloadFilterBase GetFilterByCfgName(String name)
        {
           
            DownloadFilterSection section = (DownloadFilterSection)
                                                    ConfigurationManager.GetSection(downloadFiltersCfgName);
            if (section != null)
            {
                DownloadFilterCollection filters = section.Filters;
                foreach (DownloadFilterElement filter in filters)
                {
                    if (filter.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Try to create profile instance
                        Type filterType= AssemblyUtil.LoadType(filter.Type);

                        DownloadFilterBase retVal = (DownloadFilterBase)
                                        System.Activator.CreateInstance(filterType);
                        return retVal;
                    }
                }

            }
            return null;
        }
    }
}
