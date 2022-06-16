using ÜrünYönetimi.Models;

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ÜrünYönetimi.DataAccessLayer

{
    public class ProductManager
    {
        private readonly IMongoCollection<Product> mongoCollection;

        public ProductManager(string mongoDbConnectionString, string dbName, string collectionName)
        {
            var client = new MongoClient(mongoDbConnectionString);
            var database = client.GetDatabase(dbName);
            mongoCollection = database.GetCollection<Product>(collectionName);
        }

        /* Butun Product'lari Listeleme */
        public List<Product> GetList()
        {
            return mongoCollection.Find(book => true).ToList();
        }

        /* Bir Product Getirme */
        public Product GetById(string id)
        {
            return mongoCollection.Find(m => m.Id == id).FirstOrDefault();
        }

        /* Yeni Bir Product Ekleme */
        public Product Create(Product model)
        {
            mongoCollection.InsertOne(model);
            return model;
        }

        /* Bir Product Guncelleme */
        public void Update(string id, Product model)
        {;
            mongoCollection.ReplaceOne(m => m.Id == id, model);
        }

        /* Bir Product Silme */
        public string Delete(string id)
        {
            mongoCollection.FindOneAndDelete(x => x.Id == id);
            return id;
        }
    }
}
