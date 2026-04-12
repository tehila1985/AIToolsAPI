using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using Repository.Models;
namespace Repository
{
  public class CategoryRepository : ICategoryRepository
  {
    myDBContext dbContext;
    public CategoryRepository(myDBContext dbContext)
    {
      this.dbContext = dbContext;
    }
    public async Task<List<Category>> GetCategories()
    {
      return await dbContext.Categories.ToListAsync();
    }
    public async Task<Category> AddNewCategory(Category category)
        {

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }
        public async Task<Category> Delete(int id)
        {
            
            var category = await dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                if (category.Products != null && category.Products.Any())
                {
                    dbContext.Products.RemoveRange(category.Products);
                }
                dbContext.Categories.Remove(category);

                await dbContext.SaveChangesAsync();
            }
            return category;
        }

    }
}
