using System.Net.Sockets;
using System.Net;
using System.Threading;

public class TcpReceiver
{
    public event Action<byte[]> ImageReceived;

    private Thread _listenerThread;

    public void StartListening(int port)
    {
        _listenerThread = new Thread(() =>
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                try
                {
                    using TcpClient client = listener.AcceptTcpClient();
                    using NetworkStream stream = client.GetStream();

                    byte[] lengthBuffer = new byte[4];
                    int read = stream.Read(lengthBuffer, 0, 4);
                    if (read < 4) continue;

                    int length = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] data = new byte[length];
                    int totalRead = 0;

                    while (totalRead < length)
                    {
                        int bytesRead = stream.Read(data, totalRead, length - totalRead);
                        if (bytesRead == 0) break;
                        totalRead += bytesRead;
                    }

                    if (totalRead == length)
                    {
                        ImageReceived?.Invoke(data);
                    }
                }
                catch (Exception ex)
                {
                    // Du kan logge fejl her, hvis ønsket
                }
            }
        });

        _listenerThread.IsBackground = true;
        _listenerThread.Start();
    }
}
