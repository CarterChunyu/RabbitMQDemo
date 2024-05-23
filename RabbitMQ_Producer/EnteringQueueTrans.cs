using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class EnteringQueueTrans
    {
        public static void ShowConfirm()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;
            factory.Port = 5672;

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "EnteringQueueTrans", durable: true, exclusive: false, autoDelete: false,
                        arguments: null);
                    channel.ExchangeDeclare(type: ExchangeType.Fanout, exchange: "EnteringQueueTransExchange", durable: true, autoDelete: false,
                        arguments: null);
                    channel.QueueBind(queue: "EnteringQueueTrans",
                                      exchange: "EnteringQueueTransExchange",
                                      routingKey: string.Empty);

                    // 開啟Confirm模式
                    channel.ConfirmSelect();
                    try
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string msg = $"Confirm模式====消息入對確認===={i}";
                            byte[] bytes = Encoding.UTF8.GetBytes(msg);
                            channel.BasicPublish("EnteringQueueTransExchange", string.Empty, null, bytes);
                        }
                        Console.ForegroundColor = ConsoleColor.Green;

                        if (channel.WaitForConfirms())
                            Console.WriteLine($"消息已全部發送");

                        // 失敗拋出異常
                        channel.WaitForConfirmsOrDie();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("消息發送失敗");
                    }
                }
            }
        }

        public static void ShowTx()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;
            factory.Port = 5672;

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "EnteringQueueTrans", durable: true, exclusive: false, autoDelete: false,
                        arguments: null);
                    channel.ExchangeDeclare(type: ExchangeType.Fanout, exchange: "EnteringQueueTransExchange", durable: true, autoDelete: false,
                        arguments: null);
                    channel.QueueBind(queue: "EnteringQueueTrans",
                                      exchange: "EnteringQueueTransExchange",
                                      routingKey: string.Empty);

                    // 將通道設置為事務模式
                    channel.TxSelect();
                    try
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string msg = $"Confirm模式====消息入對確認===={i}";
                            byte[] bytes = Encoding.UTF8.GetBytes(msg);
                            channel.BasicPublish("EnteringQueueTransExchange", string.Empty, null, bytes);
                        }
                        Console.ForegroundColor = ConsoleColor.Green;

                        // 事務提交 寫入消息
                        channel.TxCommit();
                    }
                    catch (Exception ex)
                    {
                        // 事務回滾
                        channel.TxRollback();
                    }
                }
            }
        }
    }
}
