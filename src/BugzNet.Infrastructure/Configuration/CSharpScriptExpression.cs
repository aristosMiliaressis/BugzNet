using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace BugzNet.Infrastructure.Configuration
{
    /// <summary>
    /// can be used to evaluate CSharpScript expression defined in string data.
    /// </summary>
    public class CSharpScriptExpression
    {
        private string _expr;
        public string Expression 
        { 
            get
            {
                return _expr;
            }
            set
            {
                _expr = value;
                try
                {
                    Evaluate();
                    IsValid = true;
                }
                catch
                {
                    IsValid = false;
                }
            }
        }

        public bool IsValid { get; set; }
        


        public bool Evaluate()
        {
            var options = ScriptOptions.Default.AddReferences(Assembly.GetAssembly(typeof(System.Diagnostics.Activity)))
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Collections.Generic.IEnumerable<>)))
                                                .WithImports("System.Collections.Generic");

            var funcType = typeof(Func<>).MakeGenericType(typeof(bool));
            var evalMethod = typeof(CSharpScript).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                .First(m => m.Name == nameof(CSharpScript.EvaluateAsync)
                                                         && m.IsGenericMethod == true)
                                                .MakeGenericMethod(funcType);
            
            var evalTask = evalMethod.Invoke(null, new object[]
            {
                _expr,
                options, null, null, CancellationToken.None
            });

            var filter = typeof(Task<>).MakeGenericType(funcType).GetProperty(nameof(Task<bool>.Result)).GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<bool>.Invoke));
            return (bool)invokeMethod.Invoke(filter, new object[] { });
        }
    }
}