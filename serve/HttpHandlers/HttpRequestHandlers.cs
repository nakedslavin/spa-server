using System;
using System.IO;

namespace serve.HttpHandlers
{
    public static class RequestHandler
    {
        public static string Handle(TextReader reader, string webRootPath)
        {
            string currentLine;
            while (!string.IsNullOrEmpty(currentLine = reader.ReadLine()))
            {
                if (!currentLine.Contains("GET /")) continue;
                Console.WriteLine(currentLine);
                (string resourcePath, bool resourceExists) = GetResourcePath(currentLine, webRootPath);
                Console.WriteLine($"Requested: {resourcePath}, Exists: {resourceExists}");
                if (resourceExists)
                    return resourcePath;
            }

            return null;
        }
        
        private static (string ResourcePath, bool ResourceExists) GetResourcePath(string header, string webRootPath)
        {
            string path = Utils.GetResourcePath(header);
            path = path.TrimStart('/');
            if (string.IsNullOrWhiteSpace(path))
                path = "index.html";
		    
            string resourcePath = Path.Combine(webRootPath, path);
            bool resourceExists = File.Exists(resourcePath);

            return (resourcePath, resourceExists);
        }
    }
}