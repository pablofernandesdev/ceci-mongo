﻿using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.Interfaces.Repository;
using FluentValidation;
using System.Linq;

namespace CeciMongo.Service.Validators.Address
{
    /// <summary>
    /// Validator for validating the address update request.
    /// </summary>
    public class AddressUpdateValidator : AbstractValidator<AddressUpdateDTO>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressUpdateValidator"/> class.
        /// </summary>
        /// <param name="userRepository">The <see cref="IUserRepository"/> instance used to access database operations.</param>
        public AddressUpdateValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(c => c)
                .Must(c => {
                    return AddressValid(c);
                }).WithMessage("Address invalid.");

            RuleFor(c => c.AddressId)
                .NotEmpty().WithMessage("Please enter the address id.")
                .NotNull().WithMessage("Please enter the address id.");

            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("Please enter the identifier user.")
                .NotNull().WithMessage("Please enter the identifier user.")
                .Must(userId => {
                    return UserValid(userId);
                }).WithMessage("User invalid.");

            RuleFor(c => c.ZipCode)
                .NotEmpty().WithMessage("Please enter the zip code.")
                .NotNull().WithMessage("Please enter the zip code.");

            RuleFor(c => c.Street)
                .NotEmpty().WithMessage("Please enter the street.")
                .NotNull().WithMessage("Please enter the street.");

            RuleFor(c => c.District)
                 .NotEmpty().WithMessage("Please enter the district.")
                 .NotNull().WithMessage("Please enter the district.");

            RuleFor(c => c.Locality)
                 .NotEmpty().WithMessage("Please enter the locality.")
                 .NotNull().WithMessage("Please enter the locality.");

            RuleFor(c => c.Number)
                 .NotEmpty().WithMessage("Please enter the number.")
                 .NotNull().WithMessage("Please enter the number.");

            RuleFor(c => c.Uf)
                 .NotEmpty().WithMessage("Please enter the uf.")
                 .NotNull().WithMessage("Please enter the uf.");
        }

        private bool AddressValid(AddressUpdateDTO obj)
        {
            var user = _userRepository.FindById(obj.UserId);

            if (user is null)
            {
                return false;
            }

            return user.Adresses.FirstOrDefault(x => x.Id.Equals(obj.AddressId)) != null;
        }

        private bool UserValid(string userId)
        {
            return _userRepository.FindById(userId) != null;
        }
    }
}
