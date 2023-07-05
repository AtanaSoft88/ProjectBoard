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

public class TeamRepository : Repository<Team, DynamoTeam>, ITeamRepository
{
    private readonly IDynamoDBContext _context;
    private readonly IMapper _mapper;
    private readonly IAmazonDynamoDB _client;

    public TeamRepository(IDynamoDBContext context
                          , IMapper mapper
                          , IAmazonDynamoDB client)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
        this._client = client;
    }

    public async Task<Team> GetById(string teamId)
    {
        string rangeKey = teamId;
        var dynamoProject = await _context.LoadAsync<DynamoTeam>(teamId, rangeKey);
        var team = _mapper.Map<Team>(dynamoProject);
        return team;
    }

    public async Task<Team> Save(Team teamDb)
    {
        var dynamoModelTeam = _mapper.Map<DynamoTeam>(teamDb);
        await _context.SaveAsync(dynamoModelTeam);
        return teamDb;
    }

    public async Task<PaginationResult<Team>> GetPaginatedTeams(int pageSize, string? pageKey)
    {

        var exclusiveStartKey = string.IsNullOrEmpty(pageKey)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(pageKey));

        var scanRequest = new ScanRequest()
        {
            TableName = "Teams",
            Limit = pageSize,
            ExclusiveStartKey = exclusiveStartKey
        };

        var response = await _client.ScanAsync(scanRequest);

        var items = response.Items.Select(a =>
        {
            var doc = Document.FromAttributeMap(a);
            return _context.FromDocument<DynamoTeam>(doc);
        }).ToList();

        var teams = _mapper.Map<List<Team>>(response.Items);


        var nextPageKey = response.LastEvaluatedKey.Count == 0
            ? null
            : Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(
                response.LastEvaluatedKey,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                }));

        var paginatedList = new PaginationResult<Team>(teams, nextPageKey);
        return paginatedList;
    }
}
