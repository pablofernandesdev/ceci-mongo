using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;
using System.Linq;

namespace CeciMongo.Service.Validators.Address
{
    /// <summary>
    /// Validator for validating the data when deleting an address.
    /// </summary>
    public class AddressDeleteValidator : AbstractValidator<AddressDeleteDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressDeleteValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public AddressDeleteValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c.AddressId)
                .NotEmpty().WithMessage("Please enter the address id.")
                .NotNull().WithMessage("Please enter the address id.");

            RuleFor(c => c)
                .Must(c => {
                    return AddressValid(c);
                }).WithMessage("Address invalid.");
        }

        private bool AddressValid(AddressDeleteDTO obj)
        {
            var user = _userRepository.FindById(obj.UserId);

            if (user is null)
            {
                return false;
            }

            return user.Adresses.FirstOrDefault(x => x.Id.Equals(obj.AddressId)) != null;
        }
    }
}
