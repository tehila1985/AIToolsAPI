using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class CategoryRepositoryUnitTests
    {
        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Furniture", Description = "Home furniture", ImageUrl = "img1.jpg" },
                new Category { CategoryId = 2, Name = "Decor", Description = "Home decor", ImageUrl = "img2.jpg" }
            };

            var mockSet = new Mock<DbSet<Category>>();
            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Category>(categories.AsQueryable().Provider));
            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Category>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Category>(categories.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Categories).Returns(mockSet.Object);

            var repository = new CategoryRepository(mockContext.Object);
            var result = await repository.GetCategories();

            Assert.Equal(2, result.Count);
            Assert.Equal("Furniture", result[0].Name);
            Assert.Equal("Decor", result[1].Name);
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList_WhenNoCategories()
        {
            var categories = new List<Category>();

            var mockSet = new Mock<DbSet<Category>>();
            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Category>(categories.AsQueryable().Provider));
            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Category>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Category>(categories.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Categories).Returns(mockSet.Object);

            var repository = new CategoryRepository(mockContext.Object);
            var result = await repository.GetCategories();

            Assert.Empty(result);
        }
    }
}
