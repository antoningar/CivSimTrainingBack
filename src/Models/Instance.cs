using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cst_back.Models
{
    public class Instance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Civilization { get; set; }
        public string? Goal { get; set; }
        public string? Map { get; set; }
        public string? Creator { get; set; }
        public string[] Mods { get; set; }
    }
}
