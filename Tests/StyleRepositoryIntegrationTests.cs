using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class StyleRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public StyleRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();
        }

        [Fact]
        public async Task GetStyles_ReturnsAllStyles_FromDatabase()
        {
            var style1 = new Style { Name = "Modern", Description = "Modern style", ImageUrl = "modern.jpg" };
            var style2 = new Style { Name = "Classic", Description = "Classic style", ImageUrl = "classic.jpg" };
            
            _fixture.Context.Styles.Add(style1);
            _fixture.Context.Styles.Add(style2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Modern");
            Assert.Contains(result, s => s.Name == "Classic");
        }

        [Fact]
        public async Task GetStyles_ReturnsEmptyList_WhenNoStylesInDatabase()
        {
            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStyles_ReturnsStyles_WithAllProperties()
        {
            var style = new Style 
            { 
                Name = "Minimalist", 
                Description = "Minimalist design with clean lines", 
                ImageUrl = "minimalist.jpg" 
            };
            
            _fixture.Context.Styles.Add(style);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Single(result);
            Assert.Equal("Minimalist", result[0].Name);
            Assert.Equal("Minimalist design with clean lines", result[0].Description);
            Assert.Equal("minimalist.jpg", result[0].ImageUrl);
        }

        [Fact]
        public async Task GetStyles_ReturnsMultipleStyles()
        {
            var styles = new List<Style>
            {
                new Style { Name = "Modern", Description = "Modern style", ImageUrl = "modern.jpg" },
                new Style { Name = "Classic", Description = "Classic style", ImageUrl = "classic.jpg" },
                new Style { Name = "Rustic", Description = "Rustic style", ImageUrl = "rustic.jpg" },
                new Style { Name = "Industrial", Description = "Industrial style", ImageUrl = "industrial.jpg" }
            };
            
            _fixture.Context.Styles.AddRange(styles);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Equal(4, result.Count);
            Assert.Contains(result, s => s.Name == "Modern");
            Assert.Contains(result, s => s.Name == "Classic");
            Assert.Contains(result, s => s.Name == "Rustic");
            Assert.Contains(result, s => s.Name == "Industrial");
        }

        [Fact]
        public async Task GetStyles_ReturnsStylesWithCorrectIds()
        {
            var style1 = new Style { Name = "Scandinavian", Description = "Scandinavian style", ImageUrl = "scandinavian.jpg" };
            var style2 = new Style { Name = "Bohemian", Description = "Bohemian style", ImageUrl = "bohemian.jpg" };
            
            _fixture.Context.Styles.AddRange(style1, style2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.True(s.StyleId > 0));
        }

        [Fact]
        public async Task GetStyles_AfterAddingNewStyle_ReturnsUpdatedList()
        {
            var style1 = new Style { Name = "Contemporary", Description = "Contemporary style", ImageUrl = "contemporary.jpg" };
            _fixture.Context.Styles.Add(style1);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result1 = await repository.GetStyles();
            Assert.Single(result1);

            var style2 = new Style { Name = "Traditional", Description = "Traditional style", ImageUrl = "traditional.jpg" };
            _fixture.Context.Styles.Add(style2);
            await _fixture.Context.SaveChangesAsync();

            var result2 = await repository.GetStyles();
            Assert.Equal(2, result2.Count);
        }

        [Fact]
        public async Task GetStyles_WithNullDescription_ReturnsCorrectly()
        {
            var style = new Style { Name = "Eclectic", Description = null, ImageUrl = "eclectic.jpg" };
            _fixture.Context.Styles.Add(style);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Single(result);
            Assert.Equal("Eclectic", result[0].Name);
            Assert.Null(result[0].Description);
        }

        [Fact]
        public async Task GetStyles_WithNullImageUrl_ReturnsCorrectly()
        {
            var style = new Style { Name = "Coastal", Description = "Coastal style", ImageUrl = null };
            _fixture.Context.Styles.Add(style);
            await _fixture.Context.SaveChangesAsync();

            var repository = new StyleRepository(_fixture.Context);
            var result = await repository.GetStyles();

            Assert.Single(result);
            Assert.Equal("Coastal", result[0].Name);
            Assert.Null(result[0].ImageUrl);
        }
    }
}
