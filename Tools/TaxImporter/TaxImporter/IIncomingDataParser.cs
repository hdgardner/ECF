using System;
using System.IO;
using System.Data;

namespace Mediachase.MetaDataPlus.Import
{
    /// <summary>
    /// Summary description for IIncomingDataParser.
    /// </summary>
    public interface IIncomingDataParser
    {
        string Name { get; }
        string Description { get; }

        DataSet Parse(string fileName, Stream stream);

        bool CanParse(string fileName, Stream stream);
    }
}