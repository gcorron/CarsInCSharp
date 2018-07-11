using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corron.CarService;
using System.Data;
using System.Configuration;
using System.Xml;

namespace Corron.Cars
{
    public static class DataAccess
    {
        private static bool _useSQL;

        public static string Initialize()
        {
            string webAddress = Corron.CarService.SQLData.WebConnection();
            _useSQL = String.IsNullOrEmpty(webAddress);
            if (_useSQL)
                return "Connected via SQL";
            else
        	{
                WebClient.Initialize(webAddress);
                return "Connected via Web";
            }
        }

        public static List<CarModel> GetCars()
        {
            if (_useSQL)
                return SQLData.GetCars();
            else
                return WebClient.GetCars();
        }

        public static List<ServiceModel> GetServices(int CarID)
        {
            if (_useSQL)
                return SQLData.GetServices(CarID);
            else
                return WebClient.GetServices(CarID);
        }

        public static bool UpdateCar(CarModel car)
        {
            if (_useSQL)
                return SQLData.UpdateCar(car);
            else
            {
                return WebClient.UpdateCar(car);
            }
        }

        public static bool UpdateService(ServiceModel service)
        {
            if (_useSQL)
                return SQLData.UpdateService(service);
            else
                return WebClient.UpdateService(service);
        }

        public static bool DeleteCar(int id)
        {
            if (_useSQL)
                return SQLData.DeleteCar(id);
            else
            {
                return WebClient.DeleteCar(id);
            }
        }

        public static bool DeleteService(int id)
        {
            if (_useSQL)
                return SQLData.DeleteService(id);
            else
                return WebClient.DeleteService(id);
        }

        public static String GetCarsXML()
        {

            return SQLData.GetCarsXML();
        }

        public static String GetXSLTSheet(int id)
        {
            return SQLData.GetXSLTSheet(id);
        }
    }
}
