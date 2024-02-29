using CeciMongo.Domain.DTO.Register;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Helper;
using FluentValidation;

namespace CeciMongo.Service.Validators.Registration
{
    /// <summary>
    /// Validator for validating the self-registration request of a user.
    /// </summary>
    public class UserSelfRegistrationValidator : AbstractValidator<UserSelfRegistrationDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSelfRegistrationValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public UserSelfRegistrationValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Please enter the name.")
                .NotNull().WithMessage("Please enter the name.");

            RuleFor(c => c.Email)
                .EmailAddress()
                .Must(email => {
                    return !RegisteredEmail(email);
                }).WithMessage("E-mail already registered.");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Please enter the password.")
                .NotNull().WithMessage("Please enter the password.");

            When(c => !string.IsNullOrEmpty(c.Password), () => {
                RuleFor(c => c.Password)
                    .Must(c => StringHelper.IsBase64String(c))
                    .WithMessage("Password must be base64 encoded.");
            });
        }

        /// <summary>
        /// Checks if the provided email is already registered.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns><c>true</c> if the email is not registered; otherwise, <c>false</c>.</returns>
        private bool RegisteredEmail(string email)
        {
            return _userRepository.FindOne(x=> x.Email.Equals(email)) != null;
        }
    }
}
