namespace Wpf.Application.Exceptions;

public sealed class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with id '{key}' was not found.")
{
    public string EntityName { get; } = entityName;
    public object Key { get; } = key;
}
