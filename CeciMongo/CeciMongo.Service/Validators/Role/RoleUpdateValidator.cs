using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.Role
{
    /// <summary>
    /// Validator for updating a role.
    /// </summary>
    public class RoleUpdateValidator : AbstractValidator<RoleUpdateDTO>
    {
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleUpdateValidator"/> class.
        /// </summary>
        /// <param name="roleRepository">The <see cref="IRoleRepository"/> instance used to access database operations.</param>
        public RoleUpdateValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(c => c.RoleId)
                .NotNull().WithMessage("Please enter the role.")
                .Must(roleId => {
                    return RoleValid(roleId);
                }).WithMessage("Role invalid.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Please enter the name.")
                .NotNull().WithMessage("Please enter the name.");
        }

        /// <summary>
        /// Checks if the role is valid.
        /// </summary>
        /// <param name="roleId">The role identifier to validate.</param>
        /// <returns>True if the role is valid, otherwise false.</returns>
        private bool RoleValid(string roleId)
        {
            return _roleRepository.FindById(roleId) != null;
        }
    }
}
