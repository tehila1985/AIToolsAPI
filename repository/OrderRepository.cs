using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        myDBContext dbContext;
        public OrderRepository(myDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async  Task<List<Order>> GetOrdersUser(int id)
        { 
             var r=await  dbContext.Orders
                     .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                     .Where(o => o.UserId == id)
                     .OrderByDescending(o => o.OrderDate)
                     .ToListAsync();
            return r;


        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await dbContext.Orders
                                   .Include(o => o.OrderItems)
                                   .ThenInclude(oi => oi.Product)
                                   .FirstOrDefaultAsync(o => o.OrderId == id);  
        }
        public async Task<Order> AddNewOrder(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            { 
                var product = await dbContext.Products.FindAsync(orderItem.ProductId);

                if (product != null)
                {
                    product.Stock -= orderItem.Quantity;
                }
            }
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }
    }

}