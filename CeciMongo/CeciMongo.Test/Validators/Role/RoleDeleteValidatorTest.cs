using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Validators.Role;
using CeciMongo.Test.Fakers.Role;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CeciMongo.Test.Validators.Role
{
    public class RoleDeleteValidatorTest
    {
        private readonly RoleDeleteValidator _validator;
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        public RoleDeleteValidatorTest()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _validator = new RoleDeleteValidator(_mockRoleRepository.Object);
        }

        [Fact]
        public void There_should_be_an_error_when_properties_are_null()
        {
            //Arrange
            var model = new RoleDeleteDTO();

            _mockRoleRepository.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldHaveValidationErrorFor(role => role.RoleId);
        }

        [Fact]
        public void There_should_not_be_an_error_for_the_properties()
        {
            //Arrange
            var model = RoleFaker.RoleDeleteDTO().Generate();

            _mockRoleRepository.Setup(x => x.FindById(model.RoleId))
                .Returns(RoleFaker.RoleEntity().Generate());

            //act
            var result = _validator.TestValidate(model);

            //assert
            result.ShouldNotHaveValidationErrorFor(role => role.RoleId);
        }
    }
}
