using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.Commerce.Profile;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Web.Security;

namespace UnitTests.ProfileSystem
{
    /// <summary>
    /// Summary description for ProfileSystem_UnitTests
    /// </summary>
    [TestClass]
    public class ProfileSystem_UnitTests
    {
        public ProfileSystem_UnitTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// This test confirms that you can retrieve or create a new account, retrieve/create the associated Account object,
        /// access its accountId property, and delete the new account.
        /// </summary>
        [TestMethod]
        public void ProfileSystem_UnitTest_CreateUserWithAccount()
        {
            Account act;
            SqlMembershipProvider provider;
            NameValueCollection collection;
            string userName;
            MembershipUser user;

            //first initialize the membership objects
            provider = new SqlMembershipProvider();
            collection = new NameValueCollection();
            collection.Add("applicationName", "eCommerceFramework");
            collection.Add("name", "CMSMembershipProvider");
            collection.Add("requiresQuestionAndAnswer", "false");
            collection.Add("connectionStringName", "MembershipSqlConnection");
            provider.Initialize(collection["name"], collection);
            userName = "nunittestprofileuser";

            try
            {
                // Retrieve user
                user = provider.GetUser(userName, false);

                if (user == null)
                {
                    // Following exception raised at Membership.CreateUser (can't step in either for whatever reason)
                    // System.NotSupportedException: Specified method is not supported.
                    user = Membership.CreateUser(userName, userName, userName + "@mediachase.com");
                    act = ProfileContext.Current.CreateAccountForUser(user);
                }
                else
                {
                    act = ProfileContext.Current.GetAccount(user.ProviderUserKey.ToString());
                }

                // Assert that account is retrieved.
                Assert.IsNotNull(act, "Failed: Account associated with user could not be retrieved.\n");
                Console.WriteLine("Created account with ID: {0}.\n", act.AccountId);
                
                // Delete created account
                Console.WriteLine("Deleting user {0}.", act.AccountId);
                act.Delete();
                act.AcceptChanges();
                Assert.IsTrue(act.ObjectState == MetaObjectState.Deleted, "Failed: Account was not deleted.\n");

                // Delete new user, assert that it happened.
                Assert.IsTrue(provider.DeleteUser(userName, true), "Failed: User was not deleted.\n");
            }
            catch (Exception exc)
            {
                // Clean-up
                provider.DeleteUser(userName, true);

                throw exc;
            }
        }
    }
}
