using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Catalog.Events;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Catalog.Data;
using System.Threading;
using System.Web;
using System.Data;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Commerce.Catalog.Loggers
{
    public class CatalogLogger : ICatalogEventListener
    {
        LogAdmin _admin = null;
        #region ICatalogEventListener Members

        /// <summary>
        /// Entries updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        public void EntryUpdating(object source, EntryEventArgs args)
        {
            CatalogEntryDto dto = (CatalogEntryDto)source;
            
            if (_admin == null)
                _admin = new LogAdmin(new LogDto());

            //dto.CatalogEntry.CatalogEntryRowDeleted += new CatalogEntryDto.CatalogEntryRowChangeEventHandler(CatalogEntry_CatalogEntryRowDeleted);
            //dto.CatalogEntry.CatalogEntryRowChanged += new CatalogEntryDto.CatalogEntryRowChangeEventHandler(CatalogEntry_CatalogEntryRowChanged);
            dto.CatalogEntry.CatalogEntryRowChanging += new CatalogEntryDto.CatalogEntryRowChangeEventHandler(CatalogEntry_CatalogEntryRowChanging);
        }

        /// <summary>
        /// Entries updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        public void EntryUpdated(object source, EntryEventArgs args)
        {
            if(_admin!=null)
                _admin.Save();
        }

        public void NodeUpdating(object source, NodeEventArgs args)
        {
        }

        public void NodeUpdated(object source, NodeEventArgs args)
        {
        }

        public void CatalogUpdating(object source, CatalogEventArgs args)
        {
        }

        public void CatalogUpdated(object source, CatalogEventArgs args)
        {
        }

        public void AssociationUpdating(object source, AssociationEventArgs args)
        {
        }

        public void AssociationUpdated(object source, AssociationEventArgs args)
        {
        }

        public void RelationUpdating(object source, RelationEventArgs args)
        {
        }

        public void RelationUpdated(object source, RelationEventArgs args)
        {
        }

        #endregion

        /// <summary>
        /// Catalogs the entry_ catalog entry row changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void CatalogEntry_CatalogEntryRowChanging(object sender, CatalogEntryDto.CatalogEntryRowChangeEvent e)
        {
            Record(e.Row, e.Action);
        }

        /// <summary>
        /// Catalogs the entry_ catalog entry row deleted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void CatalogEntry_CatalogEntryRowDeleted(object sender, CatalogEntryDto.CatalogEntryRowChangeEvent e)
        {
            Record(e.Row, e.Action);
        }

        /// <summary>
        /// Catalogs the entry_ catalog entry row changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void CatalogEntry_CatalogEntryRowChanged(object sender, CatalogEntryDto.CatalogEntryRowChangeEvent e)
        {
            Record(e.Row, e.Action);
        }

        /// <summary>
        /// Records the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="action">The action.</param>
        private void Record(CatalogEntryDto.CatalogEntryRow entry, DataRowAction action)
        {
            if (
                action == DataRowAction.Commit ||
                action == DataRowAction.Delete ||
                action == DataRowAction.Change
                )
            {
                if (entry.RowState == DataRowState.Added ||
                    entry.RowState == DataRowState.Deleted ||
                    entry.RowState == DataRowState.Modified
                    )
                {
                    LogDto dto = _admin.CurrentDto;
                    LogDto.ApplicationLogRow row = dto.ApplicationLog.NewApplicationLogRow();
                    row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    row.Created = DateTime.UtcNow;
                    row.Notes = String.Empty;
                    if (entry.RowState == DataRowState.Deleted)
                        row.ObjectKey = entry["CatalogEntryId", DataRowVersion.Original].ToString();
                    else
                        row.ObjectKey = entry.CatalogEntryId.ToString();

                    row.Source = "catalog";
                    row.ObjectType = "entry";
                    row.Operation = entry.RowState.ToString();
                    row.Succeeded = true;
                    row.Username = GetUsername();
                    row.IPAddress = GetIPAddress();
                    dto.ApplicationLog.AddApplicationLogRow(row);
                }
            }
        }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <returns></returns>
        private string GetIPAddress()
        {
            if (HttpContext.Current != null)
            {
                try
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                catch
                {
                    return String.Empty;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <returns></returns>
        private string GetUsername()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User.Identity.Name;
            }
            else
            {
                return Thread.CurrentPrincipal.Identity.Name;
            }
        }
    }
}
