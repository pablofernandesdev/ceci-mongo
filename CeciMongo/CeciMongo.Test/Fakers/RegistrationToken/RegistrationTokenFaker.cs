using Bogus;
using CeciMongo.Test.Fakers.User;

namespace CeciMongo.Test.Fakers.RegistrationToken
{
    public class RegistrationTokenFaker
    {
        public static Faker<CeciMongo.Domain.Entities.RegistrationToken> RegistrationTokenEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.RegistrationToken>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.RegistrationToken
                {
                    Active = true,
                    Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                    CreatedAt = p.Date.Recent(),
                    Token = p.Random.String2(30),
                    User = UserFaker.UserEntity().Generate()
                });
        }
    }
}
