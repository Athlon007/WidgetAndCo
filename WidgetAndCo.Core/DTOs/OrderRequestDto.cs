namespace WidgetAndCo.Core.DTOs;

public record OrderRequestDto(
    Guid[] ProductIds
    )
{
    public OrderRequestDto() : this(Array.Empty<Guid>())
    {

    }
}