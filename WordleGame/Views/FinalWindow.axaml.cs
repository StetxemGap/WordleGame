using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Buffers;
using WordleGame.Views;

namespace WordleGame;

public partial class FinalWindow : Window
{
    public FinalWindow()
    {
        InitializeComponent();
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }
    public FinalWindow(string str)
    {
        InitializeComponent();
        Congratulation.Text = str;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void OnRestart(object sender, RoutedEventArgs e)
    {
        var welcomeWindow = new WelcomeWindow();
        welcomeWindow.Show();
        this.Close();
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}