using Bogus;
using CeciMongo.Domain.DTO.Notification;
using System;

namespace CeciMongo.Test.Fakers.Notification
{
    public class NotificationFaker
    {
        public static Faker<NotificationSendDTO> NotificationSendDTO()
        {
            return new Faker<NotificationSendDTO>()
                .CustomInstantiator(p => new NotificationSendDTO
                {
                    Body = p.Random.Words(3),
                    IdUser = Guid.NewGuid().ToString(),
                    Title = p.Random.Word()
                });
        }
    }
}
