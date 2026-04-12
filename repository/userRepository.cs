using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Repository.Models;
using System.Collections.Specialized;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        myDBContext dbContext;
        public UserRepository(myDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<User>> GetUsers()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> AddNewUser(User user)
        {
           
              await dbContext.Users.AddAsync(user);
              await dbContext.SaveChangesAsync();
              return user;
        }

        public async Task<User?> Login(User value)
        {
            return await dbContext.Users.FirstOrDefaultAsync(user => user.PasswordHash == value.PasswordHash && user.Email == value.Email);
        }

        public async Task<User> update(int id, User value)
        {
            dbContext.Users.Update(value);
            await dbContext.SaveChangesAsync();
            return value;
        }
        
        public async Task<User> GetUserByIdAndPassword(int id, string password)
        {
            return await dbContext.Users
                 .FirstOrDefaultAsync(u => u.UserId == id && u.PasswordHash == password);
        }
    }
}
