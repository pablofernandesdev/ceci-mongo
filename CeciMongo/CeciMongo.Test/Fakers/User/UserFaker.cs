using Bogus;
using CeciMongo.Domain.DTO.Register;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Infra.CrossCutting.Helper;
using MongoDB.Bson;
using CeciMongo.Domain.DTO.Role;
using CeciMongo.Test.Fakers.Address;

namespace CeciMongo.Test.Fakers.User
{
    public static class UserFaker
    {
        public static Faker<CeciMongo.Domain.Entities.User> UserEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.User>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.User
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Password = PasswordExtension.EncryptPassword("dGVzdGUy"),
                    Role = new Domain.Entities.Role
                    {
                        Active = true,
                        Id = ObjectId.GenerateNewId(),
                        Name = p.Random.Word(),
                        CreatedAt = p.Date.Recent()
                    },
                    ChangePassword = p.Random.Bool(),
                    Active = p.Random.Bool(),
                    Validated = p.Random.Bool(),
                    Adresses = AddressFaker.AddressEntity().Generate(3)
                });
        }

        public static Faker<UserAddDTO> UserAddDTO()
        {
            return new Faker<UserAddDTO>()
                .CustomInstantiator(p => new UserAddDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Password = StringHelper.Base64Encode(p.Random.Word()),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                });
        }

        public static Faker<UserSelfRegistrationDTO> UserSelfRegistrationDTO()
        {
            return new Faker<UserSelfRegistrationDTO>()
                .CustomInstantiator(p => new UserSelfRegistrationDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Password = StringHelper.Base64Encode(p.Random.Word())
                });
        }

        public static Faker<UserImportDTO> UserImportDTO()
        {
            return new Faker<UserImportDTO>()
                .CustomInstantiator(p => new UserImportDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Password = StringHelper.Base64Encode(p.Random.Word()),
                    Role = new RoleResultDTO
                    {
                        RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        Name = p.Random.Word()
                    },
                    PasswordBase64Decode = p.Random.Word(),
                });
        }

        public static Faker<UserResultDTO> UserResultDTO()
        {
            return new Faker<UserResultDTO>()
                .CustomInstantiator(p => new UserResultDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Username = p.Person.Email,
                    Role = new RoleResultDTO
                    {
                        RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        Name = p.Random.Word()
                    }
                });
        }

        public static Faker<UserDeleteDTO> UserDeleteDTO()
        {
            return new Faker<UserDeleteDTO>()
                .CustomInstantiator(p => new UserDeleteDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<UserIdentifierDTO> UserIdentifierDTO()
        {
            return new Faker<UserIdentifierDTO>()
                .CustomInstantiator(p => new UserIdentifierDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<UserUpdateDTO> UserUpdateDTO()
        {
            return new Faker<UserUpdateDTO>()
                .CustomInstantiator(p => new UserUpdateDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Password = StringHelper.Base64Encode(p.Random.Word()),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<UserUpdateRoleDTO> UserUpdateRoleDTO()
        {
            return new Faker<UserUpdateRoleDTO>()
                .CustomInstantiator(p => new UserUpdateRoleDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<UserFilterDTO> UserFilterDTO()
        {
            return new Faker<UserFilterDTO>()
                .CustomInstantiator(p => new UserFilterDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email,
                    Search = p.Random.Word(),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Page = 1,
                    PerPage = 10
                });
        }

        public static Faker<UserRedefinePasswordDTO> UserRedefinePasswordDTO()
        {
            return new Faker<UserRedefinePasswordDTO>()
                .CustomInstantiator(p => new UserRedefinePasswordDTO
                {
                    CurrentPassword = "dGVzdGUx",
                    NewPassword = "dGVzdGUy"
                });
        }

        public static Faker<UserLoggedUpdateDTO> UserLoggedUpdateDTO()
        {
            return new Faker<UserLoggedUpdateDTO>()
                .CustomInstantiator(p => new UserLoggedUpdateDTO
                {
                    Name = p.Person.FullName,
                    Email = p.Person.Email
                });
        }
    }
}
