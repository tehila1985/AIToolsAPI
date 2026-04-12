using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{

    public record Dto_result_product(
        IEnumerable<DtoProduct_Id_Name_Category_Price_Desc_Image> Products,
        int TotalCount
    );
   
}
//public class Dto_result_product
//{
//    public IEnumerable<DtoProduct_Id_Name_Category_Price_Desc_Image> Products { get; set; }
//    public int TotalCount { get; set; }
//}