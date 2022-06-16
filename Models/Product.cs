using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ÜrünYönetimi.Models
{
    public class Product
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]
        public string Id { get; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Price")]
        public double Price { get; set; }
    }
}
