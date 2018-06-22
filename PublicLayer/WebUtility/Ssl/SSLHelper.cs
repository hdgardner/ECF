using System;
using System.Web;

namespace Mediachase.Cms.WebUtility.Ssl
{
	/// <summary>
	/// The SSLHelper class provides static methods for ensuring that a page is rendered 
	/// securely via SSL or unsecurely.
	/// </summary>
	public class SSLHelper
	{
		private const string UnsecureProtocolPrefix = "http://";
		private const string SecureProtocolPrefix = "https://";

		/// <summary>
		/// Intializes an instance of this class.
		/// </summary>
		public SSLHelper()
		{
		}

		/// <summary>
		/// Requests the current page over a secure connection, if it is not already.
		/// </summary>
		/// <param name="hostPath">The host path to redirect to if needed.</param>
		/// <param name="maintainApplicationPath">
		///		A flag indicating whether or not to maintain the current application path if
		///		a redirect is necessary.
		///	</param>
		public static void RequestSecurePage(SecureWebPageSettings settings)
		{
			// Is this request secure?
			string RequestPath = HttpContext.Current.Request.Url.ToString();
			if (RequestPath.StartsWith(UnsecureProtocolPrefix))
			{
				// Is there a host path to redirect to?
				if (settings.EncryptedUri == null || settings.EncryptedUri.Length == 0)
					// Replace the protocol of the requested URL with "https"
					RequestPath = RequestPath.Replace(UnsecureProtocolPrefix, SecureProtocolPrefix);
				else
					// Build the URL with the "https" protocol
					RequestPath = BuildUrl(true, settings.MaintainPath, settings.EncryptedUri, settings.UnencryptedUri);
				
				// Redirect to the secure page
				HttpContext.Current.Response.Redirect(RequestPath, true);
			}
		}

		/// <summary>
		/// Requests the current page over an insecure connection, if it is not already.
		/// </summary>
		/// <param name="hostPath">The host path to redirect to if needed.</param>
		/// <param name="maintainApplicationPath">
		///		A flag indicating whether or not to maintain the current application path if
		///		a redirect is necessary.
		///	</param>
		public static void RequestUnsecurePage(SecureWebPageSettings settings)
		{
			// Is this request secure?
			HttpRequest Request = HttpContext.Current.Request;
			string RequestPath = Request.Url.ToString();
			if (RequestPath.StartsWith(SecureProtocolPrefix))
			{
				// Is there a different URI to redirect to?
				if (settings.UnencryptedUri == null || settings.UnencryptedUri.Length == 0)
					// Replace the protocol of the requested URL with "http"
					RequestPath = RequestPath.Replace(SecureProtocolPrefix, UnsecureProtocolPrefix);
				else
					// Build the URL with the "http" protocol
					RequestPath = BuildUrl(false, settings.MaintainPath, settings.EncryptedUri, settings.UnencryptedUri);

				// Test for the need to bypass a security warning
				bool Bypass;
				if (settings.WarningBypassMode == SecurityWarningBypassMode.AlwaysBypass)
					Bypass = true;
				else if (settings.WarningBypassMode == SecurityWarningBypassMode.BypassWithQueryParam && 
					Request.QueryString[settings.BypassQueryParamName] != null)
				{
					Bypass = true;

					// Remove the bypass query parameter from the URL
					System.Text.StringBuilder NewPath = new System.Text.StringBuilder(RequestPath);
					int i = RequestPath.IndexOf(settings.BypassQueryParamName);
					NewPath.Remove(i, settings.BypassQueryParamName.Length + Request.QueryString[settings.BypassQueryParamName].Length + 1);

					// Remove any abandoned "&" character
					if (i >= NewPath.Length)
						i = NewPath.Length - 1;
					if (NewPath[i] == '&')
						NewPath.Remove(i, 1);

					// Remove any abandoned "?" character
					i = NewPath.Length - 1;
					if (NewPath[i] == '?')
						NewPath.Remove(NewPath.Length - 1, 1);

					RequestPath = NewPath.ToString();
				}
				else
					Bypass = false;
					
				// Output a redirector for the needed page to avoid a security warning
				if (Bypass)
				{
					HttpResponse Response = HttpContext.Current.Response;
					Response.Clear();

					// Refresh header
					Response.AddHeader("Refresh", string.Concat("0;URL=", RequestPath));
						
					// JavaScript to replace the current location
					Response.Write("<html><head><title></title>");
                    Response.Write("<!-- <script type=\"text/javascript\">window.location.replace(\"");
					Response.Write(RequestPath);
					Response.Write("\");</script> -->");
					Response.Write("</head><body></body></html>");

					Response.End();

					return;
				}

				// Redirect to the insecure page
				HttpContext.Current.Response.Redirect(RequestPath, true);
			}
		}
		
		/// <summary>
		/// Builds a URL from the given protocol and appropriate host path. The resulting URL 
		/// will maintain the current path if requested.
		/// </summary>
		/// <param name="secure">Is this to be a secure URL?</param>
		/// <param name="maintainPath">Should the current path be maintained during transfer?</param>
		/// <param name="encryptedUri">The URI to redirect to for encrypted requests.</param>
		/// <param name="unencryptedUri">The URI to redirect to for standard requests.</param>
		/// <returns></returns>
		private static string BuildUrl(bool secure, bool maintainPath, string encryptedUri, string unencryptedUri)
		{
			// Get the current request
			HttpRequest Request = HttpContext.Current.Request;
			string CurrentUrl = Request.Url.ToString();

			// Build the needed URL
			System.Text.StringBuilder Url = new System.Text.StringBuilder();

			// Host path (secure.mysite.com/)
			if (secure)
				Url.Append(encryptedUri);
			else
				Url.Append(unencryptedUri);
			Url.Append("/");

			if (maintainPath)
			{
				// Calculate the length of the appropriate target URI
				int RootLength = 0;
				if (CurrentUrl.StartsWith(UnsecureProtocolPrefix))
					RootLength = UnsecureProtocolPrefix.Length + unencryptedUri.Length;
				else
					RootLength = SecureProtocolPrefix.Length + encryptedUri.Length;

				// Get the path from the current URL after the target URI
				Url.Append(CurrentUrl.Substring(RootLength));
			}
			else
				// Append just the current page
				Url.Append(CurrentUrl.Substring(CurrentUrl.LastIndexOf('/') + 1));

			// Replace any double slashes with a single slash
			Url.Replace("//", "/");

			// Prepend the protocol
			if (secure)
				Url.Insert(0, SecureProtocolPrefix);
			else
				Url.Insert(0, UnsecureProtocolPrefix);

			return Url.ToString();
		}
	}
}
