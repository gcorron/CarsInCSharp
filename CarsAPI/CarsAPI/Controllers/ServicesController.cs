using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Corron.CarService;

namespace CarsAPI.Controllers
{
    /// <summary>
    /// CRUD operations for service object
    /// </summary>
    public class ServicesController : ApiController
    {
        /// <summary>
        /// gets a list of service objects for the specified car
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ServiceModel> Get(int id)
        {
            List<ServiceModel> _serviceList;
            _serviceList = SQLData.GetServices(id);
            return _serviceList;
        }

        /// <summary>
        /// add a new service object
        /// </summary>
        /// <param name="service"></param>
        /// <returns>new ServiceID</returns>
        public int Post([FromBody]IServiceModel service)
        {
            SQLData.UpdateService(service);
            return service.ServiceID;
        }

        /// <summary>
        /// update service object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        public int Put(int id, [FromBody]IServiceModel service)
        {
            SQLData.UpdateService(service);
            return service.ServiceID;
        }

        /// <summary>
        /// delete service object
        /// </summary>
        /// <param name="id"></param>
        public int Delete(int id)
        {
            SQLData.DeleteService(id);
            return 0;
        }

    }
}
