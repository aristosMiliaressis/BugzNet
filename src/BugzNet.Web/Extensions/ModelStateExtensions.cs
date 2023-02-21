using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugzNet.Web.Extensions
{
    public static class ModelStateExtensions
    { 
        public static void AddErrorMessages(this ModelStateDictionary modelState, string errorMessages)
        {
            errorMessages.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(err => modelState.AddModelError("", err));
        }

        public static string ToErrorString(this ModelStateDictionary modelState)
        {
            return string.Join(Environment.NewLine, modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }
    }
}
