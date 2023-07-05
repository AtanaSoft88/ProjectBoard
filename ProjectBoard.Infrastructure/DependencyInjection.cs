using Microsoft.Extensions.DependencyInjection;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using ProjectBoard.Data.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectBoard.Data;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Identity;
using ProjectBoard.Identity.Abstractions;

namespace ProjectBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.EUNorth1));
        services.AddSingleton<IDynamoDBContext>(_ => new DynamoDBContext(new AmazonDynamoDBClient(RegionEndpoint.EUNorth1)));
        services.AddScoped(typeof(IProjectRepository), typeof(ProjectRepository));
        services.AddScoped(typeof(ITeamRepository), typeof(TeamRepository));        
        return services;
    }

public static IServiceCollection AddIdentityServices(this IServiceCollection services)
{
    services.AddScoped<IIdentity, CognitoIdentity>();
    services.AddSingleton<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
    services.AddCognitoIdentity();

    return services;
}

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityOptions>(options => { configuration.GetSection("Identity").Bind(options); });
        return services;
    }

    public static IServiceCollection AddAutomapperProfiles(this IServiceCollection services, Profile rootProfile )
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(rootProfile);
            cfg.AddProfile<DbAutoMapperProfile>();

        });
        IMapper mapper = mapperConfiguration.CreateMapper();
        services.AddSingleton(mapper);
        return services;
    }
}
