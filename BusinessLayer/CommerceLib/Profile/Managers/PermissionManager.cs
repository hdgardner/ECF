using System;
using System.Data;
using Mediachase.Commerce.Profile.Data;
using Mediachase.Commerce.Profile.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Profile.Managers
{
	/// <summary>
	/// Implements operations for the permission manager.
	/// </summary>
	public static class PermissionManager
	{
		#region Permission Functions
		/// <summary>
		/// Gets the Permissiones.
		/// </summary>
		/// <returns></returns>
		public static PermissionDto GetPermissionDto(string roleName)
		{
			PermissionAdmin admin = new PermissionAdmin();
			admin.Load(new string[] { roleName });

			return admin.CurrentDto;
		}
		#endregion

		#region Edit Permission Functions
		/// <summary>
		/// Saves changes in PermissionDto.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SavePermission(PermissionDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("PermissionDto can not be null"));

			PermissionAdmin admin = new PermissionAdmin(dto);
			admin.Save();
		}
		#endregion
	}
}