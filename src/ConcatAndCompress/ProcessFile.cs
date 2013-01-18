namespace ConcatAndCompress
{
    using System;
    using System.IO;

    using MethodFitness.Core.Domain;

    using Microsoft.Ajax.Utilities;

    public class ProcessFile
    {
        public void Execute(string concatString, string rootPath, string buildNumber)
        {
            var processedString = ProcessFiles(concatString);
            WriteFile(processedString, rootPath, buildNumber);
        }

        public string ProcessFiles(string concatString)
        {
            var crunch = new ScriptCruncher();
            var cs = new CodeSettings();
            cs.RemoveUnneededCode = true;
            cs.CollapseToLiteral = true;
            cs.CombineDuplicateLiterals = true;
            cs.InlineSafeStrings = true;
            cs.StripDebugStatements = true;
            cs.LocalRenaming = LocalRenaming.CrunchAll;
//            cs.OutputMode = OutputMode.SingleLine;

            var minifyJavaScript = crunch.MinifyJavaScript(concatString, cs);
            return minifyJavaScript;
        }

        public void WriteFile(string concatString, string rootPath, string buildNumber)
        {
            var finalFile = rootPath + "\\javascript.min." + buildNumber + ".js";
            using (TextWriter tw = new StreamWriter(Path.GetFullPath(finalFile)))
            {
                tw.WriteLine(concatString);
                tw.Close();
            }
        } 
    }
}