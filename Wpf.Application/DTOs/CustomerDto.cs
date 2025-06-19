namespace Wpf.Application.DTOs;

public record CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}