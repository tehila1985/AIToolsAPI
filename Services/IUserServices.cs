using Dto;
using Repository.Models;

namespace Services
{
    public interface IUserServices
    {
        Task<DtoUser_Name_Gmail_Role_Id> AddNewUser(DtoUser_All user);
        Task<DtoUser_Name_Gmail_Role_Id?> GetUserById(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<DtoUser_Name_Gmail_Role_Id?> Login(DtoUser_Gmail_Password value);
        Task<DtoUser_Name_Gmail_Role_Id> update(int id, DtoUser_All value);
        Task<bool> IsAdminById(int id, string password);
    }
}
