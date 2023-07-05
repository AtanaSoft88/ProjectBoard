using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using ProjectBoard.Data.Abstractions;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Data.DynamoModels;
using System.Text.Json.Serialization;
using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace ProjectBoard.Data;
public class ProjectRepository : Repository<Project, DynamoProject>, IProjectRepository
{
    private IAmazonDynamoDB _client;
    private readonly IDynamoDBContext _context;
    private readonly IMapper _mapper;
    private readonly string _tableName = "Projects";

    public ProjectRepository(IDynamoDBContext context, IMapper mapper, IAmazonDynamoDB client)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
        _client = client;
    }

    public async Task<PaginationResult<Project>> GetPaginatedProjects(int pageSize, string? pageKey)
    {
        var exclusiveStartKey = string.IsNullOrEmpty(pageKey)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(pageKey));

        var scanRequest = new ScanRequest()
        {
            TableName = _tableName,
            Limit = pageSize,
            ExclusiveStartKey = exclusiveStartKey
        };

        var response = await _client.ScanAsync(scanRequest);

        var items = response.Items.Select(a =>
        {
            var document = Document.FromAttributeMap(a);
            return _context.FromDocument<DynamoProject>(document);
        }).ToList();

        var projects = _mapper.Map<List<Project>>(items);


        var nextPageKey = response.LastEvaluatedKey.Count == 0
            ? null
            : Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(
                response.LastEvaluatedKey,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                }));

        var paginatedList = new PaginationResult<Project>(projects, nextPageKey);
        return paginatedList;
    }

    public async Task<PaginationResult<Project>> SearchProjectByName(string searchQuery, int pageSize, string? pageKey)
    {
        var exclusiveStartKey = string.IsNullOrEmpty(pageKey)
           ? null
           : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(pageKey));

        PaginationResult<Project> projectsResponse = new(new List<Project>(), null);

        ScanRequest scanRequest = new()
        {
            TableName = "Projects",
            Limit = pageSize,
            ExclusiveStartKey = exclusiveStartKey,
            FilterExpression = "contains(#projectName, :searchQuery)",
            ExpressionAttributeNames = new Dictionary<string, string>()
            {
                { "#projectName", "Name" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            {
                { ":searchQuery", new AttributeValue { S = searchQuery } }
            }
        };

        Dictionary<string, AttributeValue> lastEvaluatedItem = null;

        var scanResult = _client.Paginators.Scan(scanRequest);

        await foreach (var response in scanResult.Responses)
        {
            var items = response.Items.Select(t =>
            {
                var doc = Document.FromAttributeMap(t);
                return _context.FromDocument<DynamoProject>(doc);
            }).ToList();

            var projects = _mapper.Map<List<Project>>(items);

            bool isLimitReached = false;

            foreach (var project in projects)
            {
                isLimitReached = projectsResponse.Items.Count == pageSize ? true : false;
                if (isLimitReached)
                {
                    break;
                }
                projectsResponse.Items.Add(project);
            }

            if (isLimitReached)
            {
                var lastItem = projectsResponse.Items.Last();
                var lastItemMap = _mapper.Map<DynamoProject>(lastItem);

                lastEvaluatedItem = new Dictionary<string, AttributeValue>
                {
                     { "pk", new AttributeValue { S = lastItemMap.Pk } },
                     { "sk", new AttributeValue { S = lastItemMap.Sk } }
                };

                var nextPageKey = Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(lastEvaluatedItem,
                        new JsonSerializerOptions()
                        {
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                        }));
                projectsResponse.NextPageKey = nextPageKey;
                break;
            }

            if (response.LastEvaluatedKey.Count == 0)
            {
                break;
            }
        }
        return projectsResponse;
    }
}
