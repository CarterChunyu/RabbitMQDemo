using RabbitMQ_Producer;

try
{
    //BasicUse.Show();
    //DirectExchange.Show();
    //FanoutExchange.Show();
    //TopicExchange.Show();
    //HeaderExchange.Show();
    //EnteringQueueTrans.ShowConfirm();
    //EnteringQueueTrans.ShowTx();
    //PersistenceQueue.Show();
    //PriopertyQueue.Show();
    DeadLeterQueue.SendMessage();
}
catch(Exception ex)
{

}