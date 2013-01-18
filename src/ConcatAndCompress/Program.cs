using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace ConcatAndCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            var buildNumber = args.Length > 0 ? args[0] : "0";
            var cf = new ConcatenateFiles();
            var content = cf.Execute();
            Console.WriteLine(cf.RootPath);
            var pf = new ProcessFile();
            pf.Execute(content, cf.RootPath, buildNumber);
        }

    }
}
