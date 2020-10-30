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
        string[,] grid;
        Point gridOrigin = new Point();
        List<(string, string)> database = new List<(string, string)>();
        List<string> questions = new List<string>();
        Random random = new Random();
        int ts = 30;
        // Used to determine empty space in grid
        List<int> xCoords = new List<int>();
        List<int> yCoords = new List<int>();

        // TODO: Mark the base word visually
        // TODO: export to docx
        // TODO: add question/answer pair from interface
        // TODO: thumbnail

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
                newTupleBTN.Enabled = false;
                deleteTupleBTN.Enabled = false;
                deleteCollectionBTN.Enabled = false;
            }
        }
        private void PutAnswerIntoCrossword(object sender, EventArgs e)
        {
            // Extract answer and question from the listBox
            ListBox senderListBox = sender as ListBox;
            if (senderListBox.Items.Count > 0)
            {
                string selectedItem = senderListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[1], array[0]);
            }
        }
        private void ReadBaseWord(object sender, EventArgs e)
        {
            string baseWord = baseWordTB.Text.ToUpper();
            if (!string.IsNullOrEmpty(baseWord))
            {
                GenerateCrossword(baseWord);
                errorMessageLBL.Visible = false;
            }
            else
                errorMessageLBL.Visible = true;

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
            int minHeight = ts * 8 + 39;
            Width = (xCoords.Max() + 1 - xCoords.Min()) * ts + 16 + UIPanel.Width;
            Height = (yCoords.Max() + 1 - yCoords.Min()) * ts + 39;
            if (Height < minHeight)
                Height = minHeight;

            xCoords.Clear();
            yCoords.Clear();

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
            string arrow = (direction.X == 1) ? "►" : "▼";
            string questionTileText = "";
            if (IsBaseWord)
                questionTileText = arrow;
            else
            {
                questions.Add(tuple.Question);
                questionTileText = questions.Count + arrow;
            }
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
            else errorMessageLBL.Text = "Zuerst Kreuzworträtsel machen";
        }
        private void UpdateTuples(object sender, EventArgs e)
        {
            tableContentListBox1.Items.Clear();
            foreach (string[] row in MySqlQueries.SELECT((string)tableMenu.SelectedItem, "Question", true))
            {
                tableContentListBox1.Items.Add(row[1] + " <---> " + row[2]);
            }
            if (tableContentListBox1.Items.Count > 0)
                deleteTupleBTN.Enabled = true;
            else
                deleteTupleBTN.Enabled = false;
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
        // TODO: When entry is deleted, the ID of that entry is missing
        private void DeleteTupleBTN_Click(object sender, EventArgs e)
        {
            MySqlQueries.DELETE((string)tableMenu.SelectedItem, "ID", tableMenu.SelectedIndex + 1);
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
