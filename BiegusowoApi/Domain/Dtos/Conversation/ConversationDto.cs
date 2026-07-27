using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Dtos.Conversation;

public record ConversationDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    ConversationParticipantDto Buyer,
    ConversationParticipantDto Seller,
    CursorPaginatedList<MessageDto> Messages);
