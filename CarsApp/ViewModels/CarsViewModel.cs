using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Corron.CarService;
using System.Windows.Data;
using System.Collections;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;

namespace Corron.Cars.ViewModels
{
    class CarsViewModel : Screen
    {
        private List<CarModel> _carList;

        private ShellViewModel.SelectedCarChanged _notifyCarChanged;
        private ShellViewModel.ScreenStateChanged _notifySSChanged;
        private ShellViewModel.ErrorHandler _notifyError;

        public CarsViewModel(ShellViewModel.SelectedCarChanged notifyCarChanged, ShellViewModel.ScreenStateChanged notifySSChanged,ShellViewModel.ErrorHandler notifyError)
        {
            _notifyCarChanged = notifyCarChanged;
            _notifySSChanged = notifySSChanged;
            _notifyError = notifyError;

            BindingList<CarModel> _cars;

            try
            {
                _carList = DataAccess.GetCars();
            }
            catch (Exception e)
            {
                notifyError(e);
                return;
            }

            _carList.Sort();

            _cars = new BindingList<CarModel>(_carList); // load up cars from DB
            _cars.RaiseListChangedEvents = true;
            SortedCars =  new BindingListCollectionView(_cars);
        }

        // Properties

        public BindingListCollectionView SortedCars { get; set; }

        public ICarModel FieldedCar
        {
            get => _fieldedCar;
            set
            {
                _fieldedCar = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CarSelected);
                _notifyCarChanged(FieldedCar);
            }
        }
        private ICarModel _fieldedCar;

        private ICarModel LastFieldedCar { get; set; }

        public bool CarSelected => !(FieldedCar is null);

        public bool CanSave(string Fieldedcar_Make, string Fieldedcar_Model, string Fieldedcar_Owner, int Fieldedcar_Year)
        {
            if (String.IsNullOrWhiteSpace(Fieldedcar_Make)) return false;
            if (String.IsNullOrWhiteSpace(Fieldedcar_Model)) return false;
            if (String.IsNullOrWhiteSpace(Fieldedcar_Owner)) return false;
            if (Fieldedcar_Year < 1900 || Fieldedcar_Year > 2050) return false;
            return true;
        }

        public bool ScreenEditingMode
        {
            get { return _screenEditingMode; }
            set
            {
                _screenEditingMode = value;
                NotifyOfPropertyChange("ScreenEditingMode");
                NotifyOfPropertyChange("NotScreenEditingMode");
                _notifySSChanged(NotScreenEditingMode);
            }
        }
        private bool _screenEditingMode;


        public bool NotScreenEditingMode
        {
            get { return !_screenEditingMode; }
        }

        // Methods

        public void Edit()
        {
            SortedCars.EditItem(FieldedCar);
            ScreenEditingMode = true;
        }

        public void Delete(bool FieldedCar_HasService)
        {
            if (MessageBox.Show("Do you want to Delete this car?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (DataAccess.DeleteCar(_fieldedCar.CarID))
                    {
                        SortedCars.Remove(_fieldedCar);
                        SortedCars.Refresh();
                    }
                }
                catch (Exception e)
                {
                    _notifyError(e);
                }
            }
        }

        public bool CanDelete(bool FieldedCar_HasService)
        {
            return FieldedCar_HasService == false;
        }

        public void Add()
        {
            LastFieldedCar = FieldedCar;
            FieldedCar = SortedCars.AddNew() as ICarModel;
            ScreenEditingMode = true;
        }

        public void Save(string Fieldedcar_Make, string Fieldedcar_Model, string Fieldedcar_Owner, int Fieldedcar_Year)
        {

            bool isnew = _fieldedCar.CarID == 0;

            try
            {
                DataAccess.UpdateCar(_fieldedCar as CarModel);
            }
            catch (Exception e)
            {
                _notifyError(e);
            }

            if (isnew)
            { 
                SortedCars.CommitNew();
                SortedCars.MoveCurrentTo(_fieldedCar);
            }
            else
            {
                SortedCars.CommitEdit();
            }
            _notifyCarChanged(_fieldedCar);
            _carList.Sort();
            SortedCars.Refresh();
            ScreenEditingMode = false;
        }

        public void Cancel()
        {
            if (SortedCars.IsAddingNew)
            {
                SortedCars.CancelNew();
                FieldedCar = LastFieldedCar;
            }
            else if (SortedCars.IsEditingItem)
            { 
                SortedCars.CancelEdit();
            }
            ScreenEditingMode = false;
         }

        public void Report()
        {
            try
            {
                DataAccess.GetCarsXML();
            }
            catch (Exception e)
            {
                _notifyError(e);
            }
        }
    }
}
