namespace WidgetAndCo.Core.DTOs;

public record OrderResponseDto(
    Guid OrderId,
    string UserId,
    List<int> ProductIds,
    decimal Total
    );