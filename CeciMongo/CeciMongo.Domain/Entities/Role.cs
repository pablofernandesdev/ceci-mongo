using CeciMongo.Infra.CrossCutting.Attributes;
using System.Collections.Generic;

namespace CeciMongo.Domain.Entities
{
    /// <summary>
    /// Represents a role entity for user authorization and access control.
    /// </summary>
    [BsonCollection("role")]
    public class Role : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        public string Name { get; set; }
    }
}
