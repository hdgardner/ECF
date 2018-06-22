using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Workflow.Activities.Rules;
using System.CodeDom;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Marketing;
using System.IO;
using System.Globalization;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using Mediachase.Commerce.Marketing.Validators.Providers.DomParser;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Orders;
using System.Web;
using System.Linq;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
	/// <summary>
	/// Represents Customer segments filter expression provider.
	/// </summary>
	public class CustomerFilterExpressionProvider : PromotionFilterExpressionProvider
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor

		/// <summary>
        /// Initializes a new instance of the <see cref="CustomerFilterExpressionProvider"/> class.
		/// </summary>
        public CustomerFilterExpressionProvider()
		{
		}
		#endregion

		#region Properties

		#region FilterElement definitions
		protected  IEnumerable<ElementDefs> AddressDefs { get; private set; }
		protected  IEnumerable<ElementDefs> AccountDefs { get; private set; }
		#endregion

		#region FilterElement Method Definition
		protected  IEnumerable<ElementDefs> AddressMethodDefs { get; set; }
		#endregion

		#region FilterElement Conditions Definitions
		#endregion

		#endregion


		#region Methods

		protected override void RegisterProviderElementDefinitions()
		{
			//First call base implementation
			base.RegisterProviderElementDefinitions();

			#region Condition definitions
			#endregion

			#region Method definitions
			AddressMethodDefs = new ElementDefs[] { base.MethodAllDef, 
													base.MethodAnyDef };
			RegisterDefinitions(AddressMethodDefs);
			#endregion

			#region element definitions
			AddressDefs =
				new ElementDefs[]{
										new ElementDefs() { Key = ElementDefs.AccountAddress_Name, Name = "this.Name", Descr="Name", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_FirstName, Name = "this.FirstName", Descr="FirstName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_LastName, Name = "this.LastName", Descr="LastName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_Organization, Name = "this.Organization", Descr="Organization", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_Line1, Name = "this.Line1", Descr="Line1", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_Line2, Name = "this.Line2", Descr="Line2", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_City, Name = "this.City", Descr="City", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_State, Name = "this.State", Descr="State", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_CountryCode, Name = "this.CountryCode", Descr="CountryCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_CountryName, Name = "this.CountryName", Descr="CountryName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_PostalCode, Name = "this.PostalCode", Descr="PostalCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_RegionCode, Name = "this.RegionCode", Descr="RegionCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_RegionName, Name = "this.RegionName", Descr="RegionName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_DaytimePhoneNumber, Name = "this.DaytimePhoneNumber", Descr="DaytimePhoneNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_EveningPhoneNumber, Name = "this.EveningPhoneNumber", Descr="EveningPhoneNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_FaxNumber, Name = "this.FaxNumber", Descr="FaxNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.AccountAddress_Email, Name = "this.Email", Descr="Email", Conditions = ConditionTextDefs }
			};
			RegisterDefinitions(AddressDefs);

			AccountDefs =
				new ElementDefs[] { 
									new ElementDefs() { Key = ElementDefs.Account_Addresses, Name = "this.CustomerAccount.Addresses.ToArray()", Descr = "Addresses", Methods = AddressMethodDefs, Childrens = AddressDefs },

									new ElementDefs() { Key = ElementDefs.Account_CustomerGroup, Name = "this.CustomerAccount.CustomerGroup", Descr = "Customer Group" , Conditions = ConditionTextDefs},
									new ElementDefs() { Key = ElementDefs.Account_Name, Name = "this.CustomerAccount.Name", Descr = "Name" , Conditions = ConditionTextDefs},
									new ElementDefs() { Key = ElementDefs.Account_Description, Name = "this.CustomerAccount.Description", Descr = "Description" , Conditions = ConditionTextDefs},
									new ElementDefs() { Key = ElementDefs.Account_Type, Name = "this.CustomerAccount.Type", Descr = "Type" , Conditions = ConditionTextDefs},
									new ElementDefs() { Key = ElementDefs.Account_State, Name = "this.CustomerAccount.State", Descr = "State" , Conditions = ConditionIntegerDefs}
				};
			RegisterDefinitions(AccountDefs);
			#endregion

		}

		#endregion

		#region Overrides FilterExpressionProvider methods

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		protected override void CreateFilterNodes()
		{
			//first call base imlementation
			base.CreateFilterNodes();

			#region create elements nodes
			//Address
			foreach (ElementDefs elementDef in AddressDefs)
			{
				CreateFilterExpressionNode(elementDef);
			}
			//Account
			foreach (ElementDefs elementDef in AccountDefs)
			{
				CreateFilterExpressionNode(elementDef);
			}
			#endregion

			#region create method elements
			#endregion

			#region create condition elements
			#endregion


		}

		/// <summary>
		/// Loads the filters.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="expressionKey">The expression key.</param>
		/// <returns></returns>
		public override FilterExpressionNodeCollection LoadFilters(string expressionPlace, string expressionKey)
		{
			return (FilterExpressionNodeCollection)DataSource;
		}

		/// <summary>
		/// Saves the filters.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="expressionKey">The expression key, should be passed in the format of segmentid:expressionid</param>
		/// <param name="filters">The filters.</param>
		public override void SaveFilters(string expressionPlace, string expressionKey, FilterExpressionNodeCollection filters)
		{
			base.SaveFilters(expressionPlace, expressionKey, filters);
		}

		/// <summary>
		/// Gets the new elements.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <returns></returns>
		/// <summary>
		/// Gets the new elements.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <returns></returns>
		public override FilterExpressionNodeCollection GetNewElements(string expressionPlace, FilterExpressionNode parent)
		{
			List<FilterExpressionNode> retVal = new List<FilterExpressionNode>();
			FilterExpressionNode parentMethodNode = null;
			FilterExpressionNode parentNext = parent;

			string[] expressionPlaceParts = expressionPlace.Split(':');
			string realExpressionPlace = expressionPlaceParts.Length > 1 ? expressionPlaceParts[1] : string.Empty;
			
			//detected presents parent block type MethodBlock	
			while (parentNext != null && parentMethodNode == null)
			{
				if (parentNext.NodeType == FilterExpressionNodeType.MethodBlock)
				{
					parentMethodNode = parentNext;
				}
				parentNext = parentNext.ParentNode;
			}
			//Get Account elements by default
			IEnumerable<ElementDefs> returnedElementDefs = AccountDefs;
			 //Get child elements if parent node is method block
			if(parentMethodNode != null)
			{
				ElementDefs collectionElementDef = FindElementDef(parentMethodNode.Key);
				if (collectionElementDef == null)
					throw new NullReferenceException("element  " + parentMethodNode.Key + " not registered");

				returnedElementDefs = collectionElementDef.Childrens;
				//Is call from custom method control
				//Do not return any elements for method control binding
				if(realExpressionPlace == "Sum")
				{
					returnedElementDefs = null;
				}
			}
			if (returnedElementDefs != null)
			{
				retVal.AddRange(returnedElementDefs.Select(x => (x.Tag as FilterExpressionNode).Clone(true)));
			}

			// Sort retVal By Name
			retVal.Sort(delegate(FilterExpressionNode x, FilterExpressionNode y)
			{ return x.Name.CompareTo(y.Name); });

			return new FilterExpressionNodeCollection(retVal.ToArray());
		}


		/// <summary>
		/// Gets the element methods.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public override MethodElementCollection GetElementMethods(string expressionPlace, FilterExpressionNode node)
		{
			MethodElementCollection retVal = new MethodElementCollection();
			foreach (ElementDefs methodDef in AddressMethodDefs)
			{
				retVal.Add(methodDef.Tag as MethodElement);
			}
			return retVal;
		}

		/// <summary>
		/// Gets the element conditions.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public override ConditionElementCollection GetElementConditions(string expressionPlace, FilterExpressionNode node)
		{
			return base.GetElementConditions(expressionPlace, node);
		}

		#endregion
	}
}
