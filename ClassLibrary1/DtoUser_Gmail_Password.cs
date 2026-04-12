using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dto
{
    public record DtoUser_Gmail_Password(
        string Email,
        string PasswordHash
    );
} 
