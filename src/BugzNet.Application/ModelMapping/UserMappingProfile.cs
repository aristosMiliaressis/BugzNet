using System.Linq;
using AutoMapper;
using BugzNet.Application.MediatR.Requests.Users.Commands;
using BugzNet.Application.MediatR.Requests.Users.Models;
using BugzNet.Application.Requests.Identity.Commands;
using BugzNet.Application.Requests.MyAccount.Models;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Application.ModelMapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<BugzUser, UserProjection>()
                .ForMember(dst => dst.Role, tgt => tgt.MapFrom(src => src.UserRoles.First().Role));

            CreateMap<UserProjection, BugzUser>();

            CreateMap<LoginCommand, BugzUser>();

            CreateMap<BugzUser, MyAccountProjection>(MemberList.Destination);

            CreateMap<BugzUser, CreateEditUserCommand>(MemberList.Destination);
        }
    }
}