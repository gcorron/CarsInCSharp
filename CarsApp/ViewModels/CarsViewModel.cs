﻿using System;
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
        private int _listBookMark;

        public event EventHandler<ICarModel> SelectedCarChanged;
        public event EventHandler<bool> ScreenStateChanged;

        public CarsViewModel()
        {
            BindingList<CarModel> _cars;
            _carList = DataAccess.GetCars();
            if (_carList is null)
                return;
            Debug.Assert(_carList[0].CarID != 0); //bad data

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
                NotifyOfPropertyChange(() => FieldedCar);
                SelectedCarChanged?.Invoke(this, FieldedCar);
            }
        }
        private ICarModel _fieldedCar;

        private ICarModel LastFieldedCar { get; set; }

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
                ScreenStateChanged?.Invoke(this, !_screenEditingMode);
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
            SortedCars.EditItem(SortedCars.CurrentItem);
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
                    MessageBox.Show(
                        $"Database Error: {e.Message}");
                    return;                
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

            DataAccess.UpdateCar(_fieldedCar as CarModel);
            if (isnew)
            { 
                SortedCars.CommitNew();
                SortedCars.MoveCurrentTo(_fieldedCar);
            }
            else
            {
                SortedCars.CommitEdit();
            }
            SelectedCarChanged.Invoke(this, _fieldedCar);
            _carList.Sort();
            SortedCars.Refresh();
            ScreenEditingMode = false;
        }

        public void Cancel()
        {
            if (SortedCars.IsAddingNew)
            {
                SortedCars.CancelNew();
                FieldedCar=LastFieldedCar;
            }
            else if (SortedCars.IsEditingItem)
                SortedCars.CancelEdit();
            ScreenEditingMode = false;

         }

        //private void _cars_ListChanged(object sender, ListChangedEventArgs e)
        //{
        //    _carList.Sort();
        //    //SortedCars.Refresh();
        //}

        //private void SortedCars_CurrentChanged(object sender, EventArgs e)
        //{
        //    FieldedCar = SortedCars.CurrentItem as ICarModel;
        //    SelectedCarChanged?.Invoke(this, FieldedCar);
        //}
    }
}
