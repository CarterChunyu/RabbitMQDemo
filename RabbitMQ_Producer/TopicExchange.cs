using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class TopicExchange
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.Port = 5672;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "TopicExchange", type: ExchangeType.Topic, durable: true,
                        autoDelete: false, arguments: null);

                    channel.QueueDeclare(queue: "ChinaQueue", durable: true, exclusive: false, arguments: null);
                    channel.QueueDeclare(queue: "NewsQueue", durable: true, exclusive: false, arguments: null);

                    channel.QueueBind(queue: "ChinaQueue", exchange: "TopicExchange", routingKey: "China.#",
                        arguments: null);
                    channel.QueueBind(queue: "NewsQueue", exchange: "TopicExchange", routingKey: "#.News",
                         arguments: null);

                    Console.ForegroundColor = ConsoleColor.Green;   
                    {
                        string message = "來自中國的新聞消息";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "China.News", basicProperties: null, body: body);
                        Console.WriteLine($"消息{message}已發送至隊列");
                    }
                    {
                        string message = "來自中國的天氣消息";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "China.weather", basicProperties: null, body: body);
                        Console.WriteLine($"消息{message}已發送至隊列");
                    }
                    {
                        string message = "來自美國的新聞消息";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "USA.News", basicProperties: null, body: body);
                        Console.WriteLine($"消息{message}已發送至隊列");
                    }
                    {
                        string message = "來自美國的天氣消息";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "USA.weather", basicProperties: null, body: body);
                        Console.WriteLine($"消息{message}已發送至隊列");
                    }
                }
            }


        }
    }
}
