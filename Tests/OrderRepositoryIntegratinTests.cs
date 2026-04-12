using Repository;
using Repository.Models;
using Xunit;

namespace Test
{
    public class OrderRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public OrderRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder_WithOrderItems()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            _fixture.Context.Products.Add(product);
            await _fixture.Context.SaveChangesAsync();

            var order = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Pending", TotalPrice = 500 };
            _fixture.Context.Orders.Add(order);
            await _fixture.Context.SaveChangesAsync();

            var orderItem = new OrderItem { OrderId = order.OrderId, ProductId = product.ProductId, Quantity = 1, PriceAtPurchase = 500 };
            _fixture.Context.OrderItems.Add(orderItem);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var result = await repository.GetOrderById(order.OrderId);

            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Single(result.OrderItems);
            Assert.Equal(product.ProductId, result.OrderItems.First().ProductId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenOrderNotFound()
        {
            var repository = new OrderRepository(_fixture.Context);
            var result = await repository.GetOrderById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewOrder_AddsOrderSuccessfully()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var newOrder = new Order 
            { 
                UserId = user.UserId, 
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 500 
            };

            var result = await repository.AddNewOrder(newOrder);

            Assert.NotNull(result);
            Assert.True(result.OrderId > 0);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(500, result.TotalPrice);
        }

        [Fact]
        public async Task AddNewOrder_WithOrderItems_AddsSuccessfully()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var newOrder = new Order 
            { 
                UserId = user.UserId, 
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 600,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = product1.ProductId, Quantity = 1, PriceAtPurchase = 500 },
                    new OrderItem { ProductId = product2.ProductId, Quantity = 1, PriceAtPurchase = 100 }
                }
            };

            var result = await repository.AddNewOrder(newOrder);

            Assert.NotNull(result);
            Assert.True(result.OrderId > 0);
            Assert.Equal(2, result.OrderItems.Count);
        }

        [Fact]
        public async Task GetOrderById_IncludesProductDetails()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            _fixture.Context.Products.Add(product);
            await _fixture.Context.SaveChangesAsync();

            var order = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Pending", TotalPrice = 500 };
            _fixture.Context.Orders.Add(order);
            await _fixture.Context.SaveChangesAsync();

            var orderItem = new OrderItem { OrderId = order.OrderId, ProductId = product.ProductId, Quantity = 1, PriceAtPurchase = 500 };
            _fixture.Context.OrderItems.Add(orderItem);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var result = await repository.GetOrderById(order.OrderId);

            Assert.NotNull(result);
            Assert.NotNull(result.OrderItems.First().Product);
            Assert.Equal("Sofa", result.OrderItems.First().Product.Name);
        }

        [Fact]
        public async Task AddNewOrder_WithMultipleQuantities_AddsSuccessfully()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            _fixture.Context.Products.Add(product);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var newOrder = new Order 
            { 
                UserId = user.UserId, 
                OrderDate = DateTime.Now, 
                Status = "Pending", 
                TotalPrice = 500,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = product.ProductId, Quantity = 5, PriceAtPurchase = 100 }
                }
            };

            var result = await repository.AddNewOrder(newOrder);

            Assert.NotNull(result);
            Assert.Single(result.OrderItems);
            Assert.Equal(5, result.OrderItems.First().Quantity);
        }

        [Fact]
        public async Task GetOrderById_WithMultipleOrderItems_ReturnsAll()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var category = new Category { Name = "Furniture", Description = "Home furniture" };
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var product1 = new Product { Name = "Sofa", Price = 500, CategoryId = category.CategoryId, Description = "Comfortable sofa", Stock = 10, IsActive = true };
            var product2 = new Product { Name = "Chair", Price = 100, CategoryId = category.CategoryId, Description = "Wooden chair", Stock = 20, IsActive = true };
            var product3 = new Product { Name = "Table", Price = 300, CategoryId = category.CategoryId, Description = "Dining table", Stock = 5, IsActive = true };
            _fixture.Context.Products.AddRange(product1, product2, product3);
            await _fixture.Context.SaveChangesAsync();

            var order = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Pending", TotalPrice = 900 };
            _fixture.Context.Orders.Add(order);
            await _fixture.Context.SaveChangesAsync();

            var orderItem1 = new OrderItem { OrderId = order.OrderId, ProductId = product1.ProductId, Quantity = 1, PriceAtPurchase = 500 };
            var orderItem2 = new OrderItem { OrderId = order.OrderId, ProductId = product2.ProductId, Quantity = 1, PriceAtPurchase = 100 };
            var orderItem3 = new OrderItem { OrderId = order.OrderId, ProductId = product3.ProductId, Quantity = 1, PriceAtPurchase = 300 };
            _fixture.Context.OrderItems.AddRange(orderItem1, orderItem2, orderItem3);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            var result = await repository.GetOrderById(order.OrderId);

            Assert.NotNull(result);
            Assert.Equal(3, result.OrderItems.Count);
            Assert.Equal(900, result.TotalPrice);
        }

        [Fact]
        public async Task AddNewOrder_WithDifferentStatuses_AddsSuccessfully()
        {
            var user = new User { Email = "user@test.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = "User", IsClubMember = false };
            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync();

            var repository = new OrderRepository(_fixture.Context);
            
            var order1 = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Pending", TotalPrice = 100 };
            var order2 = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Completed", TotalPrice = 200 };
            var order3 = new Order { UserId = user.UserId, OrderDate = DateTime.Now, Status = "Cancelled", TotalPrice = 300 };

            await repository.AddNewOrder(order1);
            await repository.AddNewOrder(order2);
            await repository.AddNewOrder(order3);

            var result1 = await repository.GetOrderById(order1.OrderId);
            var result2 = await repository.GetOrderById(order2.OrderId);
            var result3 = await repository.GetOrderById(order3.OrderId);

            Assert.Equal("Pending", result1.Status);
            Assert.Equal("Completed", result2.Status);
            Assert.Equal("Cancelled", result3.Status);
        }
    }
}
