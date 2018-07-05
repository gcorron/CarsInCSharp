using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Corron.CarService;
using System.Diagnostics;

namespace Corron.Cars
{
    public static class WebClient
    {

        private static HttpClient _client;
        private static SQLData.HandleError _handleError;

        public static void Initialize(SQLData.HandleError handleError,string WebAddress)
        {
            _handleError = handleError;               

            if (_client is null)
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(WebAddress);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                _client.Timeout = System.TimeSpan.FromSeconds(5);
            }
        }


        //CarModel CRUD

        public static List<CarModel> GetCars()
        {

            try
            {
                return GetCarsTask().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                ReportException(e);
                return null;
            }
        }

        public static async Task<List<CarModel>> GetCarsTask()
        {
            using (var response = await _client.GetAsync("api/Cars", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var reader = await response.Content.ReadAsAsync<List<CarModel>>().ConfigureAwait(false);
                    return reader.ToList<CarModel>();
                }
                else
                {
                    ThrowSQLError(response, "Get Cars");
                    return null;
                }
            }
        }

        public static List<ServiceModel> GetServices(int CarID)
        {
            try
            {
                return GetServicesTask(CarID).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                ReportException(e);
                return null;
            }
        }

        public static async Task<List<ServiceModel>> GetServicesTask(int CarID)
        {
            using (var response = await _client.GetAsync($"api/Services?id={CarID}", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var reader = await response.Content.ReadAsAsync<List<ServiceModel>>().ConfigureAwait(false);
                    return reader.ToList<ServiceModel>();
                }
                else
                {
                    ThrowSQLError(response, "Get Cars");
                    return null;
                }
            }
        }
        public static bool UpdateCar(CarModel car)
        {
            try
            {
                switch(car.CarID.CompareTo(0))
                {
                    case 0:
                        car.CarID = PostCarTask(car).GetAwaiter().GetResult();
                        break;
                    case 1:
                        car.CarID = PutCarTask(car).GetAwaiter().GetResult();
                        break;
                }
            }
            catch (Exception e)
            {
                ReportException(e);
                return false;
            }
            return true;
        }

        public static bool DeleteCar(int id)
        {
            try
            {
                return (DeleteCarTask(id).GetAwaiter().GetResult()==0);
            }
            catch (Exception e)
            {
                ReportException(e);
                return false;
            }
        }

        public static async Task<int> PutCarTask(CarModel car)
        {
            using (var response = await _client.PutAsJsonAsync<CarModel>($"api/Cars?id={car.CarID}",car).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response, "Put Car");
                    return 0;
                }
            }
        }

        public static async Task<int> PostCarTask(CarModel car)
        {
            using (var response = await _client.PostAsJsonAsync<CarModel>($"api/Cars?id={car.CarID}", car).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response, "Post Car");
                    return 0;
                }
            }
        }

        public static async Task<int> DeleteCarTask(int id)
        {
            using (var response = await _client.DeleteAsync($"api/Cars?id={id}").ConfigureAwait(false))
            {

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response, "Delete Car");
                    return 0;
                }
            }

        }


        // Service Model CRUD

        public static bool UpdateService(ServiceModel service)
        {
            try
            {
                switch (service.ServiceID.CompareTo(0))
                {
                    case 0:
                        service.ServiceID = PostServiceTask(service).GetAwaiter().GetResult();
                        break;
                    case 1:
                        service.ServiceID = PutServiceTask(service).GetAwaiter().GetResult();
                        break;
                }
            }
            catch (Exception e)
            {
                ReportException(e);
                return false;
            }
            return true;
        }

        public static bool DeleteService(int id)
        {
            try
            { 
                return (DeleteServiceTask(id).GetAwaiter().GetResult() == 0);
            }
            catch (Exception e)
            {
                ReportException(e);
                return false;
            }
        }

        public static async Task<int> PutServiceTask(ServiceModel service)
        {
            using (var response = await _client.PutAsJsonAsync<ServiceModel>($"api/Services?id={service.ServiceID}", service).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response, "Put Service");
                    return 0;
                }
            }
        }

        public static async Task<int> PostServiceTask(ServiceModel service)
        {
            using (var response = await _client.PostAsJsonAsync<ServiceModel>($"api/Services?id={service.CarID}", service).ConfigureAwait(false))
            {
                 if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response, "Post Service");
                    return 0;
                }
            }
        }

        public static async Task<int> DeleteServiceTask(int id)
        {
            using (var response = await _client.DeleteAsync($"api/Services?id={id}").ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    id = await response.Content.ReadAsAsync<int>().ConfigureAwait(false);
                    return id;
                }
                else
                {
                    ThrowSQLError(response,"Delete Service");
                    return 0;
                }
            }
        }
        
        private static void ThrowSQLError(HttpResponseMessage response, string operation)
        {
            string details = GetErrorDetailsFromResponse(response).GetAwaiter().GetResult();
            throw new Exception($"{operation} failed because: {response.ReasonPhrase}: {response.Content}");
        }

        private static async Task<string> GetErrorDetailsFromResponse(HttpResponseMessage response)
        {
            return await response.Content.ReadAsAsync<string>().ConfigureAwait(false);
        }

        private static void ReportException(Exception e)
        {
            if (e.InnerException is null)
                _handleError(e.Message);
            else
                _handleError(e.InnerException.Message);
        }

    }
}
