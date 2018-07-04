using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Runtime.Serialization;

namespace Corron.CarService
{
    [DataContract]
    public class ServiceLineModel : PropertyChangedBase, IServiceLineModel
    {

        public enum LineTypes : byte { Labor, Parts };
        const string MONEY_FORMAT = "{0:0.00}";

        private decimal[] _calcLineCharge;
        private static LineTypes _lastLineType;
        private bool? _validState;

        public delegate void RecalcDelegate(); //delegate type declaration for recalc
        private static RecalcDelegate RecalcAction; //store the recalc method once for all (static)

        public delegate void ValidChangedDelegate();
        private static ValidChangedDelegate ValidChangedAction;



        //constructors

        public ServiceLineModel()
        {
            ServiceLineType = _lastLineType;
            ServiceLineDesc = "";
            ServiceLineCharge = 0;
            _calcLineCharge = new decimal[] {0M,0M};
            _validState = null;

            if (ValidChangedAction is null)
                return;
            ValidChangedAction();
        }

        // delegate initialization
        public static void PassDelegates(ValidChangedDelegate vcd, RecalcDelegate rd)
        {
            RecalcAction = rd;
            ValidChangedAction = vcd;
        }
        public static void NullDelegates()
        {
            RecalcAction = null;
            ValidChangedAction = null;
        }

        //properties

        [DataMember]
        public int ServiceID { get; set; }

        public byte ServiceLineOrder { get; set; } //used only to retrieve from storage in original order entered

        [DataMember]
        public LineTypes ServiceLineType
        {
            get { return _serviceLineType; }
            set
            {
                _serviceLineType = value;
                _lastLineType = value;
                DoRecalc();
            }
        }
        private LineTypes _serviceLineType;

        public string ServiceLineTypeString
        {
            get { return new[] { "Labor", "Parts" }[(int)_serviceLineType]; }
        }

        [DataMember]
        public string ServiceLineDesc
        {
            get { return _serviceLineDesc; }
            set
            {
                _serviceLineDesc = value;
                NotifyIfValidChanged();
            }
        }
        private string _serviceLineDesc;

        [DataMember]
        public decimal ServiceLineCharge { get; set; }

        public string ServiceLineChargeString
        {
            get {
                if (_serviceLineChargeString is null)
                    _serviceLineChargeString=String.Format(MONEY_FORMAT,ServiceLineCharge);
                return _serviceLineChargeString;
            }
            set {
                decimal charge;
                _serviceLineChargeString = value;
                if (Validation.ValidateCostString(value, out charge) == null)
                {
                    ServiceLineCharge = charge;
                    DoRecalc();
                }
                NotifyIfValidChanged();
            }
        }
        private string _serviceLineChargeString;

        public bool Delete {
            get
            {
                return _delete;
            }
            set
            {
                _delete = value;
                DoRecalc();
                ValidChangedAction();
            }
        }
        private bool _delete;

 

        public bool IsValidState
        {
            get
            {
                if (Delete)
                    return true;
                return (new string[] { "ServiceLineDesc", "ServiceLineChargeString" }.All(s => (this[s] is null)));
            }
        }

        // Methods to notify parent class

        private void NotifyIfValidChanged()
        {
            if (ValidChangedAction is null) //used stand-alone in web service
                return;

            bool newValidState = IsValidState;
            if (newValidState == _validState) //_validState is Nullable, so only test that works is equality
                return;

            ValidChangedAction();
            _validState = newValidState;
        }

        private void DoRecalc()
        {
            if (RecalcAction is null) //used stand-alone in web service
                return;
            RecalcAction();
        }

        // Implements IDataErrorInfo
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }


        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ServiceLineDesc": return Validation.FiftyNoBlanks(ServiceLineDesc);
                    case "ServiceLineChargeString": return Validation.ValidateCostString(ServiceLineChargeString,out decimal junk);
                    default:
                        return "Invalid Column Name";
                }
            }
        }
    }
}
