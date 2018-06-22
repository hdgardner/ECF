using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Configuration.Provider;
using System.Configuration;
using System.Data.Common;

namespace Mediachase.Data.Provider
{
    public class SqlDataProvider : DataProvider
    {
        private string _ConnectionString;
        private string _ConnectionStringName;
        public string _ApplicationName;
        public override string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        public override void Initialize(string name,
            NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "SqlDataProvider";

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            _ApplicationName = config["applicationName"];

            if (string.IsNullOrEmpty(_ApplicationName))
                _ApplicationName = "/";

            config.Remove("applicationName");

            // Initialize _path
            _ConnectionStringName = config["connectionStringName"];

            if (String.IsNullOrEmpty(_ConnectionStringName))
                throw new ProviderException
                    ("Empty or missing connectionStringName");

            config.Remove("connectionStringName");

            ConnectionStringSettings settings = WebConfigurationManager.ConnectionStrings[_ConnectionStringName];
            if (settings != null)
                _ConnectionString = settings.ConnectionString;

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

        }

        public override DataResult Save(DataCommand command)
        {
            if (command.DataRows != null)
                return SaveInternal(command, command.DataRows);
            else
                return SaveInternal(command);
        }

        /// <summary>
        /// Returns DataTable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private DataResult SaveInternal(DataCommand command)
        {   
            bool inserts = false;
            bool updates = false;
            bool deletes = false;

            if (command.Table.Rows.Count > 0)
            {
                // Do we have any data to save
                foreach (DataRow row in command.Table.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            inserts = true;
                            break;
                        case DataRowState.Modified:
                            updates = true;
                            break;
                        case DataRowState.Deleted:
                            deletes = true;
                            break;
                    }
                }
            }

            // Use the command connection string if one specified, otherwise use the one configured for the provider
            string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;
            int rowsEffected = 0;
            using (TransactionScope tran = new TransactionScope())
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
				if (command.TableMapping != null && command.TableMapping.Length > 0)
				{
					if (inserts) dataAdapter.InsertCommand = command.Parameters.Count > 0 ? this.CreateInsertCommand(command) : this.CreateInsertCommand(command.Table, GetTableNameFromMapping(command.TableMapping, command.Table.TableName));
					if (updates) dataAdapter.UpdateCommand = command.Parameters.Count > 0 ? this.CreateUpdateCommand(command) : this.CreateUpdateCommand(command.Table, GetTableNameFromMapping(command.TableMapping, command.Table.TableName));
					if (deletes) dataAdapter.DeleteCommand = command.Parameters.Count > 0 ? this.CreateDeleteCommand(command) : this.CreateDeleteCommand(command.Table, GetTableNameFromMapping(command.TableMapping, command.Table.TableName));
				}
				else
				{
					if (inserts) dataAdapter.InsertCommand = command.Parameters.Count > 0 ? this.CreateInsertCommand(command) : this.CreateInsertCommand(command.Table);
					if (updates) dataAdapter.UpdateCommand = command.Parameters.Count > 0 ? this.CreateUpdateCommand(command) : this.CreateUpdateCommand(command.Table);
					if (deletes) dataAdapter.DeleteCommand = command.Parameters.Count > 0 ? this.CreateDeleteCommand(command) : this.CreateDeleteCommand(command.Table);
				}

                if (inserts) TransactionScope.Enlist(dataAdapter.InsertCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                if (updates) TransactionScope.Enlist(dataAdapter.UpdateCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                if (deletes) TransactionScope.Enlist(dataAdapter.DeleteCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));

                rowsEffected = dataAdapter.Update(command.Table);

                if (inserts) TransactionScope.DeEnlist(dataAdapter.InsertCommand);
                if (updates) TransactionScope.DeEnlist(dataAdapter.UpdateCommand);
                if (deletes) TransactionScope.DeEnlist(dataAdapter.DeleteCommand);

                tran.Complete();
            }

            DataResult result = new DataResult();
            result.Table = command.Table;
            result.RowsEffected = rowsEffected;

            return result;
        }

        /// <summary>
        /// Retiurns DataRow[]
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private DataResult SaveInternal(DataCommand command, DataRow[] rows)
        {
            bool inserts = false;
            bool updates = false;
            bool deletes = false;

            if (rows.Length > 0)
            {
                // Do we have any data to save
                foreach (DataRow row in rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            inserts = true;
                            break;
                        case DataRowState.Modified:
                            updates = true;
                            break;
                        case DataRowState.Deleted:
                            deletes = true;
                            break;
                    }
                }
            }

            // Use the command connection string if one specified, otherwise use the one configured for the provider
            string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;
            int rowsEffected = 0;
            using (TransactionScope tran = new TransactionScope())
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

				if (command.TableMapping != null && command.TableMapping.Length > 0)
				{
					if (inserts) dataAdapter.InsertCommand = command.Parameters.Count > 0 ? this.CreateInsertCommand(command) : this.CreateInsertCommand(command.Table, GetTableNameFromMapping(command.TableMapping, rows[0].Table.TableName));
					if (updates) dataAdapter.UpdateCommand = command.Parameters.Count > 0 ? this.CreateUpdateCommand(command) : this.CreateUpdateCommand(command.Table, GetTableNameFromMapping(command.TableMapping, rows[0].Table.TableName));
					if (deletes) dataAdapter.DeleteCommand = command.Parameters.Count > 0 ? this.CreateDeleteCommand(command) : this.CreateDeleteCommand(command.Table, GetTableNameFromMapping(command.TableMapping, rows[0].Table.TableName));
				}
				else
				{
					if (inserts) dataAdapter.InsertCommand = command.Parameters.Count > 0 ? this.CreateInsertCommand(command) : this.CreateInsertCommand(command.Table);
					if (updates) dataAdapter.UpdateCommand = command.Parameters.Count > 0 ? this.CreateUpdateCommand(command) : this.CreateUpdateCommand(command.Table);
					if (deletes) dataAdapter.DeleteCommand = command.Parameters.Count > 0 ? this.CreateDeleteCommand(command) : this.CreateDeleteCommand(command.Table);
				}

                if (inserts) TransactionScope.Enlist(dataAdapter.InsertCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                if (updates) TransactionScope.Enlist(dataAdapter.UpdateCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                if (deletes) TransactionScope.Enlist(dataAdapter.DeleteCommand, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));

                rowsEffected = dataAdapter.Update(rows);

                if (inserts) TransactionScope.DeEnlist(dataAdapter.InsertCommand);
                if (updates) TransactionScope.DeEnlist(dataAdapter.UpdateCommand);
                if (deletes) TransactionScope.DeEnlist(dataAdapter.DeleteCommand);

                tran.Complete();
            }

            DataResult result = new DataResult();
            result.DataRows = rows;
            result.RowsEffected = rowsEffected;

            return result;
        }

        public override DataTable LoadTable(DataCommand command)
        {
            DataTable dataTable = null;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (command.Table != null)
                    dataTable = command.Table;
                else
                    dataTable = new DataTable();

                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;
                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                dataAdapter.Fill(dataTable);
                TransactionScope.DeEnlist(cmd);

                if (command.Parameters != null)
                {
                    PopulateOutputParameters(cmd, command);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return dataTable;
        }
        
        public override DataResult LoadDataSet(DataCommand command)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if(command.DataSet == null)
                    command.DataSet = new DataSet();

                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;

                // Map tables
                if (command.TableMapping != null)
                    dataAdapter.TableMappings.AddRange(command.TableMapping);

                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                dataAdapter.Fill(command.DataSet);
                TransactionScope.DeEnlist(cmd);
                if (command.Parameters != null)
                {
                    PopulateOutputParameters(cmd, command);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            DataResult result = new DataResult();
            result.DataSet = command.DataSet;
            return result;
        }

        public override DataTable LoadTableSchema(DataCommand command)
        {
            DataTable dataTable = null;
            SqlCommand cmd = new SqlCommand();
            try
            {
                dataTable = new DataTable();
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                dataTable = reader.GetSchemaTable();
                TransactionScope.DeEnlist(cmd);
                if (command.Parameters != null)
                {
                    PopulateOutputParameters(cmd, command);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }
            finally
            {
            }

            return dataTable;
        }

        public override DataResult LoadReader(DataCommand command)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;
                bool opened = TransactionScope.OpenConnection(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));

                if (opened)
                    reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                else
                    reader = cmd.ExecuteReader();


                //TransactionScope.DeEnlist(cmd);
                if (command.Parameters != null)
                {
                    PopulateOutputParameters(cmd, command);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }
            finally
            {
            }

            DataResult result = new DataResult();
            result.DataReader = reader;
            return result;
        }

        public override void Run(DataCommand command)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                cmd.ExecuteNonQuery();
                TransactionScope.DeEnlist(cmd);
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }
        }

        public override DataResult ExecuteScalar(DataCommand command)
        {
            DataResult result = new DataResult();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;

                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                /*
                cmd.Parameters.Add(new SqlParameter("@retval", SqlDbType.Int, 4));
                cmd.Parameters["@retval"].Direction = ParameterDirection.Output;
                 * */

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;


                bool opened = TransactionScope.OpenConnection(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                result.Scalar = cmd.ExecuteScalar();

                /*
                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                result.Scalar = cmd.ExecuteScalar();
                TransactionScope.DeEnlist(cmd);
                 * */
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return result;
        }

        public override DataResult ExecuteNonExec(DataCommand command)
        {
            DataResult result = new DataResult();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;
                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;

                if (command.Parameters != null)
                {
                    AddParameters(cmd, command);
                }

                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

                bool opened = TransactionScope.OpenConnection(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                result.RowsEffected = cmd.ExecuteNonQuery();

                if (opened) // new connection has been opened and it is not transactional, so close it
                    cmd.Connection.Close();
                /*
                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                result.RowsEffected = cmd.ExecuteNonQuery();
                TransactionScope.DeEnlist(cmd);
                 * */
                if (command.Parameters != null)
                {
                    PopulateReturnParameters(cmd, command, result);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return result;
        }

        public override int RunReturnInteger(DataCommand command)
        {
            int retVal;
            SqlCommand cmd = new SqlCommand();
            try
            {
                // Use the command connection string if one specified, otherwise use the one configured for the provider
                string connectionString = CreateLoadCommand(cmd, command);

                cmd.Parameters.Add(new SqlParameter("@retval", SqlDbType.Int, 4));
                cmd.Parameters["@retval"].Direction = ParameterDirection.Output;

                TransactionScope.Enlist(cmd, connectionString, new TransactionScope.ConnectionDelegate(CreateConnectionDelegate));
                cmd.ExecuteNonQuery();
                TransactionScope.DeEnlist(cmd);
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            if (cmd.Parameters["@retval"].Value != DBNull.Value)
                retVal = (int)cmd.Parameters["@retval"].Value;
            else
                retVal = -1;

            return retVal;
        }

        protected string CreateLoadCommand(SqlCommand cmd, DataCommand command)
        {
            if (command.CommandType == CommandType.TableDirect)
            {
                cmd.CommandText = String.Format("SELECT * from [{0}]", command.CommandText);
                cmd.CommandType = CommandType.Text;
                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;
            }
            else
            {
                cmd.CommandText = command.CommandText;
                cmd.CommandType = command.CommandType;
                if (command.CommandTimeout >= 0)
                    cmd.CommandTimeout = command.CommandTimeout;
            }

            if (command.Parameters != null)
            {
                AddParameters(cmd, command);
            }

            // Use the command connection string if one specified, otherwise use the one configured for the provider
            string connectionString = String.IsNullOrEmpty(command.ConnectionString) ? this._ConnectionString : command.ConnectionString;

            return connectionString;
        }

        private static void PopulateReturnParameters(SqlCommand cmd, DataCommand command, DataResult result)
        {
            if (((cmd.Parameters.Count > 0) && (command.Parameters != null)) && (command.Parameters.Count > 0))
            {
                result.Parameters = new DataParameters();
                int index = 0;
                foreach (DataParameter param in command.Parameters)
                {
                    if (param.Direction != ParameterDirection.Input)
                    {
                        result.Parameters.Add(param);
                        SqlParameter p = null;
                        if(cmd.CommandType == CommandType.StoredProcedure)
                            p = cmd.Parameters["@" + param.ParamName];
                        else
                            p = cmd.Parameters["@" + "p" + index.ToString()];

                        param.Value = p.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Copies output parameters' values from SqlCommand to DataCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="command"></param>
        private static void PopulateOutputParameters(SqlCommand cmd, DataCommand command)
        {
            if (((cmd.Parameters.Count > 0) && (command.Parameters != null)) && (command.Parameters.Count > 0))
            {
                int index = 0;
                foreach (DataParameter param in command.Parameters)
                {
                    if (param.Direction != ParameterDirection.Input)
                    {
                        SqlParameter p = null;
                        if (cmd.CommandType == CommandType.StoredProcedure)
                            p = cmd.Parameters["@" + param.ParamName];
                        else
                            p = cmd.Parameters["@" + "p" + index.ToString()];

                        param.Value = p.Value;
                    }
                }
            }
        }

        private void CleanupCommand(SqlCommand cmd)
        {
            if (((cmd != null) && (cmd.Connection != null)) && (cmd.Connection.State == ConnectionState.Open))
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Copies parameters from DataCommand to SqlCommand
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmd"></param>
        private static void AddParameters(SqlCommand sql, DataCommand cmd)
        {
            if (((cmd.CommandType == CommandType.Text) && (cmd.CommandText != null)) && cmd.CommandText.Contains("{0}"))
            {
                int index = 0;
                foreach (DataParameter param in cmd.Parameters)
                {
                    sql.CommandText = sql.CommandText.Replace('{' + index.ToString() + '}', "@" + "p" + index.ToString());
                    sql.Parameters.AddWithValue("@" + "p" + index.ToString(), GetSafeValue(param));
                    index++;
                }
            }
            else
            {
                foreach (DataParameter param in cmd.Parameters)
                {
                    SqlParameter newparam = sql.Parameters.AddWithValue("@" + param.ParamName, GetSafeValue(param));
                    newparam.Direction = param.Direction;
                    newparam.SourceColumn = param.ParamName;
                    newparam.SourceVersion = DataRowVersion.Current;

                    // If parameter unknown, let sql server provider figure out correct type
                    if (param.ProviderType != DataParameterType.Unknown)
                        newparam.SqlDbType = (SqlDbType)param.ProviderType.GetHashCode();

                    if (param.Size != 0)
                        newparam.Size = param.Size;
                }
            }
        }

        private static object GetSafeValue(DataParameter param)
        {
            DateTime dt = new DateTime(1753, 1, 1);
            if ((param.ProviderType == DataParameterType.DateTime || param.ProviderType == DataParameterType.SmallDateTime) && param.Value != null && param.Value.GetType() == typeof(DateTime) && (DateTime)param.Value < dt)
                return dt;
			// return value if it's not null. Otherwise return DBNull.Value.
			return param.Value != null ? param.Value : DBNull.Value;
        }

        private static IDbConnection CreateConnectionDelegate()
        {
            return new SqlConnection();
        }

        /// <summary>
        /// Creates the insert command for the specified datatable
        /// </summary>
        /// <param name="dataTable">The data table for which the insert command will be created.</param>
        private SqlCommand CreateInsertCommand(DataTable dataTable)
        {
			return CreateInsertCommand(dataTable, null);
        }

		/// <summary>
		/// Creates the insert command for the specified datatable
		/// </summary>
		/// <param name="dataTable">The data table for which the insert command will be created.</param>
		/// <param name="dbTableName">Name of the corresponding table in the db.</param>
		private SqlCommand CreateInsertCommand(DataTable dataTable, string dbTableName)
		{
			StringBuilder sbColumns = new StringBuilder();
			StringBuilder sbValues = new StringBuilder();

			string autoIncrementColumn = string.Empty;

			SqlCommand newCommand = new SqlCommand();

			newCommand.CommandType = CommandType.Text;
			newCommand.CommandText = string.Empty;

			foreach (DataColumn column in dataTable.Columns)
			{
				// IF marked not to persist, skip it
				if (column.ExtendedProperties.Contains("Persist"))
				{
					if (!Boolean.Parse(column.ExtendedProperties["Persist"].ToString()))
						continue;
				}

				// Only add the column to the statement when column is not an identify column
				// or when we need to allow an identity insert.
				if (!column.AutoIncrement)
				{
					if (sbColumns.Length > 0)
					{
						sbColumns.Append(",");
						sbValues.Append(",");
					}
					sbColumns.Append(SqlName(column.ColumnName));

					sbValues.Append("@");
					sbValues.Append(column.ColumnName);

					newCommand.Parameters.Add(CreateParam(dataTable, column.ColumnName));
				}
				else
				{
					autoIncrementColumn = column.ColumnName;
				}
			}

			StringBuilder insertText = new StringBuilder();

			insertText.Append("INSERT INTO ");

			insertText.Append(!String.IsNullOrEmpty(dbTableName) ? SqlName(dbTableName) : SqlName(dataTable.TableName));
			insertText.Append(" (");
			insertText.Append(sbColumns.ToString());
			insertText.Append(") VALUES (");
			insertText.Append(sbValues.ToString());
			insertText.Append("); ");

			// When an autoidentify column has been found, we need to get the scope_identity 
			// value for that column
			if (autoIncrementColumn.Length > 0)
			{
				insertText.AppendFormat("SELECT SCOPE_IDENTITY() AS {0}", SqlName(autoIncrementColumn));
			}

			newCommand.CommandText = insertText.ToString();

			return newCommand;
		}

        /// <summary>
        /// Creates the update command for the specified datatable.
        /// </summary>
        /// <param name="dataTable">The data table on which an update command will be created.</param>
        private SqlCommand CreateUpdateCommand(DataTable dataTable)
        {
			return CreateUpdateCommand(dataTable, null);
        }

		/// <summary>
		/// Creates the update command for the specified datatable.
		/// </summary>
		/// <param name="dataTable">The data table on which an update command will be created.</param>
		/// <param name="dbTableName">Name of the corresponding table in the db.</param>
		private SqlCommand CreateUpdateCommand(DataTable dataTable, string dbTableName)
		{
			StringBuilder sbSet = new StringBuilder();

			SqlCommand newCommand = new SqlCommand();

			newCommand.CommandType = CommandType.Text;
			newCommand.CommandText = string.Empty;


			// First build a where clause for the primary key
			string whereClause = GetWhereClause(ref newCommand, dataTable);

			foreach (DataColumn column in dataTable.Columns)
			{
				// IF marked not to persist, skip it
				if (column.ExtendedProperties.Contains("Persist"))
				{
					if (!Boolean.Parse(column.ExtendedProperties["Persist"].ToString()))
						continue;
				}

				// Only columns that are not an AutoincrementKeyColumn can 
				// be used in the set statement
				if (!column.AutoIncrement)
				{
					// Check to see if the parameter already exists in the commandobject
					if (!newCommand.Parameters.Contains("@" + column.ColumnName))
					{
						newCommand.Parameters.Add(CreateParam(dataTable, column.ColumnName));
					}

					if (sbSet.Length > 0)
					{
						sbSet.Append(",");
					}
					sbSet.Append(SqlName(column.ColumnName));
					sbSet.Append("=@");
					sbSet.Append(column.ColumnName);
				}
			}
			StringBuilder updateText = new StringBuilder("UPDATE ");
			updateText.Append(!String.IsNullOrEmpty(dbTableName) ? SqlName(dbTableName) : SqlName(dataTable.TableName));
			updateText.Append(" SET ");
			updateText.Append(sbSet.ToString());
			updateText.Append(" WHERE ");
			updateText.Append(whereClause);

			newCommand.CommandText = updateText.ToString();

			return newCommand;
		}

        /// <summary>
        /// Creates the delete command for the specified table
        /// </summary>
        /// <param name="dataTable">The data table for which the command object will be created.</param>
        private SqlCommand CreateDeleteCommand(DataTable dataTable)
        {
			return CreateDeleteCommand(dataTable, null);
        }

		/// <summary>
		/// Creates the delete command for the specified table.
		/// </summary>
		/// <param name="dataTable">The data table for which the command object will be created.</param>
		/// <param name="dbTableName">Name of the corresponding table in the db.</param>
		private SqlCommand CreateDeleteCommand(DataTable dataTable, string dbTableName)
		{
			SqlCommand newCommand = new SqlCommand();

			newCommand.CommandType = CommandType.Text;
			newCommand.CommandText = string.Empty;

			string whereClause = GetWhereClause(ref newCommand, dataTable);

			StringBuilder deleteText = new StringBuilder("DELETE FROM ");
			deleteText.Append(!String.IsNullOrEmpty(dbTableName) ? SqlName(dbTableName) : SqlName(dataTable.TableName));
			deleteText.Append(" WHERE ");
			deleteText.Append(whereClause);

			newCommand.CommandText = deleteText.ToString();
			return newCommand;
		}

		/// <summary>
		/// Returns table name from mapping
		/// </summary>
		/// <returns></returns>
		private string GetTableNameFromMapping(DataTableMapping[] mapping, string dataSetTableName)
		{
			string name = dataSetTableName;
			if (mapping != null)
			{
				for (int i = 0; i < mapping.Length; i++)
				{
					if (String.Compare(mapping[i].DataSetTable, dataSetTableName, true) == 0)
					{
						name = mapping[i].SourceTable;
						break;
					}
				}
			}
			return name;
		}

        /// <summary>
        /// Create a SQL parameter for a database column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>SQL parameter.</returns>
        private SqlParameter CreateParam(DataTable table, string columnName)
        {
            SqlParameter sqlParameter = new SqlParameter();

            sqlParameter.ParameterName = "@" + columnName;
            sqlParameter.SourceColumn = columnName;

            DataColumn detail = table.Columns[columnName];

            sqlParameter.Size = detail.MaxLength;
            sqlParameter.SqlDbType = GetDBType(detail.DataType);
            //if (detail.Scale != 255) sqlParameter.Scale = detail.Scale;
            //if (detail.Precision != 255) sqlParameter.Precision = detail.Precision;

            return sqlParameter;
        }

        private SqlDbType GetDBType(System.Type theType)
        {
            if (typeof(byte[]) == theType)
                return SqlDbType.Image;

            SqlParameter param;
            System.ComponentModel.TypeConverter tc;
            param = new SqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(param.DbType);
            if (tc.CanConvertFrom(theType))
            {
                param.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            else
            {
                // try to forcefully convert
                try
                {
                    param.DbType = (DbType)tc.ConvertFrom(theType.Name);
                }
                catch (Exception e)
                {
                    // ignore the exception
                }
            }
            return param.SqlDbType;
        }

        /// <summary>
        /// Build a where clause for the update and delete commands and modifies the command objects. When the table
        /// has a primary key, only those columns are added to the 
        /// </summary>
        /// <param name="command">Current command object for which a where clause needs to be defined</param>
        /// <param name="dataTable">Table associated with the current command object</param>
        /// <returns></returns>
        private string GetWhereClause(ref SqlCommand command, DataTable dataTable)
        {
            StringBuilder whereClause = new StringBuilder();

            if (dataTable.PrimaryKey.Length > 0)
            {
                foreach (DataColumn column in dataTable.PrimaryKey)
                {
                    AddColumn(dataTable, ref command, ref whereClause, column);
                }
            }
            else
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    // IF marked not to persist, skip it
                    if (column.ExtendedProperties.Contains("Persist"))
                    {
                        if (!Boolean.Parse(column.ExtendedProperties["Persist"].ToString()))
                            continue;
                    }

                    AddColumn(dataTable, ref command, ref whereClause, column);
                }
            }

            return whereClause.ToString();
        }

        /// <summary>
        /// Add a new column and accompanying columns to the where clause and command object
        /// </summary>
        /// <param name="command">Current command object for which a where clause needs to be defined</param>
        /// <param name="whereClause">Reference to StringBuilder object that will contain the where clause</param>
        /// <param name="column">DataColumn that needs to be added</param>
        private void AddColumn(DataTable table, ref SqlCommand command, ref StringBuilder whereClause, DataColumn column)
        {
            if (whereClause.Length > 0)
            {
                whereClause.Append(" AND ");
            }

            whereClause.Append("((");
            whereClause.Append(SqlName(column.ColumnName));
            whereClause.Append(" IS NULL AND @old");
            whereClause.Append(column.ColumnName);
            whereClause.Append(" IS NULL) OR ");

            whereClause.Append(SqlName(column.ColumnName));
            whereClause.Append("=@old");
            whereClause.Append(column.ColumnName);
            whereClause.Append(")");

            command.Parameters.Add(CreateParam(table, column.ColumnName));
            command.Parameters.Add(CreateOldParam(table, column.ColumnName));
        }

        /// <summary>
        /// Creates the old parameter for database columns on tables without a primary key.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>SQL paramete.r</returns>
        private SqlParameter CreateOldParam(DataTable table, string columnName)
        {
            SqlParameter sqlParameter = CreateParam(table, columnName);

            sqlParameter.ParameterName = "@old" + columnName;
            sqlParameter.SourceVersion = DataRowVersion.Original;

            return sqlParameter;
        }

        /// <summary>
        /// Adds square brackets to the specified name.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <returns>Modified table name.</returns>
        private static string SqlName(string name)
        {
            if (name.StartsWith("["))
            {
                return name;
            }
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"[{0}]", name);
        }
        
        #region CRUD Updates using paramters
		protected SqlCommand CreateInsertCommand(DataCommand command)
		{
			SqlCommand cmd = CreateParameters(command);
			cmd.CommandText = command.CommandText;
			return cmd;
		}

        protected SqlCommand CreateUpdateCommand(DataCommand command)
        {
            SqlCommand cmd = CreateParameters(command);
            cmd.CommandText = command.CommandText;
            return cmd;
        }

        protected SqlCommand CreateDeleteCommand(DataCommand command)
        {
            SqlCommand cmd = CreateParameters(command);
            cmd.CommandText = command.CommandText;
            return cmd;
        }

        protected SqlCommand CreateParameters(DataCommand command)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = command.CommandType;
            if (command.CommandTimeout >= 0)
                cmd.CommandTimeout = command.CommandTimeout;
            AddParameters(cmd, command);
            return cmd;
        }
        #endregion
    }
}
