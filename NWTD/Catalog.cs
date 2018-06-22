using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;

namespace NWTD {

	/// <summary>
	/// A utility class for handling some very custom aspects to NWTD's catalog such as status codes and other data that's stored in ECF metadata.
	/// </summary>
	public class Catalog {


		public const string STATUS_CODE_UNAVAILABLE = "CX";
		
		/// <summary>
		/// A list of grades that a catalog item could have. This enum can be used for determining order.
		/// </summary>
		public enum Grades : int { PRESCHOOL=-1,KINDERGARTEN=0,FIRST=1,SECOND=2,THIRD=3,FOURTH=4,FIFTH=5,SIXTH=6,SEVENTH=7,EIGHTH=8,NINTH=9,TENTH=10,ELEVENTH=11,TWELFTH=12,AP=13 };

		/// <summary>
		/// The field that's used to determine state availabilty for the current user.
		/// E.G. "StateAvaiol_NV"
		/// Can be used for search filters
		/// </summary>
		public static string UserStateAvailablityField {
			get {
				if(!string.IsNullOrEmpty(NWTD.Profile.BusinessPartnerState))
					return string.Format("StateAvail_{0}", NWTD.Profile.BusinessPartnerState);
				return null;
			}
		}

		/// <summary>
		/// Returns a text representaion of a grade number.
		/// </summary>
		/// <param name="grade">The grade number to be checked.</param>
		/// <returns></returns>
		public static string GradeName(int grade) {
			switch (grade){
				case (int)Grades.PRESCHOOL:
					return "PRE";
					break;
				case  (int)Grades.KINDERGARTEN:
					return "K";
					break;
				case (int)Grades.AP: 
					return "AP";
					break;
				default:
					return grade.ToString();
					break;
			}
		}

		/// <summary>
		/// Whether the supplied entry is available for the current customer.
		/// Takes into account State availablity flags and Depository Avaialbity Flags
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public static bool IsEntryAvailable(Entry entry) {

			var statusCode = NWTD.Profile.CustomerDepository.Equals(NWTD.Depository.MSSD) ? entry.ItemAttributes["StatusCode_SLC"] : entry.ItemAttributes["StatusCode_PDX"];

			if(statusCode.Equals(STATUS_CODE_UNAVAILABLE)){
				return false;
			}

			return entry.ItemAttributes[UserStateAvailablityField].ToString().ToLower().Equals("y");
			//return true;
		}
	}
}
