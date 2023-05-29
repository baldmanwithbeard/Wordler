using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        private Dictionary<string, double> heterogramScores;

        public Game()
        {
            unguessedLetters = new List<char>("abcdefghijklmnopqrstuvwxyz");
            incorrectLetters = new List<char>();
            misplacedLetters = new Dictionary<char, List<int>>();
            answer = new char[5];
            for (int i = 0; i < 5; i++)
            {
                answer[i] = '_';
            }

            // Load heterogram_scores.json
            heterogramScores = JsonConvert.DeserializeObject<Dictionary<string, double>>(
                File.ReadAllText("heterogram_scores.json"));
        }

        public void Guess(string guess)
        {
            if (attempts >= MaxAttempts)
            {
                Console.WriteLine("Maximum attempts reached!");
                return;
            }

            var feedback = GetFeedback();

            // Update the game state based on the guess
            for (var i = 0; i < 5; i++)
            {
                switch (feedback[i])
                {
                    case '!':
                        answer[i] = guess[i];
                        break;

                    case '-' when !misplacedLetters.ContainsKey(guess[i]):
                        misplacedLetters[guess[i]] = new List<int> { i };
                        break;

                    case '-':
                        misplacedLetters[guess[i]].Add(i);
                        break;

                    default:
                        incorrectLetters.Add(guess[i]);
                        break;
                }

                unguessedLetters.Remove(guess[i]);
            }

            attempts++;

            if (IsWin())
            {
                Console.WriteLine("Congratulations, you've won!");
            }
            else if (attempts >= MaxAttempts)
            {
                Console.WriteLine("Game over, better luck next time!");
            }
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

        public string GetHighestScoreGuess()
        {
            var bestGuess = heterogramScores
                .Where(word => word.Key.All(letter => unguessedLetters.Contains(letter)
                    && !misplacedLetters.ContainsKey(letter)
                    && !misplacedLetters[letter].Contains(word.Key.IndexOf(letter))))
                .OrderByDescending(word => word.Value)
                .FirstOrDefault().Key;

            return bestGuess ?? "arose";
        }
    }
}