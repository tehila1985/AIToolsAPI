using System.ComponentModel.DataAnnotations;

namespace Dto
{
    public record DtoUser_All(
        int UserId,
        string Email,
        string FirstName,
        string LastName,
        string PasswordHash,
        string Phone,
        string Address
    );
}


//public class DtoUser_Name_Password_Gmail
//{
//    public int UserId { get; set; }

//    [Required(ErrorMessage = "Gmail is required")]
//    [EmailAddress(ErrorMessage = "Invalid email")]
//    public string Email { get; set; }
//    public string FirstName { get; set; }
//    public string LastName { get; set; }

//    [Required(ErrorMessage = "Password is required")]
//    public string PasswordHash { get; set; }
//    [StringLength(50)]
//    public string Phone { get; set; }

//    [StringLength(255)]
//    public string Address { get; set; }
//}
