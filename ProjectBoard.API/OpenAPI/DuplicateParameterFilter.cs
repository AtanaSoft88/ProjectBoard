using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectBoard.API.Filters;

public class DuplicateParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters = operation.Parameters.DistinctBy(p => p.Name).ToList();
    }
}
