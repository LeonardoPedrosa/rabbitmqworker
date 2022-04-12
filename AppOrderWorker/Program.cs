// See https://aka.ms/new-console-template for more information
using AppOrderWorker.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
  channel.QueueDeclare(queue: "orderQueue",
                       durable: false,
                       exclusive: false,
                       autoDelete: false,
                       arguments: null);

  var consumer = new EventingBasicConsumer(channel);
  consumer.Received += (model, ea) =>
  {
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var order = JsonSerializer.Deserialize<Order>(message);


    Console.WriteLine($"Order: {order.OrderNumber} | {order.ItemName} | {order.Price:N2}");
  };
  channel.BasicConsume(queue: "orderQueue",
                       autoAck: true,
                       consumer: consumer);
  Console.WriteLine(" Press [enter] to exit.");
  Console.ReadLine();
}
