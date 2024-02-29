using CeciMongo.Domain.Interfaces.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using TimeZoneConverter;

namespace CeciMongo.Domain.Entities
{
    /// <summary>
    /// Represents a base entity that provides common properties for all entities.
    /// </summary>
    public abstract class BaseEntity : IDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// Sets default values for common properties.
        /// </summary>
        public BaseEntity()
        {
            Active = true;
            CreatedAt = TimeZoneInfo.ConvertTime(DateTime.Now, TZConvert.GetTimeZoneInfo("E. South America Standard Time"));
        }

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets the registration date of the entity.
        /// The date is converted to the "E. South America Standard Time" time zone.
        /// </summary>
        public virtual DateTime CreatedAt { get; set; }
    }
}
