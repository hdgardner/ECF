using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Collections;
using System.Xml;
using System.IO;
using System.Data;
using Mediachase.Commerce.Profile;

namespace UnitTests.ProfileSystem
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ProfileSystem_Conf
    {
        private static Guid _SiteGuid = new Guid("1ab08b1a-c480-47b5-a98e-3d50b433dcb5");

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSystem_Conf"/> class.
        /// </summary>
        public ProfileSystem_Conf()
        {
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
        /// Restores the meta data.
        /// </summary>
        private static void RestoreMetaData()
        {
            ProfileConfiguration.ConfigureMetaData();
        }

        /// <summary>
        /// Initializes the specified test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]       
        public static void Initialize(TestContext testContext) 
        {
            //FrameworkConfig.Init();
            //MetaDataContext.Current.ConnectionString = FrameworkConfig.ConnectionString;
            
            //CreateCatalog();
        }

        /// <summary>
        /// Cleans up this instance.
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
            //DeleteCatalog();
        }

        #region Test Methods
        /// <summary>
        /// Initialzes the test.
        /// </summary>
        [TestInitialize]
        public void InitTest()
        {
            
        }

        /*
        /// <summary>
        /// Recreates the profile meta data.
        /// </summary>
        [TestMethod]
        [Description("Profile System: recreate Default Meta Data")]
        public void RecreateProfileMetaData()
        {
            foreach (MetaClass cl in MetaClass.GetList(ProfileContext.MetaDataContext, "Mediachase.Commerce.Profile", true))
            {
                if (!cl.IsSystem)
                    MetaClass.Delete(ProfileContext.MetaDataContext, cl.Id);
            }

            foreach (MetaClass cl in MetaClass.GetList(ProfileContext.MetaDataContext, "Mediachase.Commerce.Profile", true))
            {
                if (cl.IsSystem)
                    MetaClass.Delete(ProfileContext.MetaDataContext, cl.Id);
            }


            RestoreMetaData();
        }
         * */

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
        }
        #endregion
    }
}