using BugzNet.Core.Localization;
using System;
using System.Text.Json.Serialization;

namespace BugzNet.Core.SharedKernel
{
    public abstract class BaseEntity<TId>
    {
        protected BaseEntity()
        {
            CreatedOn = LocalizationUtility.LocalTime;
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public TId Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
    }
}
