using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.User;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.User
{
    public class UserDeleteValidatorTest
    {
        private readonly UserDeleteValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public UserDeleteValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new UserDeleteValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new UserDeleteDTO();

            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.UserId);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = UserFaker.UserDeleteDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(model.UserId))
                .ReturnsAsync(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.UserId);
        }
    }
}
