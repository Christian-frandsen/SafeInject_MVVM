using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ViewModel : INotifyPropertyChanged
{
    // Privat felt til at gemme billeddata, som UI skal vise
    private ImageSource _image;

    // Offentlig egenskab, som UI’en binder til via XAML
    // Når denne ændres, skal UI automatisk opdateres
    public ImageSource Image
    {
        get => _image; // Når UI spørger om billedet, returneres det her
        set
        {
            _image = value; // Sæt den nye værdi
            OnPropertyChanged(); // Fortæl UI, at billedet er ændret, så det kan opdatere
        }
    }

    // Konstruktøren kaldes, når ViewModel’en oprettes i MainPage.xaml.cs
    public ViewModel()
    {
        // Opretter en ny instans af TcpReceiver, som lytter efter billeder
        var receiver = new TcpReceiver();

        // "Tilmelder" ViewModel’en til at reagere, når TcpReceiver modtager et billede
        receiver.ImageReceived += OnImageReceived;

        // Starter TcpReceiver asynkront – lytter på port 5000
        // _ bruges her, fordi vi ikke har brug for at vente på opgaven
        _ = receiver.StartListeningAsync(5000);
    }

    // Denne metode bliver kaldt, når et billede modtages fra netværket
    private void OnImageReceived(byte[] imageData)
    {
        // UI må kun opdateres fra hovedtråden – ellers får vi fejl
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Konverterer byte-array til et ImageSource-objekt, som MAUI kan vise
            // MemoryStream simulerer en fil, så ImageSource kan læse det som billede
            Image = ImageSource.FromStream(() => new MemoryStream(imageData));
        });
    }

    // Dette interface gør det muligt at fortælle UI, når en egenskab ændres
    public event PropertyChangedEventHandler PropertyChanged;

    // Kaldes automatisk, når en egenskab ændrer sig (f.eks. "Image")
    // CallerMemberName betyder, at vi ikke behøver skrive navnet manuelt – det sker automatisk
    protected void OnPropertyChanged([CallerMemberName] string prop = null)
 => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
