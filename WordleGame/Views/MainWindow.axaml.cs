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

namespace WordleGame.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    private string targetWord = string.Empty;
    public ObservableCollection<GuessResult> Guesses { get; set; } = new ObservableCollection<GuessResult>();

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = this;

        LoadRandomWord();
        UpdateHintText();
    }

    private void UpdateHintText()
    {
        HintText.Text = $"Угадайте слово! Попытка: {Guesses.Count + 1}/6";
    }

    private async void LoadRandomWord()
    {
        targetWord = await GetRandomWord();
        Console.WriteLine(targetWord);
        if (string.IsNullOrEmpty(targetWord))
        {
            HintText.Text = "Не удалось загрузить слово.";
        }
    }

    //private async void SubmitGuess(object sender, KeyEventArgs e)
    //{
    //    if (e.Key != Key.Enter)
    //    {
    //        return;
    //    }

    //    string guess = WordInput.Text.ToUpper();

    //    if (guess.Length != _viewModel.WordLength)
    //    {
    //        HintText.Text = $"Слово должно состоять из {_viewModel.WordLength} букв!";
    //        return;
    //    }

    //    if (!Regex.IsMatch(guess, "^[А-ЯЁ]+$"))
    //    {
    //        HintText.Text = "Используйте только русские буквы!";
    //        return;
    //    }

    //    var dataBaseManager = new DataBaseManager();
    //    bool res = dataBaseManager.CheckWord(guess, guess.Length);

    //    if (!res)
    //    {
    //        HintText.Text = "Такого слова нет!";
    //        return;
    //    }

    //    var result = new GuessResult();
    //    for (int i = 0; i < _viewModel.WordLength; i++)
    //    {
    //        result.SetLetter(i, guess[i].ToString(), GetColor(guess[i], i));
    //    }

    //    Guesses.Add(result);

    //    for (int i = 0; i < _viewModel.InputLetters.Count; i++)
    //    {
    //        _viewModel.InputLetters[i] = "";
    //    }

    //    if (guess == targetWord)
    //    {
    //        HintText.Text = "Вы выиграли!";
    //    }
    //    else if (Guesses.Count >= 6)
    //    {
    //        HintText.Text = $"Игра окончена! Загаданное слово: {targetWord}.";
    //    }
    //    else
    //    {
    //        UpdateHintText();
    //    }

    //    WordInput.Focus();
    //}

    private async void SubmitGuess(object sender, RoutedEventArgs e)
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

        var dataBaseManager = new DataBaseManager();
        bool res = dataBaseManager.CheckWord(guess, guess.Length);

        if (!res)
        {
            HintText.Text = "Такого слова нет!";
            return;
        }

        var result = new GuessResult();
        for (int i = 0; i < _viewModel.WordLength; i++)
        {
            result.SetLetter(i, guess[i].ToString(), GetColor(guess[i], i));
        }

        Guesses.Add(result);

        if (guess == targetWord)
        {
            HintText.Text = "Вы выиграли!";
        }
        else if (Guesses.Count >= 6)
        {
            HintText.Text = $"Игра окончена! Загаданное слово: {targetWord}.";
        }
        else
        {
            UpdateHintText();
        }

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
            for (int i = 0; i < _viewModel.WordLength; i++)
            {
                if (targetWord[i] == letter && Guesses.Any(g => g.Letters[i].Letter == letter.ToString() && g.Letters[i].Background == Brushes.Green))
                {
                    correctCount++;
                }
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
        _viewModel.InputLetters.Clear();
        for (int i = 0; i < _viewModel.WordLength; i++)
        {
            _viewModel.InputLetters.Add("");
        }
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
                    filePath = "C:\\Users\\sisis\\source\\repos\\WordleGame\\WordleGame\\Views\\words4.txt";
                    break;

                case 5:
                    filePath = "C:\\Users\\sisis\\source\\repos\\WordleGame\\WordleGame\\Views\\words5.txt";
                    break;

                case 6:
                    filePath = "C:\\Users\\sisis\\source\\repos\\WordleGame\\WordleGame\\Views\\words6.txt";
                    break;

                default:
                    break;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл wordsN.txt не найден.");
                return "СЛОВО";
            }

            var lines = await File.ReadAllLinesAsync(filePath);

            var words = new List<string>();
            foreach (var line in lines)
            {
                words.Add(line.ToUpper());
            }

            if (words.Count == 0)
            {
                Console.WriteLine("В файле нет подходящих пятибуквенных слов.");
                return "СЛОВО";
            }

            var random = new Random();
            return words[random.Next(words.Count)];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке слова: {ex.Message}");
            return "СЛОВО";
        }
    }

    //private void OnTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var currentTextBox = sender as TextBox;
    //    int currentIndex = InputBoxes.GetLogicalChildren().OfType<TextBox>().ToList().IndexOf(currentTextBox);

    //    if (currentIndex >= 0 && currentIndex < _viewModel.InputLetters.Count)
    //    {
    //        _viewModel.InputLetters[currentIndex] = currentTextBox.Text;
    //    }
    //}

    //private void OnTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var currentTextBox = sender as TextBox;

    //    if (currentTextBox.Text.Length == currentTextBox.MaxLength)
    //    {
    //        MoveFocusToNextTextBox(currentTextBox);
    //    }
    //}

    //private void OnKeyRight(object sender, KeyEventArgs e)
    //{
    //    var currentTextBox = sender as TextBox;

    //    if (e.Key == Key.Right)
    //    {
    //        MoveFocusToNextTextBox(currentTextBox);
    //    }
    //}

    //private void MoveFocusToNextTextBox(TextBox currentTextBox)
    //{
    //    var textBoxes = InputBoxes.GetLogicalChildren().OfType<TextBox>().ToList();
    //    int currentIndex = textBoxes.IndexOf(currentTextBox);

    //    if (currentIndex < textBoxes.Count - 1)
    //    {
    //        var nextTextBox = textBoxes[currentIndex + 1];
    //        nextTextBox.Focus();
    //    }
    //}

    //private void OnKeyLeft(object sender, KeyEventArgs e)
    //{
    //    var currentTextBox = sender as TextBox;

    //    if (e.Key == Key.Left)
    //    {
    //        MoveFocusToPreviousTextBox(currentTextBox);
    //    }
    //}

    //private void MoveFocusToPreviousTextBox(TextBox currentTextBox)
    //{
    //    var textBoxes = InputBoxes.GetLogicalChildren().OfType<TextBox>().ToList();
    //    int currentIndex = textBoxes.IndexOf(currentTextBox);

    //    if (currentIndex > 0)
    //    {
    //        var previousTextBox = textBoxes[currentIndex - 1];
    //        previousTextBox.Focus();
    //    }
    //}
}
