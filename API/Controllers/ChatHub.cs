using Application.Comments;
using Application.Notifications;
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
        private readonly IHubContext<NotificationHub> _notificationHub;

        public ChatHub(IMediator mediator, Persistence.DataContext context, IHubContext<NotificationHub> notificationHub)
        {
            _mediator = mediator;
            _context = context;
            _notificationHub = notificationHub;

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
                var title = await _context.Activities
                    .Where(x => x.Id == command.ActivityId)
                    .Select(x => x.Title)
                    .FirstOrDefaultAsync();

                var message = $"{comment.Value.DisplayName} replied to your activity '{title}'";

                var notification = await _context.Notifications
                    .Where(n => n.RecipientId == hostId && n.Message == message)
                    .OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefaultAsync();

                if (notification != null)
                {
                    var dto = new NotificationDto
                    {
                        Id = notification.Id,
                        Message = notification.Message,
                        CreatedAt = notification.CreatedAt,
                        IsRead = notification.IsRead
                    };

                    await _notificationHub.Clients.User(hostId)
                        .SendAsync("ReceiveNotification", dto);
                }
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