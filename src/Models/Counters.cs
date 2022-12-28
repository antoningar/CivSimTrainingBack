using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cst_back.Models
{
    public class Counters
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int? userid_counter{ get; set; }
    }
}
