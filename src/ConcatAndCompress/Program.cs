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
            var rootPath = Path.GetFullPath(Directory.GetCurrentDirectory()+ @"\src\DecisionCritical.Web\Content\scripts");
            Console.WriteLine(rootPath);
            var concatString = ConcatenateFiles(MainJSFiles(),rootPath);
            var file = ProcessFiles(concatString,rootPath);
            WriteFile(file,rootPath);
        }

        public static IEnumerable<string> MainJSFiles()
        {
            var files = new List<string>();
            files.Add("jquery-1.6.2.min.js");
            files.Add("modernizr-2.0.min.js");
            files.Add("respond.js");
            files.Add("jquery.transform-0.9.3.min.js");
            files.Add("jquery-ui-1.8.14.custom.min.js");
            files.Add("dci.repeatableInputSection.js");
            files.Add("jquery.tokeninput.js");
            files.Add("jquery.validate.js");
            files.Add("jquery.form.js");
            files.Add("jquery.ie-select-style.pack.js");
            files.Add("superfish.js");
            files.Add("cc.utilities.js");
            files.Add("cc.crudForm.js");
            files.Add("jquery.metadata.js");
            files.Add("json2.js");
            files.Add("dci.userProfile.js");
            files.Add("dci.popupCrud.js");
            return files;
        }

        public static string ProcessFiles(string concatString, string rootPath)
        {
            var crunch = new ScriptCruncher();
            var cs = new CodeSettings();
            cs.RemoveUnneededCode = true;
            cs.CollapseToLiteral = true;
            cs.CombineDuplicateLiterals = true;
            cs.InlineSafeStrings = true;
            cs.StripDebugStatements = true;
            cs.LocalRenaming = LocalRenaming.CrunchAll;
            cs.OutputMode = OutputMode.SingleLine;

            var minifyJavaScript = crunch.MinifyJavaScript(concatString, cs);
            return minifyJavaScript;
        }

        public static string ConcatenateFiles(IEnumerable<string> files, string rootPath)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String file in files)
            {
                var filePath = Path.GetFullPath(rootPath +"\\"+ file);
                sb.AppendLine();
                using (TextReader tr = new StreamReader(filePath))
                {
                    sb.Append(tr.ReadToEnd());
                    tr.Close();
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static void WriteFile(string concatString, string rootPath)
        {
            var finalFile = rootPath+ "\\javascript.min.js";
            using (TextWriter tw = new StreamWriter(Path.GetFullPath(finalFile)))
            {
                tw.WriteLine(concatString);
                tw.Close();
            }
        }
    }
}
