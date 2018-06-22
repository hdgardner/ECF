using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
	/// <summary>
	/// Represents definition mapping methods and defines UID for all posible filter elements
	/// </summary>
	public class ElementDefs : ICloneable
	{
		public string Key;
		public string Name;
		public string Descr;
		public string ConverstionPattern;
		public object Tag;

		public IEnumerable<ElementDefs> Childrens;
		public IEnumerable<ElementDefs> Methods;
		public IEnumerable<ElementDefs> Conditions;

		#region Const definitions
		#region Elements in conditions expression
		#region ShoppingCart
		public const string ShoppingCart_BillingCurrency = "{F672B612-565A-475b-B3AB-60CFE0B2A420}";
		public const string ShoppingCart_CustomerName = "{A999B5F3-0112-4cbc-94D8-1336DF4D36A4}";
		public const string ShoppingCart_HandlingTotal = "{46FD38FB-B2C1-427a-9A13-1B7D23313665}";
		public const string ShoppingCart_Name = "{18FD67FE-ADA2-4617-9FCD-FF713692FE6F}";
		public const string ShoppingCart_ShippingTotal = "{6830C974-8394-4c04-B2FB-1F3B3AF92B80}";
		public const string ShoppingCart_Status = "{C7A3567B-ABE3-4950-B8E3-80CE27824B38}";
		public const string ShoppingCart_SubTotal = "{9DBB59D1-4D12-47e2-94C0-C72DFA88EAB7}";
		public const string ShoppingCart_TaxTotal = "{8CC60D88-8A07-4602-ADC8-5882FACCF51F}";
		public const string ShoppingCart_Total = "{3578F3F5-D93B-4837-9775-1C313ECCB860}";
		#endregion
		#region Shipment
		public const string Shipment_LienItems = "{33C983FD-D64A-4c08-94E8-A268891E6E4D}";
		public const string Shipment_ShippingMethodName = "{43FC04E2-E4BB-42df-96AC-EEBA4C34531C}";
		#endregion
		#region LineItem
		public const string LineItem_Address = "{E97F317F-481E-469a-8451-706968E0E062}";
		public const string LineItem_Quantity = "{E04F2245-F318-453e-95BA-727D2F57A7DF}";
		public const string LineItem_ListPrice = "{285CB8EC-9182-48ce-8F4F-BA874E129FDD}";
		public const string LineItem_Catalog = "{C2F6734B-ACEF-46e6-B953-59C4546463BF}";
		public const string LineItem_CatalogNode = "{4B36832A-99BD-451d-A188-B2490FE44E0C}";
		public const string LineItem_CatalogEntryId = "{04302538-B7B2-4782-8028-34E9F3187B7F}";
		public const string LineItem_MinQuantity = "{756DBA58-F494-4735-983A-026E456740C3}";
		public const string LineItem_MaxQuantity = "{13592E77-E524-41f8-9FA2-197ECC95C284}";
		public const string LineItem_PlacedPrice = "{9F9BA4B1-0DE2-4ebf-85CE-E73B6DD72E52}";
		public const string LineItem_LineItemDiscountAmount = "{82F792C5-075B-491f-913E-9B5852F8EB6A}";
		public const string LineItem_OrderLevelDiscountAmount = "{1B9C4DAE-1B97-4b04-AA70-158B2E73844A}";
		public const string LineItem_ShippingMethodName = "{F722A9A8-C144-4a41-A95A-177754D68033}";
		public const string LineItem_ExtendedPrice = "{C70A68B3-BD8B-4f9f-A2B8-65774722FCD9}";
		public const string LineItem_Description = "{DFD082A1-60C4-4539-AA53-EAB6112AEF25}";
		public const string LineItem_Status = "{865A86B0-260C-41fe-B9A1-3EDA6576FE25}";
		public const string LineItem_DisplayName = "{D856D3AE-5445-41f1-9C03-73AE593DC415}";
		public const string LineItem_AllowBackordersAndPreorders = "{EFD54938-78D0-4eba-BECA-1DBF407A1060}";
		public const string LineItem_InStockQuantity = "{8808A541-ECCC-4a4d-8F5D-E55F7EFCB892}";
		public const string LineItem_PreorderQuantity = "{61C332F4-5660-410e-8F09-5FDC907C91DD}";
		public const string LineItem_BackorderQuantity = "{7081F3FA-95B1-49d3-A7EE-B6A4CCA56432}";
		public const string LineItem_InventoryStatus = "{3AC7AEB0-BB1D-4d43-870B-16259E53D085}";
		#endregion

		#region OrderForm
		public const string OrderForm_LineItems = "{54A1A22D-20C7-4c3f-8A83-9683A2A2152B}";
		public const string OrderForm_BillingAddress = "{05B5822A-1544-4c4f-B50E-A92964D39CF3}";
		public const string OrderForm_Shipments = "{731C9FE7-4D00-4a31-8A56-EF9BFE628B94}";
		public const string OrderForm_DiscountAmount = "{AE45C849-071F-4b94-AAA4-B844DA6334ED}";
		public const string OrderForm_Name = "{CF531C6A-D145-49f6-80F0-907EFCEA8434}";
		public const string OrderForm_HandlingTotal = "{BD5ADC8D-574A-4c0b-A861-5B16926E4806}";
		public const string OrderForm_ShippingTotal = "{5298617D-1643-4e02-84AE-154836D4B79C}";
		public const string OrderForm_Status = "{54C47AD2-D2EE-44f0-9587-FF75100731C0}";
		public const string OrderForm_SubTotal = "{25F8F03A-6CD1-4727-9852-9C82796D25B1}";
		public const string OrderForm_TaxTotal = "{F168576A-A225-4a2c-9E9E-0F099F46A941}";
		public const string OrderForm_Total = "{BBB089FE-6EB8-4adc-8310-78FD0EF51E25}";
		#endregion

		#region OrderAddress
		public const string OrderAddress_Name = "{9EF702DD-BD64-46ac-9E82-177560B0F891}";
		public const string OrderAddress_FirstName = "{AF638243-BFCB-412a-A94E-8F55FA8EE6B5}";
		public const string OrderAddress_LastName = "{B60E139E-5F3D-4417-A4FD-A6AB9529C57A}";
		public const string OrderAddress_Organization = "{A81F83C1-461D-4388-B6B3-91E7CB129D0D}";
		public const string OrderAddress_Line1 = "{5AB2D2CE-7583-415c-B15A-D1A43FF103F3}";
		public const string OrderAddress_Line2 = "{83FCB037-CB28-43e5-BE0F-1812AE35918F}";
		public const string OrderAddress_City = "{A46CAA53-97EB-4de5-B7CC-831704B45B85}";
		public const string OrderAddress_State = "{03F0E6C2-2F41-481b-958C-43CECCF3A12A}";
		public const string OrderAddress_CountryCode = "{E88CE588-0672-4986-90A7-97D5744047D3}";
		public const string OrderAddress_CountryName = "{47CD7C1B-3D62-4140-BD30-8EE3F671714D}";
		public const string OrderAddress_PostalCode = "{C3759D20-4FAB-42ce-86F0-AE5B45366C40}";
		public const string OrderAddress_RegionCode = "{08D481A8-9CD2-4e54-A2AF-2479CB00EA24}";
		public const string OrderAddress_RegionName = "{C8E2467C-32A9-4d84-A3BF-5CEBFECBE5D0}";
		public const string OrderAddress_DaytimePhoneNumber = "{F15CB78A-DC6E-47f5-81C9-F177517FE336}";
		public const string OrderAddress_EveningPhoneNumber = "{DE0437D0-000E-4eae-BCC8-72718CAD3462}";
		public const string OrderAddress_FaxNumber = "{B791E941-DD77-48fa-B2DF-86D57720B02B}";
		public const string OrderAddress_Email = "{0E72EC8E-696D-426e-BCF8-A5731B8EE2D1}";
		#endregion

		#region AccountAddress
		public const string AccountAddress_Name = "{69CC3D53-F979-4f88-82DC-5F1FB65AC04E}";
		public const string AccountAddress_FirstName = "{6A89E393-35F8-4d9a-B3F2-AC57C9BE4DC4}";
		public const string AccountAddress_LastName = "{F2B9E1CC-B63F-482b-B5BB-8BED645D9CDB}";
		public const string AccountAddress_Organization = "{CE7BB73E-9AA2-4757-82CE-A3C4CF4D6355}";
		public const string AccountAddress_Line1 = "{0C33C729-0C25-40ed-87FC-5666F602D535}";
		public const string AccountAddress_Line2 = "{606A014E-1455-41fd-BFEB-50DB652FE40D}";
		public const string AccountAddress_City = "{1806F271-53E3-4a1d-AF77-4F96E245DF47}";
		public const string AccountAddress_State = "{C9695E76-61A3-492e-9568-CB2D0B20982F}";
		public const string AccountAddress_CountryCode = "{320C3093-89E4-4985-A693-1D43286200DB}";
		public const string AccountAddress_CountryName = "{EE026577-F670-4497-AB5E-913E95377F1A}";
		public const string AccountAddress_PostalCode = "{F38298EE-6081-42be-BEF6-3240555BC58D}";
		public const string AccountAddress_RegionCode = "{F5702C28-270A-409a-A842-65268262F2B9}";
		public const string AccountAddress_RegionName = "{FCF3BEDD-E970-484e-86F6-0B085C5B5DF9}";
		public const string AccountAddress_DaytimePhoneNumber = "{3F7EC229-806D-4c16-B57A-907A6526E6B2}";
		public const string AccountAddress_EveningPhoneNumber = "{FD52F695-B7E3-4243-A88F-2CDCA22635B6}";
		public const string AccountAddress_FaxNumber = "{03397879-0A4D-4676-A12C-E9AE2ABE9E06}";
		public const string AccountAddress_Email = "{2B38666E-ED9E-479c-8142-5E9461BC8F00}";
		#endregion
		#region TargetLineItem
		public const string TargetLineItem_Quantity = "{476E1C18-71CD-4886-8E60-FD42F347A26F}";
		public const string TargetLineItem_ListPrice = "{0344855D-1146-4a54-87FF-E40529B9D2B5}";
		public const string TargetLineItem_Catalog = "{10CECCE6-F724-4d0b-9869-C63E958EB504}";
		public const string TargetLineItem_CatalogNode = "{7771D777-E327-4200-878D-AE84FD09E6BC}";
		public const string TargetLineItem_CatalogEntryId = "{F3BE2DF8-7581-4bd5-841D-A7660DD52D8B}";
		public const string TargetLineItem_MinQuantity = "{DAD56279-D4EC-497a-BA8D-53ADD1C9084D}";
		public const string TargetLineItem_MaxQuantity = "{962FFDD7-A5EB-4241-A27D-8BD6EB555AFB}";
		public const string TargetLineItem_PlacedPrice = "{22F65F54-5228-45cd-A4C3-54868B745942}";
		public const string TargetLineItem_LineItemDiscountAmount = "{1C2B0E16-1A4B-472e-A261-A39F0F1D47C4}";
		public const string TargetLineItem_OrderLevelDiscountAmount = "{A07B64D4-0AAF-4062-A50A-7795A8DDB517}";
		public const string TargetLineItem_ShippingMethodName = "{A121F040-FEDD-4e64-90C5-BAF35A28CB8A}";
		public const string TargetLineItem_ExtendedPrice = "{11E4C83A-B8A7-4fc4-AB55-408E8DAC8C55}";
		public const string TargetLineItem_Description = "{939A898E-1D3F-4841-9EA9-34F154E38FD0}";
		public const string TargetLineItem_Status = "{85579DA2-9CD1-45ef-BA91-1277464DD1BF}";
		public const string TargetLineItem_DisplayName = "{497EB587-E338-47b3-9CC7-14E021B0C1E8}";
		public const string TargetLineItem_AllowBackordersAndPreorders = "{BD353F1F-7B56-43f2-8F14-9AED8C693D00}";
		public const string TargetLineItem_InStockQuantity = "{E10C9F8C-DC3E-4e6c-8ABC-012EC2F71E88}";
		public const string TargetLineItem_PreorderQuantity = "{8DE051C0-F76F-4ddd-AF71-92D50638C5FB}";
		public const string TargetLineItem_BackorderQuantity = "{4B164501-DA98-4086-BA13-7F9E8EE45591}";
		public const string TargetLineItem_InventoryStatus = "{9DE9DA63-6870-412a-8C87-0252E89F8E3B}";
		#endregion

		#region RunningTotal
		public const string RunningTotal = "{0588ACF1-6AAA-40fd-B8D0-C8288316F800}";
		#endregion

		#region Account
		public const string Account_Addresses = "{81492B1B-C7D2-4256-8AFE-988190EF4A00}";
		public const string Account_CustomerGroup = "{B6294440-8111-4aec-AA88-3B2E020D28EA}";
		public const string Account_Name = "{38235511-E074-466c-AC38-3B31AF216D6D}";
		public const string Account_Description = "{D68540D0-10B7-4365-8989-291F4E324502}";
		public const string Account_Type = "{6824941F-CEEE-490a-B580-61EF8E813310}";
		public const string Account_State = "{F1952F8E-F8BD-49a8-A97F-C229F3A1BC96}";
		
		
			
		#endregion

		#endregion
		#region Elements in action expression
		public const string Action_AllEntryValueOfItem = "{DF6E7414-43B3-44a5-AA98-334E72990E7E}";
		public const string Action_AllEntryPercentOfItem = "{71CF7E32-5F1B-4b71-BC09-73E74B332879}";
		public const string Action_WholeOrderValue = "{ABAA64C4-23A3-470a-8048-8BB0608957A5}";
		public const string Action_WholeOrderPercent = "{D3FB7EDE-831E-4a20-BD54-604A366BFB0F}";
		public const string Action_AllEntryValueOfSkuSet = "{B893D848-5046-4a1d-BEE4-9562B95265BA}";
		public const string Action_AllEntryPercentOfSkuSet = "{1DBD33D7-AD85-447a-8AE6-C4CBEC3263D9}";
		public const string Action_EachEntryValueOfSkuSet = "{991A1536-DADF-4727-88D2-49406481ED33}";
		public const string Action_GiftOfSkuSet = "{9C5A4250-EFB1-46c4-9B47-160F661F7B16}";
		#endregion
		#region Methods
		public const string Method_Sum = "{DAE95DCE-83F0-4637-8A90-B40D306ED7D9}";
		public const string Method_Count = "{EAC141DA-C805-47f5-9805-D55E92420739}";
		public const string Method_All = "{AB991074-1219-4385-AB12-7166CF2C778D}";
		public const string Method_Any = "{71920FC4-9A62-4647-89CD-795DBFBC22EC}";
		public const string Method_Action_EachEntry = "{E6FDF8C2-66AD-454a-AE91-C5E9B6605F4D}";
		public const string Method_Action_AllEntry = "{C69971A0-1774-4977-97D4-F4F3FA2ACE6B}";
		public const string Method_Action_AllEntryPercent = "{DDD54FA3-2C59-4852-A26A-6772F7EB8CD6}";
		public const string Method_Action_Gift = "{F48E904C-7B71-4bd4-9172-040D5B50E3FB}";
		#endregion

		#region Conditions
		#region ActionConditions
		public const string Action_Condition_SkuSelect = "{30FA45AF-94C3-43d5-8D5D-3AE5AC379B67}";
		public const string Action_Condition_FreeShiping = "{AC3DFD0A-FC1D-46e7-903C-0D142DC53676}";
		public const string Action_Condition_PercentDecimal = "{A9BD3F76-DFBE-4f3a-9013-08420B5E3D98}";
		public const string Action_Condition_Decimal = "{B0FC3DAA-DF1F-4bf7-BB30-A735EAAB9664}";
		#endregion

		#region TextConditions
		public const string Condition_Text_Equals = "{A831AA20-52D7-4ab7-8F58-6D37552A3B71}";
		public const string Condition_Text_NotEquals = "{A14496DE-BE97-4829-8A23-6D70B269E146}";
		public const string Condition_Text_Contains = "{472F4687-9033-42b6-9C06-FF3B19A90030}";
		public const string Condition_Text_NotContains = "{EFDFA591-A561-4b3c-9721-B8204371F862}";
		public const string Condition_Text_StartsWith = "{52520E6B-52BA-4e05-9219-FCD25EE12F3B}";
		public const string Condition_Text_EndsWith = "{34A5EBC4-74CF-4530-9DDA-4C3EF2BC0ECA}";
		#endregion
		#region DecimalConditions
		public const string Condition_Decimal_Equals = "{388BFBA5-56F9-47d0-8E7B-4486DBFE117A}";
		public const string Condition_Decimal_NotEquals = "{DFADE161-0060-4bf4-AAE4-8D95E2E6B534}";
		public const string Condition_Decimal_Greater = "{E8FC3BC9-DADF-4de8-AE0D-17E19D903690}";
		public const string Condition_Decimal_Less = "{F399F5A3-1EB8-45ad-80E2-477882C31DD9}";
		#endregion
		#region IntConditions
		public const string Condition_Int_Equals = "{FE807636-9B54-49a6-9FCE-C727012BC985}";
		public const string Condition_Int_NotEquals = "{BD6E2A58-AC4B-4a9e-A362-76BF04496624}";
		public const string Condition_Int_Greater = "{F3271D3B-2322-4054-948D-D00B687D95D5}";
		public const string Condition_Int_Less = "{9FBE1490-DED8-4f5a-9EB5-BCDD1F90AB4B}";
		#endregion
		#region BoolConditions
		public const string Condition_Bool_Equals = "{2BEEC453-C1C4-4425-AB69-4AE12F42CDA7}";
		#endregion
		#endregion
		#endregion
		#region Dynamic definitions
		/// <summary>
		/// Gets the dynamic key.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <returns></returns>
		public static string GetDynamicKey(string pattern)
		{
			return pattern;
		}
		#endregion

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
		public bool HasChildren
		{
			get
			{
				return Childrens != null && Childrens.Count() != 0;
			}
		}

		/// <summary>
		/// Gets the name of the condition def by.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public ElementDefs GetConditionDefByName(string key)
		{
			if (Conditions == null)
				throw new NullReferenceException("Conditions");

			return Conditions.First(x => x.Key == key);

		}
		/// <summary>
		/// Gets the method def.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public ElementDefs GetMethodDef(string key)
		{
			if (Methods == null)
				throw new NullReferenceException("Methods");

			return Methods.First(x => x.Key == key);
		}

		#region ICloneable Members

		public object Clone()
		{
			ElementDefs retVal = new ElementDefs();
			retVal.Key = this.Key;
			retVal.Name = this.Name;
			retVal.Descr = this.Descr;

			if (this.Conditions != null)
			{
				retVal.Conditions = this.Conditions.Select(x=>x.Clone() as ElementDefs);
			}
			if (this.Methods != null)
			{
				retVal.Methods = this.Methods.Select(x=>x.Clone() as ElementDefs);
			}
			if (this.Childrens != null)
			{
				retVal.Childrens = this.Methods.Select(x=>x.Clone() as ElementDefs);
			}
			if (this.Tag != null)
			{
				ICloneable clonedObjTag = this.Tag as ICloneable;
				if (clonedObjTag != null)
				{
					retVal.Tag = clonedObjTag.Clone();
				}
			}

			return retVal;
		}

		#endregion
	
	}

}
