using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class StyleRepositoryUnitTests
    {
        [Fact]
        public async Task GetStyles_ReturnsAllStyles()
        {
            var styles = new List<Style>
            {
                new Style { StyleId = 1, Name = "Modern", Description = "Modern style", ImageUrl = "modern.jpg" },
                new Style { StyleId = 2, Name = "Classic", Description = "Classic style", ImageUrl = "classic.jpg" }
            };

            var mockSet = new Mock<DbSet<Style>>();
            mockSet.As<IQueryable<Style>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Style>(styles.AsQueryable().Provider));
            mockSet.As<IQueryable<Style>>().Setup(m => m.Expression).Returns(styles.AsQueryable().Expression);
            mockSet.As<IQueryable<Style>>().Setup(m => m.ElementType).Returns(styles.AsQueryable().ElementType);
            mockSet.As<IQueryable<Style>>().Setup(m => m.GetEnumerator()).Returns(styles.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Style>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Style>(styles.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Styles).Returns(mockSet.Object);

            var repository = new StyleRepository(mockContext.Object);
            var result = await repository.GetStyles();

            Assert.Equal(2, result.Count);
            Assert.Equal("Modern", result[0].Name);
            Assert.Equal("Classic", result[1].Name);
        }

        [Fact]
        public async Task GetStyles_ReturnsEmptyList_WhenNoStyles()
        {
            var styles = new List<Style>();

            var mockSet = new Mock<DbSet<Style>>();
            mockSet.As<IQueryable<Style>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Style>(styles.AsQueryable().Provider));
            mockSet.As<IQueryable<Style>>().Setup(m => m.Expression).Returns(styles.AsQueryable().Expression);
            mockSet.As<IQueryable<Style>>().Setup(m => m.ElementType).Returns(styles.AsQueryable().ElementType);
            mockSet.As<IQueryable<Style>>().Setup(m => m.GetEnumerator()).Returns(styles.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Style>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Style>(styles.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Styles).Returns(mockSet.Object);

            var repository = new StyleRepository(mockContext.Object);
            var result = await repository.GetStyles();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStyles_ReturnsStyles_WithAllProperties()
        {
            var styles = new List<Style>
            {
                new Style { StyleId = 1, Name = "Minimalist", Description = "Minimalist design", ImageUrl = "minimalist.jpg" }
            };

            var mockSet = new Mock<DbSet<Style>>();
            mockSet.As<IQueryable<Style>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Style>(styles.AsQueryable().Provider));
            mockSet.As<IQueryable<Style>>().Setup(m => m.Expression).Returns(styles.AsQueryable().Expression);
            mockSet.As<IQueryable<Style>>().Setup(m => m.ElementType).Returns(styles.AsQueryable().ElementType);
            mockSet.As<IQueryable<Style>>().Setup(m => m.GetEnumerator()).Returns(styles.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Style>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Style>(styles.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Styles).Returns(mockSet.Object);

            var repository = new StyleRepository(mockContext.Object);
            var result = await repository.GetStyles();

            Assert.Single(result);
            Assert.Equal("Minimalist", result[0].Name);
            Assert.Equal("Minimalist design", result[0].Description);
            Assert.Equal("minimalist.jpg", result[0].ImageUrl);
        }

        [Fact]
        public async Task GetStyles_ReturnsMultipleStyles()
        {
            var styles = new List<Style>
            {
                new Style { StyleId = 1, Name = "Modern", Description = "Modern style", ImageUrl = "modern.jpg" },
                new Style { StyleId = 2, Name = "Classic", Description = "Classic style", ImageUrl = "classic.jpg" },
                new Style { StyleId = 3, Name = "Rustic", Description = "Rustic style", ImageUrl = "rustic.jpg" },
                new Style { StyleId = 4, Name = "Industrial", Description = "Industrial style", ImageUrl = "industrial.jpg" }
            };

            var mockSet = new Mock<DbSet<Style>>();
            mockSet.As<IQueryable<Style>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Style>(styles.AsQueryable().Provider));
            mockSet.As<IQueryable<Style>>().Setup(m => m.Expression).Returns(styles.AsQueryable().Expression);
            mockSet.As<IQueryable<Style>>().Setup(m => m.ElementType).Returns(styles.AsQueryable().ElementType);
            mockSet.As<IQueryable<Style>>().Setup(m => m.GetEnumerator()).Returns(styles.GetEnumerator());
            mockSet.As<IAsyncEnumerable<Style>>().Setup(m => m.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Style>(styles.GetEnumerator()));

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Styles).Returns(mockSet.Object);

            var repository = new StyleRepository(mockContext.Object);
            var result = await repository.GetStyles();

            Assert.Equal(4, result.Count);
            Assert.Contains(result, s => s.Name == "Modern");
            Assert.Contains(result, s => s.Name == "Classic");
            Assert.Contains(result, s => s.Name == "Rustic");
            Assert.Contains(result, s => s.Name == "Industrial");
        }
    }
}
