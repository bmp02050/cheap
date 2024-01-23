using System.ComponentModel.DataAnnotations;

namespace cheap.Models.Users;

public class RegisterModel
{
    public DateTime? CreateDate = DateTime.Now;

    [Required] public String? FirstName { get; set; }

    [Required] public String? LastName { get; set; }

    [Required] public String? Username { get; set; }

    [Required] public String? Password { get; set; }

    [Required] public String? Email { get; set; }

    public String? Address1 { get; set; }
    public String? Address2 { get; set; }
    public String? Address3 { get; set; }
    public String? City { get; set; }
    public String? State { get; set; }
    public String? Country { get; set; }
    public String? Zip { get; set; }
    public String? Phone { get; set; }
}