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
using Xceed.Words.NET;
using System.Diagnostics;

namespace MiniKreuzwortraetsel
{
    public partial class Form1 : Form
    {
        string[,] grid = new string[20,20];
        Point gridOrigin = new Point();
        List<(string, string)> database = new List<(string, string)>();
        List<string> questions = new List<string>();
        Random random = new Random();
        int ts = 30;
        // Used to determine empty space in grid
        List<int> xCoords = new List<int>();
        List<int> yCoords = new List<int>();

        // TODO: export to docx
        // TODO: thumbnail

        // TODO: hover effect baseWord error
        // IDEA: let user decide which candidate (placement option) to use before filling answer in
        // TODO: dynamisches Vergrößern des Grids
        //       Might use different solution, simply place the first answer in the center of the grid, that reduces the chance that it reaches the border, non-matching answers should also try to be far away from the border
        public Form1()
        {
            InitializeComponent();

            // Fill tableMenu with the tables in database
            UpdateTableMenu();
        }
        public void UpdateTableMenu()
        {
            tableMenu.Items.Clear();
            foreach (var item in MySqlQueries.SHOW_TABLES())
            {
                tableMenu.Items.Add(item);
            }
            // At least 1 table
            if (tableMenu.Items.Count > 0)
            {
                tableMenu.SelectedIndex = 0;
                // Event UpdateTuples() is called

                newTupleBTN.Enabled = true;
                deleteCollectionBTN.Enabled = true;
            }
            // No tables
            else
            {
                insertTupleBTN.Enabled = false;
                newTupleBTN.Enabled = false;
                deleteTupleBTN.Enabled = false;
                deleteCollectionBTN.Enabled = false;
            }
        }
        private void UpdateTuples(object sender, EventArgs e)
        {
            tuplesListBox.Items.Clear();
            foreach (string[] row in MySqlQueries.SELECT((string)tableMenu.SelectedItem, "Question", true))
            {
                tuplesListBox.Items.Add(row[1] + " <---> " + row[2]);
            }
            // At least 1 tuple in table
            if (tuplesListBox.Items.Count > 0)
            {
                tuplesListBox.SelectedIndex = 0;
                deleteTupleBTN.Enabled = true;
                insertTupleBTN.Enabled = true;
            }
            // No tuples in table
            else
            {
                deleteTupleBTN.Enabled = false;
                insertTupleBTN.Enabled = false;
            }
        }
        private void PutAnswerIntoCrossword(object sender, EventArgs e)
        {
            // Extract tuple from listBox, insertTupleBTN or baseWordBTN
            (string Question, string Answer) tuple = ("","");
            if (sender is Button && (sender as Button).Name == "baseWordBTN")
                    tuple = ("", baseWordTB.Text.ToUpper());
            else if ((sender is ListBox || (sender is Button && (sender as Button).Name == "insertTupleBTN")) && tuplesListBox.SelectedItem != null)
            {
                if (tuplesListBox.SelectedItem != null)
                {
                    string selectedItem = tuplesListBox.SelectedItem?.ToString();
                    string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                    tuple = (array[0], array[1].ToUpper());
                }
            }

            if (tuple.Answer != "")
            {
                // Increase grid size to make all potential word crossings/matches possible (gets shrunken later)
                IncreaseGridSize(tuple.Answer.Length);

                // Find all possible ways the answer can be placed
                // and save how many letters are crossed
                List<(Point questionTilePos, Point direction, int matches)> candidates = new List<(Point questionTilePos, Point direction, int matches)>();
                int maxMatches = 0;
                Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
                foreach (Point direction in directions)
                {
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        for (int x = 0; x < grid.GetLength(1); x++)
                        {
                            Point questionTilePos = new Point(x, y);
                            Point tileAfterAnswer = new Point(x + (direction.X * (tuple.Answer.Length + 1)), y + (direction.Y * (tuple.Answer.Length + 1)));
                            bool fits = true;
                            // Does question tile fit here? Can be null or reserved
                            if (grid[questionTilePos.Y, questionTilePos.X] != null && grid[questionTilePos.Y, questionTilePos.X] != "reserved")
                                fits = false;
                            // Is there free space after answer? Can be null or reserved
                            if (tileAfterAnswer.Y < grid.GetLength(0) && tileAfterAnswer.X < grid.GetLength(1))
                                if (grid[tileAfterAnswer.Y, tileAfterAnswer.X] != null && grid[tileAfterAnswer.Y, tileAfterAnswer.X] != "reserved")
                                    fits = false;

                            if (fits)
                            {
                                int matches = 0;
                                bool possible = true;
                                Point answerStartPos = new Point(questionTilePos.X + direction.X, questionTilePos.Y + direction.Y);
                                for (int i = 0; i < tuple.Answer.Length && possible == true; i++)
                                {
                                    int letterX = answerStartPos.X + i * direction.X;
                                    int letterY = answerStartPos.Y + i * direction.Y;
                                    // In bounds check
                                    if (letterX >= 0 && letterX <= grid.GetUpperBound(1) && letterY >= 0 && letterY <= grid.GetUpperBound(0))
                                    {
                                        // not empty
                                        if (grid[letterY, letterX] != null)
                                        {
                                            // question tile
                                            if (grid[letterY, letterX].Contains('►') == true || grid[letterY, letterX].Contains('▼') == true)
                                                possible = false;
                                            // reserved tile
                                            if (grid[letterY, letterX] == "reserved")
                                                possible = false;
                                            // letter tile matches?
                                            else if (grid[letterY, letterX] == tuple.Answer[i].ToString())
                                                matches++;
                                            // letter tile doesn't match
                                            else
                                                possible = false;
                                        }
                                    }
                                    else
                                        possible = false;
                                }

                                // Did the whole answer fit?
                                if (possible)
                                {
                                    // Then save the properties for later
                                    candidates.Add((questionTilePos, direction, matches));
                                    // Save maxMatches number for later
                                    if (matches >= maxMatches)
                                        maxMatches = matches;
                                }
                            }
                        }
                    }
                }

                // Keep only candidates with maximum Matches
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i].matches < maxMatches)
                    {
                        candidates.RemoveAt(i);
                        i--;
                    }
                }

                // Candidate analysis
                int finalCandidateIdx = 0;
                if (candidates.Count == 0)
                    MessageBox.Show("Keine passende Stelle gefunden");
                else
                {
                    // Take random one if more than one candidate
                    if (candidates.Count > 1)
                        finalCandidateIdx = random.Next(candidates.Count);
                    // Fill answer into that position
                    // if there are 0 matches, then ask for approval (unless first answer)
                    bool userApproved = true;
                    if (candidates[finalCandidateIdx].matches == 0 && questions.Count > 0)
                    {
                        DialogResult result = MessageBox.Show("Keine Überschneidungen mit anderen Worten gefunden,\ntrotzdem einfügen?", "Sicher?", MessageBoxButtons.YesNo);
                        userApproved = (result == DialogResult.Yes) ? true : false;
                    }
                    if (userApproved)
                        FillAnswer(candidates[finalCandidateIdx].questionTilePos,
                                    candidates[finalCandidateIdx].direction,
                                    tuple);
                }

                // Shrink grid to minimum again

            }
        }
        /// <summary>
        /// Increases Grid size in all directions by length
        /// </summary>
        private void IncreaseGridSize(int length)
        {
            // Create bigger Array
            string[,] biggerArray = new string[grid.GetLength(0) + length * 2, grid.GetLength(1) + length * 2];
            // Copy grid into bigger array at correct position
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    biggerArray[y + length, x + length] = grid[y, x];
                }
            }

            grid = biggerArray;
        }
        private void GenerateCrossword(string baseWord)
        {
            questions.Clear();

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
            FillAnswer(baseQuestionTilePos, new Point(0, 1), ("", baseWord));

            // Try crossing each letter of the base word
            for (int i = 0; i < baseWord.Length; i++)
            {
                //CrossBaseWord(i, baseWord[i], baseQuestionTilePos);
            }

            // Shrink the grid to minimum by moving it to the top-left corner            
            gridOrigin.X = -xCoords.Min() * ts;
            gridOrigin.Y = -yCoords.Min() * ts;
            // Resize window
            int minHeight = ts * 8 + 39;
            Width = (xCoords.Max() + 1 - xCoords.Min()) * ts + 16 + UIPanel.Width;
            Height = (yCoords.Max() + 1 - yCoords.Min()) * ts + 39;
            if (Height < minHeight)
                Height = minHeight;

            xCoords.Clear();
            yCoords.Clear();

            Refresh();
        }
        private void FillAnswer(Point questionTilePos, Point direction, (string Question, string Answer) tuple)
        {
            // Fill the question indicator into the tile
            string arrow = (direction.X == 1) ? "►" : "▼";
            string questionTileText = "";
            if (tuple.Question == "")
                questionTileText = arrow;
            else
            {
                questions.Add(tuple.Question);
                questionTileText = questions.Count + arrow;
            }
            grid[questionTilePos.Y, questionTilePos.X] = questionTileText;
            // Reserve tile after answer
            Point tileAfterAnswer = new Point(questionTilePos.X + (direction.X * (tuple.Answer.Length + 1)), questionTilePos.Y + (direction.Y * (tuple.Answer.Length + 1)));
            if (tileAfterAnswer.Y < grid.GetLength(0) && tileAfterAnswer.X < grid.GetLength(1))
                grid[tileAfterAnswer.Y, tileAfterAnswer.X] = "reserved";
            // Save coordinates for easy grid shrinking later
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

            Refresh();
        }
        private void ExportToDocx(object sender, EventArgs e)
        {
            if (grid != null)
            {
                string fileName = "output.docx";
                using (var doc = DocX.Create(fileName))
                {
                    doc.MarginBottom = 0;
                    doc.MarginTop = 0;
                    doc.MarginLeft = 0;
                    doc.MarginRight = 0;

                    var table = doc.AddTable(grid.GetLength(0), grid.GetLength(1) + gridOrigin.X / ts);
                    float[] floatArray = new float[table.ColumnCount];
                    for (int i = 0; i < table.ColumnCount; i++)
                    {
                        floatArray[i] = 15f;
                    }
                    table.SetWidths(floatArray);
                    //table.AutoFit = Xceed.Document.NET.AutoFit.Fixed;
                    for (int col = 0; col < table.ColumnCount; col++)
                    {
                        for (int row = 0; row < table.RowCount; row++)
                        {
                            string gridString = grid[row - gridOrigin.Y / ts, col - gridOrigin.X / ts];
                            table.Rows[row].Cells[col].Paragraphs[0].Append(gridString);
                        }
                    }

                    doc.InsertTable(table);

                    doc.Save();
                    Process.Start("WINWORD.EXE", fileName);
                }
            }
            //else errorMessageLBL.Text = "Zuerst Kreuzworträtsel machen";
        }
        private void NewTupleBTN_Click(object sender, EventArgs e)
        {
            TextDialogForm textDialogForm = new TextDialogForm(2, "Eintrag in \"" + (string)tableMenu.SelectedItem + "\" hinzufügen", new string[] { "Frage eingeben: ", "Antwort eingeben: " }, "Eintrag hinzufügen", "");
            bool error = true;
            while (error)
                if (textDialogForm.ShowDialog(this) == DialogResult.OK)
                {
                    if (textDialogForm.userInputs[0] != "" && textDialogForm.userInputs[1] != "")
                    {
                        MySqlQueries.INSERT( (string)tableMenu.SelectedItem, new string[] { "Question", "Answer" }, new string[] { textDialogForm.userInputs[0], textDialogForm.userInputs[1] } );
                        error = false;
                        UpdateTuples(null, null);
                    }
                    // error
                    else
                        textDialogForm.errorLBL.Text = "Beide Felder ausfüllen";
                }
                // Exited dialog
                else
                    error = false;
        }
        private void DeleteTupleBTN_Click(object sender, EventArgs e)
        {
            MySqlQueries.DELETE((string)tableMenu.SelectedItem, new string[] { "Question", "Answer" }, tuplesListBox.SelectedItem.ToString().Split(new string[] { " <---> " }, StringSplitOptions.None));
            UpdateTuples(null, null);
        }
        /// <summary>
        /// Show user dialog and create new collection
        /// </summary>
        private void NewCollectionBTN_Click(object sender, EventArgs e)
        {
            TextDialogForm textDialogForm = new TextDialogForm(1, "Neue Sammlung erstellen", new string[] { "Name der Sammlung:" }, "Erstellen", "");
            bool error = true;
            while (error)
                if (textDialogForm.ShowDialog(this) == DialogResult.OK)
                {
                    string userInput = textDialogForm.userInputs[0];
                    if (userInput != "")
                    {
                        // Check if name is available
                        bool available = true;
                        foreach (var item in MySqlQueries.SHOW_TABLES())
                        {
                            if (item == userInput)
                                available = false;
                        }

                        if (available)
                        {
                            // success
                            MySqlQueries.CREATE_TABLE(userInput);
                            UpdateTableMenu();
                            error = false;
                        }
                        // error
                        else
                            textDialogForm.errorLBL.Text = "Existiert bereits";
                    }
                    // error
                    else
                        textDialogForm.errorLBL.Text = "Name eingeben";
                }
                // Exited dialog
                else
                    error = false;
        }
        private void DeleteCollectionBTN_Click(object sender, EventArgs e)
        {
            string selectedTable = tableMenu.SelectedItem.ToString();
            DialogResult result = MessageBox.Show("Sammlung '" + selectedTable + "' unwiderruflich löschen?", "Sammlung löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                MySqlQueries.DROP_TABLE(selectedTable);
                UpdateTableMenu();
            }
        }
        private void BaseWordTB_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
                baseWordBTN.Enabled = false;
            else
                baseWordBTN.Enabled = true;
        }
        /// <summary>
        /// Hover effect
        /// </summary>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // The tile the mouse is hovering over: 
            int tileX = (e.X - gridOrigin.X) / ts;
            int tileY = (e.Y - gridOrigin.Y) / ts;
            // Out of bounds check
            if (tileX >= 0 && tileY >= 0 &&
                tileX <= grid?.GetUpperBound(1) && tileY <= grid?.GetUpperBound(0) )
                if (grid?[(e.Y - gridOrigin.Y) / ts, (e.X - gridOrigin.X) / ts]?.Contains('►') == true)
                {
                    int questionIndex = Convert.ToInt32(grid[(e.Y - gridOrigin.Y) / ts, (e.X - gridOrigin.X) / ts][0].ToString()) - 1;
                    string popupText = questions[questionIndex];
                    popupLBL.Text = popupText;
                    popupLBL.Location = new Point(e.X + ts/2, e.Y - ts/2);
                    popupLBL.Visible = true;
                }
                else
                    popupLBL.Visible = false;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(gridOrigin.X, gridOrigin.Y);
            if (grid != null)
                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    for (int x = 0; x < grid.GetLength(1); x++)
                    {
                        if (grid[y, x] != null && grid[y, x] != "reserved")
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
