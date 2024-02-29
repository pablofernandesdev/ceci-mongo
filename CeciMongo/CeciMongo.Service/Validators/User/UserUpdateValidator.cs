using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.User
{
    /// <summary>
    /// Validator for updating a user.
    /// </summary>
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateValidator"/> class.
        /// </summary>
        /// <param name="roleRepository">The <see cref="IRoleRepository"/> instance used to access role database operations.</param>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access user-related database operations.</param>
        public UserUpdateValidator(IRoleRepository roleRepository, 
            IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;

            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("Please enter the identifier user.")
                .NotNull().WithMessage("Please enter the identifier user.")
                .Must(userId => {
                    return UserValid(userId);
                }).WithMessage("User invalid.");

            RuleFor(c => c.RoleId)
                .NotNull().WithMessage("Please enter the role.")
                .Must(roleId => {
                    return RoleValid(roleId);
                }).WithMessage("Role invalid.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Please enter the name.")
                .NotNull().WithMessage("Please enter the name.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Please enter the email user.")
                .NotNull().WithMessage("Please enter the email user.")
                .EmailAddress().WithMessage("Please enter the valid email user.");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Please enter the password.")
                .NotNull().WithMessage("Please enter the password.");
        }

        /// <summary>
        /// Validates if the provided role ID is valid.
        /// </summary>
        /// <param name="roleId">The role ID.</param>
        /// <returns>Returns true if the role is valid; otherwise, false.</returns>
        private bool RoleValid(string roleId)
        {
            return _roleRepository.FindById(roleId) != null;
        }

        /// <summary>
        /// Validates if the provided user ID is valid.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>Returns true if the user is valid; otherwise, false.</returns>
        private bool UserValid(string userId)
        {
            return _userRepository.FindById(userId) != null;
        }
    }
}
