using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Test.Fakers.Address;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.WebApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Controllers
{
    public class AddressControllerTest
    {
        private readonly Mock<IAddressService> _mockAddressService;

        public AddressControllerTest()
        {
            _mockAddressService = new Mock<IAddressService>();
        }

        [Fact]
        public async Task Get_by_zip_code()
        {
            //Arrange
            var addressResultDto = AddressFaker.AddressResultDTO().Generate();
            var addressZipCodeDto = AddressFaker.AddressZipCodeDTO().Generate();
            var resultDataResponse = ResultResponseFaker.ResultResponseData<AddressResultDTO>(addressResultDto, It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.GetAddressByZipCodeAsync(addressZipCodeDto))
                .ReturnsAsync(resultDataResponse);

            var userController = AddressControllerConstrutor();

            //Act
            var result = await userController.GetByZipCode(addressZipCodeDto);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse<AddressResultDTO>>(objResult.Value);
        }

        [Fact]
        public async Task Add_address()
        {
            //Arrange
            var addressAddDto = AddressFaker.AddressAddDTO().Generate();
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.AddAsync(addressAddDto))
                .ReturnsAsync(resultResponse);

            var addressController = AddressControllerConstrutor();

            //Act
            var result = await addressController.Add(addressAddDto);
            _mockAddressService.Verify(x => x.AddAsync(addressAddDto), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Update_address()
        {
            //Arrange
            var addressUpdateDto = AddressFaker.AddressUpdateDTO().Generate();
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.UpdateAsync(addressUpdateDto))
                .ReturnsAsync(resultResponse);

            var addressController = AddressControllerConstrutor();

            //Act
            var result = await addressController.Update(addressUpdateDto);
            _mockAddressService.Verify(x => x.UpdateAsync(addressUpdateDto), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Delete_address()
        {
            //Arrange
            var addressDeleteDto = AddressFaker.AddressDeleteDTO().Generate();
            var addressId = addressDeleteDto.AddressId;
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.DeleteAsync(addressDeleteDto))
                .ReturnsAsync(resultResponse);

            var addressController = AddressControllerConstrutor();

            //Act
            var result = await addressController.Delete(addressDeleteDto);
            _mockAddressService.Verify(x => x.DeleteAsync(addressDeleteDto), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Get_all()
        {
            //Arrange
            var addressResultDto = AddressFaker.AddressResultDTO().Generate(2);
            var addressFilterDto = AddressFaker.AddressFilterDTO().Generate();
            var resultDataResponse = ResultDataResponseFaker.ResultDataResponse<IEnumerable<AddressResultDTO>>(addressResultDto, It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.GetUserAddressesAsync(It.IsAny<string>(), addressFilterDto))
                .ReturnsAsync(resultDataResponse);

            var userController = AddressControllerConstrutor();

            //Act
            var result = await userController.Get(It.IsAny<string>(), addressFilterDto);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultDataResponse<IEnumerable<AddressResultDTO>>>(objResult.Value);
        }

        [Fact]
        public async Task Get_by_id()
        {
            //Arrange
            var addressResultDto = AddressFaker.AddressResultDTO().Generate();
            var addressIdentifierDto = AddressFaker.AddressIdentifierDTO().Generate();
            var resultDataResponse = ResultResponseFaker.ResultResponseData<AddressResultDTO>(addressResultDto, It.IsAny<HttpStatusCode>());

            _mockAddressService.Setup(x => x.GetByIdAsync(addressIdentifierDto))
                .ReturnsAsync(resultDataResponse);

            var userController = AddressControllerConstrutor();

            //Act
            var result = await userController.GetById(addressIdentifierDto);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse<AddressResultDTO>>(objResult.Value);
        }

        private AddressController AddressControllerConstrutor()
        {
            return new AddressController(_mockAddressService.Object);
        }
    }
}
