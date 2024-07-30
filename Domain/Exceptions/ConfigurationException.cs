namespace Domain.Exceptions;

public class ConfigurationException(string error) : Exception(error);
