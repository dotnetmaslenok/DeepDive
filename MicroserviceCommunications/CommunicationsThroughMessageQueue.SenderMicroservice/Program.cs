using RabbitMQ.Client;
using System.Text;

namespace CommunicationsThroughMessageQueue.SenderMicroservice;

public class Program
{
	public static void Main(string[] args)
	{
		RabbitMQ_Topics_SenderConfiguration(args);
	}

	public static void RabbitMQ_HelloWorld_SenderConfiguration()
	{
		const string queue = "hello_world_queue";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(
					queue: queue,
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: default);

				var message = "hello world message";
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(
					exchange: string.Empty,
					routingKey: queue,
					basicProperties: null,
					body: body);

				Console.WriteLine($"Sent message: {message}");
			}

			Console.WriteLine("Press [enter] to exit");
			Console.ReadLine();
		}
	}

	public static void RabbitMQ_WorkQueues_SenderConfiguration(string[] args)
	{
		const string queue = "work_queue";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(
					queue: queue,
					durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: default);

				var message = GetMessage(args);
				var body = Encoding.UTF8.GetBytes(message);
				var properties = channel.CreateBasicProperties();

				properties.Persistent = true;

				channel.BasicPublish(
					exchange: string.Empty,
					routingKey: queue,
					properties,
					body: body);

				Console.WriteLine($"Sent message: {message}");

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_SubscribePublish_SenderConfiguration(string[] args)
	{
		const string exchangeName = "logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

				var message = GetMessage(args);
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(
					exchange: exchangeName,
					routingKey: string.Empty,
					basicProperties: default,
					body: body);

				Console.WriteLine($"Sent message: {message}");

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_Routing_SenderConfiguration(string[] args)
	{
		const string exchangeName = "direct_logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

				var logLevel = args.Length > 0 ? args[0] : "info";
				var message = args.Length > 1 ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World. (Default message)";

				var body = Encoding.UTF8.GetBytes(message);
				channel.BasicPublish(exchangeName, logLevel, default, body);

				Console.WriteLine($"Sent message with log level {logLevel}: {message}");

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_Topics_SenderConfiguration(string[] args)
	{
		const string exchangeName = "topic_logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

				var routingKey = args.Length > 0 ? args[0] : "anonymous.info";
				var message = args.Length > 1 ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World. (Default message)";

				var body = Encoding.UTF8.GetBytes(message);
				channel.BasicPublish(exchangeName, routingKey, default, body);

				Console.WriteLine($"Sent message with log level {routingKey}: {message}");

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	private static string GetMessage(string[] args)
	{
		return args.Length > 0 ? string.Join(" ", args) : "Hello World. (Default message)";
	}
}