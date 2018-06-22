using System;
using System.Configuration.Provider;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Data.Provider
{
    public abstract class DataProvider : ProviderBase
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public abstract string ApplicationName { get; set; }

        /// <summary>
        /// Loads the table from SP.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public abstract DataTable LoadTable(DataCommand command);
        public abstract DataResult LoadDataSet(DataCommand command);
        public abstract DataResult LoadReader(DataCommand command);
        public abstract DataTable LoadTableSchema(DataCommand command);

        /// <summary>
        /// Saves the data table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Returns either DataTable or DataRow[] depending on what was passed in the command</returns>
        public abstract DataResult Save(DataCommand command);

        /// <summary>
        /// Runs the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public abstract void Run(DataCommand command);

        /// <summary>
        /// Runs the return integer.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public abstract int RunReturnInteger(DataCommand command);

        public abstract DataResult ExecuteNonExec(DataCommand command);

        /// <summary>
        /// Runs the return integer.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public abstract DataResult ExecuteScalar(DataCommand command);

    }
}
