using Dto;
using Microsoft.AspNetCore.Mvc;


namespace Services
{
    public interface IProductService
    {
        Task<DtoProduct_Id_Name_Category_Price_Desc_Image> AddNewProduct(DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds productDto);
        Task<Dto_result_product> GetProducts([FromQuery] int position,
           [FromQuery] int skip,
           [FromQuery] string? desc,
           [FromQuery] int? minPrice,
           [FromQuery] int? maxPrice,
           [FromQuery] int?[] categoryIds,
           [FromQuery] int?[] styleIds);
        Task<DtoProduct_Id_Name_Category_Price_Desc_Image> Delete(int id);
        Task<DtoProduct_Id_Name_Category_Price_Desc_Image> GetById(int id);

    }

}