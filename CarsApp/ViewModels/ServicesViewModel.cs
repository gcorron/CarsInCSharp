using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Corron.CarService;

namespace Corron.Cars.ViewModels
{
    class ServicesViewModel:Screen
    {
        #region Private variables, Events, Constructor


        private List<ServiceModel> _serviceList;
        private BindingList<ServiceModel> _services;


        private ServiceModel _fieldedService;
        private bool _screenEditingMode;
        private ICarModel _car;
        private int _listBookMark;

        public EventHandler<bool> ScreenStateChanged;

        //Load Data, always call on activation!
        public bool LoadServiceData(ICarModel car)
        {
            ServiceModel.PassRollBackDelegate(ChangesRolledBack);

            _car = car;

            _serviceList = DataAccess.GetServices(car.CarID); // load up Services from DB
            if (_serviceList is null)
                return false;

            foreach(ServiceModel SM in _serviceList)
            {
                SM.RecalcCost();
            }

            _serviceList.Sort();

            //Binding
            _services = new BindingList<ServiceModel>(_serviceList);
            _services.RaiseListChangedEvents = true;

            SortedServices = new BindingListCollectionView(_services);
            SortedServices.MoveCurrentToFirst();
            FieldedService = SortedServices.CurrentItem as ServiceModel;


            NotifyOfPropertyChange(() => SortedServices);
            NotifyOfPropertyChange(() => CanDelete);
           // NotifyOfPropertyChange(() => FieldedService);
           // NotifyOfPropertyChange(() => ServiceLines);
            return true;
        }

        #endregion

        // Objects

        public BindingListCollectionView SortedServices { get; set; }

        public ServiceModel FieldedService
        {
            get { return _fieldedService; }
            set
            {
                _fieldedService = value;
                _cvServiceLines = null; //force refresh of bindings for detail lines
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => ServiceLines);  
            }
        }

        public List<NameValueByte> ComboBoxTypes // a list of line type Name/Value pairs for the View
        {
            get
            {
                var LineTypesList = new List<NameValueByte>();
                LineTypesList.Add(new NameValueByte(ServiceLineModel.LineTypes.Labor, "Labor"));
                LineTypesList.Add(new NameValueByte(ServiceLineModel.LineTypes.Parts,"Parts"));
                return LineTypesList;
            }
        }

        //detail lines binding

        private BindingList<ServiceLineModel> _blServiceLines;
        private BindingListCollectionView _cvServiceLines;

        public BindingListCollectionView ServiceLines
        {
            get
            {
                if (FieldedService is null)
                    return null;
                if (_cvServiceLines is null)
                    BindServiceLineList();
                return _cvServiceLines;
            }
        }

        private void BindServiceLineList()
        {
            //Binding detail lines
            _blServiceLines = new BindingList<ServiceLineModel>(FieldedService.ServiceLineList);
            _cvServiceLines = new BindingListCollectionView(_blServiceLines);
        }


        //Misc. State
        public bool ScreenEditingMode
        {
            get => _screenEditingMode;
            set
            {
                _screenEditingMode = value;
                NotifyOfPropertyChange(() => ScreenEditingMode);
                NotifyOfPropertyChange(() => NotScreenEditingMode);
                ScreenStateChanged?.Invoke(this, !_screenEditingMode);
            }
        }

        public bool NotScreenEditingMode
        {
            get => !_screenEditingMode;
        }

        //Command Methods

        public void Edit()
        {
            SortedServices.EditItem(SortedServices.CurrentItem);
            ScreenEditingMode = true;
        }
        public bool CanEdit // keep as property!
        {
            get => SortedServices.Count > 0;
        }

        public void Add()
        {
            if (SortedServices.Count == 0)
                _listBookMark = -1;
            else
                _listBookMark = SortedServices.CurrentPosition;
            FieldedService = SortedServices.AddNew() as ServiceModel;
            FieldedService.CarID = _car.CarID;
            FieldedService.ServiceDate = DateTime.Today;
            ScreenEditingMode = true;
        }

        public void Delete()
        {
            if (MessageBox.Show("Do you want to Delete this service?", "Confirm",
               MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DataAccess.DeleteService(_fieldedService.ServiceID))
                {
                    SortedServices.Remove(_fieldedService);
                    SortedServices.Refresh();
                    NotifyOfPropertyChange(() => CanDelete);
                }
            }
        }
        public bool CanDelete // keep as property!
        {
            get => SortedServices.Count > 0;
        }


        public void Save(bool fieldedService_IsValidState, bool screenEditingMode)
        {

            bool isnew = _fieldedService.ServiceID == 0;

            _fieldedService.ServiceLineList.RemoveAll(SL => SL.Delete);


            if (!DataAccess.UpdateService(_fieldedService))
                return;

            if (isnew)
            {
                SortedServices.CommitNew();
                SortedServices.MoveCurrentTo(_fieldedService);
            }
            else
            {
                SortedServices.CommitEdit();
            }
            _serviceList.Sort();
            SortedServices.Refresh();
            ServiceLines.Refresh();
            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => CanEdit);
            ScreenEditingMode = false;
        }

        public bool CanSave(bool fieldedService_IsValidState, bool screenEditingMode)
        {
            if (ScreenEditingMode == false)
                return false;

            return FieldedService.IsValidState;
        }

        public void Cancel()
        {

            if (SortedServices.IsAddingNew)
                SortedServices.CancelNew();
            else if (SortedServices.IsEditingItem)
                SortedServices.CancelEdit();
        }
        private void ChangesRolledBack()
        {
            if (_cvServiceLines.IsAddingNew)
            {
                _cvServiceLines.CancelNew();
            }
            _cvServiceLines.Refresh();
            SortedServices.Refresh();
            ScreenEditingMode = false;
        }
    }
    // Tiny helper class (immutable)
    public class NameValueByte
    {
        public ServiceLineModel.LineTypes Value { get; }
        public string Name { get;}

        internal NameValueByte(ServiceLineModel.LineTypes value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}
