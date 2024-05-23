using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class JoinCluster
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            using (IConnection conn = factory.CreateConnection(new List<AmqpTcpEndpoint>
            {
                new AmqpTcpEndpoint{HostName="127.0.0.1", Port=5672},
                new AmqpTcpEndpoint{HostName="127.0.0.1", Port=5673},
                new AmqpTcpEndpoint{HostName="127.0.0.1", Port=5674},
            }))
            {
                using (IModel channel = conn.CreateModel())
                {
                    channel.QueueDeclare(queue: "JoinClusterQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "JoinClusterExchange", type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "JoinClusterQueue", exchange: "JoinClusterExchange", routingKey: string.Empty);

                    Console.ForegroundColor = ConsoleColor.Green;

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    for (int i = 1; i >= 0; i++)
                    {
                        string msg = $"持久化消息{i}";
                        channel.BasicPublish(exchange: "JoinClusterExchange", routingKey: string.Empty, basicProperties: properties,
                            body: Encoding.UTF8.GetBytes(msg));

                        Console.WriteLine($"消息: {msg} 已發送");
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}
