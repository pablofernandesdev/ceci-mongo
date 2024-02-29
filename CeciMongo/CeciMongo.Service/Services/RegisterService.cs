using AutoMapper;
using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Email;
using CeciMongo.Domain.DTO.Register;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Infra.CrossCutting.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service for managing registrations and users.
    /// </summary>
    public class RegisterService : IRegisterService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterService"/> class.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="httpContextAccessor">The HttpContextAccessor to access the current HttpContext.</param>
        /// <param name="jobClient">The Hangfire IBackgroundJobClient for background job scheduling.</param>
        /// <param name="emailService">The email service for sending emails.</param>
        /// <param name="userRepository">The user repository for accessing user data.</param>
        /// <param name="roleRepository">The role repository for accessing role data.</param>
        public RegisterService(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IBackgroundJobClient jobClient,
            IEmailService emailService,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _jobClient = jobClient;
            _emailService = emailService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Get the logged-in user.
        /// </summary>
        /// <returns>Response with the logged-in user.</returns>
        public async Task<ResultResponse<UserResultDTO>> GetLoggedInUserAsync()
        {
            var response = new ResultResponse<UserResultDTO>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                response.Data = _mapper.Map<UserResultDTO>(user);
            }
            catch (Exception ex)
            {
                response.Message = "Error retrieving logged-in user.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Perform self-registration for a new user.
        /// </summary>
        /// <param name="obj">User registration data.</param>
        /// <returns>Response indicating the result of the registration.</returns>
        public async Task<ResultResponse> SelfRegistrationAsync(UserSelfRegistrationDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var basicProfile = await _roleRepository.GetBasicProfile();

                obj.Password = PasswordExtension.EncryptPassword(obj.Password);

                var newUser = _mapper.Map<User>(obj);
                newUser.Role = basicProfile;

                await _userRepository.InsertOneAsync(newUser);

                response.Message = "User successfully added.";

                _jobClient.Enqueue(() => _emailService.SendEmailAsync(new EmailRequestDTO
                {
                    Body = "User successfully added.",
                    Subject = obj.Name,
                    ToEmail = obj.Email
                }));
            }
            catch (Exception ex)
            {
                response.Message = "Could not add user.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Update the logged-in user's data.
        /// </summary>
        /// <param name="obj">Updated user data.</param>
        /// <returns>Response indicating the result of the update.</returns>
        public async Task<ResultResponse> UpdateLoggedUserAsync(UserLoggedUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var emailRegistered = await _userRepository
                    .FindOneAsync(c => c.Email == obj.Email && c.Id.ToString() != userId);

                if (emailRegistered != null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "E-mail already registered";
                    return response;
                }

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                user = _mapper.Map(obj, user);

                await _userRepository.ReplaceOneAsync(user);

                response.Message = "User successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated user.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Redefines the password for the logged-in user.
        /// </summary>
        /// <param name="obj">Object containing the current and new passwords.</param>
        /// <returns>Response indicating the result of the password redefinition.</returns>
        public async Task<ResultResponse> RedefinePasswordAsync(UserRedefinePasswordDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                if (!PasswordExtension.DecryptPassword(user.Password).Equals(obj.CurrentPassword))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    response.Message = "Password incorret.";
                    return response;
                };

                user.Password = PasswordExtension.EncryptPassword(obj.NewPassword);

                await _userRepository.ReplaceOneAsync(user);

                response.Message = "Password user successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated password user.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Adds a new address for the logged-in user.
        /// </summary>
        /// <param name="obj">Object containing the address data.</param>
        /// <returns>Response indicating the result of the address addition.</returns>
        public async Task<ResultResponse> AddLoggedUserAddressAsync(AddressLoggedUserAddDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is not null)
                {
                    if (obj.Main)
                    {
                        user.Adresses.ForEach(a => a.Main = false);
                    }

                    user.Adresses.ToList().Add(_mapper.Map<Address>(obj));

                    await _userRepository.ReplaceOneAsync(user);
                }

                response.Message = "Address successfully added.";

            }
            catch (Exception ex)
            {
                response.Message = "Could not add address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Updates an address for the logged-in user.
        /// </summary>
        /// <param name="obj">Object containing the updated address data.</param>
        /// <returns>Response indicating the result of the address update.</returns>
        public async Task<ResultResponse> UpdateLoggedUserAddressAsync(AddressLoggedUserUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is not null)
                {
                    if (obj.Main)
                    {
                        user.Adresses.ForEach(a => a.Main = false);
                    }

                    foreach (var item in user.Adresses.Where(x => x.Id.ToString().Equals(obj.AddressId)))
                    {
                        item.Street = obj.Street;
                        item.Uf = obj.Uf;
                        item.Number = obj.Number;
                        item.Complement = obj.Complement;
                        item.District = obj.District;
                        item.Locality = obj.Locality;
                        item.ZipCode = obj.ZipCode;
                        item.Main = obj.Main;
                    }

                    await _userRepository.ReplaceOneAsync(user);
                }

                response.Message = "Address successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Inactivates an address for the logged-in user.
        /// </summary>
        /// <param name="obj">Object containing the address identifier.</param>
        /// <returns>Response indicating the result of the address deactivation.</returns>
        public async Task<ResultResponse> InactivateLoggedUserAddressAsync(AddressDeleteDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is not null)
                {
                    foreach (var item in user.Adresses.Where(x => x.Id.ToString().Equals(obj.AddressId)))
                    {
                        item.Active = false;
                    }

                    await _userRepository.ReplaceOneAsync(user);
                }

                response.Message = "Address successfully deactivated.";

            }
            catch (Exception ex)
            {
                response.Message = "Could not deactivated address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Retrieves the addresses for the logged-in user.
        /// </summary>
        /// <param name="filter">Object containing the filter criteria for the addresses.</param>
        /// <returns>Response containing the addresses and additional information.</returns>
        public async Task<ResultDataResponse<IEnumerable<AddressResultDTO>>> GetLoggedUserAddressesAsync(AddressFilterDTO filter)
        {
            var response = new ResultDataResponse<IEnumerable<AddressResultDTO>>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                response.Data = _mapper.Map<IEnumerable<AddressResultDTO>>(await _userRepository.GetLoggedUserAddressesAsync(userId, filter));
                response.TotalItems = await _userRepository.GetTotalLoggedUserAddressesAsync(userId, filter);
                response.TotalPages = (int)Math.Ceiling((double)response.TotalItems / filter.PerPage);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Retrieves a specific address for the logged-in user.
        /// </summary>
        /// <param name="obj">Object containing the address identifier.</param>
        /// <returns>Response containing the address data.</returns>
        public async Task<ResultResponse<AddressResultDTO>> GetLoggedUserAddressAsync(AddressIdentifierDTO obj)
        {
            var response = new ResultResponse<AddressResultDTO>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                if (user is not null)
                {
                    response.Data = _mapper.Map<AddressResultDTO>(user.Adresses.Where(x => x.Main).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }
}
