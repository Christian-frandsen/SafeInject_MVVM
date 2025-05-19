using Microsoft.Maui.Controls;

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent(); // ← Denne initialiserer alt fra XAML-filen

            // Opretter og tildeler ViewModel som DataContext (BindingContext)
            BindingContext = new ViewModel();
        }
    }

