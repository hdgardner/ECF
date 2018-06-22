<%@ Application Language="C#" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="Mediachase.Cms" %>
<%@ Import Namespace="Mediachase.Cms.Pages" %>
<%@ Import Namespace="Mediachase.Cms.ResourceHandler" %>
<%@ Import Namespace="Mediachase.Commerce.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Import Namespace="Common.Logging" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        Application["ComponentArtWebUI_AppKey"] = "This edition of ComponentArt Web.UI is licensed for eCommerce Framework application only."; 
        ResourceHandler.ResourceProvider = new FileSystemResourceProvider("Repository","Archive","Temp");
		Mediachase.Cms.Controls.DynamicControlFactory.ControlsFolderPath = "~/Structure/";
    }

    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
    }
    
    void Session_Start(object sender, EventArgs e) 
    {
        //AppStart.AppInitialize();
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
    
    void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        // Check if the user actually exists in our database
        HttpApplication httpApplication = (HttpApplication)sender;

        if (this.Request.IsAuthenticated)
        {
            MembershipUser user = Mediachase.Commerce.Profile.ProfileContext.Current.User;

            bool enabled = true;
            if (user != null)
            {
                int accountState = -1;
                Mediachase.Commerce.Profile.Account account = Mediachase.Commerce.Profile.ProfileContext.Current.GetAccount(true);
                if (account != null)
                {
                    accountState = account.State;
                }

                if (accountState == 1 || accountState == 3)
                    enabled = false;

                /*                                                
                int accountState = Mediachase.Commerce.Profile.ProfileContext.Current.GetAccountState(user);
                if (accountState == 1 || accountState == 3)
                    enabled = false;
                 * */
            }
            
            if (user == null || user.IsLockedOut || !user.IsApproved || !enabled)
            {
                FormsAuthentication.SignOut();
                
                // We now need to redirect, to actually make request non authenticated
                if (!String.IsNullOrEmpty(CMSContext.Current.CurrentUrl))
                    Response.Redirect(CMSContext.Current.CurrentUrl);
                else
                    Response.Redirect(Request.RawUrl);
            }
        }
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        // Initialize log4net context
        log4net.ThreadContext.Properties["Hostname"] = HttpContext.Current.Request.UserHostAddress;
        ///log4net.ThreadContext.Properties["Username"] = HttpContext.Current.User.Identity.Name;
        
        Membership.Provider.ApplicationName = CoreConfiguration.Instance.DefaultApplicationName;
        Roles.Provider.ApplicationName = CoreConfiguration.Instance.DefaultApplicationName;
        ProfileManager.ApplicationName = CoreConfiguration.Instance.DefaultApplicationName;

        string mdCsKey = System.Configuration.ConfigurationManager.AppSettings["MetaDataConnection"];
        if (System.Configuration.ConfigurationManager.ConnectionStrings[mdCsKey] != null)
            Mediachase.Ibn.Data.DataContext.Current = new Mediachase.Ibn.Data.DataContext(System.Configuration.ConfigurationManager.ConnectionStrings["MetadataConnection"].ConnectionString);

        // Oleg Zhuk: Hot Fix Load Meta Model from Crash
        // Mediachase.Ibn.Data.Meta.Management.MetaClassManager metaClassManager = Mediachase.Ibn.Data.DataContext.Current.MetaModel;

        //Init TemplateResolver
        TemplateResolver.Current = new TemplateResolver();

        TemplateResolver.Current.AddSource("QueryString", new TemplateSource(HttpContext.Current.Request.QueryString));

        if (HttpContext.Current.Session != null)
            TemplateResolver.Current.AddSource("Session", new TemplateSource(HttpContext.Current.Session));

        TemplateResolver.Current.AddSource("HttpContext", new TemplateSource(HttpContext.Current.Items));
        TemplateResolver.Current.AddSource("DataContext", new TemplateSource(DataContext.Current.Attributes));

        TemplateResolver.Current.AddSource("DateTime", new DateTimeTemplateSource());
        TemplateResolver.Current.AddSource("Security", new SecurityTemplateSource());           
        
        // Initialize Page Document
        if (Mediachase.Cms.Pages.PageDocument.PersistentDocumentStorage == null || Mediachase.Cms.Pages.PageDocument.TemporaryDocumentStorage == null)
            Mediachase.Cms.Pages.PageDocument.Init(new Mediachase.Cms.Pages.SqlPageDocumentStorageProvider(), new Mediachase.Cms.Pages.SqlTemporaryStorageProvider());
    }

    protected void Application_EndRequest(object sender, EventArgs e)
    {
        /* uncomment this line, if you do not want anonymous id to persist across sessions
        if (AnonymousIdentificationModule.Enabled)
        {            
            // Make anonymous cookie not persistent
            HttpApplication httpApplication = (HttpApplication)sender;
            string cookieName = ".ASPXANONYMOUS";
            
            // Check if cookie exists in the outgoing response

            HttpCookie cookie = null;
            foreach (string key in this.Response.Cookies.Keys)
            {
                if (key.Equals(cookieName, StringComparison.OrdinalIgnoreCase))
                {
                    cookie = this.Response.Cookies[key];
                    break;
                }
            }
            
            if (cookie != null)
            {
                HttpCookie newCookie = new HttpCookie(cookieName, cookie.Value);
                newCookie.HttpOnly = cookie.HttpOnly;
                newCookie.Domain = cookie.Domain;
                newCookie.Path = cookie.Path;
                newCookie.Secure = cookie.Secure;
                
                HttpContext.Current.Response.Cookies.Remove(cookieName);
                HttpContext.Current.Response.Cookies.Set(newCookie);
            }
        }
         * */
    }    

    void Profile_MigrateAnonymous(Object sender, ProfileMigrateEventArgs pe)
    {
        //Get the anonymous profile here
        ProfileCommon anonProfile = Profile.GetProfile(pe.AnonymousID);

        Mediachase.Commerce.Profile.ProfileContext.Current.CreateAccountForUser();
        
        // Associate orders with a current customer
        object orderId = HttpContext.Current.Session["LatestOrderId"];
        if (orderId != null)
        {
            PurchaseOrder[] orders = OrderContext.Current.GetPurchaseOrders(new Guid(pe.AnonymousID));

            if (orders != null)
            {
                foreach (PurchaseOrder order in orders)
                {
                    order.CustomerId = Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.PrincipalId;
                    order.CustomerName = Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Name;
                    order.AcceptChanges();
                }
            }
        }

        // Migrate shopping cart
        CartHelper cart = new CartHelper(Cart.DefaultName);
        CartHelper anonymousCart = new CartHelper(Cart.DefaultName, new Guid(pe.AnonymousID));

        // Only perform merge if cart is not empty
        if (!anonymousCart.IsEmpty)
        {
            // Merge cart
            cart.Cart.Add(anonymousCart.Cart, true);
            cart.Cart.AcceptChanges();

            // Delete anonymous cart
            anonymousCart.Cart.Delete();
            anonymousCart.Cart.AcceptChanges();
        }

        //Delete the anonymous data from the database
        ProfileManager.DeleteProfile(pe.AnonymousID);        
        
        //Remove the anonymous identifier from the request so 
        //this event will no longer fire for a logged-in user
        AnonymousIdentificationModule.ClearAnonymousIdentifier();
    }

    void Profile_ProfileAutoSaving(Object sender, ProfileAutoSaveEventArgs e)
    {
        if ((e.Context.Items["CancelProfileAutoSave"] != null) && ((bool)e.Context.Items["CancelProfileAutoSave"] == true))
            e.ContinueWithProfileAutoSave = false;
    }
      
</script>
