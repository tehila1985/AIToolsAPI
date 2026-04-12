using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public record DtocategoryAll(
        string Name,
        string Description,
        string ImageUrl
    );
}

//public class DtocategoryAll
//{

//    [Required]
//    [StringLength(150)]
//    public string Name { get; set; }

//    [StringLength(500)]
//    public string Description { get; set; }

//    [StringLength(500)]
//    public string ImageUrl { get; set; }


//}
