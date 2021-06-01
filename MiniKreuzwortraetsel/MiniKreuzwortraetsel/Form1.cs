using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MySqlQueriesNamespace;

namespace MiniKreuzwortraetsel
{
    public partial class Form1 : Form
    {
        readonly Tile[,] grid = new Tile[20,20];
        readonly int ts = 30;
        readonly Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
        (Point Location, string Text, bool Visible) Popup = (new Point(), "", false);
        DeleteButton deleteButton = new DeleteButton();
        MySqlQueries mySqlQueries;
        static Pen deleteButtonPen = new Pen(Brushes.Red, 1.7f);

        // TODO: 
        /*
           wörter löschen können
           hilfswort einfärben
           automatisches einfügen
           datenbankverbindungsfehler abfangen
        */
        public Form1()
        {
            InitializeComponent();
            // Create tile instances in grid
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x] = new Tile(x, y, ts, Font);
                }
            }

            // Test database connection
            //mySqlQueries = new MySqlQueries("Server=192.168.120.9;Database=cbecker;Uid=cbecker;Pwd=mGdkqGBxuawVbqob;");
            //if (mySqlQueries.TestConnection())
            //{
            //    // Fill tableMenu with the tables in database
            //    UpdateTableMenu();
            //}
            //else
            {
                // Replace normal interface with non-DB interface
                UIPanel.Visible = false;
                NoDBPanel.Visible = true;
                NoDBPanel.Location = UIPanel.Location;
            }
        }

        public void UpdateTableMenu()
        {
            tableMenu.Items.Clear();
            foreach (var item in mySqlQueries.SHOW_TABLES())
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
                UpdateTuples(null,  null);
            }
        }
        private void UpdateTuples(object sender, EventArgs e)
        {
            tuplesListBox.Items.Clear();
            if (tableMenu.Items.Count > 0)
            {
                foreach (string[] row in mySqlQueries.SELECT((string)tableMenu.SelectedItem, "Question", true))
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
        }
        /// <summary>
        /// 1. Checks where the question word can be placed in the grid, 
        /// 2. ranks the "candidates" by how many intersections with existing words there are, 
        /// 3. highlights the candidate subtiles with green background color
        /// </summary>
        private void HighlightCandidateSubtiles((string Question, string Answer) tuple)
        {
            // Reset Highlights
            Tile.RemoveAllHighlights(grid);
            if (tuple.Answer != "")
            {
                // Find all possible ways the answer can be placed
                // and save how many letters are crossed
                List<(Tile potentialQuestionTile, Tile tileAfterAnswer, int direction, int matches)> candidates = new List<(Tile, Tile, int, int)>();
                int maxMatches = 0;
                for (int direction = 0; direction < 2; direction++)
                {
                    Point directionPoint = directions[direction];
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        for (int x = 0; x < grid.GetLength(1); x++)
                        {
                            Tile potentialQuestionTile = grid[y, x];
                            bool fits = true;
                            // Does question tile fit?
                            if (potentialQuestionTile.GetText(out _))
                                fits = false;
                            // Free space after answer?
                            Tile tileAfterAnswer = null;
                            Point tileAfterAnswerPoint = new Point(x + (directionPoint.X * (tuple.Answer.Length + 1)), y + (directionPoint.Y * (tuple.Answer.Length + 1)));
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
                                    int letterX = potentialQuestionTile.GetPosition().X + directionPoint.X + i * directionPoint.X;
                                    int letterY = potentialQuestionTile.GetPosition().Y + directionPoint.Y + i * directionPoint.Y;
                                    // In bounds check
                                    if (letterX >= 0 && letterX <= grid.GetUpperBound(1) && letterY >= 0 && letterY <= grid.GetUpperBound(0))
                                    {
                                        Tile letterTile = grid[letterY, letterX];
                                        // questionTile or reserved tile
                                        if (letterTile.IsQuestionTile() ||
                                            letterTile.IsReserved())
                                            possible = false;
                                        // letter
                                        else if (letterTile.GetText(out string text))
                                        {
                                            // letter matches
                                            if (text == tuple.Answer[i].ToString())
                                                matches++;
                                            // letter doesn't match
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
                                    candidates.Add((potentialQuestionTile, tileAfterAnswer, direction, matches));
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
                    FillAnswer(candidates[0].potentialQuestionTile,
                                    candidates[0].direction,
                                    tuple);
                else if (candidates.Count > 1)
                {
                    // Highlight candidates
                    Color minColor = Color.FromArgb(0x9be8a1);
                    Color maxColor = Color.FromArgb(0x00ff14);
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        float proportion = 0;
                        if (maxMatches > 0)
                            proportion = (float)candidates[i].matches / maxMatches;
                        else
                            proportion = 0;
                        Color color = Color.FromArgb((int)(minColor.R + (maxColor.R - minColor.R) * proportion), (int)(minColor.G + (maxColor.G - minColor.G) * proportion), (int)(minColor.B + (maxColor.B - minColor.B) * proportion));
                        SolidBrush brush = new SolidBrush(color);
                        candidates[i].potentialQuestionTile.SubtileHighlightColors[candidates[i].direction] = brush;
                    }
                    Tile.tupleToBeFilled = tuple;
                    gridPB.Refresh();
                }
            }
        }
        private void FillAnswer(Tile questionTile, int direction, (string Question, string Answer) tuple)
        {
            // Determine if the questionTile is a baseword
            if (tuple.Question == "")
            {
                questionTile.IsBaseWordTile = true;
            }

            // Mark the question tile as such
            questionTile.SetQuestionTile();

            // Generate text for the question tile
            Point directionPoint = directions[direction];
            string arrow = (directionPoint.X == 1) ? "►" : "▼";
            if (questionTile.IsBaseWordTile)
                questionTile.SetText(arrow);
            else
                questionTile.SetText(Tile.GetQuestionTileList().Count + arrow);

            // Get the tile after the answer
            Point tileAfterAnswerPos = new Point(questionTile.GetPosition().X + (directionPoint.X * (tuple.Answer.Length + 1)), questionTile.GetPosition().Y + (directionPoint.Y * (tuple.Answer.Length + 1)));
            // Out of bounds check
            if (tileAfterAnswerPos.Y < grid.GetLength(0) && tileAfterAnswerPos.X < grid.GetLength(1))
            {
                // Reserve the tile
                grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X].SetReserved(true);

                // Link the reserved tile to the question tile that points to it
                questionTile.AddLinkedLetterTile(grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X]);
            }

            // Fill the answer into the grid letter by letter
            for (int c = 0; c < tuple.Answer.Length; c++)
            {
                int letterX = questionTile.GetPosition().X + (directionPoint.X * (c + 1));
                int letterY = questionTile.GetPosition().Y + (directionPoint.Y * (c + 1));
                grid[letterY, letterX].SetText(tuple.Answer[c].ToString());

                // Link the current letter tile to the question tile that points to it
                questionTile.AddLinkedLetterTile(grid[letterY, letterX]);
            }

            gridPB.Refresh();
        }
        private void ExportToHTML(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "HTML-Datei|.html";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Read start html
                StreamReader reader = new StreamReader("htmlStart.html");
                string html = reader.ReadToEnd();
                reader.Close();

                // Generate dynamic part of html
                html += "<title>" + dialog.FileName + "</title>" + 
                        "</head>" +
                        "<body>" +
                        "<h1>" + Path.GetFileNameWithoutExtension(dialog.FileName) + "</h1>" +
                        "<table>";

                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    html += "<tr>";

                    for (int x = 0; x < grid.GetLength(1); x++)
                    {
                        Tile tile = grid[y, x];
                        // has content
                        if (tile.GetText(out string text))
                        { 
                            // question tile
                            if (tile.IsQuestionTile())
                                html += "<td style='border: 2px solid black; color: red'>" + text + "</td>";
                            // baseWord tile
                            else if (tile.IsBaseWordTile)
                                html += "<td style='border: 2px solid black'>" + text + "</td>";
                            // question letter -> dont show letter / show text field
                            else
                                html += "<td style='border: 2px solid black'><input type='text'></input></td>";

                        }
                        // empty
                        else
                            html += "<td style='border: 2px solid white'></td>";
                    }

                    html += "</tr>";
                }

                html += "</table><p>";
                // Legende
                List<Tile> questionTileList = Tile.GetQuestionTileList();
                for (int i = 0; i < questionTileList.Count; i++)
                {
                    html += i + 1 + ". " + questionTileList[i].GetQuestion() + "<br/>";
                }
                html += "</p>";

                // Read end html
                reader = new StreamReader("htmlEnd.html");
                html += reader.ReadToEnd();
                reader.Close();

                // Export HTML file
                Stream stream;
                if ((stream = dialog.OpenFile()) != null)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(html);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    Process.Start(dialog.FileName);
                }
            }
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
        // Methods for changing collections
        private void NewTupleBTN_Click(object sender, EventArgs e)
        {
            TextDialogForm textDialogForm = new TextDialogForm(2, "Eintrag in \"" + (string)tableMenu.SelectedItem + "\" hinzufügen", new string[] { "Frage eingeben: ", "Antwort eingeben: " }, "Eintrag hinzufügen", "");
            bool error = true;
            while (error)
                if (textDialogForm.ShowDialog(this) == DialogResult.OK)
                {
                    if (textDialogForm.userInputs[0] != "" && textDialogForm.userInputs[1] != "")
                    {
                        mySqlQueries.INSERT( (string)tableMenu.SelectedItem, new string[] { "Question", "Answer" }, new string[] { textDialogForm.userInputs[0], ReplaceUmlaute(textDialogForm.userInputs[1]) } );
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
            mySqlQueries.DELETE((string)tableMenu.SelectedItem, new string[] { "Question", "Answer" }, tuplesListBox.SelectedItem.ToString().Split(new string[] { " <---> " }, StringSplitOptions.None));
            UpdateTuples(null, null);
        }
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
                        foreach (var item in mySqlQueries.SHOW_TABLES())
                        {
                            if (item == userInput)
                                available = false;
                        }

                        if (available)
                        {
                            // success
                            mySqlQueries.CREATE_TABLE(userInput);
                            UpdateTableMenu();
                            // Select the newly created table
                            tableMenu.SelectedItem = userInput;
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
                mySqlQueries.DROP_TABLE(selectedTable);
                UpdateTableMenu();
            }
        }
        /// <summary>
        /// Prevents empty baseword
        /// </summary>
        private void BaseWordTB_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
                baseWordBTN.Enabled = false;
            else
                baseWordBTN.Enabled = true;
        }
        /// <summary>
        /// Hover effects
        /// </summary>
        private void GridPB_MouseMove(object sender, MouseEventArgs e)
        {
            // The tile the mouse is hovering over: 
            Tile tile;
            int tileX = e.X / ts;
            int tileY = e.Y / ts;
            // Out of bounds check
            if (tileX >= 0 && tileY >= 0 &&
                tileX <= grid.GetUpperBound(1) && tileY <= grid.GetUpperBound(0) )
            {
                tile = grid[tileY, tileX];
                // Question tile?
                if (tile.IsQuestionTile())
                {
                    // Show delete button
                    deleteButton.SetVisible();
                    deleteButton.Location = tile.GetWorldPosition(ts);

                    // Check if mouse is over the delete button
                    int buttonSize = (int)(DeleteButton.buttonSizeFactor * ts);
                    if (e.X - tile.GetWorldPosition(ts).X > ts - buttonSize &&
                        e.Y - tile.GetWorldPosition(ts).Y < buttonSize)
                        deleteButton.SetHover(true);
                    else
                        deleteButton.SetHover(false);

                    // Normal question tile?
                    if (!tile.IsBaseWordTile)
                    {
                        // Show popup
                        Popup.Text = tile.GetQuestion();
                        Popup.Location = new Point(e.X + ts/2, e.Y - ts/2);
                        Popup.Visible = true;
                        gridPB.Refresh();
                    }
                }
                // Not a question tile
                else
                {
                    // Call hover routine of non-question tile
                    tile.ActivateHover(e.X, e.Y, ts, grid, gridPB, directions);
                    
                    // Deactivate popup and delete button
                    if (Popup.Visible)
                    {
                        Popup.Visible = false;
                        deleteButton.SetInvisible();
                        gridPB.Refresh();
                    }
                }
                
            }
        }
        private void GridPB_Paint(object sender, PaintEventArgs e)
        {
            // Call the Paint function of all tiles
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Tile tile = grid[y, x];
                    Image canvas = tile.GetGraphics(ts);
                    e.Graphics.DrawImage(canvas, x * ts, y * ts);
                    canvas.Dispose();
                }
            }

            // Draw Popup
            if (Popup.Visible)
                e.Graphics.DrawString(Popup.Text, Font, Brushes.Black, Popup.Location);

            // Draw Delete Button
            if (deleteButton.IsVisible())
            {
                e.Graphics.TranslateTransform(deleteButton.Location.X - deleteButtonPen.Width, deleteButton.Location.Y);
                int buttonSize = (int)(DeleteButton.buttonSizeFactor * ts);
                e.Graphics.DrawRectangle(deleteButtonPen, ts - buttonSize, 0, buttonSize, buttonSize);
                e.Graphics.DrawLine(deleteButtonPen, ts - buttonSize, 0, ts, buttonSize);
                e.Graphics.DrawLine(deleteButtonPen, ts - buttonSize, buttonSize, ts, 0);
            }
        }
        /// <summary>
        /// Calls FillAnswer if in bounds and on hover tile
        /// </summary>
        private void GridPB_MouseClick(object sender, MouseEventArgs e)
        {
            // Am I hovering over a Highlight
            Tile tile = Tile.currentHoveringTile;
            if (tile != null)
            {
                // Highlight
                Tile.RemoveAllHighlights(grid);
                Tile.RemoveAllExtendedHover(grid);
                FillAnswer(tile, tile.hoverSubtile, Tile.tupleToBeFilled);
            }
            // Am I hovering over the delete Button?
            else if (deleteButton.GetHover())
            {
                // Get clicked tile
                Tile clickedTile = grid[e.Y / ts, e.X / ts];
                // Reset all tiles that belong to this question tile
                List<Tile> linkedLetterTiles = clickedTile.GetLinkedLetterTiles();
                for (int i = 0; i < linkedLetterTiles.Count; i++)
                {
                    Tile currentTile = linkedLetterTiles[i];
                    // To how many question tiles does this letter tile belong?
                    switch (currentTile.GetSetTextCounter())
                    {
                        case 0:
                        case 1:
                            // setTextCounter will be 0 for the reserved tile that was linked to the question tile
                            // Replace this tile in the grid with a fresh one (reset)
                            grid[currentTile.GetPosition().Y, currentTile.GetPosition().X] = new Tile(currentTile.GetPosition().X, currentTile.GetPosition().Y, ts, Font);
                            break;
                        case 2:
                            currentTile.DecreaseSetTextCounter();
                            break;
                    }

                }

                // Remove the question tile from the questionTileList
                clickedTile.RemoveFromQuestionTileList();
                // Replace the question tile in the grid with a fresh one (reset)
                grid[clickedTile.GetPosition().Y, clickedTile.GetPosition().X] = new Tile(clickedTile.GetPosition().X, clickedTile.GetPosition().Y, ts, Font);

                gridPB.Refresh();
            }
        }
        // Methods that call PutAnswerIntoCrossWord(tuple);
        private void TuplesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1].ToUpper());
                HighlightCandidateSubtiles(tuple);
            }
        }
        private void InsertTupleBTN_Click(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1].ToUpper());
                HighlightCandidateSubtiles(tuple);
            }
        }
        private void BaseWordBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = ("", baseWordTB.Text.ToUpper());
            HighlightCandidateSubtiles(tuple);
        }
        private void NoDBInsertTupleBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = (ReplaceUmlaute(NoDBQuestionTB.Text), ReplaceUmlaute(NoDBAnswerTB.Text).ToUpper());
            HighlightCandidateSubtiles(tuple);
        }

    }
}
