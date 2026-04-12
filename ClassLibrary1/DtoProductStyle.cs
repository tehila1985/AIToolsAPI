using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public record DtoProductStyle(
        int ProductStyleId,
        int ProductId,
        int StyleId
    );

}
//public class DtoProductStyle
//{
//    [Key]
//    public int ProductStyleId { get; set; }

//    public int ProductId { get; set; }

//    public int StyleId { get; set; }
//}