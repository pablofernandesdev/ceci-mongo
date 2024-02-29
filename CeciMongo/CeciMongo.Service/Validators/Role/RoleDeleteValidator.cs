using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.Role
{
    /// <summary>
    /// Validator for deleting a role.
    /// </summary>
    public class RoleDeleteValidator : AbstractValidator<RoleDeleteDTO>
    {
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleDeleteValidator"/> class.
        /// </summary>
        /// <param name="roleRepository">The <see cref="IRoleRepository"/> instance used to access database operations.</param>
        public RoleDeleteValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(c => c.RoleId)
                .NotEmpty().WithMessage("Please enter the identifier role.")
                .NotNull().WithMessage("Please enter the identifier role.")
                .Must(roleId => {
                    return RoleValid(roleId);
                }).WithMessage("Role invalid.");
        }

        /// <summary>
        /// Checks if the role with the given identifier is valid.
        /// </summary>
        /// <param name="roleId">The role identifier to validate.</param>
        /// <returns>Returns true if the role is valid; otherwise, false.</returns>
        private bool RoleValid(string roleId)
        {
            return _roleRepository.FindById(roleId) != null;
        }
    }
}
