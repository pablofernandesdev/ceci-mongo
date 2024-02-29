using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace CeciMongo.Domain.Interfaces.Repository
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTime CreatedAt { get; }

        bool Active { get; set; }
    }
}
