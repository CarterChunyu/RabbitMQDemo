using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URl_Config;

namespace RabbitMQ_Producer
{
    public class DeadLeterQueue
    {
        public static void SendMessage()
        {
            string dlxexChange = "dlx.exchange";
            string dlxQueueName = "dlx.queue";

            string exchange = "direct-exchange";
            string queueName = "queue_a";
            string routingKey = "routingKey";

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
                    // 創建死信對列和交換機
                    channel.ExchangeDeclare(exchange: dlxexChange,type: ExchangeType.Direct, durable: true,
                        autoDelete: true);
                    channel.QueueDeclare(queue: dlxQueueName, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(queue: dlxQueueName, exchange: dlxexChange, routingKey: dlxQueueName);

                    // 創建消息隊列和交換機
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true,
                        autoDelete: true);
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false
                        ,arguments: new Dictionary<string, object>
                        {
                            {"x-dead-letter-exchange", dlxexChange },
                            {"x-dead-letter-routing-key", dlxQueueName }
                        });



                    channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

                    string message = "hello rabbitmq message";
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;
                    props.Expiration = "10000000";

                    channel.BasicPublish(exchange:exchange, routingKey: routingKey, basicProperties: props,
                        body: Encoding.UTF8.GetBytes(message));

                    Console.WriteLine($"向隊列 {queueName} 發送消息: {message}");
                }
            }
        }
    }
}