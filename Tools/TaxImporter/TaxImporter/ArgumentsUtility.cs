using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TaxImporter
{
    // SEE: http://www.codeproject.com/KB/recipes/command_line.aspx

    class ArgumentsUtility
    {
        // Variables
        private StringDictionary _parametersDictionary;

        // Constructor
        public ArgumentsUtility(string[] args)
        {
            _parametersDictionary = new StringDictionary();
            Regex Splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string Parameter = null;
            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
            foreach (string Txt in args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                Parts = Splitter.Split(Txt, 3);
                switch (Parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        if (Parameter != null)
                        {
                            if (!_parametersDictionary.ContainsKey(Parameter))
                            {
                                Parts[0] = Remover.Replace(Parts[0], "$1");
                                _parametersDictionary.Add(Parameter, Parts[0]);
                            }
                            Parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;
                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!_parametersDictionary.ContainsKey(Parameter)) _parametersDictionary.Add(Parameter, "true");
                        }
                        Parameter = Parts[1];
                        break;
                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!_parametersDictionary.ContainsKey(Parameter)) 
                                _parametersDictionary.Add(Parameter, "true");
                        }
                        Parameter = Parts[1];
                        // Remove possible enclosing characters (",')
                        if (!_parametersDictionary.ContainsKey(Parameter))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            _parametersDictionary.Add(Parameter, Parts[2]);
                        }
                        Parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (Parameter != null)
            {
                if (!_parametersDictionary.ContainsKey(Parameter)) 
                    _parametersDictionary.Add(Parameter, "true");
            }
        }

        // Retrieve a parameter value if it exists
        public string this[string Param]
        {
            get
            {
                return (_parametersDictionary[Param]);
            }
        }
    }
}
