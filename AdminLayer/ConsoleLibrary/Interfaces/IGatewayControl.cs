using System;

namespace Mediachase.Web.Console.Interfaces
{
	/// <summary>
	/// Used to dynamically load payment/shipping gateways' additional configuration controls.
	/// </summary>
	public interface IGatewayControl
	{
        /// <summary>
        /// Saves the object changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
		void SaveChanges(object dto);
        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="dto">The dto.</param>
		void LoadObject(object dto);

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
		string ValidationGroup { get;set;}
	}
}