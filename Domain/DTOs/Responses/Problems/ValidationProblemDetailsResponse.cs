namespace Domain.DTOs.Responses.Problems;

public class ValidationProblemDetailsResponse
{
    public required int StatusCode { get; set; }

    public required string Title { get; set; }

    public required IDictionary<string, string[]> Errors { get; set; }
}