using CeciMongo.Infra.CrossCutting.Attributes;

namespace CeciMongo.Domain.Entities
{
    /// <summary>
    /// Represents a registration token entity used for user registration confirmation.
    /// </summary>
    [BsonCollection("registrationToken")]
    public class RegistrationToken : BaseEntity
    {
        /// <summary>
        /// Gets or sets the registration token value.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user associated with the registration token.
        /// </summary>
        public User User { get; set; }
    }
}
