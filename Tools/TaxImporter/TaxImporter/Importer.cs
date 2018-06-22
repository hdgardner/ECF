using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace TaxImporter
{
    class Importer
    {
        public static DataSet ImportCSVFile(string path)
        {
            // TODO: method assumes path exists.
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);

            CreateIniFile(path);

            System.Data.Odbc.OdbcConnection odbcconn = null;
            DataSet taxds = new DataSet();
            System.Data.Odbc.OdbcDataAdapter odbcda;
            
            try
            {
                // string connectionString = "Driver={Microsoft Text Driver (*.txt;*.csv)};Dbq="
                // + directory.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";

                string connectionString = "Driver={Microsoft Text Driver (*.txt;*.csv)};DefaultDir="
                + directory.Trim() + "Extended properties=\"ColNameHeader=True;Format=CSVDelimited;\"";

                odbcconn = new System.Data.Odbc.OdbcConnection(connectionString.Trim());
                odbcconn.Open();
                
                //Fetch records from CSV
                string odbcsql = "select * from [" + filename + "]";
                odbcda = new System.Data.Odbc.OdbcDataAdapter(odbcsql, odbcconn);
                
                //Fill dataset with the records from CSV file
                odbcda.Fill(taxds, "Taxes");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                odbcconn.Close();
                DestroyIniFile(path);
            }
            return taxds;
        }

        /* 
            Schema.ini File (Text File Driver)
            When the Text driver is used, the format of the
            text file is determined by using a schema information
            file. The schema information file, which is always named
            Schema.ini and always kept in the same directory as the
            text data source, provides the IISAM with information
            about the general format of the file, the column name
            and data type information, and a number of other data 
            characteristics.
            http://www.codeproject.com/KB/database/FinalCSVReader.aspx
         */

        // pass the full path of the csv file to import to this method
        private static void CreateIniFile(string path)
        {

            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);

            TextWriter tw = new StreamWriter(Path.Combine(directory, "schema.ini"));

            try
            {
                tw.WriteLine("[" + filename + "]");
                tw.Write("ColNameHeader=true");         // column headers?
                tw.Write("Format=CSVDelimited");        // accomodate other formats?
                tw.Write("MaxScanRows=25");             // how high can you set this?
                tw.Write("CharacterSet=OEM");
                tw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tw.Close();
            }
        }

        private static void DestroyIniFile(string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (File.Exists(Path.Combine(directory, "schema.ini")))
            {
                File.Delete(Path.Combine(directory, "schema.ini"));
            }
        }
    }
}
