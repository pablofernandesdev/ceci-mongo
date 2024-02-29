using AutoMapper;
using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Mapping;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Role;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class RoleServiceTest
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<RoleService>> _mockLogger;

        public RoleServiceTest()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockLogger = new Mock<ILogger<RoleService>>();

            //Auto mapper configuration
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Add_role_successfully()
        {
            //Arrange
            var roleEntityFaker = RoleFaker.RoleEntity().Generate();
            var userAddDTO = _mapper.Map<RoleAddDTO>(roleEntityFaker);

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.AddAsync(userAddDTO);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Add_role_exception()
        {
            //Arrange
            _mockRoleRepository.Setup(x => x.InsertOneAsync(It.IsAny<Role>()))
                .ThrowsAsync(new Exception());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.AddAsync(It.IsAny<RoleAddDTO>());

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Delete_role_successfully()
        {
            //Arrange
            var idRole = "1";
            _mockRoleRepository.Setup(x => x.FindByIdAsync(idRole))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.DeleteAsync(idRole);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Delete_role_not_found()
        {
            //Arrange
            var idRole = "1";
            _mockRoleRepository.Setup(x => x.FindByIdAsync(idRole))
                .ReturnsAsync(value: null);

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.DeleteAsync(idRole);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Delete_role_exception()
        {
            //Arrange
            var idRole = "1";
            _mockRoleRepository.Setup(x => x.FindByIdAsync(idRole))
                .ThrowsAsync(new Exception());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.DeleteAsync(idRole);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_role_successfully()
        {
            //Arrange
            var roleUpdateDTOFaker = RoleFaker.RoleUpdateDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(roleUpdateDTOFaker.RoleId))
                .ReturnsAsync(_mapper.Map<Role>(roleUpdateDTOFaker));

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.UpdateAsync(roleUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Update_role_not_found()
        {
            //Arrange
            var roleUpdateDTOFaker = RoleFaker.RoleUpdateDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(roleUpdateDTOFaker.RoleId))
                .ReturnsAsync(value: null);

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.UpdateAsync(roleUpdateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Update_role_exception()
        {
            //Arrange
            var roleUpdateDTOFaker = RoleFaker.RoleUpdateDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(roleUpdateDTOFaker.RoleId))
                .ThrowsAsync(new Exception());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.UpdateAsync(roleUpdateDTOFaker);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_role_by_id()
        {
            //Arrange
            var idRole = "1";
            _mockRoleRepository.Setup(x => x.FindByIdAsync(idRole))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.GetByIdAsync(idRole);

            //Assert
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Get_role_by_id_exception()
        {
            //Arrange
            var idRole = "1";
            _mockRoleRepository.Setup(x => x.FindByIdAsync(idRole))
                .ThrowsAsync(new Exception());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.GetByIdAsync(idRole);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_all_roles()
        {
            //Arrange
            _mockRoleRepository.Setup(x => x.FilterByAsync(x=> x.Active))
                .ReturnsAsync(RoleFaker.RoleEntity().Generate(2));

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.GetAsync();

            //Assert
            Assert.True(result.Data.Any() && result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Get_all_users_exception()
        {
            //Arrange
            _mockRoleRepository.Setup(x => x.FilterByAsync(x => x.Active))
                  .ThrowsAsync(new Exception());

            var roleService = RoleServiceConstrutor();

            //Act
            var result = await roleService.GetAsync();

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        private RoleService RoleServiceConstrutor()
        {
            return new RoleService(
                _mapper,
                _mockLogger.Object,
                _mockRoleRepository.Object);
        }
    }
}
