using AutoMapper;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Domain.Mapping;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Auth;
using CeciMongo.Test.Fakers.RefreshToken;
using CeciMongo.Test.Fakers.User;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IMapper _mapper;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshToken;

        public AuthServiceTest()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRefreshToken = new Mock<IRefreshTokenRepository>();

            //Auto mapper configuration
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Authenticate_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var loginDTOFaker = AuthFaker.LoginDTO().Generate();
            var userValidFaker = new CeciMongo.Domain.Entities.User
            {
                Id = userEntityFaker.Id,
                Name = userEntityFaker.Name,
                Email = userEntityFaker.Email,
                Password = userEntityFaker.Password,
                Role = userEntityFaker.Role
            };
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenEntity().Generate();
            //var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            userEntityFaker.Password = PasswordExtension.EncryptPassword(loginDTOFaker.Password);

            _mockUserRepository.Setup(x => x.FindOneAsync(x=> x.Email.Equals(loginDTOFaker.Username)))
                .ReturnsAsync(userEntityFaker);

            _mockUserRepository.Setup(x => x.FindByIdAsync(userEntityFaker.Id.ToString()))
                .ReturnsAsync(userValidFaker);

            //validar porque o valor esta sendo retornado nulo
            _mockTokenService.Setup(x => x.GenerateToken(_mapper.Map<UserResultDTO>(userValidFaker)))
                .Returns(It.IsAny<string>());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.AuthenticateAsync(loginDTOFaker, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Authenticate_unauthorized_password_incorret()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var loginDTOFaker = AuthFaker.LoginDTO().Generate();

            userEntityFaker.Password = PasswordExtension.EncryptPassword("bm92b3Rlc3Rl");

            _mockUserRepository.Setup(x => x.FindOneAsync(x => x.Email.Equals(loginDTOFaker.Username)))
                .ReturnsAsync(userEntityFaker);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.AuthenticateAsync(loginDTOFaker, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Authenticate_user_not_found()
        {
            //Arrange
            var loginDTOFaker = AuthFaker.LoginDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(x => x.Email.Equals(loginDTOFaker.Username)))
                .ReturnsAsync(value: null);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.AuthenticateAsync(loginDTOFaker, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Authenticate_exception()
        {
            //Arrange
            var loginDTOFaker = AuthFaker.LoginDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(x => x.Email.Equals(loginDTOFaker.Username)))
                .ThrowsAsync(new Exception());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.AuthenticateAsync(loginDTOFaker, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Refresh_token_successfully()
        {
            //Arrange
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenEntity().Generate();

            _mockRefreshToken.Setup(x => x.FindOneAsync(x=> x.Token.Equals(jwtToken)))
                .ReturnsAsync(refreshTokenFaker);

            //validar porque o valor esta sendo retornado nulo
            _mockTokenService.Setup(x => x.GenerateToken(_mapper.Map<UserResultDTO>(refreshTokenFaker.User)))
                .Returns(It.IsAny<string>());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RefreshTokenAsync(jwtToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Refresh_token_expired()
        {
            //Arrange
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenExpiredEntity().Generate();

            _mockRefreshToken.Setup(x => x.FindOneAsync(x => x.Token.Equals(jwtToken)))
                .ReturnsAsync(refreshTokenFaker);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RefreshTokenAsync(jwtToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Refresh_token_exception()
        {
            //Arrange
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            _mockRefreshToken.Setup(x => x.FindOneAsync(x => x.Token.Equals(jwtToken)))
                .ThrowsAsync(new Exception());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RefreshTokenAsync(jwtToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Revoke_token_successfully()
        {
            //Arrange
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenEntity().Generate();
            var refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

            _mockRefreshToken.Setup(x => x.FindOneAsync(x => x.Token.Equals(refreshToken) && x.IsActive))
                .ReturnsAsync(refreshTokenFaker);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RevokeTokenAsync(refreshToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Revoke_token_null_token()
        {
            //Arrange
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenEntity().Generate();
            var refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

            _mockRefreshToken.Setup(x => x.FindOneAsync(x => x.Token.Equals(refreshToken) && x.IsActive))
                .ReturnsAsync(value: null);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RevokeTokenAsync(refreshToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Revoke_token_exception()
        {
            //Arrange
            var refreshTokenFaker = RefreshTokenFaker.RefreshTokenEntity().Generate();
            var refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

            _mockRefreshToken.Setup(x => x.FindOneAsync(x => x.Token.Equals(refreshToken) && x.IsActive))
                 .ThrowsAsync(new Exception());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.RevokeTokenAsync(refreshToken, "127.0.0.1");

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        [Fact]
        public async Task Forgot_password_successfully()
        {
            //Arrange
            var userEntityFaker = UserFaker.UserEntity().Generate();
            var forgotPasswordDtoFaker = AuthFaker.ForgotPasswordDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(x => x.Email.Equals(forgotPasswordDtoFaker.Email)))
                .ReturnsAsync(userEntityFaker);

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.ForgotPasswordAsync(forgotPasswordDtoFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Forgot_password_exception()
        {
            //Arrange
            var forgotPasswordDtoFaker = AuthFaker.ForgotPasswordDTO().Generate();

            _mockUserRepository.Setup(x => x.FindOneAsync(x => x.Email.Equals(forgotPasswordDtoFaker.Email)))
                .ThrowsAsync(new Exception());

            var authService = AuthServiceConstrutor();

            //act
            var result = await authService.ForgotPasswordAsync(forgotPasswordDtoFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        private AuthService AuthServiceConstrutor()
        {
            return new AuthService(_mockTokenService.Object,
                _mockEmailService.Object,
                _mapper,
                _mockBackgroundJobClient.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockRefreshToken.Object);
        }
    }
}
