using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Dtos.Conversation;

public record ConversationDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    ConversationParticipantDto Receipient,
    CursorPaginatedList<MessageDto> Messages);
