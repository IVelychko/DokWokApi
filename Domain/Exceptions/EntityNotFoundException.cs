namespace Domain.Exceptions;

public class EntityNotFoundException(string? message) : NotFoundException(message);