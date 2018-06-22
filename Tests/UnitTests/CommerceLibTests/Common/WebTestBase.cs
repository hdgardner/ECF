using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

namespace UnitTests.Common
{
    /// <summary>
    /// Base class for web tests
    /// </summary>
    public abstract class WebTestBase : WebTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebTestBase"/> class.
        /// </summary>
        public WebTestBase()
        {
            this.PreAuthenticate = true;
        }
    }
}
