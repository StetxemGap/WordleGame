using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Net.Http;
using WordleGame.Views;

namespace WordleGame;

public partial class WelcomeWindow : Window
{
    private MainWindowViewModel _viewModel;
    public WelcomeWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    private void ToMainWindow(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow(_viewModel);
        mainWindow.Show();
        this.Close();
    }

    private void Set4LetterMode(object sender, RoutedEventArgs e)
    {
        _viewModel.WordLength = 4;
    }

    private void Set5LetterMode(object sender, RoutedEventArgs e)
    {
        _viewModel.WordLength = 5;
    }

    private void Set6LetterMode(object sender, RoutedEventArgs e)
    {
        _viewModel.WordLength = 6;
    }
}