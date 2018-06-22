using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager
{
    /// <summary>
    /// Summary description for EntrySearchService
    /// </summary>
    [WebService(Namespace = "http://mediachase.com/ecf50")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class EntrySearchService : System.Web.Services.WebService
    {
        public EntrySearchService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public string[] GetVariationList(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }

            CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntriesDto(String.Format("%{0}%", prefixText), EntryType.Variation);

            List<string> items = new List<string>(count);

            int newCount = 0;
            foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry)
            {
                string name = row.Name;

                name = name.Replace('|', '-');
                //name = name.Replace(']', '-');

                items.Add(String.Format("{0}|{1}", name, row.CatalogEntryId));
                newCount++;

                if (newCount == count)
                    break;
            }

            return items.ToArray();
        }

        [WebMethod]
        public string[] GetEntryList(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }

            CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntriesDto(String.Format("%{0}%", prefixText), String.Empty);

            List<string> items = new List<string>(count);

            int newCount = 0;
            foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry)
            {
                string name = row.Name;

                name = name.Replace('|', '-');
                //name = name.Replace(']', '-');

                items.Add(String.Format("{0}|{1}", name, row.CatalogEntryId));
                newCount++;

                if (newCount == count)
                    break;
            }

            return items.ToArray();
        }


    }
}