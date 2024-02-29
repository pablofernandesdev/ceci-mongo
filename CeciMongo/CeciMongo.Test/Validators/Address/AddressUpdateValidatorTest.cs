using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Address;
using CeciMongo.Test.Fakers.Address;
using CeciMongo.Test.Fakers.User;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.Address
{
    public class AddressUpdateValidatorTest
    {
        private readonly AddressUpdateValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public AddressUpdateValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new AddressUpdateValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new AddressUpdateDTO();

            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.AddressId);
            result.ShouldHaveValidationErrorFor(user => user.UserId);
            result.ShouldHaveValidationErrorFor(user => user.ZipCode);
            result.ShouldHaveValidationErrorFor(user => user.Street);
            result.ShouldHaveValidationErrorFor(user => user.District);
            result.ShouldHaveValidationErrorFor(user => user.Locality);
            result.ShouldHaveValidationErrorFor(user => user.Number);
            result.ShouldHaveValidationErrorFor(user => user.Uf);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = AddressFaker.AddressUpdateDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Domain.Entities.Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(model.AddressId)
            });

            _mockUserRepository.Setup(x => x.FindById(model.UserId))
                .Returns(userEntityFaker);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.UserId);
            result.ShouldNotHaveValidationErrorFor(user => user.AddressId);
            result.ShouldNotHaveValidationErrorFor(user => user.ZipCode);
            result.ShouldNotHaveValidationErrorFor(user => user.Street);
            result.ShouldNotHaveValidationErrorFor(user => user.District);
            result.ShouldNotHaveValidationErrorFor(user => user.Locality);
            result.ShouldNotHaveValidationErrorFor(user => user.Number);
            result.ShouldNotHaveValidationErrorFor(user => user.Uf);
        }
    }
}
