using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888;
    private const string ServerIp = "127.0.0.1";

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); //инициализира клас TcpClient свързва се към сървър и неговия порт, може да праща данни и да получава
        Console.WriteLine("Connected to server. Start chatting!");

        NetworkStream stream = client.GetStream(); //връща NetworkStream, който може да се използва за изпращане и получаване на данни

        Thread receiveThread = new Thread(ReceiveMessages); // Прави нова нишка
        receiveThread.Start(stream); //Кара нишката да започне изпълнение

        while (true)
        {
            string message = Console.ReadLine(); //поставя на пауза изпълнението на програмата, преди да изчисти конзолата и да покаже нова информация в нея
            byte[] buffer = Encoding.ASCII.GetBytes(message); //определя колко байта водят до кодиране на набор от Unicode знаци
            stream.Write(buffer, 0, buffer.Length); //записва съдържанието на буфера за поточно предаване на outStream, започвайки от буфер [0] със следващите байтове с дължина
        }
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj; //Прави нов сокет 
        byte[] buffer = new byte[1024]; //четене на файла в буфер от 1024 байта
        int bytesRead; //променлива за цикъла долу 
        

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //Общият брой байтове, прочетени в буфера.
                if (bytesRead == 0) //Връща 0
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //определя колко байта водят до кодиране на набор от Unicode знаци
                Console.WriteLine(message);
            }
            catch (Exception)
            {
                break;
            }
        }
    }
}
