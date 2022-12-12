using System.Net;

namespace DeepDive.CustomHttpServer.ServerHost
{
	internal static class ResponseWriter
	{
		public static async Task WriteStatusAsync(HttpStatusCode code, Stream stream)
		{
			using (var writer = new StreamWriter(stream))
			{
				await writer.WriteLineAsync($"HTTP/1.1 {(int)code} {code}{Environment.NewLine}");
			}
		}
	}
}
