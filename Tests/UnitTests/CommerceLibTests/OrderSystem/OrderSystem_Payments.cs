using System;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Exceptions;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Plugins.Payment.Authorize;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus;
using MetaDataTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.OrderSystem
{
    /// <summary>
    /// Summary description for OrderSystem_Perf
    /// </summary>
    [TestClass]
    public class OrderSystem_Payments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSystem_Payments"/> class.
        /// </summary>
        public OrderSystem_Payments()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        /// <summary>
        /// Initialize performance.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]
        public static void PerformanceInitialize(TestContext testContext) 
        {
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void InitializeUser() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Payment with encrypted credit card.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_EncryptCreditCard()
        {
            string encrypted = EncryptionManager.Encrypt("hello");
            Console.Write(encrypted);
        }

        /// <summary>
        /// Credit card payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_CreditCardSucceed()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateCreditCardPayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            // Following line throws exception:
            // "Authorize.NET payment gateway is not configured correctly. User is not set.."
            cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }

        /// <summary>
        /// Cash card payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_CashCardSucceed()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateCashCardPayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }

        /// <summary>
        /// Tests the creation of a cart, a gift card payment, a PO, and subsequent deletion of PO and cart.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_GiftCardSucceed()
        {
            // Creating cart fails
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateGiftCardPayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Validate purchase order
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
            // Delete PO
            po.Delete();
            po.AcceptChanges();
            // Assert that PO is deleted
            Assert.IsTrue(po.ObjectState == MetaObjectState.Deleted);
            // Delete cart
            cart.Delete();
            cart.AcceptChanges();
            // Assert that cart is deleted
            Assert.IsTrue(cart.ObjectState == MetaObjectState.Deleted);
        }

        /// <summary>
        /// Invoice payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_InvoiceSucceed()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateInvoicePayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }

        /// <summary>
        /// Other payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_OtherSucceed()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateOtherPayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }

		#region Authorize.NET Recurring Payments testing
		/// <summary>
		/// Test all of the payment types that inherit from the Mediachase.Commerce.Orders.
		/// Payment class. Each of the test involves creating a cart, adding an order form, 
		/// adding an item, adding the particular payment type, saving the cart and then
		/// retrieving the cart and verifying that the payment type saved properly.
		/// </summary>
		[TestMethod]
        public void OrderSystem_Payments_AuthorizeNET_CreateNew()
		{
			CreditCardPayment payment;
			string cardType = "Visa";
			int expMonth = 4;
			int expYear = DateTime.UtcNow.AddYears(5).Year;

			// Create cart
			Cart cart = OrderHelper.CreateCartForPaymentPlan();

			// Create payment plan
			PaymentPlan plan = cart.SaveAsPaymentPlan();

			// Set some payment plan values
			plan.CycleMode = PaymentPlanCycle.Months;
			plan.CycleLength = 1;
			plan.MaxCyclesCount = 12;
			plan.StartDate = DateTime.UtcNow;
			plan.EndDate = DateTime.UtcNow.AddMonths(13);
			plan.LastTransactionDate = DateTime.UtcNow;

			//create cc payment to validate test account
			payment = (CreditCardPayment)plan.OrderForms[0].Payments[0];
			payment.CardType = cardType;
			payment.CreditCardNumber = "4012888818888";
			payment.ExpirationMonth = expMonth;
			payment.ExpirationYear = expYear;

			// Execute workflow
			plan.RunWorkflow("CartCheckout");

			// Update last transaction date
			plan.LastTransactionDate = DateTime.UtcNow;
			// Encrease cycle count
			plan.CompletedCyclesCount++;

			// Save changes
			plan.AcceptChanges();

			plan = OrderContext.Current.GetPaymentPlan(plan.CustomerId, plan.OrderGroupId);

			// Validate payment plan
			Assert.IsTrue(!String.IsNullOrEmpty(plan.OrderForms[0].Payments[0].AuthorizationCode));
			// Delete payment plan
			plan.Delete();
			plan.AcceptChanges();
			// Assert that payment plan is deleted
			Assert.IsTrue(plan.ObjectState == MetaObjectState.Deleted);

			// Delete cart
			cart.Delete();
			cart.AcceptChanges();
			// Assert that cart is deleted
			Assert.IsTrue(cart.ObjectState == MetaObjectState.Deleted);
		}

		/// <summary>
		/// Test all of the payment types that inherit from the Mediachase.Commerce.Orders.
		/// Payment class. Each of the test involves creating a cart, adding an order form, 
		/// adding an item, adding the particular payment type, saving the cart and then
		/// retrieving the cart and verifying that the payment type saved properly.
		/// </summary>
		[TestMethod]
		public void OrderSystem_Payments_AuthorizeNET_CreateNewError()
		{
			CreditCardPayment payment;
			string cardType = "Visa";
			int expMonth = 4;
			int expYear = DateTime.UtcNow.AddYears(5).Year;

			// Create cart
			Cart cart = OrderHelper.CreateCartForPaymentPlan();

			// Create payment plan
			PaymentPlan plan = cart.SaveAsPaymentPlan();

			// Set some payment plan values
			// Monthly subscription for a year
			plan.CycleMode = PaymentPlanCycle.Months;
			plan.CycleLength = 1;
			plan.MaxCyclesCount = 12;
			plan.StartDate = DateTime.UtcNow.AddDays(-10);
			plan.EndDate = DateTime.UtcNow.AddMonths(13);

			//create cc payment to validate test account
			payment = (CreditCardPayment)plan.OrderForms[0].Payments[0];
			payment.CardType = cardType;
			payment.CreditCardNumber = "4012888818888";
			payment.ExpirationMonth = expMonth;
			payment.ExpirationYear = expYear;

			// Execute workflow
			try
			{
				plan.RunWorkflow("CartCheckout");
			}
			catch (PaymentException ex)
			{
				if (ex.Type != PaymentException.ErrorType.ProviderError)
					throw;
				else
					Console.Error.WriteLine(ex.Message);
			}

			// Update last transaction date
			plan.LastTransactionDate = DateTime.UtcNow;
			// Encrease cycle count
			plan.CompletedCyclesCount++;
			// Save changes
			plan.AcceptChanges();

			plan = OrderContext.Current.GetPaymentPlan(plan.CustomerId, plan.OrderGroupId);

			// Validate payment plan
			Assert.AreEqual(plan.CompletedCyclesCount, 1);
			// Delete payment plan
			plan.Delete();
			plan.AcceptChanges();
			// Assert that payment plan is deleted
			Assert.IsTrue(plan.ObjectState == MetaObjectState.Deleted);

			// Delete cart
			cart.Delete();
			cart.AcceptChanges();
			// Assert that cart is deleted
			Assert.IsTrue(cart.ObjectState == MetaObjectState.Deleted);
		}

		/// <summary>
		/// Test all of the payment types that inherit from the Mediachase.Commerce.Orders.
		/// Payment class. Each of the test involves creating a cart, adding an order form, 
		/// adding an item, adding the particular payment type, saving the cart and then
		/// retrieving the cart and verifying that the payment type saved properly.
		/// </summary>
		[TestMethod]
		public void OrderSystem_Payments_AuthorizeNET_CreateNewAndCancel()
		{
			CreditCardPayment payment;
			string cardType = "MasterCard";
			int expMonth = 4;
			int expYear = DateTime.UtcNow.AddYears(5).Year;

			// Create cart
			Cart cart = OrderHelper.CreateCartForPaymentPlan();

			// Create payment plan
			PaymentPlan plan = cart.SaveAsPaymentPlan();

			// ---------- Step 1. Create subscription -----------

			// Set some payment plan values
			plan.CycleMode = PaymentPlanCycle.Months;
			plan.CycleLength = 1;
			plan.MaxCyclesCount = 12;
			plan.StartDate = DateTime.UtcNow;
			plan.EndDate = DateTime.UtcNow.AddMonths(13);
			plan.LastTransactionDate = DateTime.UtcNow;

			//create cc payment to validate test account
			payment = (CreditCardPayment)plan.OrderForms[0].Payments[0];
			payment.CardType = cardType;
			payment.CreditCardNumber = "5424000000000015";
			payment.ExpirationMonth = expMonth;
			payment.ExpirationYear = expYear;

			// Execute workflow
			plan.RunWorkflow("CartCheckout");

			// Update last transaction date
			plan.LastTransactionDate = DateTime.UtcNow;
			// Encrease cycle count
			plan.CompletedCyclesCount++;

			// Save changes
			plan.AcceptChanges();

			plan = OrderContext.Current.GetPaymentPlan(plan.CustomerId, plan.OrderGroupId);

			// Validate payment plan
			Assert.IsTrue(!String.IsNullOrEmpty(plan.OrderForms[0].Payments[0].AuthorizationCode));

			// ---------- Step 2. Cancel subscription -----------
			// get cancel status
			PaymentMethodDto dto = PaymentManager.GetPaymentMethodBySystemName("authorize", "en-us", true);
			Assert.IsTrue(dto.PaymentMethod.Count > 0, "Authorize.NET payment method not found for en-us language.");

			PaymentMethodDto.PaymentMethodParameterRow[] rows = (PaymentMethodDto.PaymentMethodParameterRow[])dto.PaymentMethodParameter.Select(String.Format("Parameter = '{0}'", AuthorizePaymentGateway._CancelStatusParameterName));
			Assert.IsTrue(rows != null && rows.Length > 0, "CancelStatus parameter for the Authorize.NET gateway not net.");

			string cancelStatus = rows[0].Value;

			// set status cancel status for the payment plan
			plan.Status = cancelStatus;

			// execute workflow
			plan.RunWorkflow("CartCheckout");

			Assert.IsTrue(String.IsNullOrEmpty(plan.OrderForms[0].Payments[0].AuthorizationCode));

			// ---------- Step 3. Perform cleanup -----------

			// Delete payment plan
			plan.Delete();
			plan.AcceptChanges();
			// Assert that payment plan is deleted
			Assert.IsTrue(plan.ObjectState == MetaObjectState.Deleted);

			// Delete cart
			cart.Delete();
			cart.AcceptChanges();
			// Assert that cart is deleted
			Assert.IsTrue(cart.ObjectState == MetaObjectState.Deleted);
		}

		/// <summary>
		/// Test all of the payment types that inherit from the Mediachase.Commerce.Orders.
		/// Payment class. Each of the test involves creating a cart, adding an order form, 
		/// adding an item, adding the particular payment type, saving the cart and then
		/// retrieving the cart and verifying that the payment type saved properly.
		/// </summary>
		[TestMethod]
		public void OrderSystem_Payments_AuthorizeNET_CreateNewAndCancelError()
		{
			CreditCardPayment payment;
			string cardType = "MasterCard";
			int expMonth = 4;
			int expYear = DateTime.UtcNow.AddYears(5).Year;

			// Create cart
			Cart cart = OrderHelper.CreateCartForPaymentPlan();

			// Create payment plan
			PaymentPlan plan = cart.SaveAsPaymentPlan();

			// ---------- Step 1. Create subscription -----------

			// Set some payment plan values
			plan.CycleMode = PaymentPlanCycle.Months;
			plan.CycleLength = 1;
			plan.MaxCyclesCount = 12;
			plan.StartDate = DateTime.UtcNow;
			plan.EndDate = DateTime.UtcNow.AddMonths(13);
			plan.LastTransactionDate = DateTime.UtcNow;

			//create cc payment to validate test account
			payment = (CreditCardPayment)plan.OrderForms[0].Payments[0];
			payment.CardType = cardType;
			payment.CreditCardNumber = "5424000000000015";
			payment.ExpirationMonth = expMonth;
			payment.ExpirationYear = expYear;

			// Execute workflow
			plan.RunWorkflow("CartCheckout");

			// Update last transaction date
			plan.LastTransactionDate = DateTime.UtcNow;
			// Encrease cycle count
			plan.CompletedCyclesCount++;

			// Save changes
			plan.AcceptChanges();

			plan = OrderContext.Current.GetPaymentPlan(plan.CustomerId, plan.OrderGroupId);

			// Validate payment plan
			Assert.IsTrue(!String.IsNullOrEmpty(plan.OrderForms[0].Payments[0].AuthorizationCode));

			// ---------- Step 2. Cancel subscription -----------
			// get cancel status
			PaymentMethodDto dto = PaymentManager.GetPaymentMethodBySystemName("authorize", "en-us", true);
			Assert.IsTrue(dto.PaymentMethod.Count > 0, "Authorize.NET payment method not found for en-us language.");

			PaymentMethodDto.PaymentMethodParameterRow[] rows = (PaymentMethodDto.PaymentMethodParameterRow[])dto.PaymentMethodParameter.Select(String.Format("Parameter = '{0}'", AuthorizePaymentGateway._CancelStatusParameterName));
			Assert.IsTrue(rows != null && rows.Length > 0, "CancelStatus parameter for the Authorize.NET gateway not net.");

			string cancelStatus = rows[0].Value;

			// set status cancel status for the payment plan
			plan.Status = cancelStatus;
			// change AuthorizationCode to generate payment provider error
			plan.OrderForms[0].Payments[0].AuthorizationCode = "000000";

			// Execute workflow
			try
			{
				plan.RunWorkflow("CartCheckout");
			}
			catch (PaymentException ex)
			{
				if (ex.Type != PaymentException.ErrorType.ProviderError)
					throw;
				else
					Console.Error.WriteLine(ex.Message);
			}

			// ---------- Step 3. Perform cleanup -----------

			// Delete payment plan
			plan.Delete();
			plan.AcceptChanges();
			// Assert that payment plan is deleted
			Assert.IsTrue(plan.ObjectState == MetaObjectState.Deleted);

			// Delete cart
			cart.Delete();
			cart.AcceptChanges();
			// Assert that cart is deleted
			Assert.IsTrue(cart.ObjectState == MetaObjectState.Deleted);
		}
		#endregion

        /// <summary>
        /// Test all of the payment types that inherit from the Mediachase.Commerce.Orders.
        /// Payment class. Each of the test involves creating a cart, adding an order form, 
        /// adding an item, adding the particular payment type, saving the cart and then
        /// retrieving the cart and verifying that the payment type saved properly.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_CashCard_SaveAndRetrieveSucceed()
        {
            Cart cart;
            Cart cartVerify;
            CashCardPayment payment;
            CashCardPayment paymentVerify;
            string cashCardNumber = "111";
            string securityCode = "2323";
            string propertyMismatchError = "CashCardPayment fields not saved to the database.";

            //A cart has to exist for a payment instance to be created
            cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();

            //create cash card payment 
            payment = (CashCardPayment)OrderHelper.CreateCashCardPayment();
            payment.CashCardNumber = cashCardNumber;
            payment.CashCardSecurityCode = securityCode;

            //save the cart and then retrieve the cart from the database
            cartVerify = SaveAndRetrieveCart(cart, payment);

            //confirm the payment is retrieved correctly
            Assert.AreNotEqual(cartVerify.OrderForms[0].Payments.Count, 0, "Cash Card Payment not saved to the database");

            //confirm the payment is of the right type
            Assert.AreEqual(cartVerify.OrderForms[0].Payments[0].GetType(),typeof(CashCardPayment),"CashCardPayment type not saved properly.");

            //confirm payment properties
            paymentVerify = (CashCardPayment)cartVerify.OrderForms[0].Payments[0];
            Assert.AreEqual(paymentVerify.CashCardNumber, cashCardNumber, propertyMismatchError);
            Assert.AreEqual(paymentVerify.CashCardSecurityCode, securityCode, propertyMismatchError);
        }

        /// <summary>
        /// Save and retrieve the credit card payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_CreditCard_SaveAndRetrieveSucceed()
        {
            Cart cart;
            Cart cartVerify;
            CreditCardPayment payment;
            CreditCardPayment paymentVerify;
            string cardType = "Visa";
            int expMonth = 3;
            int expYear = DateTime.Now.AddYears(1).Year;
            string propertyMismatchError = "CreditCardPayment fields not saved to the database.";

            //A cart has to exist for a payment instance to be created
            cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();

            //create cc payment to validate test account
            payment = (CreditCardPayment)OrderHelper.CreateCreditCardPayment();
            payment.CardType = cardType;
            payment.ExpirationMonth = expMonth;
            payment.ExpirationYear = expYear;

            //save the cart and then retrieve the cart from the database
            cartVerify = SaveAndRetrieveCart(cart, payment);

            //confirm the payment is retrieved
            Assert.AreNotEqual(cartVerify.OrderForms[0].Payments.Count, 0, "Credit Card Payment not saved to the database");

            //verify returned type is correct
            Assert.AreEqual(cartVerify.OrderForms[0].Payments[0].GetType(), typeof(CreditCardPayment), "CreditCardPayment type not saved properly.");

            //verify properties set and returned correctly
            paymentVerify = (CreditCardPayment)cartVerify.OrderForms[0].Payments[0];
            Assert.AreEqual(paymentVerify.CardType, cardType, propertyMismatchError);
            Assert.AreEqual(paymentVerify.ExpirationMonth, expMonth, propertyMismatchError);
            Assert.AreEqual(paymentVerify.ExpirationYear, expYear, propertyMismatchError);
        }

        /// <summary>
        /// Save and retrieve the invoice payment.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_Invoice_SaveAndRetrieveSucceed()
        {
            Cart cart;
            Cart cartVerify;
            InvoicePayment payment;
            InvoicePayment paymentVerify;
            string invoiceNumber = "3511122A";

            //A cart has to exist for a payment instance to be created
            cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();

            //create invoice payment to test
            payment = (InvoicePayment)OrderHelper.CreateInvoicePayment();
            payment.InvoiceNumber = invoiceNumber;

            //save the cart and then retrieve the cart from the database
            cartVerify = SaveAndRetrieveCart(cart, payment);

            //confirm the payment is retrieved
            Assert.AreNotEqual(cartVerify.OrderForms[0].Payments.Count, 0, "Invoice Payment not saved to the database");

            //confirm the correct type returned
            Assert.AreEqual(cartVerify.OrderForms[0].Payments[0].GetType(), typeof(InvoicePayment), "InvoicePayment type not saved properly.");

            //verify properties set and returned correctly
            paymentVerify = (InvoicePayment)cartVerify.OrderForms[0].Payments[0];
            Assert.AreEqual(paymentVerify.InvoiceNumber, invoiceNumber, "InvoicePayment fields not saved to the database.");
        }

        /// <summary>
        /// Save and retrieve the gift card payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_GiftCard_SaveAndRetrieveSucceed()
        {
            Cart cart;
            Cart cartVerify;
            GiftCardPayment payment;
            GiftCardPayment paymentVerify;
            string cardNumber = "315152C";
            string securityNumber = "5112";
            string cardType = "Mastercard";
            int expMonth = 3;
            int expYear = 2009;
            string propertyMismatchError = "GiftCardPayment fields not saved to the database.";

            //A cart has to exist for a payment instance to be created
            cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();

            //create gift card payment to validate test account
            payment = (GiftCardPayment)OrderHelper.CreateGiftCardPayment();
            payment.CardType = cardType;
            payment.GiftCardNumber = cardNumber;
            payment.GiftCardSecurityCode = securityNumber;
            payment.ExpirationMonth = expMonth;
            payment.ExpirationYear = expYear;

            //save the cart and then retrieve the cart from the database
            cartVerify = SaveAndRetrieveCart(cart, payment);

            //confirm the payment is retrieved
            Assert.AreNotEqual(cartVerify.OrderForms[0].Payments.Count, 0, "Gift Card Payment not saved to the database");

            Assert.AreEqual(cartVerify.OrderForms[0].Payments[0].GetType(), typeof(GiftCardPayment), "GiftCardPayment type not saved properly.");

            paymentVerify = (GiftCardPayment)cartVerify.OrderForms[0].Payments[0];
            Assert.AreEqual(paymentVerify.GiftCardNumber, cardNumber, propertyMismatchError);
            Assert.AreEqual(paymentVerify.GiftCardSecurityCode, securityNumber, propertyMismatchError);
            Assert.AreEqual(paymentVerify.CardType, cardType, propertyMismatchError);
        }

        /// <summary>
        /// Save and retrieve other payments.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_Payments_Other_SaveAndRetrieveSucceed()
        {
            Cart cart;
            Cart cartVerify;
            OtherPayment payment;

            //A cart has to exist for a payment instance to be created
            cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();

            //create other payment to validate test account
            payment = (OtherPayment)OrderHelper.CreateOtherPayment();

            //save the cart and then retrieve the cart from the database
            cartVerify = SaveAndRetrieveCart(cart, payment);

            //confirm the payment is retrieved
            Assert.AreNotEqual(cartVerify.OrderForms[0].Payments.Count, 0, "OtherPayment not saved to the database");

            //confirm the correct type is returned
            Assert.AreEqual(cartVerify.OrderForms[0].Payments[0].GetType(), typeof(OtherPayment), "OtherPayment type not saved properly.");
        }

        /// <summary>
        /// Saves the and retrieve cart.
        /// </summary>
        /// <param name="testCart">The test cart.</param>
        /// <param name="testPayment">The test payment.</param>
        /// <returns></returns>
        public Cart SaveAndRetrieveCart(Cart testCart, Payment testPayment)
        {
            Cart retrievedCart;

            //Set generic payment properties
            testPayment.Amount = 3;
            testPayment.BillingAddressId = "15";
            testPayment.CustomerName = "Harry Potter";
            testPayment.Created = DateTime.UtcNow;
            testPayment.CreatorId = "Jimmy Dean";
            testPayment.Modified = DateTime.UtcNow;
            testCart.OrderForms[0].Payments.Add(testPayment);
            testCart.Status = "Submitted";

            testCart.AcceptChanges();

            retrievedCart = OrderContext.Current.GetCart(Cart.DefaultName, testCart.CustomerId);

            return retrievedCart;
        }
    }
}
