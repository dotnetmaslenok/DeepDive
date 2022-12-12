using System.Net;
using System.Net.Sockets;

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
					await _handler.HandleAsync(stream);
				}
			}
		}
	}
}