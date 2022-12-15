using System.Net;
using System.Net.Sockets;
using DeepDive.CustomHttpServer.ServerHost.Interfaces;

namespace DeepDive.CustomHttpServer.ServerHost
{
	public class Server
	{
		private readonly IHandler _handler;

		public Server(IHandler handler)
		{
			_handler = handler;
		}

		public async Task StartAsync()
		{
			var listener = new TcpListener(IPAddress.Any, 80);
			listener.Start();

			while (true)
			{
				var client = await listener.AcceptTcpClientAsync();
				using (var stream = client.GetStream())
				{
					using (var reader = new StreamReader(stream))
					{
						string? line;
						var firstLine = await reader.ReadLineAsync();
						while ((line = await reader.ReadLineAsync()) != string.Empty)
						{
						
						}

						var request = RequestParser.Parse(firstLine);
						await _handler.HandleAsync(stream, request);
					}
				}
			}
		}
	}
}