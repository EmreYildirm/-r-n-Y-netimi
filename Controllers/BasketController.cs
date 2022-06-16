using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ÜrünYönetimi.DataAccessLayer;
using ÜrünYönetimi.Models;

namespace ÜrünYönetimi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasketController : ControllerBase
    {
        private IDistributedCache _distributedcache;
        ProductManager productManager;
        public BasketController(IDistributedCache distributedcache, ProductManager productManager)
        {
            _distributedcache = distributedcache;
            this.productManager = productManager;
        }
        [HttpPost("SetCache")]
        public async Task<Product> Set(string key, string productKey)
        {
            var product = productManager.GetById(productKey);

            var cacheKey = key;

            List<Product> products;

            if (product != null)
            {
                var byteproducts = await _distributedcache.GetAsync(cacheKey);
                if (byteproducts != null)
                {
                    products = this.DeserializeObject(byteproducts);

                    products.Add(product);
                }
                else
                {
                    products = new List<Product>();
                    products.Add(product);
                }

                var productsCache = this.SerializeObject(products);

                await _distributedcache.SetAsync(cacheKey, productsCache, SetOptions());
            }
            return product;
        }

        [HttpPost("GetBasket")]
        public async Task<IEnumerable<Product>> Get(string key)
        {

            IEnumerable<Product> products;

            var cacheKey = key;

            var byteproducts = await _distributedcache.GetAsync(cacheKey);

            if (byteproducts != null)
                products = this.DeserializeObject(byteproducts);
            else
                products = new List<Product>();

            return products;
        }


        [HttpPost("DeleteToBasket")]
        public async void DeleteToBasket(string key, string productKey)
        {
            var cacheKey = key;

            var byteproducts = await _distributedcache.GetAsync(cacheKey);

            if (byteproducts != null)
            {
                var products = this.DeserializeObject(byteproducts);

                products.Remove(products.FirstOrDefault(x => x.Id == productKey));

                var productsCache = this.SerializeObject(products);

                await _distributedcache.SetAsync(cacheKey, productsCache, SetOptions());
            }

        }
        private DistributedCacheEntryOptions SetOptions()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

            options.AbsoluteExpiration = DateTime.Now.AddMinutes(60); //1 saat cache'te duracak.

            options.SetSlidingExpiration(TimeSpan.FromDays(1)); // belirli bir süre erişilmemiş ise expire etme işlemi

            return options;
        }

        private List<Product> DeserializeObject(byte[] byteproducts)
        {
            var productsJson = Encoding.UTF8.GetString(byteproducts);
            var products = JsonConvert.DeserializeObject<List<Product>>(productsJson);
            return products;
        }

        private byte[] SerializeObject(List<Product> products)
        {
            string jsonproducts = JsonConvert.SerializeObject(products);
            var productsCache = Encoding.UTF8.GetBytes(jsonproducts);
            return productsCache;
        }
    }
}
