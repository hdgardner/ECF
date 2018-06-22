using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Mediachase.Data.Provider
{
    public class DataResult
    {
        // Methods
        public DataResult() { }

        // Fields
        public IDataReader DataReader;
        public DataParameters Parameters;
        public int RowsEffected;
        public object Scalar;
        public DataTable Table;
        public DataSet DataSet;
        public DataRow[] DataRows;
    }
}
