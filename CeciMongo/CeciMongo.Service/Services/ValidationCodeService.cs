using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Email;
using CeciMongo.Domain.DTO.ValidationCode;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Infra.CrossCutting.Helper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service responsible for managing the logic of generating and validating validation codes.
    /// </summary>
    public class ValidationCodeService : IValidationCodeService
    {
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IValidationCodeRepository _validationCodeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationCodeService"/> class.
        /// </summary>
        /// <param name="emailService">The <see cref="IEmailService"/> instance used to send emails.</param>
        /// <param name="jobClient">The <see cref="IBackgroundJobClient"/> instance used to enqueue background jobs.</param>
        /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> instance used to access the current HTTP context.</param>
        /// <param name="userRepository">An instance of the <see cref="IUserRepository"/> used for user-related database operations.</param>
        /// <param name="validationCodeRepository">An instance of the <see cref="IValidationCodeRepository"/> used for user-related database operations.</param>

        public ValidationCodeService(
            IEmailService emailService,
            IBackgroundJobClient jobClient,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IValidationCodeRepository validationCodeRepository)
        {
            _emailService = emailService;
            _jobClient = jobClient;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _validationCodeRepository = validationCodeRepository;
        }

        /// <summary>
        /// Sends a validation code to the currently authenticated user.
        /// </summary>
        /// <returns>A <see cref="ResultResponse"/> object indicating the result of the operation.</returns>
        public async Task<ResultResponse> SendAsync()
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var user = await _userRepository.FindByIdAsync(userId);

                var code = PasswordExtension.GeneratePassword(0, 0, 6, 0);
                
                await _validationCodeRepository.InsertOneAsync(new ValidationCode {
                    Code = PasswordExtension.EncryptPassword(StringHelper.Base64Encode(code)),
                    UserId = userId,
                    Expires = System.DateTime.UtcNow.AddMinutes(10)
                });

                user.Validated = false;
                await _userRepository.ReplaceOneAsync(user);

                response.Message = "Code sent successfully.";

                EnqueueEmailSending(user.Name, user.Email, code);
            }
            catch (Exception ex)
            {
                response.Message = "Could not send code.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Validates a validation code provided by the user.
        /// </summary>
        /// <param name="obj">A <see cref="ValidationCodeValidateDTO"/> object containing the validation code to be validated.</param>
        /// <returns>A <see cref="ResultResponse"/> object indicating the result of the operation.</returns>
        public async Task<ResultResponse> ValidateCodeAsync(ValidationCodeValidateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId();

                var validationCode = await _validationCodeRepository.FilterByAsync(x => x.UserId.Equals(userId));

                if (validationCode == null || !IsValidCode(validationCode.OrderByDescending(x=> x.CreatedAt).FirstOrDefault(), obj.Code))
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = "Invalid or expired validation code.";
                    return response;
                }

                var user = await _userRepository.FindByIdAsync(userId);

                user.Validated = true;
                await _userRepository.ReplaceOneAsync(user);

                response.Message = "Code validated successfully.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not validate code.";
                response.Exception = ex;
            }

            return response;
        }

        private bool IsValidCode(ValidationCode validationCode, string code)
        {
            return PasswordExtension.DecryptPassword(validationCode.Code).Equals(code)
                   && !validationCode.IsExpired;
        }

        private void EnqueueEmailSending(string userName, string userEmail, string code)
        {
            _jobClient.Enqueue(() => _emailService.SendEmailAsync(new EmailRequestDTO
            {
                Body = $"A new validation code was requested. Use the code <b>{code}</b> to complete validation.",
                Subject = userName,
                ToEmail = userEmail
            }));
        }
    }
}
