using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Notification;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Domain.Interfaces.Service.External;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service responsible for sending notifications.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IFirebaseService _firebaseService;
        private readonly IUserRepository _userRepository;
        private readonly IRegistrationTokenRepository _registrationTokenRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="firebaseService">An instance of the <see cref="IFirebaseService"/> used for interacting with Firebase services.</param>
        /// <param name="userRepository">An instance of the <see cref="IUserRepository"/> used for user-related database operations.</param>
        /// <param name="registrationTokenRepository">An instance of the <see cref="IRegistrationTokenRepository"/> used for registration token-related database operations.</param>
        public NotificationService(
            IFirebaseService firebaseService, IUserRepository userRepository, IRegistrationTokenRepository registrationTokenRepository)
        {
            _firebaseService = firebaseService;
            _userRepository = userRepository;
            _registrationTokenRepository = registrationTokenRepository;
        }

        /// <summary>
        /// Sends a notification asynchronously.
        /// </summary>
        /// <param name="obj">The notification information.</param>
        /// <returns>A response indicating the success of the notification sending operation.</returns>
        public async Task<ResultResponse> SendAsync(NotificationSendDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var user = await _userRepository.FindByIdAsync(obj.IdUser);

                if (user != null)
                {
                    var registrationToken = await _registrationTokenRepository.FindOneAsync(c => c.User.Id == user.Id);

                    if (registrationToken != null)
                    {
                        response = await _firebaseService.SendNotificationAsync(registrationToken.Token, obj.Title, obj.Body);

                        if (response.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            response.Message = "Notification sent successfully.";
                        }
                    }
                }
             
            }
            catch (Exception ex)
            {
                response.Message = "Could not send notification.";
                response.Exception = ex;
            }

            return response;
        }
    }
}
