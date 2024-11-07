namespace WidgetAndCo.Core.DTOs;

public record ReviewResponseDto(
    string PartitionKey,
    string RowKey,
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