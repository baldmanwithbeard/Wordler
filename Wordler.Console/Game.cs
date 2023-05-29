using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Wordler.Library;

namespace Wordler.ConsoleApp
{
    internal class Game
    {
        private const int MaxAttempts = 6;
        private int attempts = 0;
        private List<char> unguessedLetters;
        private List<char> incorrectLetters;
        private Dictionary<char, List<int>> misplacedLetters;
        private char[] answer;
        private List<string> guesses = new() { "arose" };
        private Dictionary<string, double> heterogramScores;
        private bool gameOver = false;
        private Dictionary<string, double> wordScores;
        private HashSet<string> possibleAnswers;

        public Game()
        {
            unguessedLetters = new List<char>("abcdefghijklmnopqrstuvwxyz");
            incorrectLetters = new List<char>();
            misplacedLetters = new Dictionary<char, List<int>>();
            answer = new char[5];
            for (var i = 0; i < 5; i++)
            {
                answer[i] = '_';
            }

            heterogramScores = LoadHeterogramScores();
            wordScores = LoadWordScores();
            possibleAnswers = new HashSet<string>(wordScores.Keys);
        }

        private Dictionary<string, double> LoadHeterogramScores()
        {
            var assembly = Assembly.GetAssembly(typeof(Command));
            var resourceName = $"Wordler.Library.WordleData.HeterogramScores.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) throw new ArgumentException("The specified resource was not found", nameof(resourceName));
            using var reader = new StreamReader(stream);
            var jsonContent = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Dictionary<string, double>>(jsonContent);
        }

        private Dictionary<string, double> LoadWordScores()
        {
            var assembly = Assembly.GetAssembly(typeof(Command));
            var resourceName = $"Wordler.Library.WordleData.WordScores.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) throw new ArgumentException("The specified resource was not found", nameof(resourceName));
            using var reader = new StreamReader(stream);
            var jsonContent = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Dictionary<string, double>>(jsonContent);
        }

        public void Run()
        {
            while (!gameOver) { Guess(); }
        }

        private void Guess()
        {
            var guess = GuessCalculation();
            guesses.Add(guess);
            possibleAnswers.Remove(guess);
            Console.WriteLine($"Guess #{attempts + 1}: {guess}");

            var feedback = GetFeedback();

            // Update the game state based on the feedback
            for (var i = 0; i < 5; i++)
            {
                switch (feedback[i])
                {
                    case '!':
                        answer[i] = guess[i];
                        possibleAnswers.RemoveWhere(word => word[i] != guess[i]);
                        break;

                    case '-':
                        if (!misplacedLetters.ContainsKey(guess[i]))
                        {
                            misplacedLetters[guess[i]] = new List<int> { i };
                        }
                        else
                        {
                            misplacedLetters[guess[i]].Add(i);
                        }

                        possibleAnswers.RemoveWhere(word => !word.Contains(guess[i]));
                        possibleAnswers.RemoveWhere(word => word[i] == guess[i]);
                        break;

                    default: // for '_'
                        incorrectLetters.Add(guess[i]);
                        possibleAnswers.RemoveWhere(word => word.Contains(guess[i]));
                        break;
                }

                unguessedLetters.Remove(guess[i]);
            }

            attempts++;

            if (IsWin())
            {
                Console.WriteLine("haha! i beat you");
                gameOver = true;
            }

            if (attempts < MaxAttempts) return;

            Console.WriteLine("ah... nicely done...");
            gameOver = true;
        }

        private static string GetFeedback()
        {
            var feedback = "";
            do
            {
                Console.WriteLine("Enter your feedback for the guess");
                feedback = Console.ReadLine() ?? "";
            }
            while (!InputIsValid(feedback));

            return feedback;
        }

        private static bool InputIsValid(string input)
        {
            var isValidInput = Regex.IsMatch(input, @"^[!-_]{5}$");
            if (!isValidInput)
            {
                Console.WriteLine("Input is incorrect! Please enter a string of 5 characters, where each character must be \"_\", \"-\", or \"!\"");
            }
            return isValidInput;
        }

        private bool IsWin()
        {
            // Check if the game has been won
            return !answer.Contains('_');
        }

        private string? GetGuessFromPossibleAnswers()
        {
            var wordQuery = possibleAnswers.OrderByDescending(word => wordScores[word]);
            Console.WriteLine($"Total words in query: {wordQuery.Count()}"); // Debugging output
            return wordQuery.FirstOrDefault();
        }

        private string? GetGuessFromWordScores()
        {
            var totalCorrectMisplaced = answer.Count(c => c != '_') + misplacedLetters.Count;
            if (totalCorrectMisplaced >= 4 || attempts > 3)
            {
                // Get the set of all letters that are in any of the possible answers
                var possibleAnswerLetters = new HashSet<char>(possibleAnswers.SelectMany(word => word));

                // Get a guess from word scores that covers as many of the remaining unguessed letters in the possible answers as possible
                var guessFromWordScores = wordScores
                    .Where(word => word.Key.All(letter => unguessedLetters.Contains(letter) && possibleAnswerLetters.Contains(letter)
                            && (!misplacedLetters.ContainsKey(letter) || (misplacedLetters.ContainsKey(letter) && !misplacedLetters[letter].Contains(word.Key.IndexOf(letter))))))
                        .Where(word => !guesses.Contains(word.Key))  // Exclude already guessed words
                        .OrderByDescending(word => word.Value);

                Console.WriteLine($"Total words in query: {guessFromWordScores.Count()}"); // Debugging output

                return guessFromWordScores.FirstOrDefault().Key;
            }

            return null;
        }

        /// <summary>
        /// hub for algo
        /// </summary>
        /// <returns></returns>
        private string GuessCalculation()
        {
            if (attempts == 0) return "arose";

            string guess = null;
            var totalCorrectMisplaced = answer.Count(c => c != '_') + misplacedLetters.Count;

            if (attempts < 2 && totalCorrectMisplaced < 4) guess = GetHeterogramGuess();
            if (guess != null) return guess;

            guess = GetGuessFromWordScores();
            if (guess != null) return guess;

            return guess ?? GetGuessFromPossibleAnswers();
        }

        private string? GetHeterogramGuess()
        {
            return heterogramScores
                .Where(word => word.Key.All(letter => unguessedLetters.Contains(letter)))
                .OrderByDescending(word => word.Value)
                .FirstOrDefault().Key;
        }
    }
}