using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using ÜrünYönetimi.Helpers;
using ÜrünYönetimi.Models;
using ÜrünYönetimi.DataAccessLayer;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ÜrünYönetimi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        UserManager userManager;
        private readonly AppSettings _appSettings;
        private IDistributedCache _distributedcache;

        public UserController(IDistributedCache distributedcache, UserManager userManager, IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            _appSettings = appSettings.Value;
            _distributedcache = distributedcache;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult AuthenticateUser(User userModel)
        {
            User user = userManager.Authenticate(userModel.Name, userModel.Password);
            if (user != null)
            {

                // Token oluşturmak için önce JwtSecurityTokenHandler sınıfından instance alıyorum.
                var tokenHandler = new JwtSecurityTokenHandler();
                //İmza için gerekli gizli anahtarımı alıyorum.
                var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    //Tokenın hangi tarihe kadar geçerli olacağını ayarlıyoruz.
                    Expires = DateTime.UtcNow.AddMinutes(180),
                    //Son olarak imza için gerekli algoritma ve gizli anahtar bilgisini belirliyoruz.
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                //Token oluşturuyoruz.
                var token = tokenHandler.CreateToken(tokenDescriptor);
                //Oluşturduğumuz tokenı string olarak bir değişkene atıyoruz.
                string generatedToken = tokenHandler.WriteToken(token);
                user.Token = generatedToken;

                var tokenCache = this.SerializeObject(user.Token);

                var cacheKey = "token";
                
                _distributedcache.SetAsync( cacheKey , tokenCache, SetOptions());

                return Ok(new { Username = user.Name, Token = user.Token });
            }
            return BadRequest("Username or password incorrect!");
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public User Create(User userModel)
        {
            var user = userManager.Create(userModel);
            return user;
        }
        private string DeserializeObject(byte[] bytetoken)
        {
            var tokenCache = Encoding.UTF8.GetString(bytetoken);
            return tokenCache;
        }

        private byte[] SerializeObject(string token)
        {
            var tokenCache = Encoding.UTF8.GetBytes(token);
            return tokenCache;
        }
        private DistributedCacheEntryOptions SetOptions()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

            options.AbsoluteExpiration = DateTime.Now.AddMinutes(180); //3 saat cache'te duracak.

            options.SetSlidingExpiration(TimeSpan.FromDays(1)); // belirli bir süre erişilmemiş ise expire etme işlemi

            return options;
        }
    }
}
