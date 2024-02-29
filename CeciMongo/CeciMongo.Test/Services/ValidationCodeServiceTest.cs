using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.Email;
using CeciMongo.Test.Fakers.User;
using CeciMongo.Test.Fakers.ValidationCode;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class ValidationCodeServiceTest
    {
        private readonly string _claimNameIdentifier = "1";
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IValidationCodeRepository> _mockValidationCodeRepository;

        public ValidationCodeServiceTest()
        {
            _mockEmailService = new Mock<IEmailService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockValidationCodeRepository = new Mock<IValidationCodeRepository>();

            //http context configuration
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, _claimNameIdentifier),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(user);
        }

        [Fact]
        public async Task Send_new_code_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate();
            var emailRequestDTOFaker = EmailFaker.EmailRequestDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(userEntityFaker);

            _mockEmailService.Setup(x => x.SendEmailAsync(emailRequestDTOFaker))
                .ReturnsAsync(ResultResponseFaker.ResultResponse(HttpStatusCode.OK).Generate());

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.SendAsync();

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Send_new_code_exception()
        {
            //Arrange
            var userId = _claimNameIdentifier;

            _mockUserRepository.Setup(x => x.FindByIdAsync(userId))
                     .ThrowsAsync(new Exception());

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.SendAsync();

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Validate_code_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();

            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate(2);
            validationCodeEntityFaker.OrderByDescending(x=> x.CreatedAt).FirstOrDefault().Code = PasswordExtension.EncryptPassword(validateCodeValidateDTOFaker.Code);

            var userId = _claimNameIdentifier; 

            _mockValidationCodeRepository.Setup(x => x
                .FilterByAsync(x=> x.UserId.Equals(userId)))
                .ReturnsAsync(validationCodeEntityFaker);

            _mockUserRepository.Setup(x => x.FindByIdAsync(userId))
                     .ReturnsAsync(userEntityFaker);

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Validate_code_invalid_code()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var validationCodeEntityFaker = ValidationCodeFaker.ValidationCodeEntity().Generate(3);
            var userId = _claimNameIdentifier;

            _mockValidationCodeRepository.Setup(x => x
                .FilterByAsync(x => x.UserId.Equals(userId)))
                .ReturnsAsync(validationCodeEntityFaker);

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Validate_code_not_found()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockValidationCodeRepository.Setup(x => x
                     .FilterByAsync(x => x.UserId.Equals(userId)))
                     .ReturnsAsync(value: null);

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task Validate_code_exception()
        {
            //Arrange
            var validateCodeValidateDTOFaker = ValidationCodeFaker.ValidationCodeValidateDTO().Generate();
            var userId = _claimNameIdentifier;

            _mockValidationCodeRepository.Setup(x => x
                     .FilterByAsync(x => x.UserId.Equals(userId)))
                     .ThrowsAsync(new Exception());

            var validationCodeService = ValidationCodeServiceConstrutor();

            //Act
            var result = await validationCodeService.ValidateCodeAsync(validateCodeValidateDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        private ValidationCodeService ValidationCodeServiceConstrutor()
        {
            return new ValidationCodeService(
                _mockEmailService.Object,
                _mockBackgroundJobClient.Object,
                _mockHttpContextAccessor.Object,
                _mockUserRepository.Object,
                _mockValidationCodeRepository.Object);
        }
    }
}
