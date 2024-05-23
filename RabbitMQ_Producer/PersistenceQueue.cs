using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class PersistenceQueue
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;
            factory.Port = 5672;
            
            using(IConnection connection = factory.CreateConnection())
            {
                using(IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "PersistenceQueue", durable: true, exclusive: false,
                        autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "PersistenceExchange", type: ExchangeType.Fanout,
                        durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "PersistenceQueue", exchange: "PersistenceExchange", routingKey: string.Empty);

                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;

                    for (int i = 1; i <= 300; i++)
                    {
                        string msg = $"持久化消息入隊---{i}";
                        byte[] bytes = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish("PersistenceExchange", string.Empty, props, bytes);
                    }
                }
            }
        }
    }
}
