using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using System.Reflection;
using System.IO;
using Lucene.Net.Index;
using Common.Logging;

namespace Mediachase.Search
{
    public class SearchManager
    {
        private readonly object _lockObject = new object();
        private double _IndexBuilderIndex = 1;
        private double _IndexBuilderCount = 1;
        private readonly ILog Logger;

        /// <summary>
        /// Gets the sync object. Used to synchronize threads.
        /// </summary>
        /// <value>The sync object.</value>
        public object SyncObject
        {
            get { return _lockObject; }
        } 

        public event SearchMessageHandler SearchMessage;
        public event SearchIndexHandler SearchIndexMessage;
        /// <summary>
        /// Called when [search message].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSearchMessage(object source, SearchEventArgs args)
        {
            if (this.SearchMessage != null)
                this.SearchMessage(source, args);
        }

        /// <summary>
        /// Called when [search index message].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchIndexEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSearchIndexMessage(object source, SearchIndexEventArgs args)
        {
            Logger.Debug(String.Format("\"{0}\" - {1}%.", args.Message, Convert.ToInt32(args.CompletedPercentage).ToString()));
            if (this.SearchIndexMessage != null)
                this.SearchIndexMessage(source, args);
        }

        /// <summary>
        /// Raises the search index event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchIndexEventArgs"/> instance containing the event data.</param>
        public void RaiseSearchIndexEvent(object source, SearchIndexEventArgs args)
        {
            // Modify the percentage to take into account how many index builders we have
            args.CompletedPercentage = args.CompletedPercentage * (_IndexBuilderIndex / _IndexBuilderCount);            
            OnSearchIndexMessage(source, args);
        }

        /// <summary>
        /// Raises the search event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchEventArgs"/> instance containing the event data.</param>
        public void RaiseSearchEvent(object source, SearchEventArgs args)
        {
            OnSearchMessage(source, args);
        }


        private Analyzer _Analyzer = null;

        /// <summary>
        /// Gets or sets the analyzer.
        /// </summary>
        /// <value>The analyzer.</value>
        public Analyzer Analyzer
        {
            get { 
                if(_Analyzer == null)
                    _Analyzer = new StandardAnalyzer();
                return _Analyzer; }
            set { _Analyzer = value; }
        }

        Dictionary<string, IndexSearcher> _SearchList = new Dictionary<string, IndexSearcher>();
        /// <summary>
        /// Searches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public SearchResults Search(ISearchCriteria criteria)
        {
            if (_SearchList == null || _SearchList.Count == 0)
            {
                foreach (IndexerDefinition indexer in SearchConfiguration.Instance.Indexers)
                {
                    try
                    {
                        _SearchList.Add(indexer.Name, new IndexSearcher(GetApplicationPath(indexer)));
                    }
                    catch (FileNotFoundException ex)
                    {
                        string msg = String.Format("No search indexers found for \"{0}\" indexer.", indexer.Name);
                        IndexNotFoundException ne = new IndexNotFoundException(msg, ex);
                        Logger.Error(msg, ne);
                        throw ne;
                    }
                }
            }

            if (_SearchList.Count == 0)
            {
                string msg = String.Format("No Search Indexers defined in the configuration file.");
                Logger.Error(msg);
                throw new IndexNotFoundException(msg);
            }

            if (!_SearchList.ContainsKey(criteria.Scope))
            {
                string msg = String.Format("Specified scope \"{0}\" not declared in the configuration file.", criteria.Scope);
                Logger.Error(msg);
                throw new IndexNotFoundException(msg);
            }

            IndexSearcher searcher = _SearchList[criteria.Scope];
            if (_SearchList.Count == 0)
            {
                string msg = String.Format("No Search Indexers defined in the configuration file.");
                Logger.Error(msg);
                throw new IndexNotFoundException(msg);
            }

            Hits hits = null;
            try
            {
                if (criteria.Sort != null)
                    hits = searcher.Search(criteria.Query, new Sort(criteria.Sort.FieldName, criteria.Sort.IsDescending));
                else
                    hits = searcher.Search(criteria.Query);
            }
            catch (SystemException ex)
            {
                string msg = String.Format("Search failed.");
                Logger.Error(msg, ex);
                throw;
            }

            SearchResults results = new SearchResults(searcher.GetIndexReader(), hits, criteria);
            return results;
        }

        /// <summary>
        /// Builds the index.
        /// </summary>
        /// <param name="rebuild">if set to <c>true</c> [rebuild].</param>
        public void BuildIndex(bool rebuild)
        {
            _IndexBuilderIndex = 1;
            _IndexBuilderCount = SearchConfiguration.Instance.Indexers.Count;
            foreach (IndexerDefinition indexer in SearchConfiguration.Instance.Indexers)
            {
                Logger.Debug(String.Format("Getting the type \"{0}\".", indexer.ClassName));
                ClassInfo classInfo = new ClassInfo(indexer.ClassName);
                Logger.Debug(String.Format("Creating instance of \"{0}\".", classInfo.Type.Name));
                ISearchIndexBuilder builder = ((ISearchIndexBuilder)classInfo.CreateInstance());
                builder.Manager = this;
                IndexBuilder buildIndexer = null;

                try
                {
                    string path = GetApplicationPath(indexer);
                    buildIndexer = new IndexBuilder(path, rebuild, indexer.Name);
                    builder.Indexer = buildIndexer;
                    if (rebuild)
                        Logger.Info(String.Format("Starting new index build in \"{0}\" using \"{1}\" indexer.", path, indexer.Name));
                    else
                        Logger.Info(String.Format("Starting incremental index build in \"{0}\" using \"{1}\" indexer.", path, indexer.Name));

                    builder.BuildIndex(rebuild);
                    buildIndexer.SaveBuild(Status.Completed);
                    buildIndexer.Close(!buildIndexer.RebuildIndex);
                    Logger.Info(String.Format("Successfully finished index build in \"{0}\" using \"{1}\" indexer.", path, indexer.Name));
                }
                catch (Exception ex)
                {
                    if (buildIndexer != null)
                        buildIndexer.Close();

                    string msg = String.Format("Build Failed using \"{0}\" indexer. \"{1}\"", indexer.Name, ex.Message);
                    IndexBuildException ne = new IndexBuildException(msg, ex);                    
                    Logger.Info(msg, ne);
                    throw ne;
                }
                finally
                {
                    
                }
                _IndexBuilderIndex++;
            }
        }

        private string _ApplicationName = String.Empty;
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchManager"/> class.
        /// </summary>
        /// <param name="appName">Name of the app.</param>
        public SearchManager(string appName)
        {
            Logger = LogManager.GetLogger(GetType());
            _ApplicationName = appName;
        }


        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <returns></returns>
        private string GetApplicationPath(IndexerDefinition indexer)
        {
            return indexer.BasePath + "\\" + _ApplicationName + "\\" + indexer.Name;
        }

        /// <summary>
        /// Checks the configuration.
        /// </summary>
        public void CheckConfiguration()
        {
            foreach (IndexerDefinition indexer in SearchConfiguration.Instance.Indexers)
            {
                // Check if folder exists
                if (!Directory.Exists(indexer.BasePath))
                {
                    throw new DirectoryNotFoundException(indexer.BasePath);
                }

                // Check if we have access
                if (!HasWriteAccess(new DirectoryInfo(indexer.BasePath)))
                {
                    new UnauthorizedAccessException(indexer.BasePath);
                }
            }
        }

        /// <summary>
        /// Checks the indexes and throws FileNotFoundException exception if no idexers found.
        /// </summary>
        public void CheckIndexes()
        {
            List<Searchable> list = new List<Searchable>();
            foreach (IndexerDefinition indexer in SearchConfiguration.Instance.Indexers)
            {
                list.Add(new IndexSearcher(GetApplicationPath(indexer)));
            }
        }

        /// <summary>
        /// Determines whether [has write access] [the specified index dir].
        /// </summary>
        /// <param name="indexDir">The index dir.</param>
        /// <returns>
        /// 	<c>true</c> if [has write access] [the specified index dir]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasWriteAccess(DirectoryInfo indexDir)
        {
            string tempFileName = Path.Combine(indexDir.FullName, Guid.NewGuid().ToString());
            //Yuck! but it is the simplest way
            try
            {
                File.CreateText(tempFileName).Close();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            try
            {
                File.Delete(tempFileName);
            }
            catch (UnauthorizedAccessException)
            {
                //we may have permissions to create but not delete, ignoring
            }
            return true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public delegate void SearchMessageHandler(object source, SearchEventArgs args);
    public delegate void SearchIndexHandler(object source, SearchIndexEventArgs args);
    public class SearchEventArgs : EventArgs
    {
        private string _Message = String.Empty;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchEventArgs"/> class.
        /// </summary>
        public SearchEventArgs()
            : base()
        {
        }
    }

    public class SearchIndexEventArgs : SearchEventArgs
    {
        private double _CompletePercentage = 0;

        /// <summary>
        /// Gets or sets the completed percentage.
        /// </summary>
        /// <value>The completed percentage.</value>
        public double CompletedPercentage
        {
            get { return _CompletePercentage; }
            set { _CompletePercentage = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndexEventArgs"/> class.
        /// </summary>
        public SearchIndexEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndexEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="percentage">The percentage.</param>
        public SearchIndexEventArgs(string message, double percentage)
            : base()
        {
            this.Message = message;
            this.CompletedPercentage = percentage;
        }
    }    

    /// <summary>
    /// Implements operations for and represents commerce class information.
    /// </summary>
    public class ClassInfo
    {
        private Type _Type;
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get { return _Type; }
        }

        private ConstructorInfo _DefaultConstructor;
        /// <summary>
        /// Gets the default constructor.
        /// </summary>
        /// <value>The default constructor.</value>
        public ConstructorInfo DefaultConstructor
        {
            get
            {
                return _DefaultConstructor;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ClassInfo(string type)
            : this(Type.GetType(type))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ClassInfo(Type type)
        {
            _Type = type;

            if (_Type == null)
            {
                throw new TypeLoadException(String.Format("Failed to load {0}.", type));
            }

            _DefaultConstructor = Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        public object CreateInstance()
        {
            return DefaultConstructor.Invoke(Type.EmptyTypes);
        }
    }
}
