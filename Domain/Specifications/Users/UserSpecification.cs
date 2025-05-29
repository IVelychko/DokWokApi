namespace Domain.Specifications.Users;

public class UserSpecification
{
    public bool IncludeRole { get; set; }
    
    public bool NoTracking { get; set; }
    
    public long? Id { get; set; }
    
    public string? UserName { get; set; }
    
    public long? RoleId { get; set; }
    
    public static UserSpecification IncludeAll => new()
    {
        IncludeRole = true
    };
}