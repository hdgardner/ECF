using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Threading;
using System.Web;
using System.Data;
using Mediachase.Commerce.Profile.Events;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Commerce.Profile.Loggers
{
    /// <summary>
    /// Profile logger, logs all the events related to profile into the application log database.
    /// </summary>
    public class ProfileLogger : IProfileEventListener
    {
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

        #region IProfileEventListener Members
        /// <summary>
        /// Principals the created.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.PrincipalEventArgs"/> instance containing the event data.</param>
        public void PrincipalCreated(object source, PrincipalEventArgs args)
        {
            Record((Principal)source);
        }

        /// <summary>
        /// Principals the updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.PrincipalEventArgs"/> instance containing the event data.</param>
        public void PrincipalUpdated(object source, PrincipalEventArgs args)
        {
            Record((Principal)source);
        }

        /// <summary>
        /// Principals the deleted.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.PrincipalEventArgs"/> instance containing the event data.</param>
        public void PrincipalDeleted(object source, PrincipalEventArgs args)
        {
            Record((Principal)source);
        }

        /// <summary>
        /// Addresses the created.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.AddressEventArgs"/> instance containing the event data.</param>
        public void AddressCreated(object source, AddressEventArgs args)
        {
            Record((CustomerAddress)source);
        }

        /// <summary>
        /// Addresses the updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.AddressEventArgs"/> instance containing the event data.</param>
        public void AddressUpdated(object source, AddressEventArgs args)
        {
            Record((CustomerAddress)source);
        }

        /// <summary>
        /// Addresses the deleted.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Profile.Events.AddressEventArgs"/> instance containing the event data.</param>
        public void AddressDeleted(object source, AddressEventArgs args)
        {
            Record((CustomerAddress)source);
        }

        #endregion

        /// <summary>
        /// Records the specified principal.
        /// </summary>
        /// <param name="principal">The principal.</param>
        private void Record(Principal principal)
        {
            LogAdmin admin = new LogAdmin(new LogDto());
            LogDto dto = admin.CurrentDto;
            LogDto.ApplicationLogRow row = dto.ApplicationLog.NewApplicationLogRow();
            row.ApplicationId = AppContext.Current.ApplicationId;
            row.Created = DateTime.UtcNow;
            row.Notes = String.Empty;
            row.ObjectKey = principal.PrincipalId.ToString();
            row.ObjectType = principal.Type;

            string operation = String.Empty;
            if (principal.ObjectState == MetaObjectState.Added)
                operation = DataRowState.Added.ToString();
            else if (principal.ObjectState == MetaObjectState.Deleted)
                operation = DataRowState.Deleted.ToString();
            else if (principal.ObjectState == MetaObjectState.Modified)
                operation = DataRowState.Modified.ToString();
            else if (principal.ObjectState == MetaObjectState.Unchanged)
                operation = DataRowState.Unchanged.ToString();

            row.Operation = operation;
            row.Succeeded = true;
            row.Source = "profile";
            row.Username = GetUsername();
            row.IPAddress = GetIPAddress();
            dto.ApplicationLog.AddApplicationLogRow(row);
            admin.Save();
        }

        /// <summary>
        /// Records the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        private void Record(CustomerAddress address)
        {
            LogAdmin admin = new LogAdmin(new LogDto());
            LogDto dto = admin.CurrentDto;
            LogDto.ApplicationLogRow row = dto.ApplicationLog.NewApplicationLogRow();
            row.ApplicationId = AppContext.Current.ApplicationId;
            row.Created = DateTime.UtcNow;
            row.Notes = String.Empty;
            row.ObjectKey = address.PrincipalId.ToString();
            row.ObjectType = "address";

            string operation = String.Empty;
            if (address.ObjectState == MetaObjectState.Added)
                operation = DataRowState.Added.ToString();
            else if (address.ObjectState == MetaObjectState.Deleted)
                operation = DataRowState.Deleted.ToString();
            else if (address.ObjectState == MetaObjectState.Modified)
                operation = DataRowState.Modified.ToString();
            else if (address.ObjectState == MetaObjectState.Unchanged)
                operation = DataRowState.Unchanged.ToString();
            
            row.Operation = operation;

            row.Succeeded = true;
            row.Source = "profile";
            row.Username = GetUsername();
            row.IPAddress = GetIPAddress();
            dto.ApplicationLog.AddApplicationLogRow(row);
            admin.Save();
        }
    }
}
