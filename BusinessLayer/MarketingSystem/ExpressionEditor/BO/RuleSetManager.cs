using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities.Rules;
using System.IO;
using System.Xml.Serialization;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Workflow.Activities.Rules.Design;
using System.Globalization;
using System.Drawing;
using System.Configuration;
using System.Reflection;

namespace ExpressionEditor.BO
{
    public class RuleSetManager
    {
        #region Fields

        static RuleSetObject _defaultRuleSet = null;
        static List<RuleSetObject> ruleSets = new List<RuleSetObject>();
        private static Type _defaultType = typeof(Mediachase.Commerce.Marketing.Validators.RulesContext);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current DefaultRuleSet object
        /// </summary>
        /// <value>The default rule set.</value>
        private static RuleSetObject DefaultRuleSet
        {
            get
            {
                return ruleSets.Find(delegate(RuleSetObject rso)
                {
                    return rso == _defaultRuleSet;
                });
            }
        }

        /// <summary>
        /// Returns current RuleSet Count
        /// </summary>
        /// <value>The rule set count.</value>
        public static int RuleSetCount
        {
            get
            {
                return ruleSets.Count;
            }
        }

        #endregion

        #region Create New Expression
        /// <summary>
        /// Creates a New DefaultRuleSet object
        /// </summary>
        public static void CreateNewExpression()
        {
            CreateNewExpression(false);
        }

        /// <summary>
        /// Creates a New DefaultRuleSet object with the ability to remove existing default if exists
        /// </summary>
        /// <param name="deleteDefault">Do you want to delete the current DefaultRuleSet</param>
        public static void CreateNewExpression(bool deleteDefault)
        {
            RuleSetObject ruleObject = new RuleSetObject(_defaultType, "");

            if (deleteDefault)
                ruleSets.Remove(_defaultRuleSet);

            LoadRuleSet(ruleObject);
        }
        #endregion

        #region Edit Expression
        /// <summary>
        /// Open RuleSet Editor against DefaultRuleSet object
        /// </summary>
        /// <returns>Xml Format of Changed RuleSet object</returns>
        public static string EditExpression()
        {
            if (_defaultRuleSet == null)
                throw new ArgumentNullException("You need to load a RulSet prior to calling Edit");

            return EditExpression(DefaultRuleSet.Activity, DefaultRuleSet.ActivityRuleSet);
        }

        /// <summary>
        /// Opens RuleSet Editor
        /// </summary>
        /// <param name="ruleSet">Existing RuleSet</param>
        /// <returns>Xml Format of Changed RuleSet object</returns>
        public static string EditExpression(RuleSet ruleSet)
        {
            return EditExpression(DefaultRuleSet.Activity, ruleSet);
        }

        /// <summary>
        /// Opens RuleSet Editor
        /// </summary>
        /// <param name="type">Assembly Type to Edit</param>
        /// <param name="ruleSet">Existing RuleSet</param>
        /// <returns>Xml Format of Changed RuleSet object</returns>
        public static string EditExpression(Type type, RuleSet ruleSet)
        {
            RuleSetDialog ruleSetDialog = new RuleSetDialog(type, null, ruleSet);
            DialogResult result = ruleSetDialog.ShowDialog();

            if (result == DialogResult.OK)
            {

                DefaultRuleSet.ActivityRuleSet = ruleSetDialog.RuleSet;
                DefaultRuleSet.Name = string.Format("RuleSet{0}", ruleSets.IndexOf(_defaultRuleSet));
                return SerializeRuleSet(ruleSetDialog.RuleSet);
            }
            else
                return DefaultRuleSet.RuleSetDefinition;
        }
        #endregion


        /// <summary>
        /// Validates the given Expression and outputs a list of Errors if exist
        /// </summary>
        /// <param name="Expression">The Xml Expression to Validate</param>
        /// <param name="errors">The List generic of Error Objects that will be outputed</param>
        /// <returns>true if Valid, otherwise false</returns>
        public static bool IsExpressionValid(string Expression, out List<ErrorObject> errors)
        {

            List<ErrorObject> listErrors = new List<ErrorObject>();
            try
            {
                // Create a new RuleSet object so we dont lose our DefaultRuleSet
                RuleSetObject ruleObject = new RuleSetObject(_defaultType, Expression);
                RuleValidation ruleValid = new RuleValidation(_defaultType, null);
                ruleObject.ActivityRuleSet.Validate(ruleValid);

                // Add the errors if any
                for (int i = 0; i < ruleValid.Errors.Count; i++)
                {
                    ErrorObject err = new ErrorObject(ruleValid.Errors[i].ErrorText, ruleValid.Errors[i].ErrorNumber, ruleValid.Errors[i].IsWarning);
                    listErrors.Add(err);
                }

                errors = listErrors;
                return ruleValid.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                ErrorObject err = new ErrorObject(ex.Message, 0, false);
                listErrors.Add(err);
                errors = listErrors;
                return false;
            }
        }

        #region Load Expression
        /// <summary>
        /// Assigns DefaultRuleSet the given Xml Expression if DefaultRuleSet is not null
        /// otherwise creates new DefaultRuleSet with given Expression
        /// </summary>
        /// <param name="xmlData">Xml Expression</param>
        public static void LoadRuleSet(string xmlData)
        {
            if (_defaultRuleSet == null)
            {
                RuleSetObject ruleObject = new RuleSetObject(_defaultType, xmlData);
                LoadRuleSet(ruleObject);
            }
            else
                DefaultRuleSet.RuleSetDefinition = xmlData;
        }

        /// <summary>
        /// Loads the given RuleSet and assigns it as DefaultRuleSet
        /// </summary>
        /// <param name="ruleSet">RuleSet to add</param>
        public static void LoadRuleSet(RuleSetObject ruleSet)
        {
            ruleSets.Add(ruleSet);
            _defaultRuleSet = ruleSet;
        }
        #endregion


        /// <summary>
        /// Copy's DefaultRuleSet to Clipboard
        /// </summary>
        public static void CopyToClipboard()
        {
            Clipboard.Clear();
            Clipboard.SetText(ExportRuleSets(), TextDataFormat.Text);
        }

        #region Helper Methods

        /// <summary>
        /// Serializes the given ruleset into Xml Format
        /// </summary>
        /// <param name="ruleSet">RuleSet to Serialize</param>
        /// <returns>Xml Formated representation of RuleSet</returns>
        private static string SerializeRuleSet(RuleSet ruleSet)
        {
            StringBuilder ruleDefinition = new StringBuilder();

            if (ruleSet != null)
            {
                try
                {
                    #region Serialize

                    StringWriter stringWriter = new StringWriter(ruleDefinition, CultureInfo.InvariantCulture);
                    XmlTextWriter writer = new XmlTextWriter(stringWriter);
                    WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                    serializer.Serialize(writer, ruleSet);
                    #endregion

                    #region Cleanup

                    writer.Flush();
                    writer.Close();
                    stringWriter.Flush();
                    stringWriter.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    if (ruleSet != null)
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet: '{0}'. \r\n\n{1}", ruleSet.Name, ex.Message), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet. \r\n\n{0}", ex.Message), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (ruleSet != null)
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet: '{0}'.", ruleSet.Name), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Error serializing RuleSet.", "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ruleDefinition.ToString();
        }

        /// <summary>
        /// Exports the DefaultRuleSet into xml
        /// </summary>
        /// <returns>Xml Representation of DefaultRuleSet</returns>
        private static string ExportRuleSets()
        {
            
            StringBuilder ruleDefinition = new StringBuilder();

            #region Serialize

            StringWriter stringWriter = new StringWriter(ruleDefinition, CultureInfo.InvariantCulture);
            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
            serializer.Serialize(writer, DefaultRuleSet.ActivityRuleSet);
            #endregion

            #region Cleanup

            writer.Flush();
            writer.Close();
            stringWriter.Flush();
            stringWriter.Close();
            #endregion

            return ruleDefinition.ToString();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            // Get default RuleSet assembly
            try
            {
                string configSetting = ConfigurationManager.AppSettings.Get("RuleSetAssembly");

                if (!string.IsNullOrEmpty(configSetting))
                    _defaultType = Type.GetType(configSetting);
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }

    #region Error Object Class

    public class ErrorObject
    {
        private bool _isWarning;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is warning.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is warning; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarning
        {
            get
            {
                return _isWarning;
            }
            set
            {
                _isWarning = value;
            }
        }

        private string _errorMessage;
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        private int _errorNumber;
        /// <summary>
        /// Gets or sets the error number.
        /// </summary>
        /// <value>The error number.</value>
        public int ErrorNumber
        {
            get
            {
                return _errorNumber;
            }
            set
            {
                _errorNumber = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ErrorObject class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorNumber">The error number.</param>
        /// <param name="IsWarning">if set to <c>true</c> [is warning].</param>
        public ErrorObject(string errorMessage, int errorNumber, bool IsWarning)
        {
            _errorMessage = errorMessage;
            _errorNumber = errorNumber;

            _isWarning = IsWarning;
        }
    }
    #endregion
}
