﻿using Bogus;
using CeciMongo.Infra.CrossCutting.Settings;

namespace CeciMongo.Test.Fakers.Settings
{
    public static class EmailSettingsFaker
    {
        public static Faker<EmailSettings> EmailSettings()
        {
            return new Faker<EmailSettings>()
                .CustomInstantiator(p => new EmailSettings
                {
                    DisplayName = "Nicholas Beahan",
                    Mail = "nicholas.beahan77@ethereal.email",
                    Password = "5UBfeFUCpd8GMAg9kp",
                    Host = "smtp.ethereal.email",
                    Port = 587
                });
        }
    }
}
