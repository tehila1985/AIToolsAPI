using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Dto
{
    public record DtoCategory_Name_Id(
        int CategoryId,
        string Name
    );
    //public class DtoCategory_Name_Id
    //{
    //    public int CategoryId { get; set; }

    //    public string Name { get; set; }
    //}
}
