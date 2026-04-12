using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
  public class ProductRepository : IProductRepository
  {
    myDBContext dbContext;
    public ProductRepository(myDBContext dbContext)
    {
      this.dbContext = dbContext;
    }
        public async Task<(List<Product> Items, int TotalCount)> getProducts(
           [FromQuery] int position,
           [FromQuery] int skip,
           [FromQuery] string? desc,
           [FromQuery] int? minPrice,
           [FromQuery] int? maxPrice,
           [FromQuery] int?[] categoryIds,
           [FromQuery] int?[] styleIds)
        {
            var query = dbContext.Products.AsQueryable();

            query = query.Where(product =>
                (desc == null || product.Description.Contains(desc))
                && (!minPrice.HasValue || product.Price >= minPrice.Value)
                && (!maxPrice.HasValue || product.Price <= maxPrice.Value)
                && (categoryIds == null || categoryIds.Length == 0 || categoryIds.Contains(product.CategoryId))
                && (styleIds == null || styleIds.Length == 0 || product.ProductStyles.Any(ps => styleIds.Contains(ps.StyleId)))
            );

            query = query.OrderBy(product => product.Price);

            var total = await query.CountAsync();

            int recordsToSkip = (position > 0) ? (position - 1) * skip : 0;

            List<Product> products = await query
                .Skip(recordsToSkip)
                .Take(skip)
                .Include(product => product.Category)
                .ToListAsync();

            return (products, total);
        }
        public async Task<Product> GetById(int id)
        {
            return await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task<Product> AddNewProduct(Product product)
        {

            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }
        public async Task<Product> Delete(int id)
        {
            var product = await dbContext.Products
                .Include(p => p.ProductStyles)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                dbContext.ProductStyles.RemoveRange(product.ProductStyles);
                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();
            }
            return product;
        }

    }
}
