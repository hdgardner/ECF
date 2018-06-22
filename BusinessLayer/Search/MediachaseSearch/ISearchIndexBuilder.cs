using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;

namespace Mediachase.Search
{
    public interface ISearchIndexBuilder
    {
        /// <summary>
        /// Builds the index.
        /// </summary>
        /// <param name="rebuild">if set to <c>true</c> the full rebuild will be done, if not. The last build date will be used.</param>
        void BuildIndex(bool rebuild);

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        SearchManager Manager {get;set;}

        /// <summary>
        /// Gets or sets the build indexer.
        /// </summary>
        /// <value>The build indexer.</value>
        IndexBuilder Indexer { get; set; }
    }
}
