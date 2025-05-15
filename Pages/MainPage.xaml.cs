public partial class MainPage : ContentPage
{
    // Denne metode kaldes automatisk
    public MainPage()
    {
        // Indlæser alt, hvad der er defineret i XAML-filen
        InitializeComponent();

        // Det gør, at XAML kan se ViewModel og binde til dens egenskaber
        BindingContext = new ViewModel();
    }
}
