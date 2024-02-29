using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.User
{
    /// <summary>
    /// Validator for deleting a user.
    /// </summary>
    public class UserDeleteValidator : AbstractValidator<UserDeleteDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDeleteValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public UserDeleteValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("Please enter the identifier user.")
                .NotNull().WithMessage("Please enter the identifier user.")
                .Must(userId => {
                    return UserValid(userId);
                }).WithMessage("User invalid.");
        }

        /// <summary>
        /// Checks if the user is valid.
        /// </summary>
        /// <param name="userId">The user identifier to validate.</param>
        /// <returns>True if the user is valid, otherwise false.</returns>
        private bool UserValid(string userId)
        {
            return _userRepository.FindByIdAsync(userId) != null;
        }
    }
}
