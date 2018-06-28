using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Corron.CarService
{
    internal static class ObjectCopier
    {
        public static CarModel CopyCar(CarModel car)
        {
            CarModel copy;
            CopyFields(copy = new CarModel(), car);
            return copy;
        }

        public static ServiceModel CopyService(ServiceModel service)
        {
            ServiceModel copy;
            CopyFields(copy = new ServiceModel(), service);
            return copy;
        }

        public static void CopyFields<T>(this T dest, T source)
        {
            PropertyInfo[] properties = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.CanWrite)
                    propertyInfo.SetValue(dest, propertyInfo.GetValue(source, null), null);
            }
        }
        public static List<T> CopyList<T>(List<T> source)
        {
            var dest = new List<T>();
            foreach(T item in source)
            {
                dest.Add(item);
            }
            return dest;
        }
    }
}
