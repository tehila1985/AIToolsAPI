
using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace Repository
{
  public interface IProductRepository
  {
        
        Task<Product> AddNewProduct(Product product);
       Task<(List<Product> Items, int TotalCount)> getProducts([FromQuery] int position,
           [FromQuery] int skip,
           [FromQuery] string? desc,
           [FromQuery] int? minPrice,
           [FromQuery] int? maxPrice,
           [FromQuery] int?[] categoryIds,
           [FromQuery] int?[] styleIds);
        Task<Product> Delete(int id);
        Task<Product> GetById(int id);

    }

}