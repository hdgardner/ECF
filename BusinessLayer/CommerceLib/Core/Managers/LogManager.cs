using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Data;
using System.Web;
using System.Threading;

namespace Mediachase.Commerce.Core.Managers
{
    public static class LogManager
    {
        /// <summary>
        /// Gets the system log.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="created">The created.</param>
        /// <param name="startingRecord">The starting record.</param>
        /// <param name="numberOfRecords">The number of records.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static LogDto GetSystemLog(string operation, string objectType, DateTime created, int startingRecord, int numberOfRecords, ref int totalRecords)
        {
            LogDto dto = null;
            LogAdmin admin = new LogAdmin();
            admin.Load(true, null, operation, objectType, created, startingRecord, numberOfRecords, ref totalRecords);
            dto = admin.CurrentDto;
            return dto;
        }

        /// <summary>
        /// Gets the application log.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="created">The created.</param>
        /// <param name="startingRecord">The starting record.</param>
        /// <param name="numberOfRecords">The number of records.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static LogDto GetAppLog(string source, string operation, string objectType, DateTime created, int startingRecord, int numberOfRecords, ref int totalRecords)
        {
            LogDto dto = null;
            LogAdmin admin = new LogAdmin();
            admin.Load(false, source, operation, objectType, created, startingRecord, numberOfRecords, ref totalRecords);
            dto = admin.CurrentDto;
            return dto;
        }

        /// <summary>
        /// Gets the log by id.
        /// </summary>
        /// <returns></returns>
        public static LogDto GetAppLog(int logId)
        {
            LogDto dto = null;
            LogAdmin admin = new LogAdmin();
            admin.Load(logId);
            dto = admin.CurrentDto;
            return dto;
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="key">The key.</param>
        /// <param name="type">The type.</param>
        /// <param name="source">The source.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        public static void WriteLog(string operation, string key, string type, string source, string notes, bool succeeded)
        {
            WriteLog(operation, GetUsername(), key, type, source, notes, succeeded);
        }

        /// <summary>
        /// Writes the log, truncates the strings to the maximum allowed length.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="key">The key.</param>
        /// <param name="type">The type.</param>
        /// <param name="source">The source.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        public static void WriteLog(string operation, string userName, string key, string type, string source, string notes, bool succeeded)
        {
            LogAdmin admin = new LogAdmin(new LogDto());
            LogDto dto = admin.CurrentDto;
            LogDto.ApplicationLogRow row = dto.ApplicationLog.NewApplicationLogRow();
            row.ApplicationId = AppContext.Current.ApplicationId;
            row.Created = DateTime.UtcNow;
            row.Notes = (notes.Length > 255) ? notes.Substring(0, 255) : notes;
            row.ObjectKey = (key.Length > 100) ? key.Substring(0, 100) : key;
            row.Source = (source.Length > 100) ? source.Substring(0, 100) : source;
            row.ObjectType = (type.Length > 50) ? type.Substring(0, 50) : type;
            row.Operation = (operation.Length > 50) ? operation.Substring(0, 50) : operation;
            row.Succeeded = succeeded;
            row.Username = (userName.Length > 50) ? userName.Substring(0, 50) : userName;
            row.IPAddress = GetIPAddress();
            dto.ApplicationLog.AddApplicationLogRow(row);
            admin.Save();
        }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <returns></returns>
        private static string GetIPAddress()
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
        private static string GetUsername()
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
        /// Saves the log.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveAppLog(LogDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("LogDto can not be null"));

            LogAdmin admin = new LogAdmin(dto);
            admin.Save();
        }
    }
}
