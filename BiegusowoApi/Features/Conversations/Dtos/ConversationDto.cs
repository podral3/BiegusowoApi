using BiegusowoApi.Shared.Helpers;

namespace BiegusowoApi.Features.Conversations.Dtos;

public record ConversationDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    ConversationParticipantDto Receipient,
    CursorPaginatedList<MessageDto> Messages);
