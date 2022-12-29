using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cst_back.Models
{
    public class Counters
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int? userid_counter{ get; set; }
    }
}
