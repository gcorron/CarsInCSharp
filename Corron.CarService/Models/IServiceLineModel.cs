using System.ComponentModel;

namespace Corron.CarService
{
    public interface IServiceLineModel:IDataErrorInfo
    {
//core properties
        ServiceLineModel.LineTypes ServiceLineType { get; set; }
        string ServiceLineTypeString { get; }
        string ServiceLineDesc { get; set; }
        decimal ServiceLineCharge { get; set; }
        bool Delete { get; set; }

//for database access only
        int ServiceID { get; set; }
        byte ServiceLineOrder { get; set; }

//validation
        bool IsValidState { get; }

    }
}