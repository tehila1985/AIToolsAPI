using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System;
using System.Linq;

namespace Test
{
    public class DatabaseFixture : IDisposable
    {
        public myDBContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<myDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            Context = new myDBContext(options);
        }

        public void ClearDatabase()
        {
            Context.ChangeTracker.Clear();
            if (Context.OrderItems.Any()) Context.OrderItems.RemoveRange(Context.OrderItems);
            if (Context.Orders.Any()) Context.Orders.RemoveRange(Context.Orders);
            if (Context.ProductStyles.Any()) Context.ProductStyles.RemoveRange(Context.ProductStyles);
            if (Context.Products.Any()) Context.Products.RemoveRange(Context.Products);
            if (Context.Categories.Any()) Context.Categories.RemoveRange(Context.Categories);
            if (Context.Styles.Any()) Context.Styles.RemoveRange(Context.Styles);
            if (Context.Users.Any()) Context.Users.RemoveRange(Context.Users);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}