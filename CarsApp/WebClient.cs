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

        public static void Initialize(string WebAddress)
        {

            if (_client is null)
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(WebAddress);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                _client.Timeout = System.TimeSpan.FromSeconds(15); //timeout 15 seconds
            }
        }


        //CarModel CRUD

        public static List<CarModel> GetCars()
        {
            return GetCarsTask().GetAwaiter().GetResult();
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
            return GetServicesTask(CarID).GetAwaiter().GetResult();
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
            switch(car.CarID.CompareTo(0))
            {
                case 0:
                    car.CarID = PostCarTask(car).GetAwaiter().GetResult();
                    break;
                case 1:
                    car.CarID = PutCarTask(car).GetAwaiter().GetResult();
                    break;
            }
            return true;
        }

        public static bool DeleteCar(int id)
        {
            return (DeleteCarTask(id).GetAwaiter().GetResult()==0);
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
            switch (service.ServiceID.CompareTo(0))
            {
                case 0:
                    service.ServiceID = PostServiceTask(service).GetAwaiter().GetResult();
                    break;
                case 1:
                    service.ServiceID = PutServiceTask(service).GetAwaiter().GetResult();
                    break;
            }
            return true;
        }

        public static bool DeleteService(int id)
        {
            return (DeleteServiceTask(id).GetAwaiter().GetResult() == 0);
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
            CustomException myException;
            try
            {
                myException = GetErrorDetailsFromResponse(response).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new Exception($"{operation} failed: internal server error");
            }
            throw new Exception($"Server error: {myException.ExceptionMessage}");
        }

        private static async Task<CustomException> GetErrorDetailsFromResponse(HttpResponseMessage response)
        {
            return await response.Content.ReadAsAsync<CustomException>().ConfigureAwait(false);
        }

    }
    public class CustomException
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string MessageType { get; set; }
    }
}
