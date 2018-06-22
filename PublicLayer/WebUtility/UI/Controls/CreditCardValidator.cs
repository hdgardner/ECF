using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.Web.UI.Controls
{
    /// <summary>
    /// Credit Card Validator provides basic offline server side validation for credit cards.
    /// </summary>
    public class CreditCardValidator : BaseValidator
    {
        /// <summary>
        /// When overridden in a derived class, this method contains the code to determine whether the value in the input control is valid.
        /// </summary>
        /// <returns>
        /// true if the value in the input control is valid; otherwise, false.
        /// </returns>
        protected override bool EvaluateIsValid()
        {
            //-- Set valueToValidate to the validation control's controltovalidate value.
            string valueToValidate = this.GetControlValidationValue(this.ControlToValidate);
            int indicator = 1;		//-- will be indicator for every other number
            int firstNumToAdd = 0;  //-- will be used to store sum of first set of numbers
            int secondNumToAdd = 0; //-- will be used to store second set of numbers
            string num1;	//-- will be used if every other number added is greater
            //-- than 10, store the left-most integer here
            string num2;	//-- will be used if ever yother number added
            //-- is greater than 10, store the right-most integer here

            //-- Convert our creditNo string to a char array
            char[] ccArr = valueToValidate.ToCharArray();

            for (int i = ccArr.Length - 1; i >= 0; i--)
            {
                char ccNoAdd = ccArr[i];
                int ccAdd = 0;

                bool result = Int32.TryParse(ccNoAdd.ToString(), out ccAdd);
                if(!result)
                    return false;

                //int ccAdd = Int32.Parse(ccNoAdd.ToString());
                if (indicator == 1)
                {
                    //-- If we are on the odd number of numbers, add that number to our total
                    firstNumToAdd += ccAdd;
                    //-- set our indicator to 0 so that our code will know to skip to the next piece
                    indicator = 0;
                }
                else
                {
                    //-- if the current integer doubled is greater than 10
                    //-- split the sum in to two integers and add them together
                    //-- we then add it to our total here
                    if ((ccAdd + ccAdd) >= 10)
                    {
                        int temporary = (ccAdd + ccAdd);
                        num1 = temporary.ToString().Substring(0, 1);
                        num2 = temporary.ToString().Substring(1, 1);
                        secondNumToAdd += (Convert.ToInt32(num1) + Convert.ToInt32(num2));
                    }
                    else
                    {
                        //-- otherwise, just add them together and add them to our total
                        secondNumToAdd += ccAdd + ccAdd;
                    }
                    //-- set our indicator to 1 so for the next integer
                    //-- we will perform a different set of code
                    indicator = 1;
                }
            }
            //-- If the sum of our 2 numbers is divisible by 10,
            //-- then the card is valid. Otherwise, it is not
            bool isValid = false;
            if ((firstNumToAdd + secondNumToAdd) % 10 == 0)
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }
    }
}