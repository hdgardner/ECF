using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Mediachase.Data.Provider
{
    [Serializable]
    public enum DataParameterType
    {
		// SQL Data Types [April 24, 2008]
        Unknown         = -1,
        BigInt = 0,
        Binary = 1,
        Bit = 2,
        Char = 3,
        Date = 0x1f,
        DateTime = 4,
        DateTime2 = 0x21,
        DateTimeOffset = 0x22,
        Decimal = 5,
        Float = 6,
        Image = 7,
        Int = 8,
        Money = 9,
        NChar = 10,
        NText = 11,
        NVarChar = 12,
        Real = 13,
        SmallDateTime = 15,
        SmallInt = 0x10,
        SmallMoney = 0x11,
        Sysname = 30,
        Text = 0x12,
        Time = 0x20,
        Timestamp = 0x13,
        TinyInt = 20,
        Udt = 0x1d,
        UniqueIdentifier = 14,
        VarBinary = 0x15,
        VarChar = 0x16,
        Variant = 0x17,
        Xml = 0x19
    }
 
    public class DataParameter : IComparable
    {
        private string name;
        private object value;
        private ParameterDirection direction = ParameterDirection.Input;
        private DataParameterType providerType = DataParameterType.Unknown;
        private string columnName;
        private int size;

        public DataParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public DataParameter(string name, DataParameterType providerType)
        {
            this.name = name;
            this.providerType = providerType;
        }

        public DataParameter(string name, object value, DataParameterType providerType)
        {
            this.name = name;
            this.providerType = providerType;
            this.value = value;
        }

        public DataParameter(string name, DataParameterType providerType, int size)
        {
            this.name = name;
            this.providerType = providerType;
            this.size = size;
        }

        public DataParameter(string name, object value, DataParameterType providerType, int size)
        {
            this.name = name;
            this.value = value;
            this.providerType = providerType;
            this.size = size;
        }

        // Properties
        public string ParamName
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
            set
            {
                this.columnName = value;
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        public DataParameterType ProviderType
        {
            get
            {
                return this.providerType;
            }
            set
            {
                this.providerType = value;
            }
        }

        public override string ToString()
        {
            return name;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        #endregion
    }
}
