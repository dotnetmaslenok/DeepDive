namespace DeepDive.CustomHttpServer.ServerHost
{
	internal static class RequestParser
	{
		public static Request Parse(string header)
		{
			var split = header.Split(" ");
			return new Request(GetMethod(split[0]), split[1]);
		}

		private static HttpMethod GetMethod(string headerMethod)
		{
			if (headerMethod == "GET")
			{
				return HttpMethod.Get;
			}

			return HttpMethod.Post;
		}
	}
}
