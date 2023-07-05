using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectBoard.API.Requests.Base;

namespace ProjectBoard.API;

public static class MediatREndpointExtensions
{
    public static WebApplication MediateGet<TRequest>(WebApplication app, string group, string policy)
        where TRequest : IGetOperation
    {
        app.MapGet($"/{group}s/{{id}}", async (IMediator mediator, [FromRoute] string id, [AsParameters] TRequest req) =>
        {
            req.Id = id;
            return await mediator.Send(req);
        }).WithTags(group).RequireAuthorization(policy);
        return app;
    }

    public static WebApplication MediateGetAll<TRequest>(WebApplication app, string group, string policy)
    where TRequest : BaseRequest
    {
        app.MapGet($"/{group}s", async (IMediator mediator, [AsParameters] TRequest req) => await mediator.Send(req))
            .WithTags(group).RequireAuthorization(policy);
        return app;
    }

    public static WebApplication MediateUpdate<TRequest>(WebApplication app, string group, string policy)
        where TRequest : IUpdateOperation
    {
        app.MapPut($"/{group}s/{{id}}", async (IMediator mediator, [FromRoute] string id, [FromBody] TRequest req) =>
        {
            req.Id = id;
            return await mediator.Send(req);
        }).WithTags(group).RequireAuthorization(policy);
        return app;
    }

    public static WebApplication MediateCreate<TRequest>(WebApplication app, string group, string policy)
        where TRequest : BaseRequest
    {
        app.MapPost($"/{group}s", async (IMediator mediator, [FromBody] TRequest req) => await mediator.Send(req))
            .WithTags(group).RequireAuthorization(policy);
        return app;
    }

    public static WebApplication MediateSearch<TRequest>(WebApplication app, string group, string policy)
    where TRequest : ISearchOperation
    {
        app.MapGet($"/{group}s/Search", async (IMediator mediator, [AsParameters] TRequest req) => await mediator.Send(req))
            .WithTags(group).RequireAuthorization(policy);
        return app;
    }

    public static WebApplication MediateRemove<TRequest>(WebApplication app, string group, string policy)
        where TRequest : IRemoveOperation
    {
        app.MapDelete($"/{group}s/{{id}}", async (IMediator mediator, [FromRoute] string id, [FromBody] TRequest req) =>
        {
            req.Id = id;
            return await mediator.Send(req);
        }).WithTags(group).RequireAuthorization(policy);
        return app;
    }
}
