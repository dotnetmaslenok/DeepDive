using System.Net;
using DeepDive.CustomHttpServer.ServerHost.Interfaces;

namespace DeepDive.CustomHttpServer.ServerHost.Handlers
{
	public class StaticFileHandler : IHandler
	{
		private readonly string _path;

		public StaticFileHandler(string path)
		{
			_path = path;
		}

		public async Task HandleAsync(Stream networkStream, Request request)
		{
			using (var writer = new StreamWriter(networkStream))
			{
				var filePath = Path.Combine(_path, request.Path.Substring(1));

				if (File.Exists(filePath))
				{
					using (var fileStream = File.OpenRead(filePath))
					{
						await fileStream.CopyToAsync(networkStream);
						await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, networkStream);
					}
				}
				else
				{
					await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, networkStream);
				}
			}
		}
	}
}
