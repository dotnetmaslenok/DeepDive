using System.Net;

namespace DeepDive.CustomHttpServer.ServerHost
{
	public class StaticFileHandler : IHandler
	{
		private readonly string _path;

		public StaticFileHandler(string path)
		{
			_path = path;
		}

		public async Task HandleAsync(Stream networkStream)
		{
			using (var reader = new StreamReader(networkStream))
			{
				using (var writer = new StreamWriter(networkStream))
				{
					string? line;
					var firstLine = await reader.ReadLineAsync();
					while ((line = await reader.ReadLineAsync()) != string.Empty)
					{
						
					}

					var request = RequestParser.Parse(firstLine);
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
}
