namespace WidgetAndCo.Core.DTOs;

public record OrderRequestDto(
    string UserId,
    List<Guid> ProductIds
    )
{
    public OrderRequestDto() : this("", new List<Guid>())
    {

    }
}