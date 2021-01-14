using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace serve
{
    public static class Utils
    {
        public static string EnumToString(HttpHeaders headers)
        {
            string header = headers.ToString();
            string res = string.Empty;
            for (int i = 0; i < header.ToCharArray().Length; i++)
            {
                char el = header.ToCharArray()[i];
                if (Regex.IsMatch(el.ToString(),"^[A-Z]+$") && i > 0)
                {
                    res += '-';
                }
                res += el;
            }
            return res;
        }

        public static string EnumToString(HttpVerb verb) => verb.ToString().ToUpper();

        public static (string hostname, int port, string webRootPath) ParseArguments(IEnumerable<string> args)
        {
            var port = 8080;
            var hostname = "0.0.0.0";
            var webRootPath = Environment.CurrentDirectory;
            
            var arguments = args.ToList();
            if (arguments.Contains("-p"))
            {
                var p = arguments.ElementAtOrDefault(arguments.IndexOf("-p") + 1);
                if (p != null)
                {
                    port = int.Parse(p);
                }
            }
            if (arguments.Contains("-h"))
            {
                var h = arguments.ElementAtOrDefault(arguments.IndexOf("-h") + 1);
                if (h != null)
                {
                    hostname = h;
                }
            }
            if (arguments.Contains("-h") && !arguments.Contains("-p"))
            {
                port = 80;
            }
            if (arguments.Contains("--path"))
            {
                var path = arguments.ElementAtOrDefault(arguments.IndexOf("--path") + 1);
                if (path != null)
                {
                    webRootPath = path;
                }
            }            
            
            return (hostname, port, webRootPath);
        }
        public static string GetResourcePath(string header)
        {
            string path = null;
            string[] headerParts = header.Split(' ');
            if (headerParts.Length == 3)
            {
                path = headerParts.ElementAt(1);
            }

            return path;
        }

        public static bool AlterHostsFile(string hostsFilePath, string host, bool clean = false) {
            if (!File.Exists(hostsFilePath)) {
                Console.WriteLine($"Cannot find the hosts file");
                return false;
            }
            
            var entries = File.ReadAllLines(hostsFilePath);
            var newEntries = new List<string>();
            
            for(int i = 0; i < entries.Length; i++) {
                var entry = entries[i];
                var splits = entry.Split('\t');

                if (splits.Length > 1 && !splits[0].StartsWith('#')) {
                    var ipAddress = splits[0];
                    var mask = splits[1];
                    var comment = splits.ElementAtOrDefault(2);

                    if ((mask == host || mask == $"www.{host}") && comment == "#SPA-SERVER")
                        continue;
                    
                }
                newEntries.Add(entry);
            }

            try {
                if (!clean)
                {
                    newEntries.Add($"127.0.0.1\t{host}\t#SPA-SERVER");
                    if (Regex.IsMatch(host, "[a-z\\-]+\\.[a-z]+"))
                        newEntries.Add($"127.0.0.1\twww.{host}\t#SPA-SERVER");
                }

                File.WriteAllLines(hostsFilePath, newEntries);
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Please rerun the tool as SUDO: {ex.Message}");
                return false;
            }
        }
        
        public static Dictionary<string, FileType> ExtensionFileTypes => new Dictionary<string, FileType>
        {
            {".html", FileType.Text}, {".htm", FileType.Text},
            {".jpg", FileType.Binary}, {".jpeg", FileType.Binary},
            {".ico", FileType.Binary}, {".css", FileType.Text}, {".js", FileType.Text},{".svg", FileType.Text},
            {".json", FileType.Text}, {".txt", FileType.Text},{".map", FileType.Text},
            {".gif", FileType.Binary}, {".png", FileType.Binary},
        };

        public static string MimeTypeFromExtension(string extension)
        {
            if (ExtensionFileTypes[extension] == FileType.Text)
            {
                string mime = $"text/{extension.TrimStart('.')}";
                if (extension == ".svg") mime = "image/svg+xml";
                return mime;
            }
            else
                return $"image/{extension.TrimStart('.').Replace("jpg", "jpeg")}";
        }
    }
}