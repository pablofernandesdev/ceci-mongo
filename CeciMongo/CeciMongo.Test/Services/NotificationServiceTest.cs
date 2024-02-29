using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service.External;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.Notification;
using CeciMongo.Test.Fakers.RegistrationToken;
using CeciMongo.Test.Fakers.User;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class NotificationServiceTest
    {
        private readonly Mock<IFirebaseService> _mockFirebaseService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRegistrationTokenRepository> _mockRegistrationTokenRepository;

        public NotificationServiceTest()
        {
            _mockFirebaseService = new Mock<IFirebaseService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRegistrationTokenRepository = new Mock<IRegistrationTokenRepository>();
        }

        [Fact]
        public async Task Send_notification_successfully()
        {
            //Arrange
            var notificationSendDTOFaker = NotificationFaker.NotificationSendDTO().Generate(); 
            var registrationTokenEntityFaker = RegistrationTokenFaker.RegistrationTokenEntity().Generate();
            var userEntityFaker = UserFaker.UserEntity().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(notificationSendDTOFaker.IdUser))
                .ReturnsAsync(userEntityFaker);

            _mockRegistrationTokenRepository.Setup(x => x.FindOneAsync(x=> x.User.Id == userEntityFaker.Id))
                .ReturnsAsync(registrationTokenEntityFaker);

            _mockFirebaseService.Setup(x => x.SendNotificationAsync(registrationTokenEntityFaker.Token,
                notificationSendDTOFaker.Title,
                notificationSendDTOFaker.Body))
                .ReturnsAsync(ResultResponseFaker.ResultResponse(HttpStatusCode.OK));

            var importService = NotificationServiceConstrutor();

            //Act
            var result = await importService.SendAsync(notificationSendDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Send_notification_exception()
        {
            //Arrange
            var notificationSendDTOFaker = NotificationFaker.NotificationSendDTO().Generate();

            _mockUserRepository.Setup(x => x.FindByIdAsync(notificationSendDTOFaker.IdUser))
                .ThrowsAsync(new System.Exception());

            var importService = NotificationServiceConstrutor();

            //Act
            var result = await importService.SendAsync(notificationSendDTOFaker);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.InternalServerError));
        }

        private NotificationService NotificationServiceConstrutor()
        {
            return new NotificationService(
                _mockFirebaseService.Object,
                _mockUserRepository.Object,
                _mockRegistrationTokenRepository.Object);
        }
    }
}
