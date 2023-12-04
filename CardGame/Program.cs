using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlackJack
{
    class Program
    {
        static void Main()
        {
            try
            {
                // Read command-line arguments
                string inputFile = "input.txt";
                string outputFile = "output.txt";

                // Check if input and output file names are provided
                if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFile))
                {
                    Console.WriteLine("Exception: Input and output files are required.");
                    return;
                }

                // Read player data from input file
                List<Player> players = ReadInput(inputFile);

                // Check if input is valid
                if (players == null)
                {
                    Console.WriteLine("Exception: Invalid input format.");
                    return;
                }

                // Determine the winner(s)
                List<Player> winners = DetermineWinners(players);
                winners = winners.Where(i => i.Score == winners.OrderByDescending(j => j.Score).First().Score).ToList();


                // Write winners and their scores to the output file
                WriteOutput(outputFile, $"{string.Join(",", winners.Select(p => p.Name))}:{winners.First().Score}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }


        // Read player data from the input file
        static List<Player> ReadInput(string inputFile)
        {
            try
            {
                List<Player> players = new List<Player>();

                string[] lines = File.ReadAllLines(inputFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string playerName = parts[0].Trim();
                        string[] cards = parts[1].Split(',');

                        // Check if each player has exactly 5 cards
                        if (cards.Length != 5)
                        {
                            throw new Exception("Invalid number of cards for a player.");
                        }

                        List<Card> playerCards = new List<Card>();
                        foreach (string card in cards)
                        {
                            playerCards.Add(new Card(card.Trim()));
                        }

                        players.Add(new Player(playerName, playerCards));
                    }
                    else
                    {
                        throw new Exception("Invalid input format.");
                    }
                }

                return players;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Determine the winner(s)
        static List<Player> DetermineWinners(List<Player> players)
        {
            int SuitScore = 0;
            int highestScore = players.Max(p => p.CalculateTotalScore());
            List<Player> highestScorers = players.Where(p => p.CalculateTotalScore() == highestScore).ToList();
            var highestPlayerName = "";

            // Check for a tie and calculate suit score
            if (highestScorers.Count > 1)
            {
                //Gets the highest card suit value for the tied players
                foreach (var player in highestScorers)
                {
                    int highestSuitScore = player.GetHighestCardSuitValue();

                    if (highestSuitScore > SuitScore)
                    {
                        highestPlayerName = player.Name;
                        SuitScore = highestSuitScore;
                    }
                    //If tied players have the same highest card suit value, both players are returned
                    else if (highestSuitScore == SuitScore)
                    {
                        highestPlayerName = "";
                        SuitScore = 0;
                    }
                }

                if (highestPlayerName != "")
                {
                    highestScorers.Where(p => p.Name == highestPlayerName).First().AddSuitValueToScore(SuitScore);
                }

            }
            return highestScorers;
        }

        // Write output to the output file
        static void WriteOutput(string outputFile, string output)
        {
            try
            {
                File.WriteAllText(outputFile, output);
                System.Diagnostics.Process.Start(outputFile);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception: Unable to write to the output file.");
            }
        }
    }

    // Class to represent a card
    class Card
    {
        public int Value { get; }
        public int SuitValue { get; }

        // Constructor to initialize a card based on its string representation
        public Card(string card)
        {
            string rank = card.Substring(0, card.Length - 1).ToUpper();
            string suit = card.Last().ToString().ToUpper();

            // Assign values based on the card rank
            if (int.TryParse(rank, out int numericValue))
            {
                Value = numericValue;
            }
            else if (rank == "J")
            {
                Value = 11;
            }
            else if (rank == "Q")
            {
                Value = 12;
            }
            else if (rank == "K")
            {
                Value = 13;
            }
            else if (rank == "A")
            {
                Value = 11;
            }

            // Assign suit value based on the card suit
            switch (suit)
            {
                case "H":
                    SuitValue = 1;
                    break;
                case "S":
                    SuitValue = 2;
                    break;
                case "C":
                    SuitValue = 3;
                    break;
                case "D":
                    SuitValue = 4;
                    break;
                default:
                    SuitValue = 0;
                    break;
            }
        }
    }

    // Class to represent a player
    class Player
    {
        public string Name { get; }
        public List<Card> Cards { get; }
        public int Score { get; set; }

        // Constructor to initialize a player with a name and a list of cards
        public Player(string name, List<Card> cards)
        {
            Name = name;
            Cards = cards;
        }

        // Calculate the total score for a player
        public int CalculateTotalScore()
        {
            // Calculate base score by summing up card values
            int baseScore = Cards.Sum(card => card.Value);
            //Sets the score field of the player class
            Score = baseScore;

            return baseScore;
        }

        public int GetHighestCardSuitValue()
        {
            // Find the card with the highest value for the player
            Card highestCard = Cards.OrderByDescending(card => card.Value).FirstOrDefault();

            // Return the suit value of the highest card

            return highestCard?.SuitValue ?? 0;
        }

        public void AddSuitValueToScore(int score)
        {
            //Adds the highest card suit value to the players score
            Score += score;
        }
    }
}

