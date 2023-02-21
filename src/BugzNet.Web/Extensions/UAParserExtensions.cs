using UAParser;

namespace BugzNet.Web.Extensions
{
    public static class UAParserExtensions
    {
        public static bool TryParse(this Parser parser, string uaString, out string parsedUA)
        {
            try 
            {
                if (uaString == null) 
                {
                    parsedUA = "Unknown";
                    return false;
                }
                else if (uaString.Length < 20)
                {
                    parsedUA = uaString;
                    return true;
                }

                ClientInfo uaInfo = parser.Parse(uaString);

                parsedUA = uaInfo.UA.Family+"/"+uaInfo.UA.Major+"."+uaInfo.UA.Minor;
                if (uaInfo.Device.Family != "Other")
                    parsedUA = uaInfo.Device.Family+"/"+uaInfo;

                return true;
            }
            catch
            {
                parsedUA = "Unknown";
                return false;
            }
        }
    }
}
