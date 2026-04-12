using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Services
{
    public interface IAiService
    {
        Task<DtoChatResponse> ChatAsync(string userMessage);
        
        Task<List<int>> AnalyzeImageAsync(
            IFormFile image,
            IEnumerable<DtoProduct_Id_Name_Category_Price_Desc_Image> allProducts,
            IEnumerable<DtoSyle_id_name> styles, 
            IEnumerable<DtoCategory_Name_Id> categories
        );
    }
}