using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Avalonia.LogicalTree;
using DynamicData;

namespace WordleGame.Views;

public partial class MainWindow : Window
{
    private static readonly string ApiUrl = "https://dictionary.yandex.net/api/v1/dicservice.json/lookup";
    private static readonly string ApiKey = "dict.1.1.20250106T071243Z.0bb6b26606d67086.a9c7eb40332fb998ba30314c8834f9fca31180f1";
    public class LetterResult
    {
        public string Letter { get; set; } = string.Empty;
        public IBrush Background { get; set; } = Brushes.Gray;
    }

    public class GuessResult
    {
        public ObservableCollection<LetterResult> Letters { get; set; } = new ObservableCollection<LetterResult>();
    }

    private MainWindowViewModel _viewModel;
    private TextBox _textBox;
    private string targetWord = string.Empty;
    public ObservableCollection<GuessResult> Guesses { get; set; } = new ObservableCollection<GuessResult>();

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = this;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        LoadRandomWord();
        UpdateHintText();

        WordInput.KeyDown += WordInputKeyDown;
    }

    private void UpdateHintText()
    {
        HintText.Text = $"Попытка: {Guesses.Count + 1}/5";
    }

    private async void LoadRandomWord()
    {
        targetWord = await GetRandomWord();
        if (string.IsNullOrEmpty(targetWord))
        {
            HintText.Text = "Не удалось загрузить слово.";
        }
    }

    private void WordInputKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SubmitGuess();
        }
    }

    private void WordInputSubmitClick(object sender, RoutedEventArgs e)
    {
        SubmitGuess();
    }

    private async void SubmitGuess()
    {
        string guess = WordInput.Text.ToUpper();

        if (guess.Length != _viewModel.WordLength)
        {
            HintText.Text = $"Слово должно состоять из {_viewModel.WordLength} букв!";
            return;
        }

        if (!Regex.IsMatch(guess, "^[А-ЯЁ]+$"))
        {
            HintText.Text = "Используйте только русские буквы!";
            return;
        }

        bool res = await CheckWordExists(guess);

        if (!res)
        {
            HintText.Text = "Такого слова нет!";
            return;
        }

        var result = new GuessResult();
        result.Letters = new ObservableCollection<LetterResult>();

        for (int i = 0; i < _viewModel.WordLength; i++)
        {
            result.Letters.Add(new LetterResult
            {
                Letter = guess[i].ToString(),
                Background = GetColor(guess[i], i)
            });
        }

        Guesses.Add(result);

        if (guess == targetWord)
        {
            string str = "Вы выиграли!";
            var finalWindow = new FinalWindow(str);
            finalWindow.Show();
            this.Close();
        }
        else if (Guesses.Count >= 5)
        {
            string str = $"Игра окончена! Загаданное слово: {targetWord}.";
            var finalWindow = new FinalWindow(str);
            finalWindow.Show();
            this.Close();
        }
        else
        {
            UpdateHintText();
        }

        WordInput.Text = string.Empty;
        WordInput.Focus();
    }

    private IBrush GetColor(char letter, int index)
    {
        if (targetWord[index] == letter)
        {
            return Brushes.Green;
        }

        if (targetWord.Contains(letter))
        {
            int targetCount = targetWord.Count(c => c == letter);

            int correctCount = 0;
            foreach (var guess in Guesses)
            {
                correctCount += guess.Letters.Count(l => l.Letter == letter.ToString() && l.Background == Brushes.Green);
            }

            if (correctCount < targetCount)
            {
                return Brushes.Yellow;
            }
        }
        return Brushes.Gray;
    }

    private void ResetGame(object sender, RoutedEventArgs e)
    {
        Guesses.Clear();
        _viewModel.InputLetters.Clear();
        for (int i = 0; i < _viewModel.WordLength; i++)
        {
            _viewModel.InputLetters.Add("");
        }
        LoadRandomWord();
        UpdateHintText();
    }

    private async Task<string> GetRandomWord()
    {
        try
        {
            string filePath = null;
            switch (_viewModel.WordLength)
            {
                case 4:
                    filePath = "..\\..\\..\\..\\WordleGame\\words4.txt";
                    break;

                case 5:
                    filePath = "..\\..\\..\\..\\WordleGame\\words5.txt";
                    break;

                case 6:
                    filePath = "..\\..\\..\\..\\WordleGame\\words6.txt";
                    break;

                default:
                    break;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл wordsN.txt не найден.");
                switch (_viewModel.WordLength)
                {
                    case 4:
                        return "ПУСК";

                    case 5:
                        return "СТАРТ";

                    case 6:
                        return "НАЧАЛО";

                    default:
                        return "СТАРТ";
                }
            }

            var lines = await File.ReadAllLinesAsync(filePath);

            var words = new List<string>();
            foreach (var line in lines)
            {
                words.Add(line.ToUpper());
            }

            if (words.Count == 0)
            {
                switch (_viewModel.WordLength)
                {
                    case 4:
                        return "ПУСК";

                    case 5:
                        return "СТАРТ";

                    case 6:
                        return "НАЧАЛО";

                    default:
                        return "СТАРТ";
                }
            }

            var random = new Random();
            return words[random.Next(words.Count)];
        }
        catch (Exception ex)
        {
            switch (_viewModel.WordLength)
            {
                case 4:
                    return "ПУСК";

                case 5:
                    return "СТАРТ";

                case 6:
                    return "НАЧАЛО";

                default:
                    return "СТАРТ";
            }
        }
    }

    private static async Task<bool> CheckWordExists(string word)
    {
        using (HttpClient client = new HttpClient())
        {
            string requestUrl = $"{ApiUrl}?key={ApiKey}&lang=ru-en&text={Uri.EscapeDataString(word)}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse.Contains("\"def\":") && !jsonResponse.Contains("\"def\":[]");
            }
            else
            {
                throw new Exception($"Ошибка API: {response.StatusCode}");
            }
        }
    }
}
