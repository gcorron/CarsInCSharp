using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Corron.CarService;

namespace CarsAPI.Controllers
{
    /// <summary>
    /// CRUD operations for car object
    /// </summary>
    public class CarsController : ApiController
    {
        /// <summary>
        /// get all car objects in a list
        /// </summary>
        /// <returns>List(of CarModel)</returns>
        public IEnumerable<CarModel> Get()
        {

            List<CarModel> _carList;

            _carList = SQLData.GetCars();
            if (!(_carList is null))
                _carList.Sort();
            return _carList;
        }

        /// <summary>
        /// add new car object. expects carID to be 0, returns new carID
        /// </summary>
        /// <param name="car"></param>
        /// <returns>new CarID</returns>
        public int Post([FromBody]CarModel car) 
        {
            SQLData.UpdateCar(car);
            return car.CarID;
        }

        /// <summary>
        /// update car object
        /// negative id is for delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="car"></param>
        /// <returns>CarID</returns>
        public int Put(int id, [FromBody]CarModel car)
        {
            SQLData.UpdateCar(car);
            return car.CarID;
        }

        /// <summary>
        /// delete car object
        /// </summary>
        /// <param name="id"></param>
        /// <returns>0</returns>
        public int Delete(int id)
        {
            SQLData.DeleteCar(id);
            return 0;
        }

    }
}
