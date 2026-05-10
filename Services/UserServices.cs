using AutoMapper;
using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
namespace Services
{


    public class UserServices : IUserServices
    {
        IUserRepository _r;
        IMapper _mapper;
        IPasswordService _passwordService;
        IDistributedCache _cache;
        IConfiguration _configuration;
        public UserServices(IUserRepository i, IMapper mapperr, IPasswordService passwordService, IDistributedCache cache, IConfiguration configuration)
        {
            _r = i;
            _mapper = mapperr;
            _passwordService = passwordService;
            _cache = cache;
            _configuration = configuration;
        }
        private const string CacheKey = "all_users_list";
        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                var cachedUsers = await _cache.GetStringAsync(CacheKey);
                if (!string.IsNullOrEmpty(cachedUsers))
                {
                    return JsonSerializer.Deserialize<List<User>>(cachedUsers) ?? new List<User>();
                }

                var users = await _r.GetUsers();

                var ttlMinutes = _configuration.GetValue<int>("RedisSettings:DefaultTTLInMinutes");
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(ttlMinutes));

                var serializedData = JsonSerializer.Serialize(users);
                await _cache.SetStringAsync(CacheKey, serializedData, options);

                return users;
            }
            catch
            {
                return await _r.GetUsers();
            }
        }
        public async Task<DtoUser_Name_Gmail_Role_Id?> GetUserById(int id)
        {
            var u = await _r.GetUserById(id);
            var r = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(u);
            return r;
        }
        public async Task<DtoAuthResponse?> AddNewUser(DtoUser_All user)
        {
            int d = _passwordService.getStrengthByPassword(user.PasswordHash);
            if (d >= 2)
            {
                
                var userEntity = _mapper.Map<DtoUser_All, User>(user);
                userEntity.Role = "Customer";
                var res = await _r.AddNewUser(userEntity);
                await TryRemoveUsersCache();
                var dtoUser = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(res);
                return new DtoAuthResponse
                {
                    User = dtoUser,
                    Token = GenerateJwtToken(res)
                };
            }

            return null;
        }

        public async Task<DtoAuthResponse?> Login(DtoUser_Gmail_Password value)
        {
            var a = _mapper.Map<DtoUser_Gmail_Password, User>(value);
            var u = await _r.Login(a);

            if (u == null) return null;

            var dtoUser = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(u);
            return new DtoAuthResponse
            {
                User = dtoUser,
                Token = GenerateJwtToken(u)
            };
        }
       
        public async Task<DtoUser_Name_Gmail_Role_Id> update(int id, DtoUser_All userDto)
        {
           
            int d = _passwordService.getStrengthByPassword(userDto.PasswordHash);
            if (d < 2) return null;

            var existingUser = await _r.GetUserById(id);
            if (existingUser == null) return null;
            _mapper.Map(userDto, existingUser);
            existingUser.UserId = id;
            var res = await _r.update(id, existingUser);
            await TryRemoveUsersCache();

            return _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(res);
        }
      
        public async Task<bool> IsAdminById(int id, string password)
        {
           
            var user = await _r.GetUserByIdAndPassword(id, password);
            if (user != null && user.Role == "Admin")
            {
                return true;
            }
            return false;
        }

        private async Task TryRemoveUsersCache()
        {
            try
            {
                await _cache.RemoveAsync(CacheKey);
            }
            catch
            {
                // Continue without failing the main flow when Redis is unavailable.
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key is missing.");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes <= 0 ? 60 : expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}