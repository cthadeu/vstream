using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using video_streamming_proxy.Domain;

public class ProcessedVideoStreamConsumerTask : BackgroundService
{
    public override Task ExecuteTask => base.ExecuteTask;
    private IConnection _connection;
    private IModel _channel;

    private IMediaRepository _mediaRepository;

    public ProcessedVideoStreamConsumerTask(IMediaRepository mediaRepository)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "video-stream-completed", durable: false, exclusive: false, autoDelete: false,
                arguments: null);
        _mediaRepository = mediaRepository;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"RECEIVED MESSAGE => {message}");
            var id = Guid.Parse(message);
            Console.WriteLine($"ID MESSAGE => {id}");
            var media = await _mediaRepository.GetById(id);
            Console.WriteLine($"MEDIA => {media}");
            if (media != null) {
                Console.WriteLine("UPDATING STATUS");
                media.Status = MediaStatus.Completed;
                await _mediaRepository.Update(media);
            }
            
        };
        _channel.BasicConsume(queue: "video-stream-completed", autoAck: true, consumer: consumer);        
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}