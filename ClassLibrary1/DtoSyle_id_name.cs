using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public record DtoSyle_id_name(
        int StyleId,
        string Name,
        string Description,
        string ImageUrl
    );
}



//public class DtoSyle_id_name
//{
//    public int StyleId { get; set; }

//    [Required]
//    [StringLength(150)]
//    public string Name { get; set; }

//    [StringLength(500)]
//    public string Description { get; set; }

//    [StringLength(500)]
//    public string ImageUrl { get; set; }
//}