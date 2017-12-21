using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTiffTool
{
    abstract class CommandBase : CommandLineApplication
    {
        protected void OutLn(string str)
        {
            Console.WriteLine(str);
        }
    }
}
