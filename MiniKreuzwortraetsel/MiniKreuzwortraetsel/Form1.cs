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
        Tile[,] grid = new Tile[20,20];
        static public int ts = 30;
        Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
        (Point Location, string Text, bool Visible) Popup = (new Point(), "", false);
        DeleteButton deleteButton = new DeleteButton();
        MySqlQueries mySqlQueries;
        Pen deleteButtonPen = new Pen(Brushes.Red, 1.7f);

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
                    grid[y, x] = new EmptyTile(new Point(x, y));
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
            EmptyTile.RemoveAllHighlights(grid);
            if (tuple.Answer != "")
            {
                // Find all possible ways the answer can be placed
                // and save how many letters are crossed
                List<(EmptyTile potentialQuestionTile, EmptyTile tileAfterAnswer, int direction, int matches)> candidates = new List<(Tile, Tile, int, int)>();
                int maxMatches = 0;
                for (int direction = 0; direction < 2; direction++)
                {
                    Point directionPoint = directions[direction];
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        for (int x = 0; x < grid.GetLength(1); x++)
                        {
                            // Does question tile fit?
                            bool questionTileFits = true;
                            if (!(grid[y, x] is EmptyTile))
                                questionTileFits = false;
                            else
                            {
                                EmptyTile potentialQuestionTile = grid[y, x] as EmptyTile;
                                // Free space after answer?
                                Tile tileAfterAnswer = null;
                                Point tileAfterAnswerPoint = new Point(x + (directionPoint.X * (tuple.Answer.Length + 1)), y + (directionPoint.Y * (tuple.Answer.Length + 1)));
                                // Out of bounds check
                                if (tileAfterAnswerPoint.Y < grid.GetLength(0) && tileAfterAnswerPoint.X < grid.GetLength(1))
                                {
                                    tileAfterAnswer = grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X];
                                
                                    if (!(tileAfterAnswer is EmptyTile))
                                        questionTileFits = false;
                                }

                                // Question tile fits into the spot, now find out if the answer fits
                                if (questionTileFits)
                                {
                                    int matchesForCurrentAnswer = 0;
                                    bool answerFits = true;
                                    // This loop goes along the path that the answer takes in the grid
                                    for (int i = 0; i < tuple.Answer.Length && answerFits == true; i++)
                                    {
                                        int letterX = potentialQuestionTile.GetPosition().X + directionPoint.X + i * directionPoint.X;
                                        int letterY = potentialQuestionTile.GetPosition().Y + directionPoint.Y + i * directionPoint.Y;
                                        // Are the selected coordinates in the grid?
                                        if (letterX >= 0 && letterX <= grid.GetUpperBound(1) && letterY >= 0 && letterY <= grid.GetUpperBound(0))
                                        {
                                            Tile potentialLetterTile = grid[letterY, letterX];
                                            // Tile is QuestionTile
                                            if (potentialLetterTile is QuestionTile)
                                                // Answer can't go over a QuestionTile
                                                answerFits = false;
                                            // Tile is EmptyTile
                                            else if (potentialLetterTile is EmptyTile)
                                            {
                                                // Answer can't go over reserved EmptyTile
                                                if ((potentialLetterTile as EmptyTile).Reserved)
                                                    answerFits = false;
                                            }
                                            // Tile is LetterTile
                                            else if (potentialLetterTile is LetterTile)
                                            {
                                                // Answer can't go over LetterTile with different letter
                                                string text = (potentialLetterTile as LetterTile).Text;
                                                // Letter matches
                                                if (text == tuple.Answer[i].ToString())
                                                    matchesForCurrentAnswer++;
                                                // Letter doesn't match
                                                else
                                                    answerFits = false;
                                            }
                                        }
                                        else
                                            answerFits = false;
                                    }

                                    // Did the whole answer fit?
                                    if (answerFits)
                                    {
                                        // Then save the properties for later
                                        candidates.Add((potentialQuestionTile, tileAfterAnswer, direction, matchesForCurrentAnswer));
                                        // Save maxMatches number for later
                                        if (matchesForCurrentAnswer >= maxMatches)
                                            maxMatches = matchesForCurrentAnswer;
                                    }
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
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        float colorLevel = 0;
                        if (maxMatches > 0)
                            colorLevel = (float)candidates[i].matches / maxMatches;
                        else
                            colorLevel = 0;
                        (candidates[i].potentialQuestionTile as EmptyTile).SubTiles[candidates[i].direction].SetHighlight(colorLevel);
                    }
                    Tile.TupleToBeFilled = tuple;
                    gridPB.Refresh();
                }
            }
        }
        private void FillAnswer(QuestionTile questionTile, int direction, (string Question, string Answer) tuple)
        {
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
            SaveFileDialog dialog = new SaveFileDialog { Filter = "HTML-Datei|.html" };
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
            Tile tile;
            // Out of bounds check
            Point mousePosition = new Point(e.X, e.Y);
            if (mousePosition.X >= 0 && mousePosition.Y >= 0 &&
                mousePosition.X <= grid.GetUpperBound(1) * ts && mousePosition.Y <= grid.GetUpperBound(0) * ts )
            {
                // The tile the mouse is hovering over: 
                tile = grid[mousePosition.X / ts, mousePosition.Y / ts];
                // Question tile?
                if (tile is QuestionTile)
                {
                    QuestionTile questionTile = tile as QuestionTile;
                    // Show delete button
                    deleteButton.SetVisible();
                    deleteButton.Location = questionTile.GetWorldPosition(ts);

                    // Check if mouse is over the delete button
                    int buttonSize = (int)(DeleteButton.buttonSizeFactor * ts);
                    if (e.X - questionTile.GetWorldPosition(ts).X > ts - buttonSize &&
                        e.Y - questionTile.GetWorldPosition(ts).Y < buttonSize)
                        deleteButton.SetHover(true);
                    else
                        deleteButton.SetHover(false);

                    // Normal question questionTile?
                    if (!questionTile.HasNumber())
                    {
                        // Show popup
                        Popup.Text = questionTile.GetQuestion();
                        Popup.Location = new Point(e.X + ts / 2, e.Y - ts / 2);
                        Popup.Visible = true;
                        gridPB.Refresh();
                    }
                }
                // Not a question tile
                else
                {
                    SubTile oldHoveringSubTile = SubTile.HoverSubTile;

                    SubTile newHoveringSubTile;
                    // Does the new tile have subtiles?
                    if (tile is EmptyTile)
                    {
                        EmptyTile emptyTile = tile as EmptyTile;
                        // Determine the subtile the mouse is hovering over
                        int mouseSubtile = 0;
                        if (e.X - tile.GetWorldPosition(ts).X < e.Y - tile.GetWorldPosition(ts).Y)
                            mouseSubtile = 1;
                        newHoveringSubTile = emptyTile.SubTiles[mouseSubtile];
                    }
                    // Letter tile
                    else
                        newHoveringSubTile = null;

                    // If old and new hoveringSubTile are different objects, then old hoveringSubTile is not hovering anymore
                    if (oldHoveringSubTile != newHoveringSubTile)
                    {   
                        SubTile.HoverSubTile = null;
                        Tile.RemoveAllExtendedHover(grid);

                        // Check if newHoveringSubTile should be set to hover
                        if (newHoveringSubTile?.IsHighlighted() == true)
                        {
                            SubTile.HoverSubTile = newHoveringSubTile;

                            // Activate extendedHover for adjacent tiles
                            Point directionPoint = directions[SubTile.HoverSubTile.Direction == "horizontal" ? 0 : 1];
                            for (int i = 0; i < Tile.TupleToBeFilled.Answer.Length; i++)
                            {
                                int letterX = SubTile.HoverSubTile.ParentTile.GetPosition().X + directionPoint.X * (1 + i);
                                int letterY = SubTile.HoverSubTile.ParentTile.GetPosition().Y + directionPoint.Y * (1 + i);
                                // Out of bounds check
                                if (letterX <= grid.GetUpperBound(1) && letterY <= grid.GetUpperBound(0))
                                {
                                    // End or middle outline
                                    if (i < Tile.TupleToBeFilled.Answer.Length - 1)
                                        grid[letterY, letterX].extendedHover = Tile.ExtendedHover.Two_Outlines_Horizontal;
                                    else
                                        grid[letterY, letterX].extendedHover = Tile.ExtendedHover.Three_Outlines_Horizontal;

                                    // Vertical mode
                                    if (directionPoint.Y == 1)
                                        grid[letterY, letterX].extendedHover += 2;
                                }
                            }                    
                        }
                        gridPB.Refresh();
                    }




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
            // Am I hovering over a Highlight?
            if (SubTile.HoverSubTile != null)
            {
                EmptyTile.RemoveAllHighlights(grid);
                Tile.RemoveAllExtendedHover(grid);
                int hoverSubTile = SubTile.HoverSubTile.Direction == "horizontal" ? 0 : 1;
                FillAnswer(SubTile.HoverSubTile.ParentTile, hoverSubTile, Tile.TupleToBeFilled);
            }
            // Am I hovering over the delete Button?
            else if (deleteButton.GetHover())
            {
                // Get clicked tile
                Tile clickedTile = grid[e.Y / ts, e.X / ts];
                if (clickedTile is QuestionTile)
                {
                    QuestionTile clickedQuestionTile = (QuestionTile)grid[e.Y / ts, e.X / ts];
                    // Reset all letterTiles that belong to this questionTile
                    List<LetterTile> linkedLetterTiles = clickedQuestionTile.LinkedLetterTiles;
                    foreach (LetterTile letterTile in linkedLetterTiles)
                        letterTile.ToEmptyTile(grid);

                    // Reset the questionTile
                    clickedQuestionTile.ToEmptyTile(grid);

                    gridPB.Refresh();
                }
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
