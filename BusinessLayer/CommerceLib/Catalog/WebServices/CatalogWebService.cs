using System;
using System.ServiceModel;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Impl;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Search;

namespace Mediachase.Commerce.Catalog.WebServices
{

    // The following settings must be added to your configuration file in order for 
    // the new WCF service item added to your project to work correctly.

    // <system.serviceModel>
    //    <services>
    //      <!-- Before deployment, you should remove the returnFaults behavior configuration to avoid disclosing information in exception messages -->
    //      <service type="Mediachase.Commerce.Catalog.WebServices.CatalogWebService" behaviorConfiguration="returnFaults">
    //        <endpoint contract="Mediachase.Commerce.Catalog.WebServices.ICatalogWebService" binding="wsHttpBinding"/>
    //      </service>
    //    </services>
    //    <behaviors>
    //      <serviceBehaviors>
    //        <behavior name="returnFaults" >
    //          <serviceDebug includeExceptionDetailInFaults="true" />
    //        </behavior>
    //       </serviceBehaviors>
    //    </behaviors>
    // </system.serviceModel>


    // A WCF service consists of a contract (defined below), 
    // a class which implements that interface, and configuration 
    // entries that specify behaviors and endpoints associated with 
    // that implementation (see <system.serviceModel> in your application
    // configuration file).

   /// <summary>
    /// The actual web service implementation called by the CatalogContextProxyImpl classes on the server.
    /// This is a Server object and has direct access to the database/backend.
    /// </summary>
    public class CatalogWebService : CatalogContextProxyImpl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogWebService"/> class.
        /// </summary>
        public CatalogWebService() : base() {}
    }
}

