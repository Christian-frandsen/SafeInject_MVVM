using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ViewModel : INotifyPropertyChanged
{
    private ImageSource _image;
    public ImageSource Image
    {
        get => _image;
        set
        {
            _image = value;
            OnPropertyChanged();
        }
    }

    public ViewModel()
    {
        var receiver = new TcpReceiver();
        receiver.ImageReceived += OnImageReceived;
        receiver.StartListening(5000); // ← Her bruges nu Thread-versionen
    }

    private void OnImageReceived(byte[] imageData)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Image = ImageSource.FromStream(() => new MemoryStream(imageData));
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
