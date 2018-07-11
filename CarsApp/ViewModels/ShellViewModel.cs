using Caliburn.Micro;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Corron.CarService;

namespace Corron.Cars.ViewModels
{
    class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private ICarModel _selectedCar;
        private CarsViewModel _carsScreen;
        private ServicesViewModel _servicesScreen;
        private ReportViewModel _reportScreen;

        private enum ScreenTypes
        {
            Car,
            Service,
            Report
        }

        private ScreenTypes _screentype;
        private bool _canChangeScreen;
        private bool _waitingForCar;

        public delegate void ErrorHandler(Exception e);
        public delegate void SelectedCarChanged(ICarModel CarModel);
        public delegate void ScreenStateChanged(bool CanChangeScreen);

        //Constructor
        public ShellViewModel()
        {
            ConnectMethod=DataAccess.Initialize();
            CarsScreen = true;
            CanChangeScreen = false;
            _waitingForCar = true;
            NotifyOfPropertyChange(() => ErrorMessageVisible);
        }

     //My Notification Methods
        private void OnScreenStateChanged(bool canChangeScreen)
        {
            if (SelectedCar is null)
                _waitingForCar = true;
            else
                CanChangeScreen = canChangeScreen;
        }

        private void OnSelectedCarChanged(ICarModel car)
        {
            SelectedCar = car;
            if (_waitingForCar)
            {
                CanChangeScreen = true;
                _waitingForCar = false;

            }
        }

     //Properties

        public string ConnectMethod { get; set; }

        public bool CarsScreen
        {
            get
            {
                return (_screentype == ScreenTypes.Car);
            }
            set
            {
                _screentype = ScreenTypes.Car;
                NotifyScreenTypeChange();
                if (_carsScreen == null)
                {
                    _carsScreen = new CarsViewModel(OnSelectedCarChanged,OnScreenStateChanged,ShowErrorMessage);
                }
                this.ActivateItem((IScreen)_carsScreen);
            }
        }

        public bool ServicesScreen
        {
            get
            {
                return (_screentype == ScreenTypes.Service);
            }
            set
            {
                if (_servicesScreen == null)
                {
                    _servicesScreen = new ServicesViewModel(OnScreenStateChanged,ShowErrorMessage);
                }
                if (!_servicesScreen.LoadServiceData(_carsScreen.FieldedCar))
                    return; //error loading from DB, don't show services screen

                this.ActivateItem((IScreen)_servicesScreen);
                _screentype = ScreenTypes.Service;
                NotifyScreenTypeChange();
            }
        }

        public bool ReportScreen
        {
            get
            {
                return (_screentype == ScreenTypes.Report);
            }
            set
            {
                if (_reportScreen == null)
                {
                    _reportScreen = new ReportViewModel(OnScreenStateChanged,ShowErrorMessage);
                }
 
                this.ActivateItem((IScreen)_reportScreen);
                _screentype = ScreenTypes.Report;
                NotifyScreenTypeChange();
            }
        }

        private void NotifyScreenTypeChange()
        {
            NotifyOfPropertyChange(() => CarsScreen);
            NotifyOfPropertyChange(() => ServicesScreen);
            NotifyOfPropertyChange(() => ReportScreen);
        }

        public ICarModel SelectedCar
        {
            get => _selectedCar;
            set
            {
                _selectedCar = value;
                NotifyOfPropertyChange();
            }

        }

        public bool CanChangeScreen
        {
            get => _canChangeScreen;
            set
            {
                _canChangeScreen = value;
                NotifyOfPropertyChange();
            }
        }

        public string ErrorMessage { get; set; }

        private Exception CurrentException { get; set; }

        public Visibility ErrorMessageVisible
        {
            get {
                if (string.IsNullOrEmpty(ErrorMessage))
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }       
        }


        //Methods
        private void ShowErrorMessage(Exception e)
        {
            if (e is null)
                ErrorMessage = "";
            else
                ErrorMessage=e.Message;

            CurrentException = e;
            NotifyOfPropertyChange(() => ErrorMessage);
            NotifyOfPropertyChange(() => ErrorMessageVisible);
        }

        public void ErrorDetails()
        {
            if (CurrentException is null)
                MessageBox.Show("No details available","Details", MessageBoxButton.OK);
            else
                MessageBox.Show($"Error occured {CurrentException.StackTrace}" , "Details", MessageBoxButton.OK);

        }

        public void ClearError()
        {
            ShowErrorMessage(null);
        }

    }

}