namespace cheap.Models.Users;

public class UpdateModel
{
    public Guid Id { get; set; }
    public Boolean IsPrivateProfile { get; set; }
    public DateTime? UpdatedDate = DateTime.Now;
    public String? FirstName { get; set; }
    public String? LastName { get; set; }
    public String? Username { get; set; }
    public String? Password { get; set; }
    public String? Address1 { get; set; }
    public String? Address2 { get; set; }
    public String? Address3 { get; set; }
    public String? City { get; set; }
    public String? State { get; set; }
    public String? Country { get; set; }
    public String? Zip { get; set; }
    public String? Phone { get; set; }
}