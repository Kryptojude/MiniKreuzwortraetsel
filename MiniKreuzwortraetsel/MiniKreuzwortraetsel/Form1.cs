﻿using System;
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
        public const int TS = 30;
        Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
        Popup popup;
        MySqlQueries mySqlQueries;

        // TODO: 
        /*
         * leere frage abfangen
         * Title des HTML-Dokuments sollte nicht ganzer Pfad sein
         * HTML-Dokument zeigt Hilfsworte nicht
           Exportieren Fehler beim Anzeigen
           DB Verbindung retry knopf
           automatisches einfügen macht das wort immer so weit oben rechts wie möglich
           hilfswort einfärben
           datenbankverbindungsfehler abfangen
           Baseword class?
           einstellungs menü zbsp für datenbank verbindung
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

            // Instantiate Popup
            popup = new Popup();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Shows program to user while attempting DB connection
            Show();
            // Test database connection
            mySqlQueries = new MySqlQueries("Server=192.168.120.9;Database=cbecker;Uid=cbecker;Pwd=mGdkqGBxuawVbqob;");
            if (mySqlQueries.TestConnection())
            {
                // Fill tableMenu with the tables in database
                UpdateTableMenu();
            }
            else
            {
                // Replace normal interface with non-DB interface
                questionAnswerPanel.Visible = true;
                questionAnswerPanel.Parent = UIContainerPanel;
                questionAnswerPanel.Dock = DockStyle.Top;

                NoDBErrorLBL.Visible = true;
                NoDBErrorLBL.Parent = UIContainerPanel;
                NoDBErrorLBL.Dock = DockStyle.Top;

                collectionsPanel.Visible = false;
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
        private void DetermineCandidateSubtiles((string Question, string Answer) tuple, bool highlightCandidates)
        {
            // Reset Highlights
            EmptyTile.RemoveAllHighlights(grid);
            // Find all possible ways the answer can be placed
            // and save how many letters are crossed
            List<(EmptyTile potentialQuestionTile, EmptyTile tileAfterAnswer, int direction, int matches)> candidates = new List<(EmptyTile, EmptyTile, int, int)>();
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
                            EmptyTile tileAfterAnswer = null;
                            Point tileAfterAnswerPoint = new Point(x + (directionPoint.X * (tuple.Answer.Length + 1)), y + (directionPoint.Y * (tuple.Answer.Length + 1)));
                            // Out of bounds check
                            if (tileAfterAnswerPoint.Y < grid.GetLength(0) && tileAfterAnswerPoint.X < grid.GetLength(1))
                            {
                                // Is the tile after the answer empty?
                                if (!(grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X] is EmptyTile))
                                {
                                    tileAfterAnswer = grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X] as EmptyTile;
                                    questionTileFits = false;
                                }
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
                                            if ((potentialLetterTile as EmptyTile).IsReserved())
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

                    // Candidate analysis
                    if (candidates.Count == 0)
                        MessageBox.Show("Keine passende Stelle gefunden");
                    else if (candidates.Count == 1)
                    {
                        // Fill answer into that position, convert the EmptyTile to QuestionTile
                        FillAnswer(candidates[0].potentialQuestionTile.ToQuestionTile(grid, tuple.Question, candidates[0].direction), tuple);
                    }
                    else if (candidates.Count > 1)
                    {
                        if (highlightCandidates)
                        {
                            // There are multiple candidates and they should be highlighted
                            for (int i = 0; i < candidates.Count; i++)
                            {
                                float colorLevel = 0;
                                if (maxMatches > 0)
                                    colorLevel = (float)candidates[i].matches / maxMatches;

                                candidates[i].potentialQuestionTile.SubTiles[candidates[i].direction].SetHighlight(colorLevel);
                            }
                            Tile.TupleToBeFilled = tuple;
                            gridPB.Refresh();
                        }
                        else
                        {
                            // There are multiple candidates and the best one should be autofilled, no highlights
                            // Autofill the first candidate with max matches that is found
                            for (int i = 0; i < candidates.Count; i++)
                                if (candidates[i].matches == maxMatches)
                                {
                                    FillAnswer(candidates[i].potentialQuestionTile.ToQuestionTile(grid, tuple.Question, candidates[i].direction), tuple);
                                    break;
                                }
                        }
                    }
                }
            }
        }
        private void FillAnswer(QuestionTile questionTile, (string Question, string Answer) tuple)
        {
            // Generate Coordinates for the direction
            Point directionPoint = directions[questionTile.Direction];

            // Get the tile after the answer
            Point tileAfterAnswerPos = new Point(questionTile.GetPosition().X + (directionPoint.X * (tuple.Answer.Length + 1)), questionTile.GetPosition().Y + (directionPoint.Y * (tuple.Answer.Length + 1)));
            // Out of bounds check
            if (tileAfterAnswerPos.Y < grid.GetLength(0) && tileAfterAnswerPos.X < grid.GetLength(1))
            {
                if (!(grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X] is EmptyTile))
                    throw new Exception("Tile after the answer is not an EmptyTile, previous check in HighlightCandidateSubtiles() failed");

                EmptyTile tileAfterAnswer = grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X] as EmptyTile;
                // Reserve the tile and link to questionTile
                tileAfterAnswer.Reserve();
                questionTile.LinkedReservedTile = tileAfterAnswer;
            }

            // Fill the answer into the grid letter by letter
            for (int c = 0; c < tuple.Answer.Length; c++)
            {
                int letterX = questionTile.GetPosition().X + (directionPoint.X * (c + 1));
                int letterY = questionTile.GetPosition().Y + (directionPoint.Y * (c + 1));
                Tile tile = grid[letterY, letterX];
                // Tile can be EmptyTile or (matching) LetterTile
                if (tile is EmptyTile)
                {
                    EmptyTile emptyTile = tile as EmptyTile;
                    // Convert the EmptyTile to LetterTile
                    string text = tuple.Answer[c].ToString();
                    LetterTile letterTile = emptyTile.ToLetterTile(grid, questionTile, text);
                }
                else if (tile is LetterTile)
                {
                    LetterTile letterTile = tile as LetterTile;
                    if (letterTile.Text != tuple.Answer[c].ToString())
                        throw new Exception("HightlightCandidateSubtiles() failed to see a non-matching letter in the path of the answer");
                    // Link this LetterTile to the QuestionTile
                    questionTile.AddLinkedLetterTile(letterTile);
                }
                else if (tile is QuestionTile)
                    throw new Exception("HighlightCandidateSubtiles() failed to see that a QuestionTile is in the way of the answer");
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
                string crosswordTitle = Path.GetFileNameWithoutExtension(dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1));
                html += "<title>" + crosswordTitle + "</title>" + 
                        "</head>" +
                        "<body>" +
                        "<h1>" + crosswordTitle + "</h1>" +
                        "<table>";

                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    html += "<tr>";

                    for (int x = 0; x < grid.GetLength(1); x++)
                    {
                        Tile tile = grid[y, x];
                        // has content
                        // question tile
                        if (tile is QuestionTile)
                        {
                            QuestionTile questionTile = tile as QuestionTile;
                            if (questionTile.IsBaseWord())
                                html += "<td style='border: 2px solid black; color: red'>" + questionTile.Text + "</td>";
                            else
                                html += "<td style='border: 2px solid black'>" + questionTile.Text + "</td>";
                        }
                        // LetterTile -> dont show letter / show text field
                        else if (tile is LetterTile)
                            html += "<td style='border: 2px solid black'><input type='text'></input></td>";
                        // empty
                        else if (tile is EmptyTile)
                            html += "<td style='border: 2px solid white'></td>";
                    }

                    html += "</tr>";
                }

                html += "</table><p>";
                // Legende
                List<QuestionTile> questionTileList = QuestionTile.questionTileList;
                for (int i = 0; i < questionTileList.Count; i++)
                {
                    html += i + 1 + ". " + questionTileList[i].Question + "<br/>";
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
                InsertBaseWordBTN.Enabled = false;
            else
                InsertBaseWordBTN.Enabled = true;
        }
        /// <summary>
        /// Hover effects
        /// </summary>
        private void GridPB_MouseMove(object sender, MouseEventArgs e)
        {
            bool refresh = false;
            // Out of bounds check
            Point mousePosition = new Point(e.X, e.Y);
            if (mousePosition.X >= 0 && mousePosition.Y >= 0 &&
                mousePosition.X <= grid.GetUpperBound(1) * TS && mousePosition.Y <= grid.GetUpperBound(0) * TS )
            {
                // The tile the mouse is hovering over: 
                Tile tile = grid[mousePosition.Y / TS, mousePosition.X / TS];
                // Question tile
                if (tile is QuestionTile)
                {
                    QuestionTile questionTile = tile as QuestionTile;
                    questionTile.MouseMove(e, out bool needs_refresh, gridPB);
                    if (needs_refresh)
                    {
                        refresh = true;
                    }

                    // Baseword questionTile -> No popup
                    if (questionTile.IsBaseWord())
                    {
                        if (popup.IsVisible())
                        {
                            popup.Hide();
                            refresh = true;
                        }
                    }
                    // Normal question questionTil -> Show the popup
                    else
                    {
                        popup.SetText(questionTile.Question);
                        popup.SetPosition(new Point(e.X + TS / 2, e.Y - TS / 2));
                        popup.Show();
                        refresh = true;
                    }
                }
                // Not a question tile
                else
                {
                    DeleteButton.SetInvisible(gridPB);

                    SubTile oldHoveringSubTile = SubTile.HoverSubTile;

                    SubTile newHoveringSubTile;
                    // Does the new tile have subtiles?
                    if (tile is EmptyTile)
                    {
                        EmptyTile emptyTile = tile as EmptyTile;
                        // Determine the subtile the mouse is hovering over
                        int mouseSubtile = 0;
                        if (e.X - tile.GetWorldPosition(TS).X < e.Y - tile.GetWorldPosition(TS).Y)
                            mouseSubtile = 1;
                        newHoveringSubTile = emptyTile.SubTiles[mouseSubtile];
                    }
                    // Letter tile
                    else
                        newHoveringSubTile = null;

                    // If old and new hoveringSubTile are different objects, then old hoveringSubTile is not hovering anymore
                    if (oldHoveringSubTile != newHoveringSubTile)
                    {   
                        refresh = true;
                        SubTile.HoverSubTile = null;
                        Tile.RemoveAllExtendedHover(grid);

                        // Check if newHoveringSubTile should be set to hover
                        if (newHoveringSubTile?.IsHighlighted() == true)
                        {
                            SubTile.HoverSubTile = newHoveringSubTile;

                            // Activate extendedHover for adjacent tiles
                            Point directionPoint = directions[SubTile.HoverSubTile.Direction];
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
                    }

                    // Deactivate popup
                    if (popup.IsVisible())
                    {
                        popup.Hide();
                        refresh = true;
                    }
                }
            }

            if (refresh)
                gridPB.Refresh();
        }
        private void GridPB_Paint(object sender, PaintEventArgs e)
        {
            // Call the Paint function of all tiles
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                //if (y == )
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Tile tile = grid[y, x];
                    using (Image tileVisuals = tile.GetImage(TS))
                        e.Graphics.DrawImage(tileVisuals, x * TS, y * TS);
                }
            }

            // Draw Popup
            if (popup.IsVisible())
                e.Graphics.DrawString(popup.GetText(), Font, Brushes.Black, popup.GetPosition());
        }
        /// <summary>
        /// Calls FillAnswer if in bounds and on hover tile
        /// </summary>
        private void GridPB_MouseClick(object sender, MouseEventArgs e)
        {
            Tile clickedTile = grid[e.Y / TS, e.X / TS];
            // Am I hovering over a Highlight?
            if (SubTile.HoverSubTile != null)
            {
                EmptyTile.RemoveAllHighlights(grid);
                Tile.RemoveAllExtendedHover(grid);
                EmptyTile emptyTile = SubTile.HoverSubTile.ParentTile;
                QuestionTile questionTile = emptyTile.ToQuestionTile(grid, Tile.TupleToBeFilled.Question, SubTile.HoverSubTile.Direction);
                SubTile.HoverSubTile = null;
                FillAnswer(questionTile, Tile.TupleToBeFilled);
            }
            else if (clickedTile is QuestionTile)
            {
                QuestionTile clickedQuestionTile = clickedTile as QuestionTile;
                clickedQuestionTile.MouseClick(e, grid);

                gridPB.Refresh();
            }
        }
        private void NoDBBaseWordCHBox_CheckedChanged(object sender, EventArgs e)
        {
            NoDBQuestionTB.Enabled = !(sender as CheckBox).Checked;
            NoDBQuestionTB.Text = "";
            NoDBQuestionLBL.Enabled = !(sender as CheckBox).Checked;
        }

        /// <summary>
        /// Checks if the tuple is legal, if yes, then call DetermineCandidateSubtiles();
        /// </summary>
        private void ValidateTuple((string Answer, string Question) tuple, bool highlightCandidates, bool makeBaseWord)
        {
            if (tuple.Answer != "")
                MessageBox.Show("Antwort kann nicht leer sein");
            else if (tuple.Question != "" || makeBaseWord)
                // Question can't be empty unless it's a baseWord
                MessageBox.Show("Frage kann nicht leer sein");
            else
                DetermineCandidateSubtiles(tuple, highlightCandidates);
        }
        // Methods that call ValidateTuple()
        private void TuplesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1].ToUpper());
                ValidateTuple(tuple, highlightCandidates: false, makeBaseWord: false);
            }
        }
        private void InsertTupleBTN_Click(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1].ToUpper());
                ValidateTuple(tuple, highlightCandidates: false, makeBaseWord: false);
            }
        }
        private void InsertBaseWordBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = ("", baseWordTB.Text.ToUpper());
            ValidateTuple(tuple, highlightCandidates: true, makeBaseWord: true);
        }
        private void NoDBInsertBaseWordBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = ("", baseWordTB.Text.ToUpper());
            ValidateTuple(tuple, highlightCandidates: true, makeBaseWord: true);
        }
        private void NoDBInsertTupleBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = (ReplaceUmlaute(NoDBQuestionTB.Text), ReplaceUmlaute(NoDBAnswerTB.Text).ToUpper());
            ValidateTuple(tuple, highlightCandidates: false, makeBaseWord: false);
        }
        private void NoDBShowMatchesBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = (ReplaceUmlaute(NoDBQuestionTB.Text), ReplaceUmlaute(NoDBAnswerTB.Text).ToUpper());
            ValidateTuple(tuple, highlightCandidates: true, makeBaseWord: false);
        }
        private void ShowMatchesBTN_Click(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1].ToUpper());
                ValidateTuple(tuple, highlightCandidates: true, makeBaseWord: false);
            }
        }

    }
}