using AutoMapper;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Domain.Mapping;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.Email;
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
    public class UserServiceTest
    {
        private readonly string _claimNameIdentifier = "1";
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly IMapper _mapper;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

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
        public async Task Add_user_successfully()
        {
            //Arrange
            var userAddDTOFaker = UserFaker.UserAddDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var emailRequestDTOFaker = EmailFaker.EmailRequestDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(userAddDTOFaker.RoleId))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            _mockEmailService.Setup(x => x.SendEmailAsync(emailRequestDTOFaker))
                .ReturnsAsync(ResultResponseFaker.ResultResponse(HttpStatusCode.OK).Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.AddAsync(userAddDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_to_add_user()
        {
            //Arrange
            var userAddDTOFaker = UserFaker.UserAddDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var emailRequestDTOFaker = EmailFaker.EmailRequestDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(userAddDTOFaker.RoleId))
                .ReturnsAsync(value: null);

            _mockEmailService.Setup(x => x.SendEmailAsync(emailRequestDTOFaker))
                .ReturnsAsync(ResultResponseFaker.ResultResponse(HttpStatusCode.BadRequest).Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.AddAsync(userAddDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_all_users()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate(3);
            var userFilterDto = UserFaker.UserFilterDTO().Generate();

            _mockUserRepository.Setup(x => x.GetByFilterAsync(userFilterDto))
                .ReturnsAsync(userEntityFaker);

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.GetAsync(userFilterDto);

            //Assert
            Assert.True(result.Data.Any() && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_get_all_users_by_filter()
        {
            //Arrange
            var userFilterDto = UserFaker.UserFilterDTO().Generate();
            _mockUserRepository.Setup(x => x.GetByFilterAsync(userFilterDto))
                .ThrowsAsync(new Exception());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.GetAsync(userFilterDto);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Delete_user_successfully()
        {
            //Arrange
            var userDeleteDTOFaker = UserFaker.UserDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(userDeleteDTOFaker.UserId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.DeleteAsync(userDeleteDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_delete_user()
        {
            //Arrange
            var userDeleteDTOFaker = UserFaker.UserDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(userDeleteDTOFaker.UserId))
                .ThrowsAsync(new Exception());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.DeleteAsync(userDeleteDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_user_successfully()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserUpdateDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(c => c.Email == userUpdateDTOFaker.Email && c.Id.ToString() != userUpdateDTOFaker.UserId))
                .ReturnsAsync(value: null);

            _mockRoleRepository.Setup(x => x.FindByIdAsync(userUpdateDTOFaker.RoleId))
               .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            _mockUserRepository.Setup(x => x.FindByIdAsync(userUpdateDTOFaker.UserId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.UpdateAsync(userUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

       
        [Fact]
        public async Task Failed_to_update_user_with_already_registered_email()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserUpdateDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(c => c.Email == userUpdateDTOFaker.Email && c.Id.ToString() != userUpdateDTOFaker.UserId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.UpdateAsync(userUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Failed_update_user()
        {
            //Arrange
            var userUpdateDTOFaker = UserFaker.UserUpdateDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(c => c.Email == userUpdateDTOFaker.Email && c.Id.ToString() != userUpdateDTOFaker.UserId))
                .ThrowsAsync(new Exception());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.UpdateAsync(userUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_user_role_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var userRoleUpdateDTOFaker = _mapper.Map<UserUpdateRoleDTO>(userEntityFaker);

            _mockRoleRepository.Setup(x => x.FindByIdAsync(userRoleUpdateDTOFaker.RoleId))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            _mockUserRepository.Setup(x => x.FindByIdAsync(userRoleUpdateDTOFaker.UserId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.UpdateRoleAsync(userRoleUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_user_role_exception()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var userRoleUpdateDTOFaker = _mapper.Map<UserUpdateRoleDTO>(userEntityFaker);

            _mockRoleRepository.Setup(x => x.FindByIdAsync(userRoleUpdateDTOFaker.RoleId))
                .ThrowsAsync(new Exception());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.UpdateRoleAsync(userRoleUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_user_by_id_successfully()
        {
            //Arrange
            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync(UserFaker.UserEntity().Generate());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.GetByIdAsync(It.IsAny<string>());

            //Assert
            Assert.True(result.Data != null && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_user_by_id_exception()
        {
            //Arrange
            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var userService = UserServiceConstrutor();

            //Act
            var result = await userService.GetByIdAsync(It.IsAny<string>());

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        private UserService UserServiceConstrutor()
        {
            return new UserService(
                _mapper, 
                _mockEmailService.Object, 
                _mockBackgroundJobClient.Object,
                _mockUserRepository.Object,
                _mockRoleRepository.Object);
        }
    }
}
