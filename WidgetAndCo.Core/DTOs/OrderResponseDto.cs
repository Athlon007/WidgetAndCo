namespace WidgetAndCo.Core.DTOs;

public record OrderResponseDto(
    Guid OrderId,
    string UserId,
    List<Guid> ProductIds,
    decimal Total
    );