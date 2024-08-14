using System.Reflection;

namespace Domain;

public static class DomainAssemblyReference
{
    public static Assembly Assembly => typeof(DomainAssemblyReference).Assembly;
}
