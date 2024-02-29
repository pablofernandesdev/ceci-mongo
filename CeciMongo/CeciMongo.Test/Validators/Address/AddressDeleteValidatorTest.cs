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
    public class AddressDeleteValidatorTest
    {
        private readonly AddressDeleteValidator _validator;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public AddressDeleteValidatorTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _validator = new AddressDeleteValidator(_mockUserRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new AddressDeleteDTO();

            _mockUserRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(user => user.AddressId);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var addressDeleteDTOFaker = AddressFaker.AddressDeleteDTO().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();
            userEntityFaker.Adresses.Add(new Domain.Entities.Address
            {
                Id = MongoDB.Bson.ObjectId.Parse(addressDeleteDTOFaker.AddressId)
            });

            _mockUserRepository.Setup(x => x.FindByIdAsync(addressDeleteDTOFaker.UserId))
                .ReturnsAsync(userEntityFaker);

            //act
            var result = _validator.TestValidate(addressDeleteDTOFaker);

            //assert
            result.ShouldNotHaveValidationErrorFor(user => user.AddressId);
        }
    }
}
