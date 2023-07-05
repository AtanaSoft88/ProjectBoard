using Amazon.DynamoDBv2.DataModel;

namespace ProjectBoard.Data.DynamoModels
{
    [DynamoDBTable("Teams")]
    public class DynamoTeam : DynamoDocument
    {
        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public List<string> DeveloperIds { get; set; }

        [DynamoDBProperty]
        public string? TeamLeadId { get; set; }

        public DynamoTeam()
        {
            this.Name = String.Empty;

            this.DeveloperIds = new List<string>();
        }
    }
}
