using System;
using System.ComponentModel;

namespace Corron.CarService
{
    public interface ICarModel: IComparable<ICarModel>, IDataErrorInfo, IEditableObject
    {
        int CarID { get; set; }
        string Make { get; set; }
        string Model { get; set; }
        string Owner { get; set; }
        int Year { get; set; }
    }
}