namespace CeciMongo.Domain.DTO.Notification
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a notification to be sent.
    /// </summary>
    public class NotificationSendDTO
    {
        /// <summary>
        /// Gets or sets the identifier of the user who will receive the notification.
        /// </summary>
        public string IdUser { get; set; }

        /// <summary>
        /// Gets or sets the title of the notification.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body of the notification.
        /// </summary>
        public string Body { get; set; }
    }
}