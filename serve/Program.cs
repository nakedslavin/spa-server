using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using serve.HttpHandlers;

namespace serve
{
	/// <summary>
	/// Simple SOCKET-based HTTP server to serve static files form the local folder.
	/// Used for SPA development purposes.
	/// </summary>
	internal static class Program
    {
	    private static Socket CreateServer(int port)
	    {
		    var ip = IPAddress.Any;
		    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		    socket.Bind(new IPEndPoint(ip, port));
		    socket.Listen(10);
		    return socket;
	    }

	    // ... -p 80 -h domain
        static void Main(string[] args)
        {
	        bool hostAltered = false;
	        (string hostname, int port, string webRootPath) = Utils.ParseArguments(args);
	        
	        Console.CancelKeyPress += delegate {
		        Console.WriteLine("Ctrl-C. Exiting.");
		        if (hostAltered)
			        Utils.AlterHostsFile("/etc/hosts", hostname, true);
	        };
	        Func<string, bool> isRemote = host => !new List<string> {"localhost", "127.0.0.1", "0.0.0.0"}.Contains(host);
	        
	        Console.WriteLine($"Server starting at {hostname}:{port}");
	        try
	        {
		        if (isRemote(hostname))
		        {
			        if (Utils.AlterHostsFile("/etc/hosts", hostname))
				        hostAltered = true;
			        else
				        return;
		        }

		        var server = CreateServer(port);

		        while (true)
		        {
			        using var client = server.Accept();
			        using var stream = new NetworkStream(client);
			        using var textReader = new StreamReader(stream);

			        string requestedResource = RequestHandler.Handle(textReader, webRootPath);
			        if (requestedResource == null)
			        {
				        ResponseHandlers.NotFoundResponse(stream);
				        return;
			        }

			        ResponseHandlers.Handle(requestedResource, stream);
			        client.Shutdown(SocketShutdown.Both);
		        }
	        }
	        catch (Exception ex)
	        {
		        Console.WriteLine($"Cannot start the server. {ex.Message}");
	        }
	        finally
	        {
		        if (hostAltered) 
			        Utils.AlterHostsFile("/etc/hosts", hostname, true);
	        }
        }
    }
}
