using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using System.Xml.Serialization;

namespace Mediachase.Search
{
    public class IndexBuilder : IDisposable
    {
        private Directory _indexDirectory;
        private IndexWriter _indexWriter;
        private bool _isClosed = false;
        private bool _rebuildIndex;
        private string _indexerName;
        private string _directoryPath;
        private Build _BuildConfig = null;

        /// <summary>
        /// Gets a value indicating whether [rebuild index].
        /// </summary>
        /// <value><c>true</c> if [rebuild index]; otherwise, <c>false</c>.</value>
        public bool RebuildIndex
        {
            get
            {
                return _rebuildIndex;
            }
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="physicalIndexDir">Location of the index files.</param>
        /// <param name="rebuildIndex">Flag to indicate if the index should be rebuilt.
        /// NOTE: you can not update or delete content when rebuilding the index.</param>
        public IndexBuilder(string physicalIndexDir, bool rebuildIndex, string indexerName)
        {
            this._indexDirectory = FSDirectory.GetDirectory(physicalIndexDir, false);
            _directoryPath = physicalIndexDir;
            _indexerName = indexerName;
            if (GetLastBuildDate() == DateTime.MinValue)
                rebuildIndex = true;
            this._rebuildIndex = rebuildIndex;

            InitIndexWriter();
        }

        /// <summary>
        /// Adds the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public void AddDocument(Document doc)
        {
            if (this._indexWriter == null)
            {
                InitIndexWriter();
            }
            this._indexWriter.AddDocument(doc);
        }

        /// <summary>
        /// Delete existing content from the search index.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		public int DeleteContent(string field, string value)
		{
            int num = 0;
			if (this._rebuildIndex)
			{
				throw new InvalidOperationException("Cannot delete documents when rebuilding the index.");
			}
			else
			{
				this._indexWriter.Close();
				this._indexWriter = null;

				Term term = new Term(field, value);
				IndexReader rdr = IndexReader.Open(this._indexDirectory);
                num = Delete(rdr, term);
				rdr.Close();
			}

            return num;
		}

        /// <summary>
        /// Deletes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public int Delete(IndexReader reader, Term term)
        {
            TermDocs docs = reader.TermDocs(term);
            if (docs == null)
            {
                return 0;
            }
            int num = 0;
            try
            {
                while (docs.Next())
                {
                    reader.DeleteDocument(docs.Doc());
                    num++;
                }
            }
            finally
            {
                docs.Close();
            }
            return num;
        }

        /// <summary>
        /// Gets the last build date.
        /// </summary>
        /// <returns></returns>
        private DateTime GetLastBuildDate()
        {
            Build config = GetBuildConfig();

            if (config != null)
                return config.LastBuildDate;
            /*
            if (this._indexDirectory.FileExists(this._indexerName + ".build"))
            {
                return new DateTime(this._indexDirectory.FileModified(this._indexerName + ".build"));
            }
             * */

            return DateTime.MinValue;
        }

        /// <summary>
        /// Gets the build config.
        /// </summary>
        /// <returns></returns>
        public Build GetBuildConfig()
        {
            if (_BuildConfig != null)
                return _BuildConfig;

            if (!this._indexDirectory.FileExists(this._indexerName + ".build"))
            {
                _BuildConfig = new Build();
                _BuildConfig.Status = Status.NeverStarted;
                return _BuildConfig;
            }

            //string input = this._indexDirectory.OpenInput(this._indexerName + ".build").ReadString();
            System.IO.TextReader reader = new System.IO.StreamReader(this._directoryPath + "//" + this._indexerName + ".build");
            string input = reader.ReadToEnd();
            reader.Close();

            if (String.IsNullOrEmpty(input))
            {
                _BuildConfig = new Build();
                _BuildConfig.Status = Status.NeverStarted;
                return _BuildConfig;
            }

            System.IO.StringReader sreader = new System.IO.StringReader(input);
            XmlSerializer serializer = new XmlSerializer(typeof(Build));
            Build config = (Build)serializer.Deserialize(sreader);
            sreader.Close();

            _BuildConfig = config;
            return _BuildConfig;
        }

        /// <summary>
        /// Saves the build.
        /// </summary>
        public void SaveBuild(Status status)
        {
            Build config = GetBuildConfig();

            if (config == null)
            {
                config = new Build();
            }

            if (status == Status.Completed)
                config.LastBuildDate = DateTime.Now;

            config.Status = status;

            XmlSerializer serializer = new XmlSerializer(typeof(Build));
            System.IO.StringWriter writer = new System.IO.StringWriter();
            System.IO.FileStream stream = System.IO.File.Create(this._directoryPath + "//" + this._indexerName + ".build");
            serializer.Serialize(stream, config);
            stream.Close();

            /*
            IndexOutput output = this._indexDirectory.CreateOutput(this._indexerName + ".build");
            output.WriteString(writer.ToString());
            output.Close();
             * */

            /*
            if (!this._indexDirectory.FileExists(this._indexerName + ".build"))
            {
                this._indexDirectory.CreateOutput(this._indexerName + ".build").Close();
            }
            else
            {
                this._indexDirectory.TouchFile(this._indexerName + ".build");
            }
             * */
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Close(false);
        }

        /// <summary>
        /// Close the IndexWriter (saves the index).
        /// </summary>
        internal void Close(bool optimize)
        {
            if (!this._isClosed && this._indexWriter != null)
            {
                if (optimize)
                    this._indexWriter.Optimize();

                this._indexWriter.Close();
                this._isClosed = true;
                this._indexWriter = null;
                this._rebuildIndex = false;
            }
        }

        /// <summary>
        /// Inits the index writer.
        /// </summary>
        private void InitIndexWriter()
        {
            if (!IndexReader.IndexExists(this._indexDirectory) || this._rebuildIndex)
            {
                this._indexWriter = new IndexWriter(this._indexDirectory, new StandardAnalyzer(), true);
            }
            else
            {
                this._indexWriter = new IndexWriter(this._indexDirectory, new StandardAnalyzer(), false);
            }

            this._isClosed = false;
        }
		
        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
