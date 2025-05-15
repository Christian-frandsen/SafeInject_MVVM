using System.Net.Sockets;
using System.Net;

public class TcpReceiver
{
    // En event, som Viewmodel kan abonnere på
    // Den udløses, når vi har modtaget et billede som byte-array
    public event Action<byte[]> ImageReceived;

    // Denne metode starter en asynkron TCP-lytter på den port, vi angiver
    public async Task StartListeningAsync(int port)
    {
        // Opretter en TCP-lytter, som accepterer forbindelser fra alle IP-adresser på den ønskede port
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        // Starter lytteren, så den begynder at vente på forbindelser
        listener.Start();

        // En uendelig løkke, så vi hele tiden accepterer nye klientforbindelser
        while (true)
        {
            // Afventer en klient, der prøver at oprette forbindelse (f.eks. fra en Raspberry Pi)
            var client = await listener.AcceptTcpClientAsync();

            // Får adgang til netværksstrømmen, så vi kan læse data fra klienten
            var stream = client.GetStream();

            // Vi forventer, at afsenderen først sender 4 bytes, som fortæller os, hvor stort billedet er
            byte[] lengthBuffer = new byte[4];

            // Læser de 4 længdebytes ind i buffer'en
            await stream.ReadAsync(lengthBuffer);

            // Konverterer de 4 bytes til en int (angiver hvor mange bytes billedet fylder)
            int length = BitConverter.ToInt32(lengthBuffer, 0);

            // Opretter en ny buffer, hvor selve billedet gemmes
            byte[] data = new byte[length];

            // Læser billedet ind i bufferen i små dele, indtil vi har fået det hele
            int totalRead = 0;
            while (totalRead < length)
            {
                // Læser så mange bytes som mangler
                int bytesRead = await stream.ReadAsync(data, totalRead, length - totalRead);

                // Hvis forbindelsen bliver lukket midt i det hele
                if (bytesRead == 0) break;

                // Holder styr på hvor mange bytes vi har modtaget i alt
                totalRead += bytesRead;
            }

            // Når hele billedet er læst, udløses ImageReceived-eventen
            // Alle der har abonneret på denne event (f.eks. ViewModel) får billedet
            ImageReceived?.Invoke(data);
        }
    }
}
