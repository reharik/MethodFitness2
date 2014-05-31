using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using ImageResizer;

namespace CC.Core.Services
{
    public interface IFileHandlerService
    {
        string SaveAndReturnUrlForFile(string root, int tenantId = 0);
        bool DoesImageExist(string url);
        void DeleteFile(string url);
        bool RequsetHasFile();
    }

    public class FileHandlerService : IFileHandlerService
    {
        private readonly ICCSessionContext _sessionContext;

        public FileHandlerService(ICCSessionContext sessionContext)
        {
            _sessionContext = sessionContext;
        }

        public bool DoesImageExist(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;
            }
            catch
            {
                exists = false;
            }
            return exists;

        }

        private HttpPostedFile getFile()
        {
             var file = HttpContext.Current.Request.Files.AllKeys.Length > 0 &&
                   HttpContext.Current.Request.Files[0].ContentLength > 0
                       ? HttpContext.Current.Request.Files[0]
                       : null;
            return file;
        }

        public bool RequsetHasFile()
        {
            var file = getFile();
            return file != null && file.ContentLength > 0;
        }

        public string SaveAndReturnUrlForFile(string root,int tenantId=0)
        {
            var file = getFile();
            if (file == null || file.ContentLength <= 0) return null;

            var pathForFile = GetPhysicalPathForFile(root,tenantId);
            var exists = Directory.Exists(pathForFile);
            if (!exists)
            {
                Directory.CreateDirectory(pathForFile);
            }

            var fileExtension = new FileInfo(file.FileName).Extension.ToLower();
            var generatedFileName = Guid.NewGuid() + fileExtension;
            if (fileExtension == ".gif" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
            {
                GenerateVersions(file, pathForFile, generatedFileName);
                generatedFileName = generatedFileName.Substring(0, generatedFileName.LastIndexOf(".")) + ".jpg";
            }
            else
            {
                file.SaveAs(pathForFile + generatedFileName);
            }
            return GetUrlForFile(generatedFileName, root, tenantId);
        }

        public void DeleteFile(string url)
        {
            var mapPath = _sessionContext.MapPath(url);
            File.Delete(mapPath);
            if (url.EndsWith(".jpg"))
            {
                File.Delete(mapPath.AddImageSizeToName("thumb"));
                File.Delete(mapPath.AddImageSizeToName("large"));
            }
        }

        public void GenerateVersions(HttpPostedFile file, string pathForFile, string origFileName)
        {
            var versions = new Dictionary<string, string>
                                                      {
                                                          {"_thumb", "width=100&height=100&crop=auto&format=jpg"},
                                                          {"", "maxwidth=200&maxheight=200&crop=autoformat=jpg"},
                                                          {"_large", "maxwidth=400&maxheight=400format=jpg"}
                                                      };
            var fileNameNoExtension = origFileName.Substring(0, origFileName.LastIndexOf("."));
            foreach (var suffix in versions.Keys)
            {
                var appndFileName = fileNameNoExtension + suffix;
                var fullFilePath = Path.Combine(pathForFile, appndFileName);
                ImageBuilder.Current.Build(file, fullFilePath, new ResizeSettings(versions[suffix]), false, true);
            }
        }

        private string GetUrlForFile(string fileName, string root, int tenantId = 0)
        {
            root = root.Replace("\\", "/");
            var _tenantId = tenantId>0? @"/" + tenantId:"";
            return @"/" + root + _tenantId + @"/" + fileName;
        }

        private string GetPhysicalPathForFile(string root, int tenantId = 0)
        {
            return ImageResizer.Util.PathUtils.AppPhysicalPath + root + @"\" + (tenantId>0? tenantId + @"\":"");
        }

    }
}