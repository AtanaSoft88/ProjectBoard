using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using ProjectBoard.Data.Abstractions;
using ProjectBoard.Data.Abstractions.Repositories;
using ProjectBoard.Data.DynamoModels;

namespace ProjectBoard.Data;

public class Repository<TAbstraction, TDocument> : IRepository<TAbstraction> 
    where TAbstraction : IDocument 
    where TDocument : IDynamoDocument

{
    private readonly IDynamoDBContext _context;
    private readonly IMapper _mapper;

    public Repository(IDynamoDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<TAbstraction> Create(TAbstraction model)
    {
        TDocument dynamoModel = _mapper.Map<TDocument>(model);
        await _context.SaveAsync(dynamoModel);
        TAbstraction abstraction = _mapper.Map<TAbstraction>(dynamoModel);
        return abstraction;
    }

    public async Task<TAbstraction> GetSingle(string modelId)
    {
        string rangeKey = modelId;
        TDocument dynamoProject = await _context.LoadAsync<TDocument>(modelId, rangeKey);
        TAbstraction abstraction = _mapper.Map<TAbstraction>(dynamoProject);
        return abstraction;
    }

    public async Task<TAbstraction> Update(TAbstraction model)
    {
        TDocument dynamoModel = _mapper.Map<TDocument>(model);
        await _context.SaveAsync(dynamoModel);
        TAbstraction abstraction = _mapper.Map<TAbstraction>(dynamoModel);
        return abstraction;
    }

    public async Task<IEnumerable<TAbstraction>> GetAll()
    {
        List<TDocument> dynamoDbProjects = await _context.ScanAsync<TDocument>(new List<ScanCondition>()).GetRemainingAsync();
        List<TAbstraction> abstractionModels = _mapper.Map<List<TAbstraction>>(dynamoDbProjects);
        return abstractionModels;
    }
}
