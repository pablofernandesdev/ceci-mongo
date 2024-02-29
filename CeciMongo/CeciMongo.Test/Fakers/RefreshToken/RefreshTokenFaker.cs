using Bogus;
using CeciMongo.Test.Fakers.Role;
using System;

namespace CeciMongo.Test.Fakers.RefreshToken
{
    public static class RefreshTokenFaker
    {
        public static Faker<CeciMongo.Domain.Entities.RefreshToken> RefreshTokenEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.RefreshToken>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.RefreshToken
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                    Token = p.Random.String2(100),
                    Expires = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    CreatedByIp = "127.0.0.1",
                    UserId = Guid.NewGuid().ToString(),
                    User = new Domain.Entities.User{
                        Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                        Name = p.Person.FullName,
                        Email = p.Person.Email,
                        Password = p.Random.Word(),
                        Role = RoleFaker.RoleEntity().Generate()
                    }
                });
        }

        public static Faker<CeciMongo.Domain.Entities.RefreshToken> RefreshTokenExpiredEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.RefreshToken>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.RefreshToken
                {
                    Token = p.Random.String2(100),
                    Expires = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedByIp = "127.0.0.1",
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }
    }
}
