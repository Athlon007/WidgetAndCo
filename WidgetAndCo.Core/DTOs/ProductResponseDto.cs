namespace WidgetAndCo.Core.DTOs;

public record ProductResponseDto(
    Guid Id,
    string Name,
    decimal Price,
    string Description,
    Uri ImageUri
    )
{
    public ProductResponseDto() : this(Guid.Empty, string.Empty, 0, string.Empty, null)
    {
    }
}