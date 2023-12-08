using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace BlackJack
{
    class Program
    {
        

        public static void Main(string[] args)
        {
            try
            {
                // Read command-line arguments
                string inputFile = null;
                string outputFile = null;

                //Sets input and output files from the command parameters
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("--in", StringComparison.OrdinalIgnoreCase))
                    {
                        inputFile= args[i + 1];
                    }
                    else if (args[i].Equals("--out", StringComparison.OrdinalIgnoreCase))
                    {
                        outputFile = args[i + 1];
                    }
                }

                if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentException("Invalid command parameters");
                }

                // Read player data from input file
                List<Player> players = ReadInput(inputFile);

                // Check if input is valid
                if (players == null)
                {
                    Console.WriteLine($"Exception: Invalid input format");
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
            //Array of all playing cards in a deck
            string[] deck = {"AH","AD","AC","AS",
                                   "2H","2D","2C","2S",
                                   "3H","3D","3C","3S",
                                   "4H","4D","4C","4S",
                                   "5H","5D","5C","5S",
                                   "6H","6D","6C","6S",
                                   "7H","7D","7C","7S",
                                   "8H","8D","8C","8S",
                                   "9H","9D","9C","9S",
                                   "10H","10D","10C","10S",
                                   "JH","JD","JC","JS",
                                   "QH","QD","QC","QS",
                                   "KH","KD","KC","KS",
                                   };
           
            try
            {
                List<Player> players = new List<Player>();

                string[] lines = File.ReadAllLines(inputFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Replace(" ", "").Split(':');
                    if (parts.Length == 2)
                    {
                        string playerName = parts[0].Replace(" ", "");
                        string[] cards = parts[1].Replace(" ", "").Split(',');

                        // Check if each player has exactly 7 players
                        if (File.ReadAllLines(inputFile).Length < 7)
                        {
                            Console.WriteLine("Invalid number of players, minimum of 7 players are required.");
                            throw new Exception("Invalid number of cards for a player.");
                        }
                        // Check if each player has exactly 5 cards
                        if (cards.Length != 5)
                        {
                            Console.WriteLine("Invalid number of cards for a player.");
                            throw new Exception("Invalid number of cards for a player.");
                        }

                        List<Card> playerCards = new List<Card>();
                        foreach (string card in cards)
                        {
                            //Checks if players cards exists in the deck and ignores case sensitivity
                            if (deck.Contains(card,StringComparer.OrdinalIgnoreCase))
                            {
                                playerCards.Add(new Card(card.Trim()));
                            }
                            else
                            {
                                Console.WriteLine("Invalid card/cards for a player/players.");
                                throw new Exception("Invalid cards/cards for a player/players.");
                            }

                        }

                        players.Add(new Player(playerName, playerCards));
                    }
                    //else
                    //{
                    //    throw new Exception("Invalid input format.");
                    //}
                }

                return players;
            }
            catch (Exception ex)
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
                //Checks if output file exists
                if (!File.Exists(outputFile))
                {
                    //if output files doesnt exists, file is created and Read/Write access is granted
                    var file = new FileStream(outputFile, FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read);
                    file.Dispose();
                }
                Console.WriteLine("Please wait while output file is generated.............");
                File.WriteAllText(outputFile, output);
                System.Diagnostics.Process.Start(outputFile);                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: Unable to write to the output file.: {ex.Message}");
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

