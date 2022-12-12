namespace DeepDive.CustomHttpServer.ServerHost
{
	public interface IHandler
	{
		Task HandleAsync(Stream networkStream);
	}
}
