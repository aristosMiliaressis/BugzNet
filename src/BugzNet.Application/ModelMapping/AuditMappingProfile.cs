using AutoMapper;
using BugzNet.Application.Requests.Audit.Models;
using BugzNet.Core.Entities;

namespace BugzNet.Application.ModelMapping
{
    public class AuditMappingProfile : Profile
    {
        public AuditMappingProfile()
        {
            CreateMap<AuditLog, AuditEntryProjection>()
                .ForMember(src => src.Id, dst => dst.MapFrom(s => s.Id))
                .ForMember(src => src.Action, dst => dst.MapFrom(s => s.Action))
                .ForMember(src => src.User, dst => dst.MapFrom(s => s.Username))
                .ForMember(src => src.Table, dst => dst.MapFrom(s => s.Table))
                .ForMember(src => src.At, dst => dst.MapFrom(s => s.StartDate.ToString("HH:mm M/dd/yyyy")))
                .ForMember(src => src.StrPrimaryKey, dst => dst.MapFrom(s => s.PrimaryKey))
                .ForMember(src => src.TraceIdentifier, dst => dst.MapFrom(s => s.TraceIdentifier));

            CreateMap<AuditLog, DetailedAuditEntryProjection>()
                .ForMember(src => src.Id, dst => dst.MapFrom(s => s.Id))
                .ForMember(src => src.Action, dst => dst.MapFrom(s => s.Action.ToString()))
                .ForMember(src => src.User, dst => dst.MapFrom(s => s.Username))
                .ForMember(src => src.Table, dst => dst.MapFrom(s => s.Table))
                .ForMember(src => src.StartDate, dst => dst.MapFrom(s => s.StartDate.ToString("dd/MM/yyyy HH:mm")))
                .ForMember(src => src.StrPrimaryKey, dst => dst.MapFrom(s => s.PrimaryKey))
                .ForMember(src => src.Changes, dst => dst.MapFrom(s => s.ChangesDictionary()))
                .ForMember(src => src.RequestPath, dst => dst.MapFrom(s => s.RequestPath));
        }
    }
}