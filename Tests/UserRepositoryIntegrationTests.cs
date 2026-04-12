using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class UserRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public UserRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_FromDatabase()
        {
            var user1 = new User { Email = "user1@test.com", PasswordHash = "hash1", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            var user2 = new User { Email = "user2@test.com", PasswordHash = "hash2", FirstName = "Jane", LastName = "Smith", Role = "Admin", IsClubMember = true };
            
            _fixture.Context.Users.Add(user1);
            _fixture.Context.Users.Add(user2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            var result = await repository.GetUsers();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Email == "user1@test.com");
            Assert.Contains(result, u => u.Email == "user2@test.com");
        }

        [Fact]
        public async Task GetUserById_ReturnsCorrectUser()
        {
            var user = new User { Email = "test@test.com", PasswordHash = "hash", FirstName = "Test", LastName = "User", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            var result = await repository.GetUserById(user.UserId);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal("test@test.com", result.Email);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenUserNotFound()
        {
            var repository = new UserRepository(_fixture.Context);
            var result = await repository.GetUserById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewUser_AddsUserSuccessfully()
        {
            var repository = new UserRepository(_fixture.Context);
            var newUser = new User 
            { 
                Email = "newuser@test.com", 
                PasswordHash = "newhash", 
                FirstName = "New", 
                LastName = "User", 
                Role = "User", 
                IsClubMember = false,
                Phone = "1234567890",
                Address = "123 Test St"
            };

            var result = await repository.AddNewUser(newUser);

            Assert.NotNull(result);
            Assert.True(result.UserId > 0);
            Assert.Equal("newuser@test.com", result.Email);

            var userInDb = await repository.GetUserById(result.UserId);
            Assert.NotNull(userInDb);
            Assert.Equal("New", userInDb.FirstName);
        }

        [Fact]
        public async Task Login_ReturnsUser_WithCorrectCredentials()
        {
            var user = new User { Email = "login@test.com", PasswordHash = "correcthash", FirstName = "Login", LastName = "Test", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            var loginAttempt = new User { Email = "login@test.com", PasswordHash = "correcthash" };
            
            var result = await repository.Login(loginAttempt);

            Assert.NotNull(result);
            Assert.Equal("login@test.com", result.Email);
            Assert.Equal("Login", result.FirstName);
        }

        [Fact]
        public async Task Login_ReturnsNull_WithIncorrectPassword()
        {
            var user = new User { Email = "login@test.com", PasswordHash = "correcthash", FirstName = "Login", LastName = "Test", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            var loginAttempt = new User { Email = "login@test.com", PasswordHash = "wronghash" };
            
            var result = await repository.Login(loginAttempt);

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ReturnsNull_WithIncorrectEmail()
        {
            var user = new User { Email = "login@test.com", PasswordHash = "correcthash", FirstName = "Login", LastName = "Test", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            var loginAttempt = new User { Email = "wrong@test.com", PasswordHash = "correcthash" };
            
            var result = await repository.Login(loginAttempt);

            Assert.Null(result);
        }

        [Fact]
        public async Task Update_UpdatesUserSuccessfully()
        {
            var user = new User { Email = "update@test.com", PasswordHash = "hash", FirstName = "Original", LastName = "Name", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new UserRepository(_fixture.Context);
            user.FirstName = "Updated";
            user.IsClubMember = true;
            
            var result = await repository.update(user.UserId, user);

            Assert.Equal("Updated", result.FirstName);
            Assert.True(result.IsClubMember);

            _fixture.Context.ChangeTracker.Clear();
            var updatedUser = await repository.GetUserById(user.UserId);
            Assert.Equal("Updated", updatedUser.FirstName);
            Assert.True(updatedUser.IsClubMember);
        }

        [Fact]
        public async Task AddNewUser_WithAllProperties_SavesCorrectly()
        {
            var repository = new UserRepository(_fixture.Context);
            var newUser = new User 
            { 
                Email = "complete@test.com", 
                PasswordHash = "hash123", 
                FirstName = "Complete", 
                LastName = "User", 
                Role = "Admin", 
                IsClubMember = true,
                Phone = "9876543210",
                Address = "456 Complete Ave"
            };

            var result = await repository.AddNewUser(newUser);

            Assert.Equal("complete@test.com", result.Email);
            Assert.Equal("Complete", result.FirstName);
            Assert.Equal("User", result.LastName);
            Assert.Equal("Admin", result.Role);
            Assert.True(result.IsClubMember);
            Assert.Equal("9876543210", result.Phone);
            Assert.Equal("456 Complete Ave", result.Address);
        }
    }
}
