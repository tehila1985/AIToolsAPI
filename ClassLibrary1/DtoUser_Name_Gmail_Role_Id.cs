using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Dto
{
    public record DtoUser_Name_Gmail_Role_Id(
        int UserId,
        string Email,
        string FirstName,
        string LastName,
        string Role
    );

}
