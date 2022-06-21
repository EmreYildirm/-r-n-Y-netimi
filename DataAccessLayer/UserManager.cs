using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ÜrünYönetimi.Helpers;
using ÜrünYönetimi.Models;

namespace ÜrünYönetimi.DataAccessLayer
{
    public class UserManager
    {
        private readonly IMongoCollection<User> mongoCollection;

        public UserManager(string mongoDbConnectionString, string dbName, string collectionName)
        {
            var client = new MongoClient(mongoDbConnectionString);
            var database = client.GetDatabase(dbName);
            mongoCollection = database.GetCollection<User>(collectionName);
        }

        /* Yeni Bir User Ekleme */
        public User Create(User model)
        {
            mongoCollection.InsertOne(model);
            return model;
        }

        /* Bir User Getirme */
        public User Authenticate(string kullaniciAdi, string sifre)
        {
            return mongoCollection.Find(m => m.Name == kullaniciAdi && m.Password == sifre).FirstOrDefault();
        }
    }
}
