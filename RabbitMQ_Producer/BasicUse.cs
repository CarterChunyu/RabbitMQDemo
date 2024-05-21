using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class BasicUse
    {
        public static void Show()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string producerName = Path.GetFileName(Path.GetDirectoryName(dir));

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.Port = 5672;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            using(IConnection connection = factory.CreateConnection())
            {
                using(IModel channel =  connection.CreateModel())
                {
                    // 沒有就創建一個對列
                    channel.QueueDeclare(queue: "OnlyProducerMessage", durable: true, exclusive: false,
                        autoDelete: false, arguments: null) ;

                    // 創建一個交換機
                    channel.ExchangeDeclare(exchange: "OnlyProducerMessageExchange", type: ExchangeType.Direct,
                        durable: true, autoDelete: false, arguments: null);

                    // 交換機綁定對列
                    channel.QueueBind(queue: "OnlyProducerMessage", exchange: "OnlyProducerMessageExchange",
                        routingKey: string.Empty, arguments: null);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"================生產者:{producerName} 已準備發就緒==============");


                    int i = 1;
                    while (true)
                    {
                        string message = $"{producerName} :　這是一條消息{i}";
                        // AMPQ 協定 只能發送byte數組
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "OnlyProducerMessageExchange",
                            routingKey: string.Empty, basicProperties: null, body: body);
                        Console.WriteLine($"=====================消息: {message} 已發送 ================================");
                        i++;
                        Thread.Sleep(500);
                    }


                }
            }
        }
    }
}
