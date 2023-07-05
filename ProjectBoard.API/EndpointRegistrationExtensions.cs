using ProjectBoard.API.Configuration;

namespace ProjectBoard.API;

public static class EndpointRegistrationExtensions
{
    public static WebApplication RegisterFeatures(this WebApplication app, List<FeatureConfig> configs)
    {
        foreach (var feature in configs)
        {
            RegisterFeature(app, feature);
        }
        return app;
    }

    private static WebApplication RegisterFeature(WebApplication app, FeatureConfig feature)
    {
        var requestTypes = typeof(Program).Assembly.GetTypes().Where(t => t.Namespace == feature.NameSpace);
        foreach (var requestType in requestTypes)
        {
            string operation = requestType.Name.Substring(0, requestType.Name.IndexOf(feature.Name));

            string? policy = null;

            feature.MergeDefaults();
            
            policy = feature.Operations.First(o => o.OperationName == operation).Policy;
                           
            var register  = typeof(MediatREndpointExtensions)
                .GetMethod($"Mediate{operation}")!
                .MakeGenericMethod(requestType);
            register.Invoke(null,new object[] { app, feature.Name, policy });
        }
        return app;
    }
}
