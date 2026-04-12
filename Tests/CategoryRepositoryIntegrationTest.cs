using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class CategoryRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public CategoryRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories_FromDatabase()
        {
            var category1 = new Category { Name = "Living Room", Description = "Living room furniture", ImageUrl = "living.jpg" };
            var category2 = new Category { Name = "Bedroom", Description = "Bedroom furniture", ImageUrl = "bedroom.jpg" };
            
            _fixture.Context.Categories.Add(category1);
            _fixture.Context.Categories.Add(category2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new CategoryRepository(_fixture.Context);
            var result = await repository.GetCategories();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Living Room");
            Assert.Contains(result, c => c.Name == "Bedroom");
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList_WhenNoCategoriesInDatabase()
        {
            var repository = new CategoryRepository(_fixture.Context);
            var result = await repository.GetCategories();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategories_ReturnsCategories_WithAllProperties()
        {
            var category = new Category 
            { 
                Name = "Office", 
                Description = "Office furniture and supplies", 
                ImageUrl = "office.jpg" 
            };
            
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var repository = new CategoryRepository(_fixture.Context);
            var result = await repository.GetCategories();

            Assert.Single(result);
            Assert.Equal("Office", result[0].Name);
            Assert.Equal("Office furniture and supplies", result[0].Description);
            Assert.Equal("office.jpg", result[0].ImageUrl);
        }
    }
}
