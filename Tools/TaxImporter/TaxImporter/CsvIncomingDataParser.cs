using System;
using System.Data;
using System.IO;
using System.Text;

namespace Mediachase.MetaDataPlus.Import.Parser
{
    /// <summary>
    /// Summary description for CsvIncomingDataParser.
    /// </summary>
    public class CsvIncomingDataParser : IIncomingDataParser
    {
        public class CSVException : Exception
        {
            public CSVException(string message)
                : base(message)
            {
            }
        }

        protected string _SourceFolder = null;
        protected bool _withHeader = true;
        protected bool _MultiLines = true;
        protected char _delimiter = ';';
        protected char _textQualifier = '\0';

        protected Encoding _encoding = System.Text.Encoding.GetEncoding(1251);

        public string Name
        {
            get
            {
                return "CsvIncomingDataParser";
            }
        }

        public string Description
        {
            get
            {
                return "CsvIncomingDataParser";
            }
        }

        protected void FillTableMetaData(DataTable table, string line)
        {
            string[] columns = line.Split(new char[] { _delimiter });

            for (int index = 0; index < columns.Length; index++)
            {
                if (_withHeader)
                {
                    if (String.Empty.CompareTo(columns[index]) == 0)
                    {
                        int i = index;
                        do
                        {
                            columns[index] = "F" + i.ToString();
                        }
                        while (table.Columns.IndexOf(columns[index]) != -1);
                    }
                    table.Columns.Add(columns[index]);
                }
                else table.Columns.Add("F" + index.ToString());
            }
        }

        protected void FillTable(string FullPath, DataTable table)
        {
            using (StreamReader reader = new StreamReader(FullPath, _encoding))
            {
                bool first = true;

                while (reader.Peek() >= 0)
                {
                    string s = reader.ReadLine();
                    string value;
                    int col_begin = 0, col_end = 0;

                    if (first)
                    {
                        FillTableMetaData(table, s);
                        first = false;
                        if (_withHeader) continue;
                    }
                    DataRow row = table.NewRow();

                    for (int index = 0; index < table.Columns.Count && col_end != -1; index++)
                    {
                        while (s.Length > col_begin && s[col_begin] == ' ')
                        {
                            col_begin++;
                        }
                        value = "";
                        if (_textQualifier != '\0' && s.Length > col_begin && s[col_begin] == _textQualifier)
                        {
                            do
                            {
                                col_begin++;
                                col_end = s.IndexOf(_textQualifier, col_begin);
                                while (col_end == -1 && _MultiLines)
                                {
                                    value += s.Substring(col_begin) + "\n";
                                    if (reader.Peek() == 0) throw new CSVException("Invalid file structure");
                                    s = reader.ReadLine();
                                    col_begin = 0;
                                    col_end = s.IndexOf(_textQualifier, col_begin);
                                }
                                if (col_end == -1) throw new CSVException("Invalid file structure");

                                value += s.Substring(col_begin, col_end - col_begin + 1);
                                col_begin = col_end + 1;
                            }
                            while (s.Length > col_begin && s[col_begin] == _textQualifier);
                            value = value.Substring(0, value.Length - 1);
                        }
                        col_end = s.IndexOf(_delimiter, col_begin);

                        if (col_end != -1)
                        {
                            value += s.Substring(col_begin, col_end - col_begin).TrimEnd(null);
                            col_begin = col_end + 1;
                        }
                        else value += s.Substring(col_begin).TrimEnd(null);
                        row[index] = value;
                    }
                    table.Rows.Add(row);
                }
            }
        }

        public virtual DataSet Parse(string FileName, Stream stream)
        {
            if (stream != null)
            {
                throw new NotSupportedException();
            }
            DataSet ds = new DataSet();
            /*
                        OleDbConnection cnn = new OleDbConnection(_connectionString);
                        cnn.Open();
                        try
                        {
            */
            DataTable table = new DataTable();

            FillTable(Path.Combine(_SourceFolder, FileName), table);
            /*
                            OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + FileName + "]", cnn);
                            da.Fill(table);
            */
            table.TableName = FileName;

            ds.Tables.Add(table);
            /*
                        }
                        finally
                        {
                            cnn.Close();
                        }
            */
            return ds;
        }

        public virtual bool CanParse(string FileName, Stream stream)
        {
            if (stream != null)
            {
                return false;
            }
            try
            {
                Parse(FileName, stream);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CsvIncomingDataParser(string SourceFolder, bool withHeader, char Delimiter, char textQualifier, bool MultiLines, Encoding textEncoding)
        {
            _SourceFolder = SourceFolder;
            _withHeader = withHeader;
            _delimiter = Delimiter;
            _textQualifier = textQualifier;
            _encoding = textEncoding;
            _MultiLines = MultiLines;
        }

        public CsvIncomingDataParser(string SourceFolder, bool withHeader, char Delimiter, char textQualifier, bool MultiLines)
        {
            _SourceFolder = SourceFolder;
            _withHeader = withHeader;
            _delimiter = Delimiter;
            _textQualifier = textQualifier;
            _MultiLines = MultiLines;
        }

        public CsvIncomingDataParser(string SourceFolder, bool withHeader, char Delimiter, char textQualifier)
        {
            _SourceFolder = SourceFolder;
            _withHeader = withHeader;
            _delimiter = Delimiter;
            _textQualifier = textQualifier;
        }

        public CsvIncomingDataParser(string SourceFolder, bool withHeader, char Delimiter)
        {
            _SourceFolder = SourceFolder;
            _withHeader = withHeader;
            _delimiter = Delimiter;
        }
    }
}
