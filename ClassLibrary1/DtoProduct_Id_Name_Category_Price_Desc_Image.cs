using Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public record DtoProduct_Id_Name_Category_Price_Desc_Image(
        int ProductId,
        string Name,
        int? CategoryId,
        string CategoryName,
        int? Price,
        int Stock,
        string Description,
        string FrontImageUrl,
        string BackImageUrl,
        ICollection<DtoProductStyle> ProductStyles
    );
}




//public class DtoProduct_Id_Name_Category_Price_Desc_Image
//{
//    public int ProductId { get; set; }

//    [Required(ErrorMessage = "Product name is required")]
//    public string Name { get; set; }

//    public int? CategoryId { get; set; }

//    public string CategoryName { get; set; }

//    [Range(0, int.MaxValue, ErrorMessage = "Price must be bigger than 0")]
//    public int? Price { get; set; }
//    public int Stock { get; set; }

//    public string Description { get; set; }

//    [StringLength(500)]
//    public string FrontImageUrl { get; set; }

//    [StringLength(500)]
//    public string BackImageUrl { get; set; }

//    public virtual ICollection<DtoProductStyle> ProductStyles { get; set; } = new List<DtoProductStyle>();

//}
