using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Cms.Controls
{
	public class DynamicControlCategory : IComparable
	{
		#region Fields
		private string _name = string.Empty;
		private List<DynamicControlInfo> _controls = new List<DynamicControlInfo>();
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicControlCategory"/> class.
		/// </summary>
		public DynamicControlCategory()
		{
		}
		#endregion

		#region Properties
		

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets the dynamic control infos.
		/// </summary>
		/// <value>The dynamic control infos.</value>
		public IList<DynamicControlInfo> DynamicControlInfos
		{
			get { return _controls; }
		}
	
		#endregion

		#region IComparer Members

        /// <summary>
        /// Compares the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
		public int Compare(object x, object y)
		{
			return string.Compare(((DynamicControlCategory)x).Name, ((DynamicControlCategory)y).Name);
		}

		#endregion

		#region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance. </exception>
		public int CompareTo(object obj)
		{
			return this.Name.CompareTo(((DynamicControlCategory)obj).Name);
		}

		#endregion
	}
}
