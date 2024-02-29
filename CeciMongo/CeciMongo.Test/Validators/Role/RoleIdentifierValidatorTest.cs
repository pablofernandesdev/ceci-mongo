using CeciMongo.Domain.DTO.Role;
using CeciMongo.Service.Validators.Role;
using CeciMongo.Test.Fakers.Role;
using FluentValidation.TestHelper;
using Xunit;

namespace CeciMongo.Test.Validators.Role
{
    public class RoleIdentifierValidatorTest
    {
        private readonly RoleIdentifierValidator _validator;

        public RoleIdentifierValidatorTest()
        {
            _validator = new RoleIdentifierValidator();
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new IdentifierRoleDTO();

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(role => role.RoleId);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = RoleFaker.IdentifierRoleDTO().Generate();

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(role => role.RoleId);
        }
    }
}
