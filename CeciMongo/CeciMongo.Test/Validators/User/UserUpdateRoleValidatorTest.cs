using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.User;
using CeciMongo.Test.Fakers.Role;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.User
{
    public class UserUpdateRoleValidatorTest
    {
        private UserUpdateRoleValidator validator;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        public UserUpdateRoleValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            validator = new UserUpdateRoleValidator(_mockRoleRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new UserUpdateRoleDTO();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(value: null);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.RoleId);
            result.ShouldHaveValidationErrorFor(user => user.UserId);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = UserFaker.UserUpdateRoleDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindById(model.RoleId))
                .Returns(RoleFaker.RoleEntity().Generate());

            _mockUserRepository.Setup(x => x.FindById(model.UserId))
              .Returns(UserFaker.UserEntity().Generate());

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.RoleId);
            result.ShouldNotHaveValidationErrorFor(user => user.UserId);
        }
    }
}
