using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Data.Provider
{
    public class DataParameters : IEnumerable
    {
        private List<DataParameter> parameters;

        public DataParameters()
        {
            this.parameters = new List<DataParameter>();
        }
        
        public void Add(DataParameter param)
        {
            this.parameters.Add(param);
        }

        public void Add(DataParameter[] parameters)
        {
            foreach (DataParameter param in parameters)
                this.parameters.Add(param);
        }

        public void Insert(int index, DataParameter param)
        {
            this.parameters.Insert(index, param);
        }

        public void RemoveAt(int index)
        {
            this.parameters.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.parameters.RemoveRange(index, count);
        }

        public void Remove(DataParameter item)
        {
            this.parameters.Remove(item);
        }

        public void Sort()
        {
            this.parameters.Sort();
        }

        public void Sort(Comparison<DataParameter> comparison)
        {
            this.parameters.Sort(comparison);
        }

        public void Sort(IComparer<DataParameter> comparer)
        {
            this.parameters.Sort(comparer);
        }

        public void Clear()
        {
            this.parameters.Clear();
        }

        public int Count
        {
            get
            {
                return this.parameters.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        public DataParameter this[int index]
        {
            get
            {
                return parameters[index];
            }
        }

        public DataParameter this[string name]
        {
            get
            {
                foreach (DataParameter param in parameters)
                {
                    if (param.ParamName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return param;
                }
                return null;
            }
        }
    }
}
