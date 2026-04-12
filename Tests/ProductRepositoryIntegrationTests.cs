using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class ProductRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public ProductRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts_WithNoFilters()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_Description()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, "sofa", null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.Equal("Sofa", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_MinPrice()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, 400, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.Equal("Sofa", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_MaxPrice()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, 200, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.Equal("Chair", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_PriceRange()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            var product3 = new Product { Name = "Table", Price = 300, CategoryId = category.CategoryId, Description = "Dining table", Stock = 5, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2, product3);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, 200, 400, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.Equal("Table", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_CategoryId()
        {
            var category1 = new Category { Name = "Furniture", Description = "Home furniture" };
            var category2 = new Category { Name = "Decor", Description = "Home decor" };
            _fixture.Context.Categories.AddRange(category1, category2);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category1.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Vase", Price = 50, CategoryId = category2.CategoryId, Description = "Decorative vase", Stock = 30, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, null, new int?[] { category1.CategoryId }, Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.Equal("Sofa", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_FiltersBy_StyleId()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var style1 = new Style { Name = "Modern", Description = "Modern style" };
            var style2 = new Style { Name = "Classic", Description = "Classic style" };
            _fixture.Context.Styles.AddRange(style1, style2);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var productStyle1 = new ProductStyle { ProductId = product1.ProductId, StyleId = style1.StyleId };
            var productStyle2 = new ProductStyle { ProductId = product2.ProductId, StyleId = style2.StyleId };
            _fixture.Context.ProductStyles.AddRange(productStyle1, productStyle2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, null, Array.Empty<int?>(), new int?[] { style1.StyleId });

            Assert.Single(result.Items);
            Assert.Equal("Sofa", result.Items[0].Name);
        }

        [Fact]
        public async Task GetProducts_Pagination_ReturnsCorrectPage()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                var product = new Product { Name = $"Product{i}", Price = i * 100, CategoryId = category.CategoryId, Description = $"Product {i}", Stock = 10, IsActive = true };
                _fixture.Context.Products.Add(product);
            }
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(2, 2, null, null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetProducts_OrdersByPrice()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Expensive", Price = 1000, CategoryId = category.CategoryId, Description = "Expensive item", Stock = 1, IsActive = true };
            var product2 = new Product { Name = "Cheap", Price = 50, CategoryId = category.CategoryId, Description = "Cheap item", Stock = 100, IsActive = true };
            var product3 = new Product { Name = "Medium", Price = 300, CategoryId = category.CategoryId, Description = "Medium item", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2, product3);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Equal(3, result.Items.Count);
            Assert.Equal("Cheap", result.Items[0].Name);
            Assert.Equal("Medium", result.Items[1].Name);
            Assert.Equal("Expensive", result.Items[2].Name);
        }

        [Fact]
        public async Task AddNewProduct_AddsProductSuccessfully()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var newProduct = new Product 
            { 
                Name = "New Product", 
                Price = 250, 
                CategoryId = category.CategoryId, 
                Description = "New product description", 
                Stock = 15, 
                IsActive = true,
                FrontImageUrl = "front.jpg",
                BackImageUrl = "back.jpg"
            };

            var result = await repository.AddNewProduct(newProduct);

            Assert.NotNull(result);
            Assert.True(result.ProductId > 0);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(250, result.Price);
        }

        [Fact]
        public async Task GetProducts_IncludesCategory()
        {
            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            _fixture.Context.Products.Add(product);
            await _fixture.Context.SaveChangesAsync();

            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, null, null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Single(result.Items);
            Assert.NotNull(result.Items[0].Category);
            Assert.Equal("Furniture", result.Items[0].Category.Name);
        }

        [Fact]
        public async Task GetProducts_ReturnsEmpty_WhenNoProductsMatchFilters()
        {
            var repository = new ProductRepository(_fixture.Context);
            var result = await repository.getProducts(1, 10, "nonexistent", null, null, Array.Empty<int?>(), Array.Empty<int?>());

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
