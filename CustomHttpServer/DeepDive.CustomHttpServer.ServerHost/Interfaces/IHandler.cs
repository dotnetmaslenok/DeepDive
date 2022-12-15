namespace DeepDive.CustomHttpServer.ServerHost.Interfaces
{
	public interface IHandler
	{
		Task HandleAsync(Stream networkStream, Request request);
	}
}
