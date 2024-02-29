using CeciMongo.Domain.DTO.Notification;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Notification;
using CeciMongo.Test.Fakers.Notification;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.Notification
{
    public class NotificationSendValidatorTest
    {
        private readonly NotificationSendValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public NotificationSendValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new NotificationSendValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new NotificationSendDTO();

            _mockUserRepository.Setup(x => x.FindByIdAsync(model.IdUser))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.IdUser);
            result.ShouldHaveValidationErrorFor(user => user.Title);
            result.ShouldHaveValidationErrorFor(user => user.Body);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = NotificationFaker.NotificationSendDTO().Generate();

            _mockUserRepository.Setup(x => x.FindById(model.IdUser))
                .Returns(UserFaker.UserEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.IdUser);
            result.ShouldNotHaveValidationErrorFor(user => user.Title);
            result.ShouldNotHaveValidationErrorFor(user => user.Body);
        }
    }
}
