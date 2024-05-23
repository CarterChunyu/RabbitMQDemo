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
                using (IModel channel =  connection.CreateModel())
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"消費者{producerName} 已準備就緒");

                        // 事件驅動模式
                        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                        
                        // 接收到消息之後, 觸發事件--就會觸發給事件註冊動作                         
                        consumer.Received += (model, ea) => // 接受消息處理的事件
                        {
                            ReadOnlyMemory<byte> body = ea.Body;
                            string message = Encoding.UTF8.GetString(body.ToArray());
                            Console.WriteLine($"{producerName}: 接收消息 {message}");
                            Thread.Sleep(200);
                        };

                        // 消息確認 -- 自動確認
                        channel.BasicConsume(queue: "OnlyProducerMessage",
                            autoAck: true,
                            consumer: consumer);

                        Console.WriteLine(" Press enter to exit");
                        Console.ReadLine();

                    }
                    catch(Exception ex)
                    {

                    }

                }
            }
        }
    }
}
