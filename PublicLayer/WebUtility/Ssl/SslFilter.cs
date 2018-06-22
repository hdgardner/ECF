using System;
using System.Configuration;
using System.Web;
using Mediachase.Cms.Util;

namespace Mediachase.Cms.WebUtility.Ssl
{
    /// <summary>
    /// The SslFilter class hooks the application's BeginRequest event
    /// in order to request the current page securely if specified in the 
    /// configuration file.
    /// </summary>
    public class SslFilter : IHttpModule
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public SslFilter()
        {
        }

        /// <summary>
        /// Disposes of any resources used.
        /// </summary>
        public void Dispose()
        {
            // No resources were used.
        }

        /// <summary>
        /// Initializes the module by hooking the application's BeginRequest event if indicated by the config settings.
        /// </summary>
        /// <param name="application">The HttpApplication this module is bound to.</param>
        public void Init(HttpApplication application)
        {
            // Get the settings for the secureWebPages section
            SecureWebPageSettings Settings = (SecureWebPageSettings)ConfigurationManager.GetSection("CommerceFramework/SSL");
            if (Settings != null && Settings.Mode != SecureWebPageMode.Off)
            {
                // Store the settings in application state for quick access on each request
                application.Application["SecureWebPageSettings"] = Settings;

                // Add a reference to the private Application_BeginRequest handler for the
                // application's BeginRequest event
                application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
            }
        }

        /// <summary>
        /// Tests the given request to see if it matches the specified mode.
        /// </summary>
        /// <param name="request">A HttpRequest to test.</param>
        /// <param name="mode">The SecureWebPageMode used in the test.</param>
        /// <returns>
        ///		Returns true if the request matches the mode as follows:
        ///		<list type="disc">
        ///			<item>If mode is On.</item>
        ///			<item>If mode is set to RemoteOnly and the request is from a computer other than the server.</item>
        ///			<item>If mode is set to LocalOnly and the request is from the server.</item>
        ///		</list>
        ///	</returns>
        private bool RequestMatchesMode(HttpRequest request, SecureWebPageMode mode)
        {
            switch (mode)
            {
                case SecureWebPageMode.On:
                    return true;

                case SecureWebPageMode.RemoteOnly:
                    return (request.ServerVariables["REMOTE_ADDR"] != request.ServerVariables["LOCAL_ADDR"]);

                case SecureWebPageMode.LocalOnly:
                    return (request.ServerVariables["REMOTE_ADDR"] == request.ServerVariables["LOCAL_ADDR"]);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Handle the application's BeginRequest event by requesting the current
        /// page securely, if specified.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">EventArgs passed in.</param>
        private void Application_BeginRequest(Object source, EventArgs e)
        {
			// Cast the source as an HttpApplication instance
            HttpApplication Application = (HttpApplication)source;

            // Retrieve the settings from application state
            SecureWebPageSettings Settings = (SecureWebPageSettings)Application.Application["SecureWebPageSettings"];

			// Determine if this request should be ignored based on the settings' Mode
            if (RequestMatchesMode(Application.Request, Settings.Mode))
            {
                // Intialize
                bool MatchFound = false;
                SecurityType Secure = SecurityType.Insecure;

                // Get the relative file path of the current request from the application root
                string RelativeFilePath = Application.Request.Url.AbsolutePath.Remove(0, Application.Request.ApplicationPath.Length).ToLower();
                if (!RelativeFilePath.StartsWith("/"))
                    // Add a leading "/"
                    RelativeFilePath = "/" + RelativeFilePath;

                // Get the relative directory of the current request by removing the last segment of the RelativeFilePath
                string RelativeDirectory = RelativeFilePath.Substring(0, RelativeFilePath.LastIndexOf('/') + 1);

                // Determine if there is a matching file path for the current request
                int i = Settings.Files.IndexOf(RelativeFilePath);
                if (i >= 0)
                {
                    MatchFound = true;
                    Secure = Settings.Files[i].Secure;
                }

                // Try to find a matching directory path, if no file was found
                i = 0;
                while (!MatchFound && i < Settings.Directories.Count)
                {
                    if (Settings.Directories[i].Recurse)
                        // Match the beginning of the directory if recursion is allowed
                        MatchFound = (RelativeDirectory.StartsWith(Settings.Directories[i].Path));
                    else
                        // Match the entire directory
                        MatchFound = (RelativeDirectory == Settings.Directories[i].Path);

                    if (MatchFound)
                        Secure = Settings.Directories[i].Secure;
                    i++;
                }

				bool design = CommonHelper.CheckDesignMode(Application.Context);

                // Test for match for a secure connection
                if (MatchFound && Secure == SecurityType.Secure && !design)
                    SSLHelper.RequestSecurePage(Settings);
                else if (Secure != SecurityType.Ignore)
                    SSLHelper.RequestUnsecurePage(Settings);
            }
        }
    }

}