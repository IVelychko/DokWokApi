namespace Domain.Exceptions;

public class DbException(string error) : Exception(error);
