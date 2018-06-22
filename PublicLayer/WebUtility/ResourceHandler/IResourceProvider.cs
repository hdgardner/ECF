using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Cms.ResourceHandler
{
    public interface IResourceProvider
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        void ProcessRequest(HttpContext context);
    }
}
