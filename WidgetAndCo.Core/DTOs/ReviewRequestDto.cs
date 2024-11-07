namespace WidgetAndCo.Core.DTOs;

public record ReviewRequestDto(
    Guid ProductId,
    string Title,
    string Description,
    int Rating
    );