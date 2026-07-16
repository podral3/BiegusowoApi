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
        string userId = Context.UserIdentifier;
        if (userId == null) return;

        var paginatedList = await _conversationService.GetUserConversationsAsync(userId, 1, 10);
        List<MinimalConversationDto> conversations = paginatedList.Items;
        foreach (var conversation in conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id);
        }
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string conversationId, string message)
    {
        string userId = Context.UserIdentifier;
        if (userId == null) return;

        if(!await _conversationService.IsUserParticipantInConversationAsync(userId, conversationId))
            return;

        //if this is a first message we dont have a group

        if (!await _conversationService.ValidateMessageAsync(message)) 
            return;

        MessageDto? savedMessage =  await _conversationService.SaveMessageToDbAsync(conversationId, userId, message);
        
        if(savedMessage != null)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", savedMessage);
        }
    }
}
