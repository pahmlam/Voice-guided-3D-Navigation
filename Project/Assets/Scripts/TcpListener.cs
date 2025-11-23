using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine;

public class SpeechReceiver : MonoBehaviour
{
    private TcpClient client;
    private StreamReader reader;
    private Thread receiveThread;
    private string receivedText = "";

    void Start()
    {
        receiveThread = new Thread(new ThreadStart(ConnectToPython));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ConnectToPython()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 5000);
            reader = new StreamReader(client.GetStream());

            while (true)
            {
                string message = reader.ReadLine();
                if (message != null)
                {
                    Debug.Log("🗣 Giọng nói: " + message);
                    receivedText = message;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Lỗi kết nối: " + e.Message);
        }
    }


    void OnApplicationQuit()
    {
        if (client != null) client.Close();
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
    }
}
