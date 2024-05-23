using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class PriopertyQueue
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
                    channel.ExchangeDeclare(exchange: "PriorityExchange", type: ExchangeType.Direct,
                        durable: true, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "PriorityQueue", durable: true, exclusive: false, autoDelete: false,
                        arguments: new Dictionary<string, object>
                        {
                            {"x-max-priority",10 }
                        });
                    channel.QueueBind(queue: "PriorityQueue", exchange: "PriorityExchange", routingKey: "Priority");
                    
                    string[] msgRole = { "vip1", "hello2", "word3", "common4", "vip5" };
                    IBasicProperties props = channel.CreateBasicProperties();

                    foreach (string role in msgRole)
                    {
                        if (role.StartsWith("vip"))
                        {
                            props.Priority = 9;
                            channel.BasicPublish(exchange: "PriorityExchange", routingKey: "Priority", basicProperties: props,
                                body: Encoding.UTF8.GetBytes($"顧客等級=>{role}"));
                        }
                        else
                        {
                            props.Priority = 1;
                            channel.BasicPublish(exchange: "PriorityExchange", routingKey: "Priority", basicProperties: props,
                                body: Encoding.UTF8.GetBytes($"顧客等級=>{role}"));
                        }
                    }
                }
            }
        }
    }
}
