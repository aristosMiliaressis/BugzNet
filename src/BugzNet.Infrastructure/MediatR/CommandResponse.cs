using System.Collections.Generic;
using System.Linq;

namespace BugzNet.Infrastructure.MediatR
{
    public class CommandResponse
    {
        protected CommandResponse()
        {
            Errors = new CommandError[0];
        }

        public bool IsSuccess => Errors.Count==0;

        public string Message { get; set; } = string.Empty;

        public IReadOnlyCollection<CommandError> Errors { get; set; }

        public string ErrorsAsString => string.Join(",", Errors.Select(e => e.Message));

        public static CommandResponse Ok() {  return new CommandResponse();}

        public static CommandResponse OkWithMessage(string message) { return new CommandResponse() { Message = message }; }

        public static CommandResponse WithError(string error)
        {
            return new CommandResponse
            {
                Errors = new []{new CommandError(error)}
            };
        }

        public static CommandResponse WithError(string template, params string[] args)
        {
            return new CommandResponse
            {
                Errors = new[] { new CommandError(string.Format(template, args)) }
            };
        }

        public static CommandResponse WithErrors(IReadOnlyCollection<CommandError> errors)
        {
            return new CommandResponse
            {
                Errors = errors
            };
        }

        public override string  ToString()
        {
            if (IsSuccess)
            {
                return string.IsNullOrEmpty(Message) ? "Success" : $"Success: {Message}";
            }
            else
            {
                return Errors.Count > 0 ? ErrorsAsString : "Unknown Error";
            }
        }
    }

    public class CommandResponse<T> : CommandResponse
    {
        public T Result { get; set; }

        public static CommandResponse<T> Ok(T result) { return new CommandResponse<T> { Result = result}; }

        public static CommandResponse<T> OkWithMessage(T result,string message) { return new CommandResponse<T> { Result = result, Message = message }; }

        public new static CommandResponse<T> WithError(string error)
        {
            return new CommandResponse<T>
            {
                Errors = new []{new CommandError(error) }
            };
        }

        public static CommandResponse<T> WithError(string key, string error)
        {
            return new CommandResponse<T>
            {
                Errors = new[] { new CommandError(key, error) }
            };
        }

        public new static CommandResponse<T> WithErrors(IReadOnlyCollection<CommandError> errors)
        {
            return new CommandResponse<T>
            {
                Errors = errors
            };
        }

        public static implicit operator T(CommandResponse<T> from)
        {
            return from.Result;
        }
    }
}
