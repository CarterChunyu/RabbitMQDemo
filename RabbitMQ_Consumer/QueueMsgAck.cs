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
    public class QueueMsgAck
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


                        //consumer.Received += (model, ea) =>
                        //{
                        //    // 即使拋出異常, 消息依舊會被消費掉
                        //    //throw new Exception();
                        //    ReadOnlyMemory<byte> body = ea.Body;
                        //    string msg = Encoding.UTF8.GetString(body.ToArray());
                        //    Console.WriteLine($"接收到消息: {msg}");
                        //};

                        ////自動確認
                        //channel.BasicConsume(queue: "PersistenceQueue", autoAck: true, consumer: consumer);



                        // 服務質量: 方法中參數為預取長度,一般設置為0即可表示長度不限, prefetchCount表示預取得數量,
                        // global表示是否在Connection中全局設置,true表示Connection下所有的channel都設置這個配置。
                        // 僅對顯示確認有效果
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 2, global: false);

                        int i = 1;
                        consumer.Received += (model, ea) =>
                        {
                            ReadOnlyMemory<byte> body = ea.Body;
                            string msg = Encoding.UTF8.GetString(body.ToArray());

                            if (i <= 100)
                            {
                                Console.WriteLine($"{msg} 消息已經被消費, 同時從 RabbitMQ服務器刪除---i:{i}");
                                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                            else
                            {
                                Console.WriteLine($"{msg} 消息未被消費, 也不會被RabbitMQ服務器刪除---i:{i}");
                                // requeue : false, 消息樣會被消費掉
                                channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
                                Thread.Sleep(500);
                            }

                            i++;
                        };

                        // 手動確認 顯示確認
                        channel.BasicConsume(queue: "PersistenceQueue", autoAck: false, consumer: consumer);





                        Console.WriteLine(" Press enter to exit");
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
