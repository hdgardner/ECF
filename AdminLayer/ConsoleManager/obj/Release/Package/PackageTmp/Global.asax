<%@ Application Language="C#" %>
<%@ Import Namespace="Mediachase.Commerce.Core.Dto" %>
<%@ Import Namespace="Mediachase.Commerce.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Import Namespace="Common.Logging" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        Application["ComponentArtWebUI_AppKey"] = "This edition of ComponentArt Web.UI is licensed for eCommerce Framework application only.";

        ControlPathResolver current = new ControlPathResolver();
        current.Init(new string[] {"~/Apps/MetaDataBase/Primitives/",
                                    "~/Apps/MetaDataBase/MetaUI/Primitives/"});

        ControlPathResolver.Current = current;

        Mediachase.Ibn.Web.UI.Layout.DynamicControlFactory.ControlsFolderPath = "~/Apps/";
        Mediachase.Ibn.Web.UI.Layout.WorkspaceTemplateFactory.ControlsFolderPath = "~/Apps/";

		Mediachase.Ibn.Web.UI.Controls.Util.FormController.Init();

        InitializeGlobalContext();
    }
      
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        Exception ex = Server.GetLastError().GetBaseException();

        if (ex != null)
        {
            if (typeof(System.ComponentModel.LicenseException) == ex.GetType())
            {
                Response.Redirect(String.Format("~/Licensing.aspx?m={0}", ex.Message));
            }
            else if (typeof(UnauthorizedAccessException) == ex.GetType())
            {
                Response.Redirect(String.Format("~/Unauthorized.html"));
            }
            else if (typeof(HttpException) == ex.GetType())
            {
                int errorCode = ((HttpException)ex).ErrorCode;
                if (errorCode == 500) // consider 500 a fatal exception
                {
                    // Log the exception
                    LogManager.GetLogger(GetType()).Fatal("Backend encountered unhandled error.", ex);
                    return;
                }
            }            
        }

        // Code that runs when an unhandled error occurs
        // Log the exception
        LogManager.GetLogger(GetType()).Error("Backend encountered unhandled error.", ex);

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        log4net.ThreadContext.Properties["Hostname"] = HttpContext.Current.Request.UserHostAddress;
        
        string mdCsKey = System.Configuration.ConfigurationManager.AppSettings["MetaDataConnection"];
        if (System.Configuration.ConfigurationManager.ConnectionStrings[mdCsKey] != null)
            Mediachase.Ibn.Data.DataContext.Current = new Mediachase.Ibn.Data.DataContext(System.Configuration.ConfigurationManager.ConnectionStrings[mdCsKey].ConnectionString);

        //Mediachase.Ibn.Data.Meta.Management.MetaClassManager metaClassManager = Mediachase.Ibn.Data.DataContext.Current.MetaModel;

        //Init TemplateResolver
        TemplateResolver.Current = new TemplateResolver();

        TemplateResolver.Current.AddSource("QueryString", new TemplateSource(HttpContext.Current.Request.QueryString));

        if (HttpContext.Current.Session != null)
            TemplateResolver.Current.AddSource("Session", new TemplateSource(HttpContext.Current.Session));

        TemplateResolver.Current.AddSource("HttpContext", new TemplateSource(HttpContext.Current.Items));
        TemplateResolver.Current.AddSource("DataContext", new TemplateSource(DataContext.Current.Attributes));

        TemplateResolver.Current.AddSource("DateTime", new DateTimeTemplateSource());
        TemplateResolver.Current.AddSource("Security", new SecurityTemplateSource());
        
        //Init PathTemplateResolver
        PathTemplateResolver.Current = new PathTemplateResolver();

        PathTemplateResolver.Current.AddSource("QueryString", new PathTemplateSource(HttpContext.Current.Request.QueryString));

        if (HttpContext.Current.Session != null)
            PathTemplateResolver.Current.AddSource("Session", new PathTemplateSource(HttpContext.Current.Session));

        PathTemplateResolver.Current.AddSource("HttpContext", new PathTemplateSource(HttpContext.Current.Items));
        PathTemplateResolver.Current.AddSource("DataContext", new PathTemplateSource(DataContext.Current.Attributes));

        PathTemplateResolver.Current.AddSource("DateTime", new Mediachase.Ibn.Web.UI.Controls.Util.DateTimePathTemplateSource());
        PathTemplateResolver.Current.AddSource("Security", new Mediachase.Ibn.Web.UI.Controls.Util.SecurityPathTemplateSource());

        InitializeGlobalContext();
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        InitializeGlobalContext();
    }
    
    protected void Application_AuthorizeRequest(object sender, EventArgs e)
    {
        HttpApplication httpApplication = (HttpApplication)sender;

        if (this.Request.IsAuthenticated)
        {
            // Check current 
            string fullName = User.Identity.Name;
            string appName = String.Empty;
            
            // If user authenticated, recreate the authentication cookie with a new value
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                appName = ticket.UserData;
            }

            if (appName.Length == 0)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
                httpApplication.CompleteRequest();
            }

            
            AppDto dto = AppContext.Current.GetApplicationDto(appName);

            // If application does not exists or is not active, prevent login
            if (dto == null || dto.Application.Count == 0 || !dto.Application[0].IsActive)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
                httpApplication.CompleteRequest();
            }
            else
            {
                Membership.Provider.ApplicationName = appName;
                Roles.Provider.ApplicationName = appName;
                ProfileManager.ApplicationName = appName;
                AppContext.Current.ApplicationId = dto.Application[0].ApplicationId;
                AppContext.Current.ApplicationName = dto.Application[0].Name;
                log4net.ThreadContext.Properties["ApplicationId"] = AppContext.Current.ApplicationId;
                log4net.ThreadContext.Properties["Username"] = Mediachase.Commerce.Profile.ProfileContext.Current.UserName;
            }

            // Check permissions
            // Check permissions
            if (Mediachase.Commerce.Profile.ProfileConfiguration.Instance.EnablePermissions)
            {
                if (!Mediachase.Commerce.Profile.ProfileContext.Current.CheckPermission("core:mng:login"))
                {
                    FormsAuthentication.SignOut();
                    this.Response.Redirect("~/Unauthorized.html");
                    httpApplication.CompleteRequest();               
                }

                Mediachase.Commerce.Profile.ProfileContext context = Mediachase.Commerce.FrameworkContext.Current.Profile;

                try
                {
                    if (context != null && context.Profile != null && context.Profile.Account != null)
                    {
                        if (context.Profile.Account.State != 2)
                        {
                            FormsAuthentication.SignOut();
                            this.Response.Redirect("~/Unauthorized.html");
                            httpApplication.CompleteRequest();
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage();
                    httpApplication.CompleteRequest();                                  
                }                
            }            
            else if (!Mediachase.Commerce.Shared.SecurityManager.CheckPermission(
                  new string[] { Mediachase.Commerce.Core.AppRoles.AdminRole,
                                 Mediachase.Commerce.Core.AppRoles.ManagerUserRole}))
            {
                FormsAuthentication.SignOut();
                this.Response.Redirect("~/Unauthorized.html");
                httpApplication.CompleteRequest();
            }
        }
    }

    protected void Application_PostAcquireRequestState(object sender, EventArgs e)
    {
        try
        {
            SetCulture(Mediachase.Web.Console.ManagementContext.Current.ConsoleUICulture);
        }
        catch (Exception)
        {
        }
    }

    public static void SetCulture(System.Globalization.CultureInfo culture)
    {
        // Set the CurrentCulture property to the requested culture.
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;

        // Initialize the CurrentUICulture property
        // with the CurrentCulture property.
        System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
    }

    void InitializeGlobalContext()
    {
        string modulesVirtualPath = "~/Apps/";
        string modulesDirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath(modulesVirtualPath);

        Mediachase.Ibn.GlobalContext context = Mediachase.Ibn.GlobalContext.Current;
        if (context == null)
        {
            context = new Mediachase.Ibn.GlobalContext(null);
            Mediachase.Ibn.GlobalContext.Current = context;
        }

        context.ModulesDirectoryPath = modulesDirectoryPath;
        context.ModulesVirtualPath = modulesVirtualPath;
    }
</script>