using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace serve.HttpHandlers
{
    public static class ResponseHandlers
    {
        public static void Handle(string requestedResource, NetworkStream stream)
        {
            var fileInfo = new FileInfo(requestedResource);
            var type = Utils.ExtensionFileTypes[fileInfo.Extension];
            var mime = Utils.MimeTypeFromExtension(fileInfo.Extension);

            if (type == FileType.Text)
            {
                TextResponse(stream, requestedResource, mime);
            }
            else
            {
                BinaryResponse(stream, requestedResource, mime);
            }
        }
        public static void NotFoundResponse(NetworkStream stream)
        {
            using var sw = new StreamWriter(stream);
            sw.WriteLine("HTTP/1.1 404 Not found");
            sw.WriteLine($"Content-Type: text/plain");
            sw.WriteLine();
            sw.Flush();
        }

        private static void BinaryResponse(NetworkStream stream, string requestedResource, string mime)
        {
            using var bw = new BinaryWriter(stream);
            byte[] bytes = File.ReadAllBytes(requestedResource);
            bw.Write(Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK{Environment.NewLine}"));
            bw.Write(Encoding.UTF8.GetBytes($"Content-Type: {mime}{Environment.NewLine}"));
            bw.Write(Encoding.UTF8.GetBytes($"Content-Length: {bytes.Length}{Environment.NewLine}"));
            bw.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            bw.Write(bytes);
            bw.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            bw.Flush();
        }

        private static void TextResponse(NetworkStream stream, string requestedResource, string mime)
        {
            using var sw = new StreamWriter(stream);
            string html = File.ReadAllText(requestedResource);
            sw.WriteLine("HTTP/1.1 200 OK");
            sw.WriteLine($"Content-Type: {mime}");
            sw.WriteLine($"Content-Length: {html.Length + 1}");
            sw.WriteLine();
            sw.Write(html);
            sw.WriteLine();
            sw.Flush();
        }
    }
}