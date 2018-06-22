using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Mediachase.Data.Provider
{
    public class DataCommand
    {
        public string CommandText;
        public string ConnectionString;
        public DataParameters Parameters = new DataParameters();
        public DataTable Table;
        public DataSet DataSet;
        public DataRow[] DataRows;
        public DataTableMapping[] TableMapping;
        public CommandType CommandType = CommandType.StoredProcedure;
        public int CommandTimeout = -1;
    }
}
