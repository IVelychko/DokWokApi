namespace Domain.Models;

public class ProblemDetailsModel
{
    public required int StatusCode { get; set; }

    public required string Title { get; set; }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
