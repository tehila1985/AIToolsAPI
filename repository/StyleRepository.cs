using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class StyleRepository : IStyleRepository
    {
        myDBContext dbContext;
        public StyleRepository(myDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Style>> GetStyles()
        {
            return await dbContext.Styles.ToListAsync();
        }

        public async Task<Style> AddNewStyle(Style style)
        {

            await dbContext.Styles.AddAsync(style);
            await dbContext.SaveChangesAsync();
            return style;
        }
        public async Task<Style> Delete(int id)
        {
            var style = await dbContext.Styles
                .Include(p => p.ProductStyles)
                .FirstOrDefaultAsync(p => p.StyleId == id);

            if (style != null)
            {
                dbContext.ProductStyles.RemoveRange(style.ProductStyles);
                dbContext.Styles.Remove(style);
                await dbContext.SaveChangesAsync();
            }
            return style;
        }

      

    }
}
