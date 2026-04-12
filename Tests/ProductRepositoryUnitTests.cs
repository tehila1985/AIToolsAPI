using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class ProductRepositoryUnitTests
    {
        [Fact]
        public async Task AddNewProduct_AddsProductToDatabase()
        {
            var mockSet = new Mock<DbSet<Product>>();
            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Products).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new ProductRepository(mockContext.Object);
            var newProduct = new Product 
            { 
                Name = "New Product", 
                Price = 250, 
                CategoryId = 1, 
                Description = "New product description", 
                Stock = 15, 
                IsActive = true 
            };
            
            var result = await repository.AddNewProduct(newProduct);

            mockSet.Verify(m => m.AddAsync(newProduct, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(newProduct, result);
        }
    }
}
