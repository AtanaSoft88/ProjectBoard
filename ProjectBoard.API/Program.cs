using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ProjectBoard.API.Behaviors;
using ProjectBoard.API.Configuration;
using ProjectBoard.API.Features.Responses.Base;
using ProjectBoard.API.Filters;
using ProjectBoard.API.Http;
using ProjectBoard.API.Requests.Base;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Infrastructure;

namespace ProjectBoard.API;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddValidatorsFromAssemblyContaining<BaseRequest>(ServiceLifetime.Scoped);
        builder.Services.AddScoped<IExecutionContext, HttpExecutionContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMediatR((c) =>
        {
            c.Lifetime = ServiceLifetime.Scoped;
            c.RegisterServicesFromAssemblyContaining(typeof(BaseResponse));
            c.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddIdentityServices()
            .AddDataServices();
        builder.Services.AddAutomapperProfiles(new APIAutoMapperProfile());
        builder.Services.AddIdentityConfiguration(builder.Configuration);        
        builder.Configuration.AddEnvironmentVariables();

        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        IdentityOptions identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;
        builder.Services.AddCors(); // add for Api Call
        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(config =>
        {
            config.Authority = builder.Configuration.GetSection("Identity:Authority").Value;
            config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
                RoleClaimType = "cognito:groups"
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("admins"));
            options.AddPolicy("User", policy => policy.RequireRole("users", "admins"));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.OperationFilter<DuplicateParameterFilter>();
            c.AddSecurityDefinition("oidc", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(identityOptions.AuthorizationUrl!),
                        TokenUrl = new Uri(identityOptions.TokenUrl!),
                        Scopes = {
                            { "openid", "required" },
                            { "profile", "profile info" },
                            { "phone","user phone"}
                        }
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                        Reference = new OpenApiReference
                        {
                            Id = "oidc",
                            Type = ReferenceType.SecurityScheme
                        }
                     },
                     new List<string>()
                 }
            });
        });        
        WebApplication app = builder.Build();
        app.UseCors(c => c.WithOrigins("http://localhost:3000") //Use Cors
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.OAuthClientId(identityOptions.ClientId!));
        }

        app.UseAuthentication();
        app.UseAuthorization();        

        var config = new List<FeatureConfig>();
        app.Configuration.Bind("Features", config);
        app.RegisterFeatures(config);

        app.Run();
    }
}
