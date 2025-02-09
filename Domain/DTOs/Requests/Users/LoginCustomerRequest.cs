namespace Domain.DTOs.Requests.Users;

public sealed record LoginCustomerRequest(string UserName, string Password);
