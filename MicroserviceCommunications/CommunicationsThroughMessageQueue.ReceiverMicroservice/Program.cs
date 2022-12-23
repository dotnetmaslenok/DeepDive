using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommunicationsThroughMessageQueue.ReceiverMicroservice;

public class Program
{
	public static void Main(string[] args)
	{
		RabbitMQ_Topics_ReceiverConfiguration(args);
	}

	public static void RabbitMQ_HelloWorld_ReceiverConfiguration()
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

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var content = Encoding.UTF8.GetString(body);

					Console.WriteLine($"Content from sender microservice: {content}");
				};

				channel.BasicConsume(
					queue: queue,
					autoAck: true,
					consumer: consumer);

				Console.WriteLine("Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_WorkQueues_ReceiverConfiguration()
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

				channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

				Console.WriteLine("Waiting for messages");

				var consumer = new EventingBasicConsumer(channel);

				consumer.Received += (model, ea) =>
				{
					var body = ea.Body.Span;
					var content = Encoding.UTF8.GetString(body);

					Console.WriteLine($"Content from sender microservice: {content}");
					var dots = content.Count(x => x == '.');
					Thread.Sleep(1000 * dots);
					Console.WriteLine($"Done after {dots} seconds");

					channel.BasicAck(ea.DeliveryTag, false);
				};

				channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_SubscribePublish_ReceiverConfiguration()
	{
		const string exchangeName = "logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

				var temporaryQueue = channel.QueueDeclare().QueueName;

				channel.QueueBind(temporaryQueue, exchangeName, string.Empty);

				Console.WriteLine("Waiting for logs");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body.Span;
					var logs = Encoding.UTF8.GetString(body);
					Console.WriteLine($"Logs from sender microservice: {logs}");
				};

				channel.BasicConsume(temporaryQueue, true, consumer);

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public static void RabbitMQ_Routing_ReceiverConfiguration(string[] args)
	{
		const string exchangeName = "direct_logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

				var temporaryQueue = channel.QueueDeclare().QueueName;

				channel.QueueBind(temporaryQueue, exchangeName, string.Empty);

				if (CheckInvalidArgsCount(args))
				{

				}

				foreach (var logLevel in args)
				{
					channel.QueueBind(temporaryQueue, exchangeName, logLevel);
				}

				Console.WriteLine("Waiting for logs");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += OnConsumerReceived;

				channel.BasicConsume(temporaryQueue, true, consumer);

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();

				bool CheckInvalidArgsCount(string[] args)
				{
					if (args.Length >= 1)
					{
						return false;
					}

					Console.Error.WriteLine($"Usage {Environment.GetCommandLineArgs()[0]} [info] [warning] [error]");

					Console.WriteLine("Press [enter] to exit");
					Console.ReadLine();

					Environment.ExitCode = 1;
					return true;
				}

				void OnConsumerReceived(object? model, BasicDeliverEventArgs ea)
				{
					var body = ea.Body.Span;
					var logs = Encoding.UTF8.GetString(body);
					var routingKey = ea.RoutingKey;
					Console.WriteLine($"Sent logs by routing key {routingKey}: {logs}");
				}
			}
		}
	}

	public static void RabbitMQ_Topics_ReceiverConfiguration(string[] args)
	{
		const string exchangeName = "topic_logs";
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnection())
		{
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

				var temporaryQueue = channel.QueueDeclare().QueueName;

				if (args.Length < 1)
				{
					EmitErrorState();

					return;
				}

				foreach (var bindingKey in args)
				{
					channel.QueueBind(temporaryQueue, exchangeName, bindingKey);
				}

				Console.WriteLine("Waiting for messages");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += OnConsumerReceived;
				channel.BasicConsume(temporaryQueue, true, consumer);

				Console.WriteLine("Press [enter] to exit");
				Console.ReadLine();

				void EmitErrorState()
				{
					Console.Error.WriteLine($"Usage {Environment.GetCommandLineArgs()[0]} [binding key...]");
					Console.WriteLine("Press [enter] to exit");
					Console.ReadLine();
					Environment.ExitCode = 1;
				}

				void OnConsumerReceived(object? model, BasicDeliverEventArgs ea)
				{
					var body = ea.Body.Span;
					var content = Encoding.UTF8.GetString(body);
					var routingKey = ea.RoutingKey;
					Console.WriteLine($"Sent logs by routing key {routingKey}: {content}");
				}
			}
		}
	}
}