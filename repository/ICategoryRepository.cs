
using Repository.Models;

namespace Repository
{
  public interface ICategoryRepository
  {
        Task<List<Category>> GetCategories();
        Task<Category> Delete(int id);
        Task<Category> AddNewCategory(Category category);
  }
}
