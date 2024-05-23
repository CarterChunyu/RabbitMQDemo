using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class FanoutExchange
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.Port = 5672;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            using(var connection = factory.CreateConnection())
            {
                using(IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "FanoutExchange001_CN", durable: true, exclusive: false,
                        autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "FanoutExchange002_TW", durable: true, exclusive: false,
                      autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "FanoutExchange003_USA", durable: true, exclusive: false,
                      autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "FanoutExchange004_UK", durable: true, exclusive: false,
                      autoDelete: false, arguments: null);

                    channel.ExchangeDeclare(exchange: "FanoutExchange", type: ExchangeType.Fanout, durable: true,
                        autoDelete: false, arguments: null);

                    channel.QueueBind(queue: "FanoutExchange001_CN", exchange: "FanoutExchange",
                        routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "FanoutExchange002_TW", exchange: "FanoutExchange",
                        routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "FanoutExchange003_USA", exchange: "FanoutExchange",
                        routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "FanoutExchange004_UK", exchange: "FanoutExchange",
                        routingKey: string.Empty, arguments: null);

                    Console.ForegroundColor = ConsoleColor.Green;
                    int i = 1;
                    while (true)
                    {
                        var message = $"這是一條消息...{i}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange:"FanoutExchange", routingKey: string.Empty, basicProperties: null,
                            body: body);

                        Console.WriteLine($"通知{message} 已發送到對列");
                        Thread.Sleep(800);
                        i++;
                    }

                }
            }

        }
    }
}
