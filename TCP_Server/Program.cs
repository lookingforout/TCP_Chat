using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>(); // Създава списък, за да съхранява всички свързани клиенти
    private const int Port = 8888; //Определя номера на порта, който сървърът ще използва

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port); // Стартира сървър, който слуша за връзки на дадения порт
        server.Start(); //Стартира сървъра
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient(); //Приема клиент, който иска да се свърже със сървъра
            clients.Add(client); //Добавя клиента към списъка на свързаните клиенти
            Thread clientThread = new Thread(HandleClient); //Създава нова нишка, за да обработва свързания клиент
            clientThread.Start(client); //Стартира нишката за обработка на клиента
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj; //Преобразува предадения обект в TcpClient
        NetworkStream stream = tcpClient.GetStream(); //Взема мрежовия поток за изпращане/получаване на данни от клиента

        byte[] buffer = new byte[1024]; //Създава буфер, за да съхранява получените данни от клиента
        int bytesRead; //Дефинира променлива за съхранение на броя прочетени байтове от клиента

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //Чете данни от клиента и ги съхранява в буфера
                if (bytesRead == 0) //Проверява дали няма получени данни, което означава, че клиентът е прекъснал връзката
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //Преобразува получените данни в низ чрез ASCII кодиране
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }

        clients.Remove(tcpClient); //Премахва клиента от списъка, след като той прекъсне връзката
        tcpClient.Close(); //Затваря връзката с клиента
    }

    static void BroadcastMessage(TcpClient sender, string message) //Изпраща съобщение до всички клиенти, освен този, който го е изпратил
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message); //Преобразува съобщението в байтове, за да може да бъде изпратено

        foreach (TcpClient client in clients) //Обхожда всеки свързан клиент
        {
            if (client != sender) //Проверява дали текущият клиент не е този, който е изпратил съобщението
            {
                NetworkStream stream = client.GetStream(); //Взема мрежовия поток за текущия клиент, за да изпрати данни
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length); //Изпраща съобщението до текущия клиент
            }
        }
    }
}
