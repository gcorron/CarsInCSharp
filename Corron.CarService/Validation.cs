using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corron.CarService
{
    public static class Validation
    {
        public const string NNV =  "Non-numeric value.";
        public const string MD = "Missing decimal.";
        public const string ICA = "Invalid currency amount.";
        public const string AMNBN = "Amount must not be negative.";
        public const string FMNBB = "Field must not be blank.";
        public const string FITL = "Field is too long.";
        public const string OK = null;

        public static string ValidateCostString(string cost, out decimal dcost)
        {

            dcost = 0;
            if (String.IsNullOrWhiteSpace(cost))
                return FMNBB;

            if (!decimal.TryParse(cost, out dcost))
                return NNV;


            int i = cost.IndexOf('.');
            if (i < 0)
                return MD;

            if (i == cost.Length - 3)
                return ValidateCost(ref dcost);
            else
            {
                dcost = 0;
                return ICA;
            }
        }

        public static string ValidateCost(ref decimal cost)
        {
            if (cost < 0)
            {
                cost = 0;
                return AMNBN;
            }
            else
                return OK;
        }

        public static string FiftyNoBlanks(string Test)
        {
            if (String.IsNullOrWhiteSpace(Test))
                return FMNBB;
            else if (Test.Length > 50)
                return FITL;
            else
                return OK;
        }
    }
}
