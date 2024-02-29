using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.User;
using CeciMongo.Test.Fakers.Role;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;
using System;
using Xunit;

namespace CeciMongo.Test.Validators.User
{
    public class UserAddValidatorTest
    {
        private readonly UserAddValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        public UserAddValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _validator = new UserAddValidator(_mockRoleRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new UserAddDTO { Email = "asdf"};

            _mockRoleRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            _mockUserRepository.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.RoleId);
            result.ShouldHaveValidationErrorFor(user => user.Name);
            result.ShouldHaveValidationErrorFor(user => user.Email);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = UserFaker.UserAddDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindById(model.RoleId))
                .Returns(RoleFaker.RoleEntity().Generate());

            _mockUserRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .Returns(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.RoleId);
            result.ShouldNotHaveValidationErrorFor(user => user.Name);
            result.ShouldNotHaveValidationErrorFor(user => user.Email);
            result.ShouldNotHaveValidationErrorFor(user => user.Password);
        }
    }
}
