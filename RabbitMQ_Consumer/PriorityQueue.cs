using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Consumer
{
    public class PriorityQueue
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();

            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.Port = 5672;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            // 創建鏈接
            using (IConnection connection = factory.CreateConnection())
            {
                // 創建頻道
                using (IModel channel = connection.CreateModel())
                {
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (obj, ea) =>
                    {
                        string msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"收到消息:{msg}");
                    };

                    channel.BasicConsume(queue: "PriorityQueue", autoAck: false, consumer: consumer);

                    Console.ReadKey();
                }
            }
        }
    } 
}
