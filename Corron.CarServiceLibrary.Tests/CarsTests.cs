using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corron.CarService;
using Xunit;

namespace Corron.CarServiceLibrary.Tests
{
    public class CarsTests
    {

        [Theory]
        [InlineData("RuWork",Validation.NNV,0)]
        [InlineData("42.2", Validation.ICA, 0)]
        [InlineData("20.00", Validation.OK, 20)]
        [InlineData("0", Validation.MD, 0)]
        [InlineData("-5.00", Validation.AMNBN, 0)]
        [InlineData("", Validation.FMNBB, 0)]


        public void CostStringShouldValidate(string sval, string sresult, decimal dresult)
        {
            //Arrange
 
            //Act
            decimal dactual;
            string sactual = Validation.ValidateCostString(sval, out dactual);

            //Assert
            Assert.Equal(sresult, sactual);
            Assert.Equal(dresult,dactual);
        }
    }
}
