using BugzNet.Core.SharedKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugzNet.Core.Entities
{
    public class AuditLog : BaseEntity<int>
    {
        public string Table { get; set; }
        public string PrimaryKey { get; set; }
        public AuditAction Action { get; set; }
        public ulong DaysSinceNineteenHundred { get; set; }
        public DateTime StartDate { get; set; }
        public string Username { get; set; }
        public string EntityIdentifier { get; set; }
        public string Changes { get; set; }
        public string RequestPath { get; set; }
        public string TraceIdentifier { get; set; }

        public string GetDecodedChanges()
        {
            try
            {
                return Encoding.UTF8.GetString(CompressUtility.Unzip(Convert.FromBase64String(Changes)));
            }
            catch
            {
                // if exception is thrown it means changes are probably not zipped,
                // so continue to preserve backward compatibility
                return Changes;
            }
        }

        public Dictionary<string, object> ChangesDictionary()
        {
            var changes = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(Changes))
                return changes;

            var decodedChanges = GetDecodedChanges();

            if (decodedChanges.StartsWith("["))
            // ugly solution to parsing json array into adictionary of <string, (old_val, new_val)>
            {
                var updated = JsonConvert.DeserializeObject<List<dynamic>>(decodedChanges);
                foreach (var change in updated)
                {
                    int i = 0;
                    string colName = string.Empty;
                    object originalVal = null;
                    object newVal = null;
                    foreach (var c in change)
                    {
                        if (i == 0)
                        {
                            colName = (string)c;
                        }
                        else if (i == 1)
                            originalVal = (string)c;
                        else
                            newVal = (string)c;
                        i++;
                    }
                    changes.Add(colName, (originalVal, newVal));
                }
            }
            else
                changes = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedChanges);

            return changes;
        }
    }

    public enum AuditAction
    {
        Insert,
        Update,
        Delete,
        BatchChange,
    }
}
