using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Role;
using CeciMongo.Test.Fakers.Role;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.Role
{
    public class RoleUpdateValidatorTest
    {
        private readonly RoleUpdateValidator _validator;
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        public RoleUpdateValidatorTest()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _validator = new RoleUpdateValidator(_mockRoleRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new RoleUpdateDTO();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(role => role.RoleId);
            result.ShouldHaveValidationErrorFor(role => role.Name);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = RoleFaker.RoleUpdateDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindById(model.RoleId))
                .Returns(RoleFaker.RoleEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(role => role.RoleId);
            result.ShouldNotHaveValidationErrorFor(role => role.Name);
        }
    }
}
