namespace BugzNet.Infrastructure.MediatR
{
    public class CommandError
    {
        public CommandError(string message)
        {
            Key = "";
            Message = message;
        }

        public CommandError(string key, string message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; }

        public string Message { get; }
    }
}
