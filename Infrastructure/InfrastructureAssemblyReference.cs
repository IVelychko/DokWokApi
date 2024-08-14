using System.Reflection;

namespace Infrastructure;

public static class InfrastructureAssemblyReference
{
    public static Assembly Assembly => typeof(InfrastructureAssemblyReference).Assembly;
}
