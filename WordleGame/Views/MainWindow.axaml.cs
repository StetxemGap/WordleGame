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

namespace WordleGame.Views;

public partial class MainWindow : Window
{
    private static readonly HttpClient HttpClient = new HttpClient();
    private string targetWord = string.Empty;
    public ObservableCollection<GuessResult> Guesses { get; set; } = new ObservableCollection<GuessResult>();

    public MainWindow()
    {
        InitializeComponent();
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

    private async void SubmitGuess(object sender, RoutedEventArgs e)
    {
        var textBoxes = InputGrid.Children.OfType<TextBox>().ToArray();
        string guess = string.Join("", textBoxes.Select(tb => tb.Text.ToUpper()));

        if (guess.Length != 5)
        {
            HintText.Text = "Слово должно состоять из 5 букв!";
            return;
        }

        if (!Regex.IsMatch(guess, "^[А-ЯЁ]+$"))
        {
            HintText.Text = "Используйте только русские буквы!";
            return;
        }

        if (!await WordExists(guess))
        {
            HintText.Text = "Такого слова нет!";
            return;
        }

        var result = new GuessResult();
        for (int i = 0; i < 5; i++)
        {
            result.SetLetter(i, guess[i].ToString(), GetColor(guess[i], i));
        }

        Guesses.Add(result);

        foreach (var textBox in textBoxes)
        {
            textBox.Text = string.Empty;
        }

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
            for (int i = 0; i < 5; i++)
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
        Guesses.Clear();
        LoadRandomWord();
        foreach (var textBox in InputGrid.Children.OfType<TextBox>())
        {
            textBox.Text = string.Empty;
        }
        UpdateHintText();
    }

    private async Task<string> GetRandomWord()
    {
        try
        {
            string filePath = "C:\\Users\\sisis\\source\\repos\\WordleGame\\WordleGame\\Views\\words5.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл words5.txt не найден.");
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

    private async Task<bool> WordExists(string word)
    {
        try
        {
            var response = await HttpClient.GetStringAsync($"https://speller.yandex.net/services/spellservice.json/checkText?text={word}");
            var spellResults = JsonSerializer.Deserialize<List<SpellResult>>(response);

            return spellResults.Count == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при проверке слова: {ex.Message}");
            return false;
        }
    }

    private class SpellResult
    {
        public string Word { get; set; }
        public List<string> Suggestions { get; set; }
    }
}
