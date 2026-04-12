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
    public record DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds(
        string Name,
        string Description,
        decimal Price,
        string FrontImageUrl,
        string BackImageUrl,
        List<DtoSyle_id_name> ProductStyles,
        int Stock,
        int CategoryId,
        bool IsActive
    );
}




//public class DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds
//{
//    [Required]
//    [StringLength(200)]
//    public string Name { get; set; }

//    [StringLength(1000)]
//    public string Description { get; set; }
//    [Column(TypeName = "decimal(10, 2)")]
//    public decimal Price { get; set; }
//    public int Stock { get; set; }
//    public int CategoryId { get; set; }
//    public bool IsActive { get; set; } = true;
//    [StringLength(500)]
//    public string FrontImageUrl { get; set; }

//    [StringLength(500)]
//    public string BackImageUrl { get; set; }
//    public List<DtoSyle_id_name> ProductStyles { get; set; } = new List<DtoSyle_id_name>();
//}



