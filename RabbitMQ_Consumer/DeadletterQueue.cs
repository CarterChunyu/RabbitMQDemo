using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;
using System.Collections;

namespace RabbitMQ_Consumer
{
    public class DeadletterQueue
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
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (obj, ea) =>
                        {
                            string msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                            {
                                // 消費消息
                                //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                            }
                            {
                                // 寫回死信隊列 requeue: true=>寫回原隊列, false=>寫回死信隊列
                                channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
                            }
                        };
                        channel.BasicConsume(queue: "queue_a", autoAck: false, consumer: consumer);
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    { 
                    }
                }
            }
        }
    }
}