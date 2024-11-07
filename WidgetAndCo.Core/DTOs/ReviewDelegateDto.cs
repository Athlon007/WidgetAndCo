namespace WidgetAndCo.Core.DTOs;

public record ReviewDelegateDto(
    Guid UserId,
    Guid ProductId,
    string Title,
    string Description,
    int Rating
    )
{
    public ReviewDelegateDto() : this(Guid.Empty, Guid.Empty, string.Empty, string.Empty, 0)
    {
    }
}