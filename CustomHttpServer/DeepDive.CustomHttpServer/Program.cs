using DeepDive.CustomHttpServer.ServerHost;

namespace DeepDive.CustomHttpServer
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var server = new Server(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www")));
			await server.StartAsync();
		}
	}
}