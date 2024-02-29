using CeciMongo.Domain.DTO.Auth;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Login;
using CeciMongo.Test.Fakers.Auth;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;
using System;
using Xunit;

namespace CeciMongo.Test.Validators.Login
{
    public class ForgotPasswordValidatorTest
    {
        private readonly ForgotPasswordValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public ForgotPasswordValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new ForgotPasswordValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new ForgotPasswordDTO();

            _mockUserRepository.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.Email);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = AuthFaker.ForgotPasswordDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<CeciMongo.Domain.Entities.User, bool>>>()))
                .Returns(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.Email);
        }
    }
}
