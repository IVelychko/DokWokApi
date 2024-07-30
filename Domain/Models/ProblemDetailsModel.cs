namespace Domain.Models;

public class ProblemDetailsModel
{
    public required int StatusCode { get; set; }

    public required string Title { get; set; }

    public IEnumerable<string> Errors { get; set; } = [];
}
