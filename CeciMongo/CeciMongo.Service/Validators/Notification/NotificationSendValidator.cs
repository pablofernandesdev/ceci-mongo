using CeciMongo.Domain.DTO.Notification;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;

namespace CeciMongo.Service.Validators.Notification
{
    /// <summary>
    /// Validator for validating the notification send request.
    /// </summary>
    public class NotificationSendValidator : AbstractValidator<NotificationSendDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSendValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public NotificationSendValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c.IdUser)
              .NotNull().WithMessage("Please enter the user identifier.")
              .Must(userId => {
                  return UserValid(userId);
              }).WithMessage("User invalid.");

            RuleFor(c => c.Title)
               .NotEmpty().WithMessage("Please enter the notification title.")
               .NotNull().WithMessage("Please enter the notification title.");

            RuleFor(c => c.Body)
               .NotEmpty().WithMessage("Please enter the notification body.")
               .NotNull().WithMessage("Please enter the notification body.");
        }

        /// <summary>
        /// Validates if the user is valid based on the provided user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the user is valid; otherwise, false.</returns>
        private bool UserValid(string userId)
        {
            return _userRepository.FindById(userId) != null;
        }
    }
}
