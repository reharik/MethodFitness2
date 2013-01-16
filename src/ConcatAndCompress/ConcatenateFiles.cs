namespace ConcatAndCompress
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using MethodFitness.Web.Config;

    public class ConcatenateFiles
    {
        public string RootPath { get; set; }
        public string Execute()
        {
            var rootDir = GetRootDir();
            return HandleFiles(rootDir);
        }

        public DirectoryInfo GetRootDir()
        {
            var exeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (exeDir.Name != "src" || exeDir == null)
            {
                exeDir = exeDir.Parent;
            }
            var rootDir = new DirectoryInfo(exeDir.FullName + "\\MethodFitness.Web\\Content\\scripts");
            RootPath = rootDir.FullName;
            return rootDir;
        }

        private string HandleFiles(DirectoryInfo dir)
        {
            var fileList = new JavascriptFilesList().FileList(true);
            var sb = new StringBuilder();
            fileList.ForEach( x =>
                    {
                        sb.AppendLine();
                        using (TextReader tr = new StreamReader(dir.FullName + "\\" + x))
                        {
                            sb.Append(tr.ReadToEnd());
                            tr.Close();
                        }
                        sb.AppendLine();
                    });
            return sb.ToString();
        }

        
    }
}