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
        Tile[,] grid = new Tile[20,20];
        Point gridOrigin = new Point();
        List<string> questions = new List<string>();
        Random random = new Random();
        int ts = 30;
        // Used to determine empty space in grid
        List<int> xCoords = new List<int>();
        List<int> yCoords = new List<int>();

        // TODO: export to docx
        // TODO: thumbnail

        // TODO: hover effect baseWord error
        // TODO: reserved tiles cant be highlighted
        public Form1()
        {
            InitializeComponent();
            // Create tile instances in grid
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x] = new Tile(x, y);
                }
            }
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
            // Clean up?
            // Handle different senders/sources (listBox, insertTupleBTN or baseWordBTN)
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
                // Find all possible ways the answer can be placed
                // and save how many letters are crossed
                List<(Tile questionTile, Tile tileAfterAnswer, Point direction, int matches)> candidates = new List<(Tile, Tile, Point, int)>();
                int maxMatches = 0;
                Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
                foreach (Point direction in directions)
                {
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        for (int x = 0; x < grid.GetLength(1); x++)
                        {
                            Tile questionTile = grid[y, x];
                            bool fits = true;
                            // Does question tile fit?
                            if (questionTile.GetText(out _))
                                fits = false;
                            // Free space after answer?
                            Tile tileAfterAnswer = null;
                            Point tileAfterAnswerPoint = new Point(x + (direction.X * (tuple.Answer.Length + 1)), y + (direction.Y * (tuple.Answer.Length + 1)));
                            if (tileAfterAnswerPoint.Y < grid.GetLength(0) && tileAfterAnswerPoint.X < grid.GetLength(1))
                            {
                                tileAfterAnswer = grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X];
                                if (tileAfterAnswer.GetText(out _))
                                    fits = false;
                            }

                            if (fits)
                            {
                                int matches = 0;
                                bool possible = true;
                                for (int i = 0; i < tuple.Answer.Length && possible == true; i++)
                                {
                                    int letterX = questionTile.GetPosition().X + direction.X + i * direction.X;
                                    int letterY = questionTile.GetPosition().Y + direction.Y + i * direction.Y;
                                    // In bounds check
                                    if (letterX >= 0 && letterX <= grid.GetUpperBound(1) && letterY >= 0 && letterY <= grid.GetUpperBound(0))
                                    {
                                        // obstacles found?
                                        if ((grid[letterY, letterX].GetText(out string text) && text != tuple.Answer[i].ToString()) ||
                                            questionTile.IsQuestionTile() || 
                                            questionTile.IsReserved())
                                                possible = false;
                                        else
                                            matches++;
                                    }
                                    else
                                        possible = false;
                                }

                                // Did the whole answer fit?
                                if (possible)
                                {
                                    // Then save the properties for later
                                    candidates.Add((questionTile, tileAfterAnswer, direction, matches));
                                    // Save maxMatches number for later
                                    if (matches >= maxMatches)
                                        maxMatches = matches;
                                }
                            }
                        }
                    }
                }

                // Candidate analysis
                if (candidates.Count == 0)
                    MessageBox.Show("Keine passende Stelle gefunden");
                else if (candidates.Count == 1)
                    // Fill answer into that position
                    FillAnswer(candidates[0].questionTile,
                                    candidates[0].direction,
                                    tuple);
                else if (candidates.Count > 1)
                {
                    // Highlight candidates
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        Brush color;
                        // Light green for few/no matches
                        if (candidates[i].matches < maxMatches)
                            color = Brushes.LightGreen;
                        // Green for maximum matches
                        else
                            color = Brushes.Green;

                        candidates[i].questionTile.SetBackgroundColor(color);
                        candidates[i].questionTile.HighlightDirections.Add(candidates[i].direction);
                    }
                    Tile.tupleToBeFilled = tuple;
                    gridPanel.Refresh();
                }
            }
        }
        private void FillAnswer(Tile questionTile, Point direction, (string Question, string Answer) tuple)
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
            questionTile.SetText(questionTileText);
            // Reserve tile after answer
            Point tileAfterAnswerPos = new Point(questionTile.GetPosition().X + (direction.X * (tuple.Answer.Length + 1)), questionTile.GetPosition().Y + (direction.Y * (tuple.Answer.Length + 1)));
            if (tileAfterAnswerPos.Y < grid.GetLength(0) && tileAfterAnswerPos.X < grid.GetLength(1))
                grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X].SetReserved(true);
            // Save coordinates for easy grid shrinking later
            xCoords.Add(questionTile.GetPosition().X);
            yCoords.Add(questionTile.GetPosition().Y);

            int letterX = 0; // Absolute position of the current letter
            int letterY = 0;
            // Fill the answer into the grid letter by letter
            for (int c = 0; c < tuple.Answer.Length; c++)
            {
                letterX = questionTile.GetPosition().X + (direction.X * (c + 1));
                letterY = questionTile.GetPosition().Y + (direction.Y * (c + 1));
                grid[letterY, letterX].SetText(tuple.Answer[c].ToString());
                // Save the letter position for easy shrinking of the grid later
                xCoords.Add(letterX);
                yCoords.Add(letterY);
            }

            gridPanel.Refresh();
        }
        /// <summary>
        /// ??? Increases Grid size in all directions by length
        /// </summary>
        private void IncreaseGridSize(int length)
        {
            // Create bigger Array
            Tile[,] biggerArray = new Tile[grid.GetLength(0) + length * 2, grid.GetLength(1) + length * 2];
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
        /// <summary>
        /// ???
        /// </summary>
        private void RealignGrid()
        {
            gridOrigin.X = -xCoords.Min() * ts;
            gridOrigin.Y = -yCoords.Min() * ts;
            // Resize window
            int minHeight = ts * 8 + 39;
            Width = (xCoords.Max() + 1 - xCoords.Min()) * ts + 16 + UIPanel.Width;
            Height = (yCoords.Max() + 1 - yCoords.Min()) * ts + 39;
            if (Height < minHeight)
                Height = minHeight;

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
                            grid[row - gridOrigin.Y / ts, col - gridOrigin.X / ts].GetText(out string gridString);
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
        private string ReplaceUmlaute(string input)
        {
            input = input.Replace("ß", "ss");
            input = input.Replace("ä", "ae");
            input = input.Replace("Ä", "Ae");
            input = input.Replace("ö", "oe");
            input = input.Replace("Ö", "Oe");
            input = input.Replace("ü", "ue");
            input = input.Replace("Ü", "Ue");

            return input;
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
                        MySqlQueries.INSERT( (string)tableMenu.SelectedItem, new string[] { "Question", "Answer" }, new string[] { textDialogForm.userInputs[0], ReplaceUmlaute(textDialogForm.userInputs[1]) } );
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
        private void GridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // The tile the mouse is hovering over: 
            int tileX = (e.X - gridOrigin.X) / ts;
            int tileY = (e.Y - gridOrigin.Y) / ts;
            // Out of bounds check
            if (tileX >= 0 && tileY >= 0 &&
                tileX <= grid.GetUpperBound(1) && tileY <= grid.GetUpperBound(0) )
                if (grid[(e.Y - gridOrigin.Y) / ts, (e.X - gridOrigin.X) / ts].IsQuestionTile())
                {
                    string tileText;
                    grid[(e.Y - gridOrigin.Y) / ts, (e.X - gridOrigin.X) / ts].GetText(out tileText);
                    int questionIndex = Convert.ToInt32(tileText[0]) - 1;
                    string popupText = questions[questionIndex];
                    popupLBL.Text = popupText;
                    popupLBL.Location = new Point(e.X + ts/2, e.Y - ts/2);
                    popupLBL.Visible = true;
                }
                else
                    popupLBL.Visible = false;
        }
        private void GridPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(gridOrigin.X, gridOrigin.Y);
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Tile tile = grid[y, x];
                    // Draw Background Color
                    e.Graphics.FillRectangle(tile.GetBackgroundColor(), x * ts, y * ts, ts, ts);
                    // Draw Rectangle
                    if (tile.HasRectangle())
                        e.Graphics.DrawRectangle(Pens.Black, x * ts, y * ts, ts - 1, ts - 1);
                    // Draw Text
                    if (tile.GetText(out string text))
                    {
                        Size textSize = TextRenderer.MeasureText(text, Font);
                        e.Graphics.DrawString(text, Font, tile.GetForegroundColor(), x * ts + ts / 2 - textSize.Width / 2, y * ts + ts / 2 - textSize.Height / 2);
                    }
                }
            }
        }
        private void GridPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Tile tile = grid[e.Y / ts, e.X / ts];
            if (tile.IsHighlighted())
            {
                int direction = 0;
                //if (tile.HighlightDirections.Count > 1)

                FillAnswer(tile, tile.HighlightDirections[direction], Tile.tupleToBeFilled);
                Tile.RemoveAllHighlights(grid);
                gridPanel.Refresh();
            }
        }
    }
}
