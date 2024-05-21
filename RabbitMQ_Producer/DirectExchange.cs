using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class DirectExchange
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = UrlConfig.rabbitMQ_Url;
            factory.Port = 5672;
            factory.UserName = UrlConfig.user;
            factory.Password = UrlConfig.password;

            using(IConnection connection = factory.CreateConnection())
            {
                using(IModel channel = connection.CreateModel())
                {
                    // 聲明一個對列接收所有日誌類型消息
                    channel.QueueDeclare(queue:"DirectExchangeLogAllQueue", durable:true, exclusive: false,
                        autoDelete:false, arguments: null);
                    // 聲明一個對列接收Error日誌類型消息
                    channel.QueueDeclare(queue: "DirectExchangeErrorQueue", durable:true, exclusive: false,
                        autoDelete:false, arguments: null);
                    // 聲明一個 Direct交換機
                    channel.ExchangeDeclare(exchange: "DirectExChange", type: ExchangeType.Direct, durable: true,
                        autoDelete: false, arguments: null);

                    // 四種日誌類型 
                    string[] logtypes = new string[] { "debug", "info", "warn", "error" };

                    foreach (var logtype in logtypes)
                    {
                        // 把交換機和對列綁定, 把日誌類型作為routingkey
                        channel.QueueBind(queue: "DirectExchangeLogAllQueue", exchange: "DirectExChange",
                            routingKey: logtype);
                    }

                    channel.QueueBind(queue: "DirectExchangeErrorQueue", exchange: "DirectExChange",
                        routingKey: "error");

                    List<LogMsgModel> list = new List<LogMsgModel>();
                    for (int i = 1; i <=100 ; i++)
                    {
                        if(i % 4 == 0)
                        {
                            list.Add(new LogMsgModel
                            {
                                LogType = "info",
                                Msg = Encoding.UTF8.GetBytes($"第{i}條消息,類型info")
                            });
                        }
                        if (i % 4 == 1)
                        {
                            list.Add(new LogMsgModel
                            {
                                LogType = "debug",
                                Msg = Encoding.UTF8.GetBytes($"第{i}條消息,類型debug")
                            });
                        }
                        if (i % 4 == 2)
                        {
                            list.Add(new LogMsgModel
                            {
                                LogType = "warn",
                                Msg = Encoding.UTF8.GetBytes($"第{i}條消息,類型warn")
                            });
                        }
                        if (i % 4 == 3)
                        {
                            list.Add(new LogMsgModel
                            {
                                LogType = "error",
                                Msg = Encoding.UTF8.GetBytes($"第{i}條消息,類型error")
                            });
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("生產者發送100條消息");

                    foreach(LogMsgModel logMsg in list)
                    {
                        channel.BasicPublish(exchange: "DirectExChange", routingKey: logMsg.LogType,
                            basicProperties: null, body: logMsg.Msg);
                        Console.WriteLine($"{Encoding.UTF8.GetString(logMsg.Msg)} 已發送");
                    }
                }
            }


        }
    }

    public class LogMsgModel
    {
        public string LogType { get; set; }

        public byte[] Msg { get; set; }
    }
}
