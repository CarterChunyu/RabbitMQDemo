using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Consumer
{
    public class BasicGet
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
                    // 消費者作為主動方
                    while (channel.MessageCount(queue: "queue_a") > 0)
                    {
                        // BasicGetResult result = channel.BasicGet(queue: "queue_a", autoAck: true);
                        BasicGetResult result = channel.BasicGet(queue: "queue_a", autoAck: false);
                        channel.BasicReject(result.DeliveryTag, requeue: false);
                        

                        string msg = Encoding.UTF8.GetString(result.Body.ToArray());
                        Console.WriteLine($"拉取到消息=> {msg}");
                    }
                }
            }
        }
    }
}
