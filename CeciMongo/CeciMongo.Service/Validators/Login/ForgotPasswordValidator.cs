using CeciMongo.Domain.DTO.Auth;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.Login
{
    /// <summary>
    /// Validator for validating the forgot password request.
    /// </summary>
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public ForgotPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c.Email)
                .EmailAddress()
                .Must(email => {
                    return RegisteredEmail(email);
                }).WithMessage("E-mail not found.");
        }

        /// <summary>
        /// Checks if the provided email is registered.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>True if the email is registered; otherwise, false.</returns>
        private bool RegisteredEmail(string email)
        {
            return _userRepository.FindOne(x=> x.Email.Equals(email)) != null;
        }
    }
}
