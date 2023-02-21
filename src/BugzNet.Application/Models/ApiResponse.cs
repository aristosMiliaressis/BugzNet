using AutoMapper;
using BugzNet.Infrastructure.MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Application.Models
{
    /// <summary>
    /// These where reused from the old Controller in order to keep 
    /// backward compatibility and not change the mediator calls
    /// </summary>
    public abstract class ApiResponse
    {
        public bool Success { get; }
        public string PerformedAction { get; }
        public string ResultMessage { get; }

        protected ApiResponse() { }

        public ApiResponse(string action, string message, bool success)
        {
            PerformedAction = action;
            ResultMessage = message;
            Success = success;
        }
    }
    public class SuccessApiResponse : ApiResponse
    {
        private SuccessApiResponse() : base() { }

        public SuccessApiResponse(string message, [CallerMemberName] string action = "")
            : base(action, message, true)
        {

        }
    }

    public class FailApiResponse : ApiResponse
    {
        private FailApiResponse() : base() { }

        public FailApiResponse(string message, [CallerMemberName] string action = "")
            : base(action, message, false)
        {

        }
    }

    public class ExceptionApiResponse : ApiResponse
    {
        public ExceptionApiResponse([CallerMemberName] string action = "")
            : base(action, "Could not Complete the Request.A Critical Error occurred.", false)
        {

        }

        public ExceptionApiResponse(string ExceptionMessage, [CallerMemberName] string action = "")
            : base(action, ExceptionMessage, false)
        {

        }
    }

    /// <summary>
    /// Mapping between Command Responses and Api Responses
    /// </summary>
    public class ApiMappingProfiles : Profile
    {
        public ApiMappingProfiles()
        {
            CreateMap<CommandResponse, SuccessApiResponse>()
                .ForMember(dst => dst.Success, src => src.MapFrom(x => x.IsSuccess))
                .ForMember(dst => dst.ResultMessage, src => src.MapFrom(x => x.Message))
                .ForMember(dst => dst.PerformedAction, src => src.MapFrom(x => x.Message));

            CreateMap<CommandResponse, FailApiResponse>()
               .ForMember(dst => dst.Success, src => src.MapFrom(x => x.IsSuccess))
               .ForMember(dst => dst.ResultMessage, src => src.MapFrom(x => x.ErrorsAsString))
               .ForMember(dst => dst.PerformedAction, src => src.MapFrom(x => x.Message));
        }
    }
}
