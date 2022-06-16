using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ÜrünYönetimi.DataAccessLayer;
using ÜrünYönetimi.Models;

namespace ÜrünYönetimi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        ProductManager productManager;

        public ProductController(ProductManager productManager)
        {
            this.productManager = productManager;
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public Product GetById(string id)
        {
            var product = productManager.GetById(id);
            return product;
        }
        [HttpPost]
        [Route("Add")]
        public Product Add(Product model)
        {
            var product = productManager.Create(model);
            return product;
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public string Delete(string id)
        {
            var product = productManager.Delete(id);
            return product;
        }
        [HttpGet]
        [Route("List")]
        public IEnumerable<Product> List()
        {
            var product = productManager.GetList();
            return product;
        }
    }
}
