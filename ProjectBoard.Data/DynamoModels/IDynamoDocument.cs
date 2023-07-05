using Amazon.DynamoDBv2.DataModel;
using ProjectBoard.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectBoard.Data.DynamoModels
{
    public interface IDynamoDocument 
    {
        [DynamoDBHashKey("pk")]
        public string Pk { get; init ; }
        [DynamoDBRangeKey("sk")]
        public string Sk { get; init; }

        public string Id { get; set; }        
    }
}
