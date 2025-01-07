using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;


public class MainWindowViewModel : ReactiveObject
{
    private int _wordLength = 5;
    public int WordLength
    {
        get => _wordLength;
        set
        {
            _wordLength = value;
            OnPropertyChanged(nameof(WordLength));
        }
    }

    public ObservableCollection<string> InputLetters { get; set; } = new ObservableCollection<string>();

    public MainWindowViewModel()
    {
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}