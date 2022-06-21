using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ÜrünYönetimi.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]
        public string Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Surname")]
        public string Surname { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Token")]
        public string Token { get; set; }
        [BsonElement("RefreshTokenEndDate")]
        public DateTime? RefreshTokenEndDate { get; set; }
    }
}
