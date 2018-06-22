using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ExpressionEditor.BO;
using System.Configuration;


namespace ExpressionEditor
{
    public partial class frmExpressionEditor : Form
    {

        public frmExpressionEditor()
        {
            InitializeComponent();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ExpressionEditorException);
            RuleSetManager.Initialize();
        }

        void ExpressionEditorException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception.InnerException != null)
                if (e.Exception.InnerException.Message != e.Exception.Message)
                AddError(e.Exception.InnerException.Message, -1, false);
            AddError(e.Exception.Message, -1, false);
        }

        private void btnEditExpression_Click(object sender, EventArgs e)
        {
            if (txtExpression.Text.Trim().Length == 0)
            {
                //MessageBox.Show("Enter an Expression in the Text Box prior to Editing.", "No Expression");
                AddError("Enter an Expression in the Text Box prior to Editing.", 400, false);
                return;
            }

            RuleSetManager.LoadRuleSet(txtExpression.Text);
            txtExpression.Text = RuleSetManager.EditExpression();
        }

        private void btnNewExpression_Click(object sender, EventArgs e)
        {
            RuleSetManager.CreateNewExpression(true);
            txtExpression.Text = RuleSetManager.EditExpression();
        }

        void AddError(string errorMessage, int errorNumber, bool isWarning)
        {
            // Check if this error is already in the list
     
                foreach (ListViewItem listItem in listErrors.Items)
                {
                    if (listItem.SubItems[2].Text == errorMessage)
                    {
                        listErrors.SelectedItems.Clear();
                        listItem.Selected = true;
                        listErrors.Focus();
                        return;
                    }
                }

            ErrorObject err = new ErrorObject(errorMessage, errorNumber, isWarning);
            ListViewItem item = new ListViewItem();
            item.ImageIndex = err.IsWarning ? 1 : 0;
            item.SubItems.Add(err.ErrorNumber.ToString(System.Globalization.CultureInfo.InvariantCulture));
            item.SubItems.Add(err.ErrorMessage);
            item.ToolTipText = err.ErrorMessage;
            listErrors.Items.Add(item);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtExpression.Text.Trim().Length == 0)
            {

                AddError("Enter an Expression in the Text Box prior to Copying.", 401, false);
                return;
            }

            if (RuleSetManager.RuleSetCount == 0)
                RuleSetManager.LoadRuleSet(txtExpression.Text);

            RuleSetManager.CopyToClipboard();
        }

        private void txtExpression_TextChanged(object sender, EventArgs e)
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            listErrors.Items.Clear();

            if (txtExpression.Text.Trim().Length == 0)
            {
                lblIsValid.ForeColor = Color.Blue;
                lblIsValid.Text = "N/A";
                return;
            }

            if (!RuleSetManager.IsExpressionValid(txtExpression.Text, out errors))
            {
                lblIsValid.ForeColor = Color.Red;
                lblIsValid.Text = "Not Valid";

                foreach (ErrorObject err in errors)
                {
                    ListViewItem item = new ListViewItem();
                    item.ImageIndex = err.IsWarning ? 1 : 0;
                    item.SubItems.Add(err.ErrorNumber.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    item.SubItems.Add(err.ErrorMessage);
                    item.ToolTipText = err.ErrorMessage;
                    listErrors.Items.Add(item);
                }
            }
            else
            {
                lblIsValid.ForeColor = Color.Green;
                lblIsValid.Text = "Valid";
            }

        }

        private void tsbtnAbout_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(ConfigurationManager.AppSettings.Get("AboutUrl"));
            }
            catch
            {
                // do nothing, returns Could not find file error
                // however the browser opens to the correct url
            }
        }
    }
}