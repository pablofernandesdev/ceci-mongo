using Bogus;
using CeciMongo.Domain.DTO.Role;
using System.Collections.Generic;

namespace CeciMongo.Test.Fakers.Role
{
    public static class RoleFaker
    {
        public static Faker<CeciMongo.Domain.Entities.Role> RoleEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.Role>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.Role
                {
                    Active = true,
                    Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                    Name = p.Random.Word(),
                    CreatedAt = p.Date.Recent()              
                });
        }

        public static Faker<RoleAddDTO> RoleAddDTO()
        {
            return new Faker<RoleAddDTO>()
                .CustomInstantiator(p => new RoleAddDTO
                {
                    Name = p.Random.Word(),
                });
        }

        public static Faker<RoleUpdateDTO> RoleUpdateDTO()
        {
            return new Faker<RoleUpdateDTO>()
                .CustomInstantiator(p => new RoleUpdateDTO
                {
                    Name = p.Random.Word(),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<RoleDeleteDTO> RoleDeleteDTO()
        {
            return new Faker<RoleDeleteDTO>()
                .CustomInstantiator(p => new RoleDeleteDTO
                {
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<IdentifierRoleDTO> IdentifierRoleDTO()
        {
            return new Faker<IdentifierRoleDTO>()
                .CustomInstantiator(p => new IdentifierRoleDTO
                {
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<RoleResultDTO> RoleResultDTO()
        {
            return new Faker<RoleResultDTO>()
                .CustomInstantiator(p => new RoleResultDTO
                {
                    Name = p.Random.Word(),
                    RoleId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }
    }
}
