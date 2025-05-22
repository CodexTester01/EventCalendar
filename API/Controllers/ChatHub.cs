using Application.Comments;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Persistence;

namespace API.Controllers
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly Persistence.DataContext _context;
        public ChatHub(IMediator mediator, Persistence.DataContext context)
        {
            _mediator = mediator;
            _context = context;

        }

        public async Task SendComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);

            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);

            var hostId = await _context.ActivityAttendees
                .Where(x => x.ActivityId == command.ActivityId && x.IsHost)
                .Select(x => x.AppUserId)
                .FirstOrDefaultAsync();

            if (hostId != null && hostId != Context.UserIdentifier)
            {
                await Clients.User(hostId)
                    .SendAsync("ReceiveNotification", comment.Value);
            }

        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"];

            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

            var result = await _mediator.Send(new List.Query { ActiviyId = Guid.Parse(activityId) });

            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }

    }
}