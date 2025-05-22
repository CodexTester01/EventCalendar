using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Notifications
{
    public class List
    {
        public class Query : IRequest<Result<List<NotificationDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<NotificationDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly Application.Interfaces.IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, Application.Interfaces.IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<NotificationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userName = _userAccessor.GetUserName();
                var notifications = await _context.Notifications
                    .Where(x => x.Recipient.UserName == userName)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<NotificationDto>>.Success(notifications);
            }
        }
    }
}
