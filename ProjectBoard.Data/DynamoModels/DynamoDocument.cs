using Amazon.DynamoDBv2.DataModel;

namespace ProjectBoard.Data.DynamoModels
{
    public class DynamoDocument : IDynamoDocument
    {
        [DynamoDBHashKey("pk")]
        public string Pk 
        {
            get => Id; init { Id = value; }
        }
        [DynamoDBRangeKey("sk")]
        public string Sk 
        { 
            get => Id; init { Id = value; }
        }

        public string Id { get; set; } = null!;
    }
}
