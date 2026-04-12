using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class UserRepositoryUnitTests
    {
        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@test.com", PasswordHash = "hash1", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false },
                new User { UserId = 2, Email = "user2@test.com", PasswordHash = "hash2", FirstName = "Jane", LastName = "Smith", Role = "Admin", IsClubMember = true }
            };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            mockSet.As<IAsyncEnumerable<User>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUsers();

            Assert.Equal(2, result.Count);
            Assert.Equal("user1@test.com", result[0].Email);
            Assert.Equal("user2@test.com", result[1].Email);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@test.com", PasswordHash = "hash1", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false }
            };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("user1@test.com", result.Email);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
        {
            var users = new List<User>();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var result = await repository.GetUserById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewUser_AddsUserToDatabase()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);
            var newUser = new User { Email = "new@test.com", PasswordHash = "hash", FirstName = "New", LastName = "User", Role = "User", IsClubMember = false };
            
            var result = await repository.AddNewUser(newUser);

            mockSet.Verify(m => m.AddAsync(newUser, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(newUser, result);
        }

        [Fact]
        public async Task Login_ReturnsUser_WhenCredentialsMatch()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user@test.com", PasswordHash = "correcthash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false }
            };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var loginUser = new User { Email = "user@test.com", PasswordHash = "correcthash" };
            
            var result = await repository.Login(loginUser);

            Assert.NotNull(result);
            Assert.Equal("user@test.com", result.Email);
        }

        [Fact]
        public async Task Login_ReturnsNull_WhenCredentialsDoNotMatch()
        {
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user@test.com", PasswordHash = "correcthash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false }
            };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var loginUser = new User { Email = "user@test.com", PasswordHash = "wronghash" };
            
            var result = await repository.Login(loginUser);

            Assert.Null(result);
        }

        [Fact]
        public async Task Update_UpdatesUserInDatabase()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);
            var updatedUser = new User { UserId = 1, Email = "updated@test.com", PasswordHash = "hash", FirstName = "Updated", LastName = "User", Role = "User", IsClubMember = true };
            
            var result = await repository.update(1, updatedUser);

            mockSet.Verify(m => m.Update(updatedUser), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(updatedUser, result);
        }
    }
}
