﻿using Bogus;
using CeciMongo.Domain.DTO.Address;
using System;
using MongoDB.Bson;

namespace CeciMongo.Test.Fakers.Address
{
    public static class AddressFaker
    {
        public static Faker<CeciMongo.Domain.Entities.Address> AddressEntity()
        {
            return new Faker<CeciMongo.Domain.Entities.Address>()
                .CustomInstantiator(p => new CeciMongo.Domain.Entities.Address
                {
                    Id = ObjectId.GenerateNewId(),
                    Active = p.Random.Bool(),
                    CreatedAt = p.Date.Past(),
                    Complement = p.Address.StreetAddress(),
                    District = p.Address.StreetAddress(),
                    Locality = p.Address.Locale,
                    Number = Convert.ToInt32(p.Address.BuildingNumber()),
                    Street = p.Address.StreetAddress(),
                    Uf = p.Address.CityPrefix(),
                    ZipCode = p.Address.ZipCode(),
                    Main = true
                });
        }

        public static Faker<AddressUpdateDTO> AddressUpdateDTO()
        {
            return new Faker<AddressUpdateDTO>()
                .CustomInstantiator(p => new AddressUpdateDTO
                {
                    AddressId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Complement = p.Address.StreetAddress(),
                    District = p.Address.StreetAddress(),
                    Locality = p.Address.Locale,
                    Number = Convert.ToInt32(p.Address.BuildingNumber()),
                    Street = p.Address.StreetAddress(),
                    Uf = p.Address.CityPrefix(),
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    ZipCode = p.Address.ZipCode()
                });
        }

        public static Faker<AddressFilterDTO> AddressFilterDTO()
        {
            return new Faker<AddressFilterDTO>()
                .CustomInstantiator(p => new AddressFilterDTO
                {
                    District = p.Address.CityPrefix(),
                    Locality = p.Locale,
                    Page = p.Random.Int(),
                    PerPage = p.Random.Int(),
                    Search = p.Random.Word(),
                    Uf = p.Address.CityPrefix()
                });
        }

        public static Faker<AddressAddDTO> AddressAddDTO()
        {
            return new Faker<AddressAddDTO>()
                .CustomInstantiator(p => new AddressAddDTO
                {
                    Complement = p.Address.StreetAddress(),
                    District = p.Address.StreetAddress(),
                    Locality = p.Address.Locale,
                    Number = Convert.ToInt32(p.Address.BuildingNumber()),
                    Street = p.Address.StreetAddress(),
                    Uf = p.Address.Random.AlphaNumeric(2),
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    ZipCode = p.Address.ZipCode(),
                    Main = true
                });
        }

        public static Faker<AddressZipCodeDTO> AddressZipCodeDTO()
        {
            return new Faker<AddressZipCodeDTO>()
                .CustomInstantiator(p => new AddressZipCodeDTO
                {
                    ZipCode = p.Address.ZipCode()
                });
        }

        public static Faker<AddressIdentifierDTO> AddressIdentifierDTO()
        {
            return new Faker<AddressIdentifierDTO>()
                .CustomInstantiator(p => new AddressIdentifierDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    AddressId = MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                });
        }

        public static Faker<AddressDeleteDTO> AddressDeleteDTO()
        {
            return new Faker<AddressDeleteDTO>()
                .CustomInstantiator(p => new AddressDeleteDTO
                {
                    UserId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    AddressId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                });
        }

        public static Faker<AddressResultDTO> AddressResultDTO()
        {
            return new Faker<AddressResultDTO>()
                .CustomInstantiator(p => new AddressResultDTO
                {
                    Uf = p.Address.CityPrefix(),
                    District = p.Address.StreetAddress(),
                    ZipCode = p.Address.ZipCode(),
                    Complement = p.Address.StreetAddress(),
                    Locality = p.Address.StreetAddress(),
                    Street = p.Address.CityPrefix(),
                    Number = p.Address.BuildingNumber(),
                    Main = true
                });
        }
    }
}