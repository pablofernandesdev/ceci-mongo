using Bogus;
using CeciMongo.Domain.DTO.ValidationCode;
using CeciMongo.Infra.CrossCutting.Extensions;

namespace CeciMongo.Test.Fakers.ValidationCode
{
    public class ValidationCodeFaker
    {
        public static Faker<CeciMongo.Domain.Entities.ValidationCode> ValidationCodeEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.ValidationCode>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.ValidationCode
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Active = true,
                    Code = PasswordExtension.EncryptPassword(p.Random.Word()),
                    Expires = p.Date.Future(),
                    Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                    CreatedAt = p.Date.Recent(),
                });
        }

        public static Faker<ValidationCodeValidateDTO> ValidationCodeValidateDTO()
        {
            return new Faker<ValidationCodeValidateDTO>()
                .CustomInstantiator(p => new ValidationCodeValidateDTO
                {
                    Code = p.Random.Word()                   
                });
        }
    }
}
