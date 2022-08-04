using System.Text;
using RabbitMQ.Client;

namespace video_streamming_proxy.RabbitMq;

public class ProcessVideoStreamQueue
{    
    public static void SendVideoToProcessQueue(string filename)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "video-stream-requested", durable: false, exclusive: false, autoDelete: false,
                arguments: null);
            
            channel.BasicPublish(exchange: "", routingKey: "video-stream-requested", basicProperties: null, Encoding.UTF8.GetBytes(filename));
        }
    }
}