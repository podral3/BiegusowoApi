using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Features.Conversations.Dtos;

public record MessageDto(
    Guid Id,
    Guid SenderId,
    string Body,
    MessageStatus MessageStatus,
    DateTimeOffset CreatedAt);
