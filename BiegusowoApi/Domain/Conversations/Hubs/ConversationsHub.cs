using BiegusowoApi.Domain.Dtos.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BiegusowoApi.Domain.Conversations.Hubs;

[Authorize]
public class ConversationsHub(IConversationService conversationService) : Hub
{
    private readonly IConversationService _conversationService = conversationService;
    public override async Task OnConnectedAsync()
    {
        bool success = Guid.TryParse(Context.UserIdentifier, out Guid parsedUserId);
        if (!success) return;

        var paginatedList = await _conversationService.GetUserConversationsAsync(parsedUserId, null, null, 10);
        List<MinimalConversationDto> conversations = paginatedList.Items;
        foreach (var conversation in conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
        }
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(Guid conversationId, string message)
    {
        bool success = Guid.TryParse(Context.UserIdentifier, out Guid parsedUserId);
        if (!success) return;

        if(!await _conversationService.IsUserParticipantInConversationAsync(parsedUserId, conversationId))
            return;

        //if this is a first message we dont have a group

        if (!await _conversationService.ValidateMessageAsync(message)) 
            return;

        MessageDto? savedMessage =  await _conversationService.SaveMessageToDbAsync(conversationId, parsedUserId, message);
        
        if(savedMessage != null)
        {
            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", savedMessage);
        }
    }
}
