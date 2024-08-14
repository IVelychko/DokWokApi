using System.Reflection;

namespace Application;

public static class ApplicationAssemblyReference
{
    public static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
}
