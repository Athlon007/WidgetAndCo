namespace WidgetAndCo.Core.DTOs;

public record ReviewRequestDto(
    Guid ProductId,
    string Title,
    string Description,
    int Rating
    )
{
    public ReviewRequestDto() : this(Guid.Empty, string.Empty, string.Empty, 0)
    {

    }
}