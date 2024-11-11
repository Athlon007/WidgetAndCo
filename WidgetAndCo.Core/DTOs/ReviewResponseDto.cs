namespace WidgetAndCo.Core.DTOs;

public record ReviewResponseDto(
    string ProductId,
    string ReviewId,
    Guid UserId,
    string Title,
    string Description,
    int Rating
    )
{
    public ReviewResponseDto() : this("", "", Guid.Empty, "", "", 0)
    {

    }
}