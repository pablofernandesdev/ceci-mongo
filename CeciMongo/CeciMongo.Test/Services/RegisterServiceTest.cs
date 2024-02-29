using AutoMapper;
using CeciMongo.Domain.DTO.Register;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Domain.Mapping;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Address;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.Email;
using CeciMongo.Test.Fakers.Register;
using CeciMongo.Test.Fakers.Role;
using CeciMongo.Test.Fakers.User;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class RegisterServiceTest
    {
        private readonly string _claimNameIdentifier = "1";
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly IMapper _mapper;

        public RegisterServiceTest()
        {
            _mockEmailService = new Mock<IEmailService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();

            //Auto mapper configuration
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();

            //http context configuration
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, _claimNameIdentifier),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(user);
        }

        [Fact]
        public async Task User_self_registration_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var emailRequestDTOFaker = EmailFaker.EmailRequestDTO().Generate();

            _mockRoleRepository.Setup(x => x.GetBasicProfile())
               .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            _mockEmailService.Setup(x => x.SendEmailAsync(emailRequestDTOFaker))
                .ReturnsAsync(ResultResponseFaker.ResultResponse(HttpStatusCode.OK).Generate());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.SelfRegistrationAsync(UserFaker.UserSelfRegistrationDTO().Generate());

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task User_self_registration_exception()
        {
            //Arrange
            _mockRoleRepository.Setup(x => x.GetBasicProfile())
               .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.SelfRegistrationAsync(UserFaker.UserSelfRegistrationDTO().Generate());

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Update_logged_user_successfully()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserLoggedUpdateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindOneAsync(x=> x.Email == userUpdateDTOFaker.Email && x.Id.ToString() != userId))
                .ReturnsAsync(value: null);

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAsync(userUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_logged_user_email_already_registered()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserLoggedUpdateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindOneAsync(x => x.Email == userUpdateDTOFaker.Email && x.Id.ToString() != userId))
                    .ReturnsAsync(UserFaker.UserEntity().Generate());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAsync(userUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Update_logged_user_exception()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserLoggedUpdateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindOneAsync(x => x.Email == userUpdateDTOFaker.Email && x.Id.ToString() != userId))
                    .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAsync(userUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Get_logged_in_user_successfully()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedInUserAsync();

            //Assert
            Assert.True(result.Data != null && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedInUserAsync();

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Redefine_user_password_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.RedefinePasswordAsync(new UserRedefinePasswordDTO
            {
                CurrentPassword = PasswordExtension.DecryptPassword(userEntityFaker.Password),
                NewPassword = "dGVzdGUy"
            });

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Redefine_user_password_invalid_password()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.RedefinePasswordAsync(new UserRedefinePasswordDTO
            {
                CurrentPassword = "xxxxxx",
                NewPassword = "dGVzdGUy"
            });

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Redefine_user_password_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.RedefinePasswordAsync(UserFaker.UserRedefinePasswordDTO().Generate());

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Add_logged_in_user_address_successfully()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressLoggedUserAddDTOFaker = AddressLoggedUserFaker.AddressLoggedUserAddDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Uf = addressLoggedUserAddDTOFaker.Uf,
                District = addressLoggedUserAddDTOFaker.District,
                ZipCode = addressLoggedUserAddDTOFaker.ZipCode,
                Complement = addressLoggedUserAddDTOFaker.Complement,
                Locality = addressLoggedUserAddDTOFaker.Locality,
                Street = addressLoggedUserAddDTOFaker.Street,
                Number = addressLoggedUserAddDTOFaker.Number
            });

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.AddLoggedUserAddressAsync(addressLoggedUserAddDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Add_logged_in_user_null()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(value: null);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.AddLoggedUserAddressAsync(AddressLoggedUserFaker.AddressLoggedUserAddDTO().Generate());

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_logged_in_user_address_successfully()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressUpdateDTOFaker = AddressLoggedUserFaker.AddressLoggedUserUpdateDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Uf = addressUpdateDTOFaker.Uf,
                District = addressUpdateDTOFaker.District,
                ZipCode = addressUpdateDTOFaker.ZipCode,
                Complement = addressUpdateDTOFaker.Complement,
                Locality = addressUpdateDTOFaker.Locality,
                Street = addressUpdateDTOFaker.Street,
                Number = addressUpdateDTOFaker.Number
            });

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAddressAsync(addressUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_logged_in_user_address_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressUpdateDTOFaker = AddressLoggedUserFaker.AddressLoggedUserUpdateDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                    .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAddressAsync(addressUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_logged_in_user_null()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressUpdateDTOFaker = AddressLoggedUserFaker.AddressLoggedUserUpdateDTO().Generate();

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                        .ReturnsAsync(value: null);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.UpdateLoggedUserAddressAsync(addressUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Inactivate_logged_in_user_address_successfully()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressDeleteDTOFaker = AddressFaker.AddressDeleteDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(addressDeleteDTOFaker.AddressId),
                Active = true
            });

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.InactivateLoggedUserAddressAsync(addressDeleteDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Inactivate_logged_in_user_address_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressIdentifierDTOFaker = AddressFaker.AddressDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x
                        .FindByIdAsync(userId))
                        .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.InactivateLoggedUserAddressAsync(addressIdentifierDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Inactivate_logged_in_user_address_not_found()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            var addressUpdateDTOFaker = AddressFaker.AddressEntity().Generate();
            var addressIdentifierDTOFaker = AddressFaker.AddressDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x
                         .FindByIdAsync(userId))
                         .ReturnsAsync(value: null);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.InactivateLoggedUserAddressAsync(addressIdentifierDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_address_by_filter()
        {
            //Arrange
            var addressEntityFaker = AddressFaker.AddressEntity().Generate(3);
            var addressFilterDto = AddressFaker.AddressFilterDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x.GetLoggedUserAddressesAsync(userId, addressFilterDto))
                .ReturnsAsync(addressEntityFaker);

            _mockUserRepository.Setup(x => x.GetTotalLoggedUserAddressesAsync(userId, addressFilterDto))
                .ReturnsAsync(addressEntityFaker.Count);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedUserAddressesAsync(addressFilterDto);

            //Assert
            Assert.True(result.Data.Any() && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_address_by_filter_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressFilterDto = AddressFaker.AddressFilterDTO().Generate();

            _mockUserRepository.Setup(x => x.GetLoggedUserAddressesAsync(userId, addressFilterDto))
                        .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedUserAddressesAsync(addressFilterDto);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_address()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressIdentifierDTOFaker = AddressFaker.AddressIdentifierDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(addressIdentifierDTOFaker.AddressId)
            });

            _mockUserRepository.Setup(x => x
                .FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedUserAddressAsync(addressIdentifierDTOFaker);

            //Assert
            Assert.True(result.Data != null && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_address_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressIdentifierDTOFaker = AddressFaker.AddressIdentifierDTO().Generate();

            _mockUserRepository.Setup(x => x
                        .FindByIdAsync(userId))
                        .ThrowsAsync(new Exception());

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedUserAddressAsync(addressIdentifierDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_logged_in_user_null()
        {
            //Arrange
            var userId = _claimNameIdentifier;
            var addressIdentifierDTOFaker = AddressFaker.AddressIdentifierDTO().Generate();

            _mockUserRepository.Setup(x => x
                        .FindByIdAsync(userId)).ReturnsAsync(value: null);

            var registerService = RegisterServiceConstrutor();

            //Act
            var result = await registerService.GetLoggedUserAddressAsync(addressIdentifierDTOFaker);

            //Assert
            Assert.Null(result.Data);
        }

        private RegisterService RegisterServiceConstrutor()
        {
            return new RegisterService(
                _mapper,
                _mockHttpContextAccessor.Object,
                _mockBackgroundJobClient.Object,
                _mockEmailService.Object,
                _mockUserRepository.Object,
                _mockRoleRepository.Object);
        }
    }
}
