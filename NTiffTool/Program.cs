using Microsoft.Extensions.CommandLineUtils;
using NTiffTool.Commands;
using System;

namespace NTiffTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = "tifftool";
            app.Description = "TIFF metadata inspection and update tool.";
            app.HelpOption("-?|-h|--help");

            app.Commands.Add(new Inspect());
            app.Commands.Add(new Compare());
            app.OnExecute(() =>
            {
                app.ShowHint();
                return 0;
            });

            app.Execute(args);

            Console.ReadKey();
        }
    }
}
