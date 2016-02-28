using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    /// <summary>
    /// This class takes text for a decision tree and compiles it into an anonymous function!
    /// Much faster this way.
    /// </summary>
    public class Compiler
    {
        private static CSharpCodeProvider _Provider = new CSharpCodeProvider();

        public static System.Reflection.MethodInfo Compile(string function, out String err)
        {
            string code = @"
            using System;
            
            namespace InLineNameSpace
            {                
                public class InLineClass
                {                
                    function_here_replace
                }
            }
            ";

            string finalCode = code.Replace("function_here_replace", function);

            CompilerResults results = _Provider.CompileAssemblyFromSource(
                new CompilerParameters(),
                finalCode);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }
                err = sb.ToString();
                return null;
            }

            Type binaryFunction = results.CompiledAssembly.GetType("InLineNameSpace.InLineClass");
            var mi = binaryFunction.GetMethod("Function");
            err = null;
            return mi;
        }
    }
}
