using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using Mediachase.MetaDataPlus.Configurator;
using System.Reflection;
using System.Data.Common;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Implements helper methods for the storage data.
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// Saves the table simple.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public static DataResult SaveTableSimple(DataCommand cmd, DataTable table)
        {
            cmd.Table = table;
            return DataService.Save(cmd);
        }

        /// <summary>
        /// Saves the table simple.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="table">The table.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public static DataResult SaveTableSimple(DataCommand cmd, DataTable table, DataRow[] rows)
        {
            cmd.Table = table;
            cmd.DataRows = rows;
            return DataService.Save(cmd);
        }

        /// <summary>
        /// Saves the table simple.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="table">The table.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static DataResult SaveTableSimple(MetaDataContext context, DataCommand cmd, DataTable table, DataViewRowState state)
        {
            cmd.Table = table;
            cmd.DataRows = table.Select("", "", state);

            // automatically handle deleting meta data related tables
            if (state == DataViewRowState.Deleted)
            {
                if (table.Columns.Contains("MetaClassId"))
                {
                    // Find primary key
                    string primaryKeyColumn = table.PrimaryKey[0].ColumnName;

                    if (cmd.DataRows.Length > 0)
                    {
                        foreach (DataRow row in cmd.DataRows)
                        {
                            MetaObject.Delete(context, Int32.Parse(row[primaryKeyColumn, DataRowVersion.Original].ToString()), Int32.Parse(row["MetaClassId", DataRowVersion.Original].ToString()));
                        }
                    }
                }
            }

            
            return DataService.Save(cmd);
        }

		/// <summary>
		/// Saves the table simple.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <param name="table">The table.</param>
		/// <param name="state">The state.</param>
		/// <returns></returns>
		public static DataResult SaveTableSimple(DataCommand cmd, DataTable table, DataViewRowState state)
		{
			cmd.Table = table;
			cmd.DataRows = table.Select("", "", state);
			return DataService.Save(cmd);
		}

        /// <summary>
        /// Saves the data set simple.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="set">The set.</param>
        /// <param name="state">The state.</param>
        public static void SaveDataSetSimple(MetaDataContext context, DataCommand cmd, DataSet set, DataViewRowState state)
        {
            foreach (DataTable table in set.Tables)
            {
                SaveTableSimple(context, cmd, table, state);
            }
        }

        /// <summary>
        /// Saves the data set simple.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="set">The set.</param>
        public static void SaveDataSetSimple(MetaDataContext context, DataCommand cmd, DataSet set)
        {
            SaveDataSetSimple(context, cmd, set, DataViewRowState.Added);
            SaveDataSetSimple(context, cmd, set, DataViewRowState.ModifiedCurrent);
            SaveDataSetSimple(context, cmd, set, DataViewRowState.Deleted);
        }

        /// <summary>
        /// Saves the data set simple.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="set">The set.</param>
        /// <param name="state">The state.</param>
        /// <param name="tables">The tables.</param>
        public static void SaveDataSetSimple(MetaDataContext context, DataCommand cmd, DataSet set, DataViewRowState state, params string[] tables)
        {
            foreach (string table in tables)
            {
                SaveTableSimple(context, cmd, set.Tables[table], state);
            }
        }

		/// <summary>
		/// Saves the data set simple.
		/// </summary>
		/// <param name="cmd">The cmd.</param>
		/// <param name="set">The set.</param>
		/// <param name="state">The state.</param>
		/// <param name="tables">The tables.</param>
		public static void SaveDataSetSimple(DataCommand cmd, DataSet set, DataViewRowState state, params string[] tables)
		{
			foreach (string table in tables)
			{
				SaveTableSimple(cmd, set.Tables[table], state);
			}
		}

		/// <summary>
		/// Saves the data set simple.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <param name="set">The set.</param>
		/// <param name="tables">The tables.</param>
		public static void SaveDataSetSimple(DataCommand cmd, DataSet set, params string[] tables)
		{
			SaveDataSetSimple(cmd, set, DataViewRowState.Added, tables);
			SaveDataSetSimple(cmd, set, DataViewRowState.ModifiedCurrent, tables);

			// do saving in reverse for deleted items
			SaveDataSetSimple(cmd, set, DataViewRowState.Deleted, ReverseStringArray(tables));
		}

        /// <summary>
        /// Saves the data set simple.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="set">The set.</param>
        /// <param name="tables">The tables.</param>
        public static void SaveDataSetSimple(MetaDataContext context, DataCommand cmd, DataSet set, params string[] tables)
        {
            SaveDataSetSimple(context, cmd, set, DataViewRowState.Added, tables);
            SaveDataSetSimple(context, cmd, set, DataViewRowState.ModifiedCurrent, tables);

            // do saving in reverse for deleted items
            SaveDataSetSimple(context, cmd, set, DataViewRowState.Deleted, ReverseStringArray(tables));
        }

        /// <summary>
        /// Reverses the string array.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <returns></returns>
        private static string[] ReverseStringArray(string[] tables)
        {
            List<string> tablesReversed = new List<string>();

            for (int index = tables.Length - 1; index >= 0; index--)
            {
                tablesReversed.Add(tables[index]);
            }

            return tablesReversed.ToArray();
        }

        /// <summary>
        /// Populates the params from columns.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="columns">The columns.</param>
        public static void PopulateParamsFromColumns(ref DataCommand command, DataColumnCollection columns)
        {
            foreach (DataColumn column in columns)
            {
                command.Parameters.Add(new DataParameter(column.ColumnName, DataParameterType.Unknown, column.MaxLength));
            }
        }

        /// <summary>
        /// Maps the tables.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public static DataTableMapping[] MapTables(params string[] names)
        {
            List<DataTableMapping> maps = new List<DataTableMapping>();
            int index = 0;
            foreach (string name in names)
            {
                DataTableMapping map = new DataTableMapping(String.Format("Table{0}", index == 0 ? String.Empty : index.ToString()), name);
                maps.Add(map);
                index++;
            }

            return maps.ToArray();
        }

		/// <summary>
		/// Maps the tables.
		/// </summary>
		/// <param name="dbNames">The names.</param>
		/// <param name="dataSetNames">The data set names.</param>
		/// <returns></returns>
		public static DataTableMapping[] MapTables2(string[] dbNames, string[] dataSetNames)
		{
			List<DataTableMapping> maps = new List<DataTableMapping>();

			if (dbNames != null && dataSetNames != null)
			{
				if (dbNames.Length != dataSetNames.Length)
					throw new Exception("Error in MapTables2 method: dbNames.Length!=dataSetNames.Length");

				for (int index = 0; index < dbNames.Length; index++)
				{
					DataTableMapping map = new DataTableMapping(dbNames[index], dataSetNames[index]);
					maps.Add(map);
				}
			}

			return maps.ToArray();
		}

        /// <summary>
        /// Creates the name of the insert update delete stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateInsertUpdateDeleteStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_InsertUpdateDelete", new object[] { TableName });
        }

        /// <summary>
        /// Creates the name of the insert stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateInsertStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Insert", new object[] { TableName });
        }

        /// <summary>
        /// Creates the name of the update stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateUpdateStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Update", new object[] { TableName });
        }

        /// <summary>
        /// Creates the name of the delete stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateDeleteStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Delete", new object[] { TableName });
        }

        /// <summary>
        /// Creates the data reader.
        /// </summary>
        /// <param name="sourceTable">The source table.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static IDataReader CreateDataReader(DataTable sourceTable, string filter)
        {
            // If table is empty, return empty data reader
            if(sourceTable == null)
                return new DataTable().CreateDataReader();

            DataView view = sourceTable.DefaultView;
            view.RowFilter = filter;

            if (view.Count == 0)
                return sourceTable.Clone().CreateDataReader();

            DataTableReader reader = view.ToTable().CreateDataReader();
            return reader;
        }

        /// <summary>
        /// Creates the data view.
        /// </summary>
        /// <param name="sourceTable">The source table.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static DataView CreateDataView(DataTable sourceTable, string filter)
        {
            // If table is empty, return empty data reader
            if (sourceTable == null)
                return new DataTable().DefaultView;

            DataView view = sourceTable.DefaultView;
            view.RowFilter = filter;

            return view;
        }
    }
}
