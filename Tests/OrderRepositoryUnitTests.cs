using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class OrderRepositoryUnitTests
    {
        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenOrderExists()
        {
            var user = new User { UserId = 1, Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            var product = new Product { ProductId = 1, Name = "Sofa", Price = 500, CategoryId = 1, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var order = new Order 
            { 
                OrderId = 1, 
                UserId = 1, 
                User = user,
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 500,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, Product = product, Quantity = 1, PriceAtPurchase = 500 }
                }
            };

            var orders = new List<Order> { order };

            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Order>(orders.AsQueryable().Provider));
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var repository = new OrderRepository(mockContext.Object);
            var result = await repository.GetOrderById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.OrderId);
            Assert.Equal("Pending", result.Status);
            Assert.Single(result.OrderItems);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenOrderDoesNotExist()
        {
            var orders = new List<Order>();

            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Order>(orders.AsQueryable().Provider));
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var repository = new OrderRepository(mockContext.Object);
            var result = await repository.GetOrderById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewOrder_AddsOrderToDatabase()
        {
            var mockSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new OrderRepository(mockContext.Object);
            var newOrder = new Order 
            { 
                UserId = 1, 
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 500 
            };
            
            var result = await repository.AddNewOrder(newOrder);

            mockSet.Verify(m => m.AddAsync(newOrder, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(newOrder, result);
        }

        [Fact]
        public async Task GetOrderById_IncludesOrderItems()
        {
            var user = new User { UserId = 1, Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            var product1 = new Product { ProductId = 1, Name = "Sofa", Price = 500, CategoryId = 1, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { ProductId = 2, Name = "Chair", Price = 100, CategoryId = 1, Description = "Wooden chair", Stock = 20, IsActive = true };
            
            var order = new Order 
            { 
                OrderId = 1, 
                UserId = 1, 
                User = user,
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 600,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, Product = product1, Quantity = 1, PriceAtPurchase = 500 },
                    new OrderItem { OrderItemId = 2, OrderId = 1, ProductId = 2, Product = product2, Quantity = 1, PriceAtPurchase = 100 }
                }
            };

            var orders = new List<Order> { order };

            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Order>(orders.AsQueryable().Provider));
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var repository = new OrderRepository(mockContext.Object);
            var result = await repository.GetOrderById(1);

            Assert.NotNull(result);
            Assert.Equal(2, result.OrderItems.Count);
        }

        [Fact]
        public async Task AddNewOrder_WithOrderItems_AddsSuccessfully()
        {
            var mockSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<myDBContext>(new DbContextOptions<myDBContext>());
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var repository = new OrderRepository(mockContext.Object);
            var newOrder = new Order 
            { 
                UserId = 1, 
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 600,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1, PriceAtPurchase = 500 },
                    new OrderItem { ProductId = 2, Quantity = 1, PriceAtPurchase = 100 }
                }
            };
            
            var result = await repository.AddNewOrder(newOrder);

            mockSet.Verify(m => m.AddAsync(newOrder, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(2, result.OrderItems.Count);
        }
    }
}
