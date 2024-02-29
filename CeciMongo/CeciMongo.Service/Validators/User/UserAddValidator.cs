using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Helper;
using FluentValidation;

namespace CeciMongo.Service.Validators.User
{
    /// <summary>
    /// Validator for adding a user.
    /// </summary>
    public class UserAddValidator : AbstractValidator<UserAddDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAddValidator"/> class.
        /// </summary>
        /// <param name="roleRepository">The <see cref="IRoleRepository"/> instance used to access database operations.</param>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access user-related database operations.</param>
        public UserAddValidator(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;

            RuleFor(c => c.RoleId)
                .NotNull().WithMessage("Please enter the role.")
                .Must(roleId =>
                {
                    return RoleValid(roleId);
                }).WithMessage("Role invalid.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Please enter the name.")
                .NotNull().WithMessage("Please enter the name.");

            RuleFor(c => c.Email)
                .EmailAddress()
                .Must(email =>
                {
                    return !RegisteredEmail(email);
                }).WithMessage("E-mail already registered.");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Please enter the password.")
                .NotNull().WithMessage("Please enter the password.");

            When(c => !string.IsNullOrEmpty(c.Password), () =>
            {
                RuleFor(c => c.Password)
                    .Must(c => StringHelper.IsBase64String(c))
                    .WithMessage("Password must be base64 encoded.");
            });
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

        /// <summary>
        /// Checks if the email is already registered.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>True if the email is not registered, otherwise false.</returns>
        private bool RegisteredEmail(string email)
        {
            return _userRepository.FindOne(x => x.Email.Equals(email)) != null;
        }
    }
}
