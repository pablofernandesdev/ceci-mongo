using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Service.Services;
using CeciMongo.Test.Fakers.User;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Services
{
    public class ReportServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<ReportService>> _mockLogger;

        public ReportServiceTest()
        {
            _mockLogger = new Mock<ILogger<ReportService>>();
            _mockUserRepository = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task Generate_users_report_successfully()
        {
            //Arrange
            var userFilterDto = UserFaker.UserFilterDTO().Generate();
            _mockUserRepository.Setup(x => x.GetByFilterAsync(userFilterDto))
                .ReturnsAsync(UserFaker.UserEntity().Generate(3));

            var reportService = ReportServiceConstrutor();

            //Act
            var result = await reportService.GenerateUsersReport(userFilterDto);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Generate_users_report_empty_list()
        {
            //Arrange
            var userFilterDto = UserFaker.UserFilterDTO().Generate();
            _mockUserRepository.Setup(x => x.GetByFilterAsync(userFilterDto))
                .ReturnsAsync(UserFaker.UserEntity().Generate(0));

            var reportService = ReportServiceConstrutor();

            //Act
            var result = await reportService.GenerateUsersReport(userFilterDto);

            //Assert
            Assert.True(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        [Fact]
        public async Task Generate_users_report_exception()
        {
            //Arrange
            var userFilterDto = UserFaker.UserFilterDTO().Generate();
            _mockUserRepository.Setup(x => x.GetByFilterAsync(userFilterDto))
                .ThrowsAsync(new Exception());

            var reportService = ReportServiceConstrutor();

            //Act
            var result = await reportService.GenerateUsersReport(userFilterDto);

            //Assert
            Assert.False(result.StatusCode.Equals(HttpStatusCode.OK));
        }

        private ReportService ReportServiceConstrutor()
        {
            return new ReportService(_mockLogger.Object, _mockUserRepository.Object);
        }
    }
}
