using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Corron.CarService
{
    public interface IServiceModel : IComparable<IServiceModel>, IEditableObject, IDataErrorInfo
    {
//core properties
        int ServiceID { get; set; }
        int CarID { get; set; }

        string TechName { get; set; }
        DateTime ServiceDate { get; set; }

        decimal LaborCost { get; set; }
        decimal PartsCost { get; set; }
        decimal TotalCost { get; }
 //validation properties
        bool ServiceLinesAreValidState { get; }
        bool IsValidState { get; }

 //detail lines
        ServiceLineModel CurrentServiceLine { get; set; }
        List<ServiceLineModel> ServiceLineList { get; }

 //notification methods called from detail line
        void NotifyValidDetail();
        void RecalcCost();

    }
}