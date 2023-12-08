# CardGame
simple multiplayer card game where:
7 players are dealt 5 cards from two 52 card decks, and the winner is the one with the highest score.
In the event of a tie, the scores are recalculated for only the tied players by calculating a "suit score" for
each player to see if the tie can be broken

Project has been created using the following technologies

  1.C# Console application

Main Method (Main):
Command-Line Arguments:

Default input and output file names are provided (inputFile = "input.txt" and outputFile = "output.txt").
Checks if both input and output file names are provided; otherwise, prints an exception message and exits.
Reading Player Data:

Calls the ReadInput method to read player data from the input file.
Validates the input format.
If the input is invalid, prints an exception message and exits.
Determining Winners:

Calls the DetermineWinners method to find the player(s) with the highest score.
If there's a tie, calculates suit scores and breaks ties based on suit values.
The resulting winners are stored in the winners list.
Writing Output:

Calls the WriteOutput method to write the winners and their scores to the output file.
Attempts to open the output file after writing.
ReadInput Method:
Reading and Parsing:
Reads each line from the input file, where each line represents a player's hand.
Splits each line into the player's name and their cards.
Validates that each player has exactly 5 cards; otherwise, throws an exception.
Creates Player objects with the parsed information.
DetermineWinners Method:
Highest Scores:

Calculates the highest score among players.
Retrieves a list of players with the highest score.
Tie Handling:

Checks for a tie by comparing scores.
If a tie occurs, application loops through the tied players and calculates suit scores.
Breaks the tie based on suit scores, updating the winner(s).

WriteOutput Method:
Writing to File:
Writes the output string to the output file.
Attempts to open the output file after writing.

Card Class:
Card Representation:
Represents a playing card with properties for its face value (Value) and suit value (SuitValue).
Initializes a card based on its string representation.

Player Class:
Player Representation:
Represents a player with properties for their name (Name), a list of cards (Cards), and a score (Score).
Calculates the total score based on the sum of card values.
Finds the suit value of the highest card.
Adds a suit value to the player's score.


