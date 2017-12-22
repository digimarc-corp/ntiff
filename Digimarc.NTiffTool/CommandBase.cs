using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digimarc.NTiffTool
{
    abstract class CommandBase : CommandLineApplication
    {
        protected void OutLn(string str)
        {
            Console.WriteLine(str);
        }
    }
}
