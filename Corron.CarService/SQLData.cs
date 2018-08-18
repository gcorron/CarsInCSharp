
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using System.Transactions;

namespace Corron.CarService
{
    public static class SQLData
    {
        private const string XMLHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        public static List<CarModel> GetCars()
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                return connection.Query<CarModel>("SelectCars").ToList<CarModel>();
            }
        }

        public static List<CarModel> GetCarsFiltered(int year, string owner)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                return connection.Query<CarModel>($"SelectCarsFiltered @year={ year }, @owner= '{owner}'").ToList<CarModel>();
            }
        }

        public static CarModel GetCar(int CarID)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                var carlist= connection.Query<CarModel>($"SelectCar @CarID={CarID}").ToList<CarModel>();
                if (carlist.Count != 1)
                    throw new RowNotInTableException();
                return carlist[0];
            }
        }

        public static string[] SearchOwners(string search)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                return connection.Query<string>($"SelectSearchOwners @search='{ search }'").ToArray<string>();
            }

        }

        public static List<ServiceModel> GetServices(int CarID)
        {
            var lookup = new Dictionary<int, ServiceModel>();
            using (IDbConnection connection = GetJoesDBConnection())
            {
                connection.Query<ServiceModel, ServiceLineModel, ServiceModel>
                    ($"SelectServicesForCar @CarID", (S, L) =>
                    {
                        if (!lookup.TryGetValue(S.ServiceID, out ServiceModel SM))
                        {
                            lookup.Add(S.ServiceID, SM = S);
                        }

                        SM.ServiceLineList.Add(L);
                        return SM;

                    }, new { CarID }, splitOn: "ServiceID");
            }
            return lookup.Values.ToList<ServiceModel>();
        }

        public static ServiceModel GetService(int ServiceID)
        {
            var lookup = new Dictionary<int, ServiceModel>();
            using (IDbConnection connection = GetJoesDBConnection())
            {
                connection.Query<ServiceModel, ServiceLineModel, ServiceModel>
                    ($"SelectService @ServiceID", (S, L) =>
                    {
                        if (!lookup.TryGetValue(S.ServiceID, out ServiceModel SM))
                        {
                            lookup.Add(S.ServiceID, SM = S);
                        }

                        SM.ServiceLineList.Add(L);
                        return SM;

                    }, new { ServiceID }, splitOn: "ServiceID");
            }
            return lookup.Values.First();
        }

        public static bool UpdateCar(CarModel car)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {

                List<int> results;
                results = connection.Query<int>("dbo.UpdateCar @CarID, @Make, @Model, @Year, @Owner", car) as List<int>;

                if (results[0] >= 0 && car.CarID <= 0)
                    car.CarID = results[0];
            }
            return true;
        }

        public static bool UpdateService(ServiceModel service)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    using (IDbConnection connection = GetJoesDBConnection())
                    {
                        List<int> results;

                        results = connection.Query<int>("dbo.UpdateService @ServiceID, @ServiceDate, @TechName, @LaborCost, @PartsCost, @CarID", service) as List<int>;

                        if (results[0] >= 0 && service.ServiceID <= 0)
                            service.ServiceID = results[0];

                        if (service.ServiceID != 0) //zero if deleted, nothing left to do
                        {
                            List<ServiceLineModel> SL = service.ServiceLineList;
                            if (SL.Count > 255)
                                throw new Exception("Too many detail lines!");
                            else if (SL.Count == 0)
                                throw new Exception("At least one detail line is required!");
                            else
                            {
                                byte b = 0;
                                foreach (IServiceLineModel s in SL)
                                {
                                    s.ServiceLineOrder = b++; //save the order
                                    s.ServiceID = service.ServiceID;
                                }
                                // stored procedure deletes all lines before inserting new ones
                                connection.Execute($"dbo.UpdateServiceLine @ServiceID, @ServiceLineOrder, @ServiceLineType, @ServiceLineDesc, @ServiceLineCharge", SL);
                            }
                        }
                        scope.Complete();
                    }
                    return true;
                }
                catch (TransactionAbortedException ex)
                {
                    // there will be other exceptions too, so do not rethrow.
                }
                return false;
            }
        }

        public static bool DeleteCar(int id)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                connection.Execute($"dbo.DeleteCar @CarID={id}");
                return true;
            }
        }

        public static bool DeleteService(int id)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                connection.Execute($"dbo.DeleteService @ServiceID={id}");
                return true;
            }
        }
        //reports

        public static string GetXSLTSheet(int id)
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                List<string> results;
                results = connection.Query<string>($"SelectXSLTSheet @id={id}") as List<string>;
                return XMLHeader + results[0];
            }

        }

        public static string GetCarsXML()
        {
            using (IDbConnection connection = GetJoesDBConnection())
            {
                List<string> results;
                results = connection.Query<string>("SelectCarsXml") as List<string>;
                return XMLHeader + results[0];
            }
        }


        public static string WebConnection()
        {
            return CnnVal("WebConnection");
        }

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
