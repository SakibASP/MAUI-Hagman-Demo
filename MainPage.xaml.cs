using HangmanDemo.Model;
using System.Collections.ObjectModel;

namespace HangmanDemo
{
    public partial class MainPage : ContentPage
    {
        private const int MaxAttempts = 4;
        private int attemptsLeft = MaxAttempts;
        private readonly string secretword = "uncopyrightable"; // Use your own 15 characters word

        private ObservableCollection<Letter> _letters = []; // for storing the mystery chars
        private ObservableCollection<Letter> _guessedletters = []; // for storing the users guess chars

        public ObservableCollection<Letter> MysteryWord
        {
            get
            {
                return _letters;
            }
            set
            {
                _letters = value;
            }
        }

        public ObservableCollection<Letter> GuessedWord
        {
            get
            {
                return _guessedletters;
            }
            set
            {
                _guessedletters = value;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            //wordCollectionView.ItemsSource = MysteryWord;
        }

        private void CheckButton_Clicked(object sender, EventArgs e)
        {
            string guess = UserInput.Text.Trim().ToLower();

            // Check if the input is a number
            if (int.TryParse(guess, out _))
            {
                DisplayAlert("Invalid Guess", "Please enter alphabets only.", "OK");
                return;
            }

            // Check if the input is the entire word
            if (guess.Length == secretword.Length)
            {
                if (guess.Equals(secretword, StringComparison.CurrentCultureIgnoreCase))
                {
                    DisplayAlert("Congratulations!", $"You've guessed the word.\n The word is {secretword.ToLower()}", "OK");
                    AddToMysteryWords(secretword, true);
                    AddToGuessWord("");
                    ResetGame();
                    attemptsLetter.Text = secretword;
                    return;
                }
                else
                {
                    attemptsLetter.Text = attemptsLetter.Text == ""?(attemptsLetter.Text + ',' + guess) : guess;
                    attemptsLeft--;
                    UpdateAttemptsLabel();
                    CheckGameOver();
                    return;
                }
            }

            // Check if the input is a single letter
            if (guess.Length == 1 && char.IsLetter(guess[0]))
            {
                bool correctGuess = false;
                // updating the word based on previous guessed words
                var _updatedSecretWord = secretword;
                if (GuessedWord.Count > 0)
                {
                    foreach (var word in GuessedWord.Select(x => x.Char))
                    {
                        _updatedSecretWord = RemoveNthOccurrence(_updatedSecretWord, word);
                    }
                }

                for (int i = 0; i < _updatedSecretWord.Length; i++)
                {
                    //ToLower for avoiding uppercase/lowercase value
                    if (char.ToLower(_updatedSecretWord[i]) == guess[0])
                    {
                        correctGuess = true;
                        AddToGuessWord(guess);
                        AddToMysteryWords(guess,false);
                        break;
                    }
                    else if (char.ToLower(secretword[i])  == guess[0] && char.ToLower(_updatedSecretWord[i]) != guess[0])
                    {
                        correctGuess = true;
                        DisplayAlert("Invalid Guess", $"You have already guessed {guess[0]}.", "OK");
                    }
                }

                if (!correctGuess)
                {
                    attemptsLetter.Text = attemptsLetter.Text != "" ? (attemptsLetter.Text + ',' + guess) : guess;
                    attemptsLeft--;
                    UpdateAttemptsLabel();
                    CheckGameOver();
                }
                else
                {
                    CheckGameWon();
                }
            }
            else
            {
                DisplayAlert("Invalid Guess", "Please enter a single alphabet or guess the entire word.", "OK");
            }

            UserInput.Text = ""; // Clear the entry field
        }

        private void UpdateAttemptsLabel()
        {
            attemptsLabel.Text = $"{attemptsLeft} Guesses Left";
        }

        private void CheckGameWon()
        {
            var guessWord = string.Join("", GuessedWord.Select(letter => letter.Char));
            if (secretword.Length == GuessedWord.Count)
            {
                if (AreStringsEqual(secretword,guessWord))
                {
                    DisplayAlert("Congratulations!", "You've guessed the word.", "OK");
                    AddToMysteryWords(secretword, true);
                    ResetGame();
                    //attemptsLetter.Text = secretword;
                    return;
                }
                else
                {
                    DisplayAlert("Sorry!", "You've lost the game.", "OK");
                    ResetGame();
                    return;
                }
            }
        }

        private void CheckGameOver()
        {
            if (attemptsLeft == 0)
            {
                string mysteryWord = string.Concat(MysteryWord.Select(letter => letter.Char));
                DisplayAlert("Game Over", $"Sorry, you've run out of attempts. The word was: {mysteryWord}", "OK");
                ResetGame();
            }
        }

        private static string RemoveNthOccurrence(string str, char charToRemove)
        {
            int indexToRemove = 0;

            while (str.Contains(charToRemove))
            {
                int nextAIndex = str.IndexOf(charToRemove, indexToRemove);
                str = str.Remove(nextAIndex, 1);
                indexToRemove = nextAIndex + 1; // Update index for next iteration
            }
            return str;
        }

        //comparing two string by avoiding their orders
        private static bool AreStringsEqual(string secretString, string guessString)
        {
            // Sort both strings
            char[] secretChar = secretString.ToCharArray();
            char[] guessedChar = guessString.ToCharArray();

            Array.Sort(secretChar);
            Array.Sort(guessedChar);

            // Compare sorted strings
            string sortedSecret = new(secretChar);
            string sortedGuess = new(guessedChar);

            return sortedSecret.Equals(sortedGuess);
        }
        private void AddToMysteryWords(string word,bool isClear)
        {
            if(isClear)
            {
                MysteryWord.Clear();
            }
            
            //adding to the ItemSource
            foreach (char letter in word)
            {
                _letters.Add(new Letter { Char = letter });
            }
            wordCollectionView.ItemsSource = MysteryWord;
        }

        private void AddToGuessWord(string word)
        {
            foreach (char letter in word)
            {
                _guessedletters.Add(new Letter { Char = letter });
            }
        }

        private void ResetGame()
        {
            attemptsLeft = MaxAttempts;
            UpdateAttemptsLabel();
            //wordCollectionView.ItemsSource = MysteryWord;
        }
    }
}
