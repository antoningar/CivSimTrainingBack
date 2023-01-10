using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace cst_back.Models
{
    public class Leaderboard
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? InstanceId { get; set; }
        public Result[] Results { get; set; }
    }

    public class Result {
        public Int32? Position { get; set; }
        public string? Username { get; set; }
        public string? Value { get; set; }

    }
}
