namespace ProjectBoard.Data.Abstractions.Repositories
{
    public interface IRepository<TDocument> 
        where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> GetAll(); 
        Task<TDocument> GetSingle(string modelId); 
        Task<TDocument> Create(TDocument model); 
        Task<TDocument> Update(TDocument project);
    }
}
