using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Net.Http;
using WordleGame.Views;

namespace WordleGame;

public partial class WelcomeWindow : Window
{
    private static readonly HttpClient HttpClient = new HttpClient();
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void ToMainWindow(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

}