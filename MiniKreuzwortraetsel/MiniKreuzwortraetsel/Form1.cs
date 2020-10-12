using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MiniKreuzwortraetsel
{
    public partial class Form1 : Form
    {
        string[,] grid;
        Point gridOrigin = new Point();
        List<(string, string)> database = new List<(string, string)>();
        Random random = new Random();
        int questionCounter = 0;
        int ts = 30;
        // Used to determine empty space in grid
        List<int> xCoords = new List<int>();
        List<int> yCoords = new List<int>();

        public Form1()
        {
            InitializeComponent();
            Paint += Draw;
        }

        private void GenerateCrossword(object sender, EventArgs e)
        {
            string baseWord = baseWordTB.Text.ToUpper();

            // Fetch and scramble database
            FetchDatabase();
            ScrambleDatabase();

            // Find longest word
            int longestWord = 0;
            for (int i = 0; i < database.Count; i++)
            {
                (string Question, string Answer) tuple = database[i];
                if (tuple.Answer.Length > longestWord)
                {
                    longestWord = tuple.Answer.Length;
                }
            }

            // Check if baseWord is longer than longest word in db
            if (baseWord.Length > longestWord)
                longestWord = baseWord.Length;

            // Initialize grid based on longest word
            int maximumGridWidth = longestWord * 2;
            grid = new string[longestWord + 1, maximumGridWidth];

            // Put the base word vertically
            Point baseQuestionTilePos = new Point(longestWord, 0);
            FillAnswer(baseQuestionTilePos, new Point(0, 1), ("", baseWord), IsBaseWord: true);

            // Try crossing each letter of the base word
            for (int i = 0; i < baseWord.Length; i++)
            {
                CrossBaseWord(i, baseWord[i], baseQuestionTilePos);
            }

            // Shrink the grid to minimum by moving it to the top-left corner            
            gridOrigin.X = -xCoords.Min() * ts;
            gridOrigin.Y = -yCoords.Min() * ts;
            // Resize window
            Width = (xCoords.Max() + 1 - xCoords.Min()) * ts + 16 + UIPanel.Width;
            Height = (yCoords.Max() + 1 - yCoords.Min()) * ts + 39;
            // Minimum size?

            xCoords.Clear();
            yCoords.Clear();
            questionCounter = 0;


            Refresh();
        }
        private void CrossBaseWord(int baseLetterIndex, char matchLetter, Point baseQuestionTilePos)
        {
            // Go through database till word is found with matching letter
            bool wordFound = false;
            int databaseIndex = 0;
            (string Question, string Answer) tuple = ("", "");
            int matchIndex = 0;
            while (!wordFound && databaseIndex < database.Count)
            {
                tuple = database[databaseIndex];
                matchIndex = tuple.Answer.IndexOf(matchLetter);
                if (matchIndex != -1)
                    wordFound = true;

                databaseIndex++;
            }
            if (wordFound)
            {
                Point newQuestionTilePos = new Point {
                    X = baseQuestionTilePos.X - 1 - matchIndex,
                    Y = baseQuestionTilePos.Y + baseLetterIndex + 1 };

                FillAnswer(newQuestionTilePos, new Point(1, 0), tuple, IsBaseWord: false);
            }
        }
        private void FillAnswer(Point questionTilePos, Point direction, (string Question, string Answer) tuple, bool IsBaseWord)
        {
            // Fill the question indicator into the tile
            string arrow = (direction.X == 1) ? "►" : "\n▼";
            string questionTileText = "";
            if (IsBaseWord)
            {
                questionTileText = arrow;
                questionCounter--;
            }
            else
                questionTileText = (questionCounter + 1) + arrow;
            grid[questionTilePos.Y, questionTilePos.X] = questionTileText;
            xCoords.Add(questionTilePos.X);
            yCoords.Add(questionTilePos.Y);

            int letterX = 0; // Absolute position of the current letter
            int letterY = 0;
            // Fill the answer into the grid letter by letter
            for (int c = 0; c < tuple.Answer.Length; c++)
            {
                letterX = questionTilePos.X + (direction.X * (c + 1));
                letterY = questionTilePos.Y + (direction.Y * (c + 1));
                grid[letterY, letterX] = tuple.Answer[c].ToString();
                // Save the letter position for easy shrinking of the grid later
                xCoords.Add(letterX);
                yCoords.Add(letterY);
            }
            database.Remove(tuple);
            questionCounter++;
        }
        private void FetchDatabase()
        {
            StreamReader reader = new StreamReader("database.txt");
            string line = reader.ReadLine();
            while (line != null)
            {
                string question = line.Substring(0, line.IndexOf(';'));
                string answer = line.Substring(line.IndexOf(';') + 1);
                database.Add((question, answer));
                line = reader.ReadLine();
            }
        }
        private void ScrambleDatabase()
        {
            List<(string Question, string Answer)> database2 = new List<(string Question, string Answer)>();
            for (int i = 0; i < database.Count; i++)
            {
                database2.Add(("", ""));
            }

            for (int i = 0; i < database.Count; i++)
            {
                while (true)
                {
                    int randomSpot = random.Next(database2.Count);
                    if (database2[randomSpot].Answer == "")
                    {
                        database2[randomSpot] = database[i];
                        break;
                    }
                }
            }

            database = database2;
        }
        private void Draw(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(gridOrigin.X, gridOrigin.Y);
            if (grid != null)
                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    for (int x = 0; x < grid.GetLength(1); x++)
                    {
                        if (grid[y, x] != null)
                        {
                            Size textSize = TextRenderer.MeasureText(grid[y, x], Font);
                            if (grid[y, x].Contains("►") ||
                                grid[y, x].Contains("▼"))
                            { // question tile
                                e.Graphics.DrawRectangle(Pens.Black, x * ts, y * ts, ts, ts);
                                e.Graphics.DrawString(grid[y, x], Font, Brushes.Red, x * ts + ts / 2 - textSize.Width / 2, y * ts + ts / 2 - textSize.Height / 2);
                            }
                            else if (grid[y, x] == "blocked")
                            { // blocked tile
                                e.Graphics.FillRectangle(Brushes.Black, x * ts, y * ts, ts, ts);
                            }
                            else
                            { // letter tile
                                e.Graphics.DrawRectangle(Pens.Black, x * ts, y * ts, ts, ts);
                                e.Graphics.DrawString(grid[y, x], Font, Brushes.DarkBlue, x * ts + ts / 2 - textSize.Width / 2, y * ts + ts / 2 - textSize.Height / 2);
                            }
                        }
                    }
                }
        }
    }
}
