namespace Domain.DTOs.Responses.Problems;

public class ProblemDetailsResponse
{
    public required int StatusCode { get; set; }

    public required string Title { get; set; }
}