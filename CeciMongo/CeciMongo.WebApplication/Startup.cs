using AutoMapper;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.Mapping;
using CeciMongo.Infra.CrossCutting.Settings;
using CeciMongo.Service.Validators.User;
using CeciMongo.WebApplication.Attributes;
using CeciMongo.WebApplication.Dependencys;
using CeciMongo.WebApplication.Extensions;
using CeciMongo.WebApplication.Filters;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;

namespace CeciMongo.WebApplication
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddRepository(Configuration);
            services.AddService(Configuration);
            services.AddControllers();
            services.AddSingleton(Configuration);

            var mongoClient = new MongoClient(Configuration.GetSection("MongoDbSettings:ConnectionString").Value);

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMongoStorage(mongoClient, Configuration.GetSection("MongoDbSettings:DatabaseName").Value, new MongoStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        Prefix = "hangfire",
                        CheckConnection = false
                    })
                );

            // Add the processing server as IHostedService
            services.AddHangfireServer(options => options.WorkerCount = 1);

            //settings
            services.Configure<SwaggerSettings>(Configuration.GetSection("SwaggerSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<ExternalProviders>(Configuration.GetSection("ExternalProviders"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<RoleSettings>(Configuration.GetSection("RoleSettings"));
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));

            //auth
            var key = Encoding.ASCII.GetBytes(Configuration["JwtToken:Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(options =>
             {
                 options.AddPolicy("Administrator", policy =>
                       policy.RequireRole("administrator"));
             });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Basic", policy =>
                      policy.RequireRole("basic","administrator"));
            });

            //auto mapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserAddValidator>());

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ceci Dapper API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("http://pablodeveloper.rf.gd/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Pablo Fernandes",
                        Email = string.Empty,
                        Url = new Uri("http://pablodeveloper.rf.gd"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("http://pablodeveloper.rf.gd/license"),
                    }
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                                   Enter 'Bearer' [space] and then your token in the text input below. 
                                   Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                    new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                // get solution documentation xmls
                var xmls = new List<string>(new string[] { Path.Combine(AppContext.BaseDirectory, "CeciMongo.WebApplication.xml"),
                    Path.Combine(AppContext.BaseDirectory, "CeciMongo.Domain.xml") });

                foreach (var item in xmls)
                {
                    options.IncludeXmlComments(item, includeControllerXmlComments: true);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //AppPath = "" //The path for the Back To Site link. Set to null in order to hide the Back To  Site link.
                DashboardTitle = "Ceci Dapper Hangfire Dashboard",
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter(
                        Configuration.GetSection("HangfireSettings:HangfireUserAuthorized").Value,
                        Configuration.GetSection("HangfireSettings:HangfireAuthorizedPassword").Value
                    )
                }
            });

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(new ResultResponse()
                {
                    Message = exception.Message,
                    Details = exception.ToString()
                }.ToString());
            }));

            // Unauthorized (401) and Forbidden(403) MiddleWare
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResultResponse()
                    {
                        Message = "Token validation has failed. Request access denied"
                    }.ToString());
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden) // 403
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResultResponse()
                    {
                        Message = "User does not have permission to access this functionality"
                    }.ToString());
                }
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ceci API v1"));
        }
    }
}
