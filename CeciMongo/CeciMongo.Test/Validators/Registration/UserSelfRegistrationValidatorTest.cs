using CeciMongo.Domain.DTO.Register;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Registration;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;
using System;
using Xunit;

namespace CeciMongo.Test.Validators.Registration
{
    public class UserSelfRegistrationValidatorTest
    {
        private readonly UserSelfRegistrationValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public UserSelfRegistrationValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new UserSelfRegistrationValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new UserSelfRegistrationDTO();

            _mockUserRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .Returns(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.Email);
            result.ShouldHaveValidationErrorFor(user => user.Name);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = UserFaker.UserSelfRegistrationDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.Email);
            result.ShouldNotHaveValidationErrorFor(user => user.Name);
            result.ShouldNotHaveValidationErrorFor(user => user.Password);
        }
    }
}
