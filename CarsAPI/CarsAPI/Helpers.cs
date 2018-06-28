using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarsAPI
{
    public static class ObjectCopier
    {
        public static void CopyFields<T>(this T dest, T source)
        {
            PropertyInfo[] properties = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.CanWrite)
                    propertyInfo.SetValue(dest, propertyInfo.GetValue(source, null), null);
            }
        }
    }

    public static class DataHelper
    {
        public static IDbConnection GetJoesDBConnection()
        {
            return new System.Data.SqlClient.SqlConnection(CnnVal("Joes"));
        }

        public static string CnnVal(string name)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch
            {
                throw new ArgumentException($"Database connection for {name} not found in app.config.");
            }
        }
    }

}
