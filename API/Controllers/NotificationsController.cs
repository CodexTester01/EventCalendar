using Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> List()
        {
            return HandleResult(await Mediator.Send(new List.Query()));
        }
    }
}
