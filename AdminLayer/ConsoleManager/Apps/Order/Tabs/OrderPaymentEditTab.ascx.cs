using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class OrderPaymentEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";
		private const string _PostBackArgumentString = "paymentChanged";

        OrderGroup _order = null;
		Payment _selectedPayment = null;

        int _PaymentId = 0;
        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        /// <value>The payment id.</value>
        public int PaymentId
        {
            get
            {
                if (_PaymentId == 0)
                {
                    if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
                    {
                        _PaymentId = Int32.Parse(this.DialogTrigger.Value);
                    }
                }

                return _PaymentId;
            }
            set
            {
                _PaymentId = value;
            }
        }

		/// <summary>
		/// Gets or sets the payment id.
		/// </summary>
		/// <value>The payment id.</value>
		private Payment SelectedPayment
		{
			get
			{
				if (_selectedPayment == null)
				{
					if (_order != null && PaymentId != 0)
					{
						PaymentCollection payments = _order.OrderForms[0].Payments;

						foreach (Payment payment in payments)
						{
							if (payment.PaymentId == PaymentId)
							{
								_selectedPayment = payment;
								break;
							}
						}
					}
				}

				return _selectedPayment;
			}
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			PaymentType.SelectedIndexChanged += new EventHandler(PaymentType_SelectedIndexChanged);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

            base.OnInit(e);

            MetaDataTab.MDContext = OrderContext.MetaDataContext;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			//string postback = Page.ClientScript.GetPostBackEventReference(SaveChangesButton, _PostBackArgumentString);

			//string scriptString = "function OrderPaymentSaveChangesButton_onClientClick(btn) {" +
			//    "if (Page_ClientValidate('" + SaveChangesButton.ValidationGroup + "')) {\r\n" +
			//    "this.disabled = true;\r\n" +
			//    postback + ";\r\n" +
			//    "}\r\n" +
			//    "}";

			//Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PaymentTabSave", scriptString, true);

			PaymentType.Attributes["onchange"] = 
				String.Format("javascript:OrderPayment_UpdateSelectedField($get('{0}'), $get('{1}'));",
				PaymentType.ClientID, SelectedPaymentTypeField.ClientID);

			PaymentMethodList.Attributes["onchange"] =
				String.Format("javascript:OrderPayment_UpdateSelectedField($get('{0}'), $get('{1}'));",
				PaymentMethodList.ClientID, SelectedPaymentMethodField.ClientID);

			PaymentStatus.Attributes["onchange"] =
				String.Format("javascript:OrderPayment_UpdateSelectedField($get('{0}'), $get('{1}'));",
				PaymentStatus.ClientID, SelectedPaymentStatusField.ClientID);

			PaymentType.Enabled = SelectedPayment == null;

			if (String.Compare(Request.Form["__EVENTTARGET"], PaymentType.UniqueID, StringComparison.Ordinal) != 0)
			{
				bool reset = false;
				if (Request.Form["__EVENTTARGET"] == DialogTrigger.UniqueID)
					reset = true;
				BindForm(reset);
			}

			//BindMetaForm();

            DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
        }

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (String.Compare(Request.Form["__EVENTTARGET"], SaveChangesButton.UniqueID, StringComparison.Ordinal) == 0)
				SaveChangesButton_Click(SaveChangesButton, null);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void PaymentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindMetaForm();
			DialogContentPanel.Update();
		}

        /// <summary>
        /// Handles the ValueChanged event of the DialogTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DialogTrigger_ValueChanged(object sender, EventArgs e)
        {
            BindForm(true);
        }

        /// <summary>
        /// Handles the Click event of the SaveChangesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveChangesButton_Click(object sender, EventArgs e)
        {
			if (String.Compare(Request.Form["__EVENTARGUMENT"], _PostBackArgumentString) == 0)
			{
				Payment payment = null;

				if (PaymentId != 0) // find existing
				{
					foreach (Payment p in _order.OrderForms[0].Payments)
					{
						if (p.PaymentId == PaymentId)
						{
							payment = p;
							break;
						}
					}
				}

				// Create payment of selected type
				if (payment == null)
				{
					// determine which payment type to create
					// temporary solution, since the algorythm below is rather slow
					MetaClass mc = MetaClass.Load(OrderContext.MetaDataContext, Int32.Parse(PaymentType.SelectedValue));
					if (mc != null)
					{
						Type[] paymentTypes = ReflectionHelper.GetInheritedClasses(typeof(Payment));
						Type paymentType = null;
						if (paymentTypes != null)
							foreach (Type pt in paymentTypes)
							{
								ClassInfo classInfo = new ClassInfo(pt);
								Payment tmpPayment = (Payment)classInfo.CreateInstance();
								if (tmpPayment.MetaClass.Id == mc.Id)
								{
									paymentType = pt;
									break;
								}
							}

						if (paymentType != null)
							payment = _order.OrderForms[0].Payments.AddNew(paymentType);
						//payment = _order.OrderForms[0].Payments.AddNew(typeof(CreditCardPayment));
					}
				}

				if (payment != null)
				{
					payment.Amount = Decimal.Parse(Amount.Text);
					ListItem selectedPaymentStatus = PaymentStatus.Items.FindByValue(SelectedPaymentStatusField.Value);
					if (selectedPaymentStatus != null)
						payment.Status = selectedPaymentStatus.Value;

					payment.PaymentMethodId = new Guid(PaymentMethodList.SelectedValue);
					payment.PaymentMethodName = Name.Text;
					ListItem selectedPaymentMethod = PaymentMethodList.Items.FindByValue(SelectedPaymentMethodField.Value);
					if (selectedPaymentMethod != null)
						payment.PaymentMethodId = new Guid(selectedPaymentMethod.Value);

					//ListItem selectedPaymentType = PaymentType.Items.FindByValue(SelectedPaymentTypeField.Value);
					//if (selectedPaymentType != null)
					//    payment.MetaClass = new Guid(selectedPaymentMethod.Value);
				}

				// Put a dictionary key that can be used by other tabs
				MetaDataTab.ObjectId = payment.PaymentId;
				IDictionary dic = new ListDictionary();
				dic.Add(MetaDataTab._MetaObjectContextKey + MetaDataTab.Languages[0], payment);
				MetaDataTab.SaveChanges(dic);
			}

            ScriptManager.RegisterStartupScript(MetaDataTab, typeof(OrderPaymentEditTab), "DialogClose", "Payment_CloseDialog();", true);
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        private void BindForm(bool reset)
        {
			if (String.IsNullOrEmpty(SelectedPaymentStatusField.Value))
				SelectedPaymentStatusField.Value = PaymentStatus.SelectedValue;

            // Bind Meta classes
			if (!IsPostBack || reset)
			{
				MetaClass paymentMetaClass = MetaClass.Load(OrderContext.MetaDataContext, "OrderFormPayment");
				PaymentType.Items.Clear();
				if (paymentMetaClass != null)
				{
					MetaClassCollection metaClasses = paymentMetaClass.ChildClasses;
					foreach (MetaClass metaClass in metaClasses)
					{
						PaymentType.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Id.ToString()));
					}
					//PaymentType.DataBind();
					PaymentType.Items[0].Selected = true;
				}
			}

            PaymentMethodList.DataSource = PaymentManager.GetPaymentMethods(CommonSettingsManager.GetDefaultLanguage());
            PaymentMethodList.DataBind();

            if (SelectedPayment != null)
            {
                if (reset)
                {
					Amount.Text = SelectedPayment.Amount.ToString("#0.00");
                    Name.Text = SelectedPayment.PaymentMethodName;
					//SelectedPaymentStatusField.Value = selectedPayment.Status;
                }

				ManagementHelper.SelectListItem(PaymentStatus, SelectedPayment.Status);
                ManagementHelper.SelectListItem(PaymentType, SelectedPayment.MetaClass.Id);
                ManagementHelper.SelectListItem(PaymentMethodList, SelectedPayment.PaymentMethodId);               

                MetaDataTab.ObjectId = SelectedPayment.PaymentId;
            }
            else if(reset)
            {
				Amount.Text = 0.ToString("#0.00");
                Name.Text = "";
				//SelectedPaymentStatusField.Value = PaymentStatus.SelectedValue;
            }

			if (String.IsNullOrEmpty(SelectedPaymentTypeField.Value))
				SelectedPaymentTypeField.Value = PaymentType.SelectedValue;
			if (String.IsNullOrEmpty(SelectedPaymentMethodField.Value))
				SelectedPaymentMethodField.Value = PaymentMethodList.SelectedValue;

            // Bind Meta Form
			BindMetaForm();
        }

		private void BindMetaForm()
		{
			// Bind Meta Form
			MetaDataTab.MetaClassId = Int32.Parse(PaymentType.SelectedValue);

			Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();
			if (SelectedPayment != null && SelectedPayment.MetaClass.Id.ToString() == PaymentType.SelectedValue)
				metaObjects.Add(MetaDataTab.Languages[0], SelectedPayment);
			else
				metaObjects.Add(MetaDataTab.Languages[0], null);

			IDictionary dic = new ListDictionary();
			dic.Add("MetaObjectsContext", metaObjects);

			MetaDataTab.LoadContext(dic);

			MetaDataTab.DataBind();
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            //OrderGroup order = (OrderGroup)context[_OrderContextObjectString];
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _order = (OrderGroup)context[_OrderContextObjectString];
        }
        #endregion
    }
}