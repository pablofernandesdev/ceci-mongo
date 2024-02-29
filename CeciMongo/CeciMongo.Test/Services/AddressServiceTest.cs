using AutoMapper;
using CeciMongo.Domain.DTO.ViaCep;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service.External;
using CeciMongo.Domain.Mapping;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Address;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.User;
using CeciMongo.Test.Fakers.ViaCep;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class AddressServiceTest
    {
        private readonly Mock<IViaCepService> _mockViaCepService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IMapper _mapper;

        public AddressServiceTest()
        {
            _mockViaCepService = new Mock<IViaCepService>();
            _mockUserRepository = new Mock<IUserRepository>();

            //Auto mapper configuration
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Add_address_successfully()
        {
            //Arrange
            //var addressEntityFaker = AddressFaker.AddressEntity().Generate();
            var addressAddDtoFaker = AddressFaker.AddressAddDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var entity = _mapper.Map<Address>(addressAddDtoFaker);

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressAddDtoFaker.UserId))
                .ReturnsAsync(userEntityFaker);

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.AddAsync(addressAddDtoFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_to_add_address()
        {
            //Arrange
            var addressAddDtoFaker = AddressFaker.AddressAddDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressAddDtoFaker.UserId))
                .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.AddAsync(addressAddDtoFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_address_successfully()
        {
            //Arrange
            var addressUpdateDTOFaker = AddressFaker.AddressUpdateDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(_mapper.Map<Address>(addressUpdateDTOFaker));

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressUpdateDTOFaker.UserId))
                .ReturnsAsync(userEntityFaker);

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.UpdateAsync(addressUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_update_address()
        {
            //Arrange
            var addressUpdateDTOFaker = AddressFaker.AddressUpdateDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressUpdateDTOFaker.UserId))
                .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.UpdateAsync(addressUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Delete_address_successfully()
        {
            //Arrange
            var addressDeleteFaker = AddressFaker.AddressDeleteDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(addressDeleteFaker.AddressId)               
            });

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressDeleteFaker.UserId))
                .ReturnsAsync(userEntityFaker);

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.DeleteAsync(addressDeleteFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_delete_address()
        {
            //Arrange
            var addressDeleteFaker = AddressFaker.AddressDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressDeleteFaker.UserId))
                     .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.DeleteAsync(addressDeleteFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_address_by_filter()
        {
            //Arrange
            var addressEntityFaker = AddressFaker.AddressEntity().Generate(3);
            var addressFilterDto = AddressFaker.AddressFilterDTO().Generate();
            var userId = Guid.NewGuid().ToString();

            _mockUserRepository.Setup(x => x.GetLoggedUserAddressesAsync(userId, addressFilterDto))
                .ReturnsAsync(addressEntityFaker);

            _mockUserRepository.Setup(x => x.GetTotalLoggedUserAddressesAsync(userId, addressFilterDto))
                .ReturnsAsync(addressEntityFaker.Count);

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetUserAddressesAsync(userId, addressFilterDto);

            //Assert
            Assert.True(result.Data.Any() && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_get_address_by_filter()
        {
            //Arrange
            var addressEntityFaker = AddressFaker.AddressEntity().Generate(3);
            var addressFilterDto = AddressFaker.AddressFilterDTO().Generate();
            var userId = Guid.NewGuid().ToString();

            _mockUserRepository.Setup(x => x.GetLoggedUserAddressesAsync(userId, addressFilterDto))
                .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetUserAddressesAsync(userId, addressFilterDto);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_by_id()
        {
            //Arrange
            var addressIdentifierFaker = AddressFaker.AddressIdentifierDTO().Generate();
            var addressEntityFaker = AddressFaker.AddressEntity().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(addressIdentifierFaker.AddressId)
            });

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressIdentifierFaker.UserId))
                .ReturnsAsync(userEntityFaker);

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetByIdAsync(addressIdentifierFaker);

            //Assert
            Assert.True(result.Data != null && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_get_by_id()
        {
            //Arrange
            var addressIdentifierFaker = AddressFaker.AddressIdentifierDTO().Generate();
            var addressEntityFaker = AddressFaker.AddressEntity().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressIdentifierFaker.UserId))
                .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetByIdAsync(addressIdentifierFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_address_by_zip_code()
        {
            //Arrange
            var viaCepAddressResponseFaker = ViaCepFaker.ViaCepAddressResponseDTO().Generate();
            var zipCodeFaker = AddressFaker.AddressZipCodeDTO().Generate();

            _mockViaCepService.Setup(x => x.GetAddressByZipCodeAsync(zipCodeFaker.ZipCode))
                .ReturnsAsync(ResultResponseFaker.ResultResponseData<ViaCepAddressResponseDTO>(viaCepAddressResponseFaker, HttpStatusCode.OK));

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetAddressByZipCodeAsync(zipCodeFaker);

            //Assert
            Assert.True(result.Data != null && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Error_getting_address_by_zip_code()
        {
            //Arrange
            var viaCepAddressResponseFaker = ViaCepFaker.ViaCepAddressResponseDTO().Generate();
            var zipCodeFaker = AddressFaker.AddressZipCodeDTO().Generate();

            _mockViaCepService.Setup(x => x.GetAddressByZipCodeAsync(zipCodeFaker.ZipCode))
                .ReturnsAsync(ResultResponseFaker.ResultResponseData<ViaCepAddressResponseDTO>(viaCepAddressResponseFaker, HttpStatusCode.InternalServerError));

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetAddressByZipCodeAsync(zipCodeFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_get_address_by_zip_code()
        {
            //Arrange
            var zipCodeFaker = AddressFaker.AddressZipCodeDTO().Generate();

            _mockViaCepService.Setup(x => x.GetAddressByZipCodeAsync(zipCodeFaker.ZipCode))
                .ThrowsAsync(new Exception());

            var addressService = AddressServiceConstrutor();

            //Act
            var result = await addressService.GetAddressByZipCodeAsync(zipCodeFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        private AddressService AddressServiceConstrutor()
        {
            return new AddressService(
                _mockUserRepository.Object,
                _mockViaCepService.Object,
                _mapper);
        }
    }
}
