using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class HeaderExchange
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
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "HeaderExchange", type: ExchangeType.Headers, durable: false,
                        autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue:"HeaderExchangeAllQueue", durable: false, exclusive: false, autoDelete:false,
                        arguments: null);
                    channel.QueueDeclare(queue: "HeaderExchangeAnyQueue", durable: false, exclusive: false, autoDelete: false,
                        arguments: null);
                    channel.QueueBind(queue: "HeaderExchangeAllQueue", exchange: "HeaderExchange", routingKey: string.Empty,
                        arguments: new Dictionary<string, object>
                        {
                            { "x-match", "all"},
                            {"teacher", "guest" },
                            {"pass", "123" }
                        });
                    channel.QueueBind(queue: "HeaderExchangeAnyQueue", exchange: "HeaderExchange", routingKey: string.Empty,
                        arguments: new Dictionary<string, object>
                        {
                            {"x-match","any"},
                            {"teacher", "guest" },
                            {"pass", "123" }
                        });

                    Console.ForegroundColor = ConsoleColor.Green;
                    {

                        IBasicProperties props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>
                        {
                           {"teacher", "guest" },
                            {"pass", "123" }
                        };
                        string message = "teacher和pass完全相同時發送的消息";
                        byte[] body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "HeaderExchange", routingKey: string.Empty, basicProperties: props,
                            body: body);

                        Console.WriteLine($"消息已發送{message}");
                    }


                    {
                        IBasicProperties props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>
                        {
                            {"teacher", "Richard" },
                            {"pass", "123" }
                        };
                        string message = "teacher和pass不完全相同時發送的消息";
                        byte[] body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "HeaderExchange", routingKey: string.Empty, basicProperties: props,
                            body: body);

                        Console.WriteLine($"消息已發送{message}");
                    }
                }
            }
        }
    }
}
