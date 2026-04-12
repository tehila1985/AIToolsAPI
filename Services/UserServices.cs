using AutoMapper;
using Dto;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System.Text.Json;
namespace Services
{


    public class UserServices : IUserServices
    {
        IUserRepository _r;
        IMapper _mapper;
        IPasswordService _passwordService;
        public UserServices(IUserRepository i, IMapper mapperr, IPasswordService passwordService)
        {
            _r = i;
            _mapper = mapperr;
            _passwordService = passwordService;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _r.GetUsers();
        }
        public async Task<DtoUser_Name_Gmail_Role_Id?> GetUserById(int id)
        {
            var u = await _r.GetUserById(id);
            var r = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(u);
            return r;
        }
        public async Task<DtoUser_Name_Gmail_Role_Id> AddNewUser(DtoUser_All user)
        {
            int d = _passwordService.getStrengthByPassword(user.PasswordHash);
            if (d >= 2)
            {
                
                var userEntity = _mapper.Map<DtoUser_All, User>(user);
                userEntity.Role = "Customer";
                var res = await _r.AddNewUser(userEntity);
                var dtoUser = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(res);
                return dtoUser;
            }

            return null;
        }

        public async Task<DtoUser_Name_Gmail_Role_Id?> Login(DtoUser_Gmail_Password value)
        {
            var a = _mapper.Map<DtoUser_Gmail_Password, User>(value);
            var u = await _r.Login(a);

            if (u == null) return null;

            var dtoUser = _mapper.Map<User, DtoUser_Name_Gmail_Role_Id>(u);
            return dtoUser;
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

    }
}