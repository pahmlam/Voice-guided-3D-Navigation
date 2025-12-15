using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SpeechClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    byte[] buffer = new byte[1024];

    // Tham chiếu đến bộ xử lý lệnh
    public VoiceCommandProcessor commandProcessor;

    // Tham chiếu UI để in text ra màn hình
    public ConsoleUI consoleUI;

    void Start()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();
            Debug.Log("Connected to Python");

        
            consoleUI?.AddMessage("Connected to Python");
        }
        catch (Exception e)
        {
            Debug.LogError("Không kết nối được Python: " + e.Message);

    
            consoleUI?.AddMessage("Không kết nối được Python: " + e.Message);
        }
    }

    void Update()
    {
        if (client == null || stream == null || !client.Connected)
            return;

        if (stream.DataAvailable)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();

            Debug.Log("Python>> " + message);

            // In text nhận được lên UI
            if (!string.IsNullOrEmpty(message))
                consoleUI?.AddMessage(message);

            // Gửi text sang bộ xử lý lệnh nếu không rỗng
            if (!string.IsNullOrEmpty(message) && commandProcessor != null)
            {
                commandProcessor.Process(message);
            }
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}
