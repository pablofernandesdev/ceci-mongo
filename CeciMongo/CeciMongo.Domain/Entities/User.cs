using CeciMongo.Infra.CrossCutting.Attributes;
using System.Collections.Generic;

namespace CeciMongo.Domain.Entities
{
    /// <summary>
    /// Represents a user entity in the system.
    /// </summary>
    [BsonCollection("user")]
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the role associated with the user.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user's email has been validated.
        /// </summary>
        public bool Validated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user needs to change their password.
        /// </summary>
        public bool ChangePassword { get; set; }

        public List<Address> Adresses { get; set ; }
    }
}
