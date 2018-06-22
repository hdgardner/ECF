using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Interfaces
{
    public interface IPreCommit
    {
        /// <summary>
        /// Pre-commit changes.
        /// </summary>
        /// <param name="context">The context.</param>
        void PreCommitChanges(IDictionary context);
    }
}
