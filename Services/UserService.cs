using Microsoft.IdentityModel.Tokens;
using Study.SignalRdemo.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Study.SignalRdemo.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", Password = "admin", Role = Role.Admin },
            new User { Id = 2, FirstName = "Normal", LastName = "User", Username = "user", Password = "user", Role = Role.User },
            new User { Id = 3, FirstName = "Normal", LastName = "User", Username = "jonny", Password = "jonny", Role = Role.User },
            new User { Id = 4, FirstName = "Normal", LastName = "User", Username = "james", Password = "james", Role = Role.User },
            new User { Id = 5, FirstName = "Normal", LastName = "User", Username = "xhl", Password = "xhl", Role = Role.User },
            new User { Id = 6, FirstName = "Normal", LastName = "User", Username = "luce", Password = "luce", Role = Role.User },
            new User { Id = 7, FirstName = "Normal", LastName = "User", Username = "jack", Password = "jack", Role = Role.User },
            new User { Id = 8, FirstName = "Normal", LastName = "User", Username = "rose", Password = "rose", Role = Role.User },
            new User { Id = 9, FirstName = "Normal", LastName = "User", Username = "curry", Password = "curry", Role = Role.User }
        };

        private IConfiguration Configuration { get; }

        public UserService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Configuration["AppSetting:Secret"];
            var expires=Double.Parse(Configuration["AppSetting:Expires"]);
            var issuer = Configuration["AppSetting:Issuer"];
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Issuer=issuer,
                Expires = DateTime.UtcNow.AddMinutes(expires), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            //user.Password = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            return _users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }

        public User GetById(int id)
        {
            var user = _users.FirstOrDefault(x => x.Id == id);

            // return user without password
            if (user != null)
                user.Password = null;

            return user;
        }
    }
}
