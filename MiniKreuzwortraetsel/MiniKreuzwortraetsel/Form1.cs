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
        public const int TS = 30;
        Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
        Popup popup;
        MySqlQueries mySqlQueries;
        enum UI_mode_enum { normal, noDB };
        UI_mode_enum UI_mode;
        Random random = new Random();
        Point oldMousePosition = new Point();
        BufferedGraphicsContext currentContext;
        BufferedGraphics myBuffer;

        // TODO: 
        /*
         * Lokale Kopie der Datenbank machen und diese benutzen falls Verbindung fehlschlägt
         * Üvbereinstimmngen anzeigen knöpfe enabled/disabled
         * Wort einfügen abbrechen können + erklärung was zu tun ist wenn Highlights gezeigt werden
         * Wartefenster anzeigen während DB-Verbindung versucht wird
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

            currentContext = BufferedGraphicsManager.Current;
            myBuffer = currentContext.Allocate(gridPB.CreateGraphics(), DisplayRectangle);
            myBuffer.Graphics.FillRectangle(Brushes.White, gridPB.Bounds);
            myBuffer.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            // Create tile instances in grid
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Tile tile = new EmptyTile(new Point(x, y));
                    grid[y, x] = tile;
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
            //mySqlQueries = new MySqlQueries("Server=192.168.120.9;Database=cbecker;Uid=cbecker;Pwd=mGdkqGBxuawVbqob;");
            mySqlQueries = new MySqlQueries("Server=localhost;Database=mini_kreuzwort_raetsel;Uid=root;Pwd=;");
            if (mySqlQueries.TestConnection())
            {
                // DB connection successful
                UI_mode = UI_mode_enum.normal;
                // Becomes enabled when there are elements in the listBox
                InsertTupleBTN.Enabled = false;
                // Fill tableMenu with the tables in database
                UpdateTableMenu();
            }
            else
            {
                // DB connection failed
                UI_mode = UI_mode_enum.noDB;
                // Show error message at the top
                UIContainerPanel.Controls.Add(NoDBErrorLBL);
                // Replace collectionsPanel with questionAnswerPanel
                UIContainerPanel.Controls.Add(questionAnswerPanel);
                UIContainerPanel.Controls.SetChildIndex(questionAnswerPanel, UIContainerPanel.Controls.GetChildIndex(collectionsPanel));
                UIContainerPanel.Controls.Remove(collectionsPanel);

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
                InsertTupleBTN.Enabled = false;
                newTupleBTN.Enabled = false;
                deleteTupleBTN.Enabled = false;
                deleteCollectionBTN.Enabled = false;
                UpdateTuplesListBox();
            }
        }
        private void UpdateTuplesListBox()
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
                    InsertTupleBTN.Enabled = true;
                }
                // No tuples in table
                else
                {
                    deleteTupleBTN.Enabled = false;
                    InsertTupleBTN.Enabled = false;
                }
            }
        }
        /// <summary>
        /// 1. Checks where the question word can be placed in the grid, 
        /// 2. ranks the "candidates" by how many intersections with existing words there are, (use struct instead?)
        /// 3. highlights the candidate subtiles with green background color
        /// </summary>
        private void DetermineCandidateSubtiles((string Question, string Answer) tuple, bool highlightCandidates, bool baseWord)
        {
            // Check tuple
            if (tuple.Answer == "")
                MessageBox.Show("Antwort kann nicht leer sein");
            else if (tuple.Question == "" && !baseWord)
                // Question can't be empty
                MessageBox.Show("Frage kann nicht leer sein");
            else
            {
                // Replace illegal characters, uppercase answer
                tuple.Answer = ReplaceUmlaute(tuple.Answer).ToUpper();
                tuple.Question = ReplaceUmlaute(tuple.Question);

                // Reset Highlights
                EmptyTile.RemoveAllHighlights();

                // Find all possible ways the answer can be placed
                // and save how many letters are crossed ("matched")
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
                            if (grid[y, x] is EmptyTile)
                            {
                                EmptyTile potentialQuestionTile = grid[y, x] as EmptyTile;

                                // Free space or out of bounds after answer?
                                EmptyTile tileAfterAnswer = null;
                                Point tileAfterAnswerPoint = new Point(x + (directionPoint.X * (tuple.Answer.Length + 1)), y + (directionPoint.Y * (tuple.Answer.Length + 1)));
                                // The tile after the answer has to either be out of bounds, or if it is not, then it has to be an EmptyTile
                                bool tileAfterAnswerOK;
                                // Is the tile after the answer in bounds?
                                if (tileAfterAnswerPoint.X >= 0 && tileAfterAnswerPoint.X <= grid.GetUpperBound(1) && tileAfterAnswerPoint.Y >= 0 && tileAfterAnswerPoint.Y <= grid.GetUpperBound(0))
                                {
                                    // Tile after answer is in bounds...
                                    if (grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X] is EmptyTile)
                                    {
                                        // ...and it is an EmptyTile
                                        tileAfterAnswerOK = true;
                                        tileAfterAnswer = grid[tileAfterAnswerPoint.Y, tileAfterAnswerPoint.X] as EmptyTile;
                                    }
                                    else
                                    {
                                        // ...but it is not an EmptyTile
                                        tileAfterAnswerOK = false;
                                    }
                                }
                                else
                                {
                                    // Tile after answer is not in bounds
                                    tileAfterAnswerOK = true;
                                }

                                if (tileAfterAnswerOK)
                                {
                                    // So far: Question tile fits and tile after answer is either out of bounds or it is an EmptyTile

                                    // Now we check for letter collisions and matches
                                    int matchesForCurrentAnswer = 0;
                                    bool answerFits = true;
                                    // This loop goes along the path that the answer takes in the grid
                                    for (int i = 0; i < tuple.Answer.Length && answerFits == true; i++)
                                    {
                                        Point letterPoint = new Point(
                                            (potentialQuestionTile.GetPosition().X + directionPoint.X + i * directionPoint.X),
                                            (potentialQuestionTile.GetPosition().Y + directionPoint.Y + i * directionPoint.Y)
                                        );
                                        // Out of bounds check for letterPoint
                                        if (letterPoint.X >= 0 && letterPoint.X <= grid.GetUpperBound(1) && letterPoint.Y >= 0 && letterPoint.Y <= grid.GetUpperBound(0))
                                        {
                                            // What type of tile is selected?
                                            Tile potentialLetterTile = grid[letterPoint.Y, letterPoint.X];
                                            // Tile is QuestionTile
                                            if (potentialLetterTile is QuestionOrBaseWordTile)
                                                // Answer can't go over a QuestionTile
                                                answerFits = false;
                                            // Tile is EmptyTile
                                            else if (potentialLetterTile is EmptyTile)
                                            {
                                                // Answer can't go over reserved EmptyTile
                                                if ((potentialLetterTile as EmptyTile).IsReservedForQuestionTile())
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
                                            // Tile is out of bounds
                                            answerFits = false;
                                    }

                                    // Did the whole answer fit?
                                    if (answerFits)
                                    {
                                        // Then save the properties for later
                                        candidates.Add((potentialQuestionTile, tileAfterAnswer, direction, matchesForCurrentAnswer));
                                        // Update maxMatches if current matches are higher
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
                {
                    // Fill answer into that position, convert the EmptyTile to QuestionTile
                    QuestionOrBaseWordTile questionOrBaseWordTile;
                    if (baseWord)
                        questionOrBaseWordTile = candidates[0].potentialQuestionTile.ToBaseWordTile(grid, candidates[0].direction);
                    else
                        questionOrBaseWordTile = candidates[0].potentialQuestionTile.ToQuestionTile(grid, tuple.Question, candidates[0].direction);
                    
                    FillAnswer(questionOrBaseWordTile, tuple);
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
                    }
                    else
                    {
                        // There are multiple candidates and the best one should be autofilled, no highlights
                        // Save all the indeces of elements with maxMatches
                        List<int> maxMatchesIndeces = new List<int>();
                        for (int i = 0; i < candidates.Count; i++)
                            if (candidates[i].matches == maxMatches)
                            {
                                maxMatchesIndeces.Add(i);
                            }

                        // Pick a random index
                        int randomIndex = maxMatchesIndeces[random.Next(maxMatchesIndeces.Count)];
                        EmptyTile potentialQuestionTile = candidates[randomIndex].potentialQuestionTile;
                        QuestionOrBaseWordTile questionOrBaseWordTile;
                        if (!baseWord)
                            questionOrBaseWordTile = potentialQuestionTile.ToQuestionTile(grid, tuple.Question, candidates[randomIndex].direction);
                        else
                            questionOrBaseWordTile = potentialQuestionTile.ToBaseWordTile(grid, candidates[randomIndex].direction);

                        FillAnswer(questionOrBaseWordTile, tuple);
                    }
                }
            }

            RepaintFlaggedTiles();
        }

        private void FillAnswer(QuestionOrBaseWordTile questionOrBaseWordTile, (string Question, string Answer) tuple)
        {
            // Generate Coordinates for the direction
            Point directionPoint = directions[questionOrBaseWordTile.GetDirection()];

            // Get the tile after the answer
            Point tileAfterAnswerPos = new Point(questionOrBaseWordTile.GetPosition().X + (directionPoint.X * (tuple.Answer.Length + 1)), questionOrBaseWordTile.GetPosition().Y + (directionPoint.Y * (tuple.Answer.Length + 1)));
            // Out of bounds check
            if (tileAfterAnswerPos.Y < grid.GetLength(0) && tileAfterAnswerPos.X < grid.GetLength(1))
            {
                if (!(grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X] is EmptyTile))
                    throw new Exception("Tile after the answer is not an EmptyTile, previous check in DetermineCandidateSubtiles() failed");
                else
                {
                    EmptyTile tileAfterAnswer = grid[tileAfterAnswerPos.Y, tileAfterAnswerPos.X] as EmptyTile;
                    // Reserve the tile and link to questionTile
                    tileAfterAnswer.Reserve();
                    questionOrBaseWordTile.SetLinkedReservedTile(tileAfterAnswer);
                }
            }

            // Fill the answer into the grid letter by letter
            for (int c = 0; c < tuple.Answer.Length; c++)
            {
                int letterX = questionOrBaseWordTile.GetPosition().X + (directionPoint.X * (c + 1));
                int letterY = questionOrBaseWordTile.GetPosition().Y + (directionPoint.Y * (c + 1));
                Tile tile = grid[letterY, letterX];
                // Tile can be EmptyTile or (matching) LetterTile
                if (tile is EmptyTile)
                {
                    EmptyTile emptyTile = tile as EmptyTile;
                    // Convert the EmptyTile to LetterTile
                    string text = tuple.Answer[c].ToString();
                    emptyTile.ToLetterTile(grid, questionOrBaseWordTile, text, gridPB);
                }
                else if (tile is LetterTile)
                {
                    LetterTile letterTile = tile as LetterTile;
                    if (letterTile.Text != tuple.Answer[c].ToString())
                        throw new Exception("HightlightCandidateSubtiles() failed to see a non-matching letter in the path of the answer");
                    else
                    {
                        // Link this LetterTile to the QuestionTile
                        questionOrBaseWordTile.AddLinkedLetterTile(letterTile);
                    }
                }
                else if (tile is QuestionTile)
                    throw new Exception("HighlightCandidateSubtiles() failed to see that a QuestionTile is in the way of the answer");
            }
        }
        private void exportBTN_Click(object sender, EventArgs e)
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
                            html += "<td style='border: 2px solid black'>" + questionTile.GetText() + "</td>";
                        }
                        else if (tile is BaseWordTile)
                        {
                            BaseWordTile baseWordTile = tile as BaseWordTile;
                            html += "<td style='border: 2px solid black; color: red'>" + baseWordTile.GetText() + "</td>";
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
                List<QuestionTile> questionTileList = QuestionTile.QuestionTileList;
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
                        UpdateTuplesListBox();
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
            UpdateTuplesListBox();
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
        /// This will call tile.MouseMove for the tile that the mouse ends up on, 
        /// and tile.MouseLeave in case the mouse has moved from one tile to another
        /// </summary>
        private void GridPB_MouseMove(object sender, MouseEventArgs e)
        {
            // Get old mouse tile
            Tile oldMouseTile = grid[oldMousePosition.Y / TS, oldMousePosition.X / TS];
            // Get new mouse tile
            Tile newMouseTile = grid[e.Y / TS, e.X / TS];
            // This will always be called, whether mouse movement was intra-tile or inter-tile
            newMouseTile.MouseMove(e, gridPB, directions, grid);
            // Check if new mouse tile is different from old mouse tile
            if (oldMouseTile != newMouseTile)
            {
                // If they are different, then the mouse has moved from one tile to another, 
                // so also call MouseLeave for old tile
                oldMouseTile.MouseLeave(e, gridPB);
            }

            RepaintFlaggedTiles();

            // Update old mouse position
            oldMousePosition = new Point(e.X, e.Y);
        }
        private void GridPB_Paint(object sender, PaintEventArgs e)
        {
            myBuffer.Render();

            // Draw Popup
            //if (popup.IsVisible())
            //    e.Graphics.DrawString(popup.GetText(), Font, Brushes.Black, popup.GetPosition());

        }
        //private void RepaintAllTiles()
        //{
        //    // 7,892 ms
        //    Action action = () =>
        //    {
        //        for (int x = 0; x < grid.GetLength(1); x++)
        //        {
        //            for (int y = 0; y < grid.GetLength(0); y++)
        //            {
        //                Tile tile = grid[x, y];
        //                tile.Paint(myBuffer.Graphics);
        //            }
        //        }
        //        myBuffer.Render();
        //    };
        //    action.Invoke();
        //    //TimeLogger.TimeLogger.Benchmark(action, TimeLogger.TimeLogger.TimeUnit.Millisecond);
        //}
        private void RepaintFlaggedTiles()
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Tile tile = grid[y, x];
                    if (tile.GetRepaintFlag())
                    {
                        tile.Paint(myBuffer.Graphics);
                        tile.SetRepaintFlag(false);
                    }
                }
            }

            myBuffer.Render();
        }
        private void GridPB_MouseClick(object sender, MouseEventArgs e)
        {
            // Determine clicked tile
            Tile clickedTile = grid[e.Y / TS, e.X / TS];
            // Hand down event
            if (clickedTile is QuestionOrBaseWordTile)
                (clickedTile as QuestionOrBaseWordTile).MouseClick(e, grid, gridPB);
            else if (clickedTile is EmptyTile)
            {
                EmptyTile emptyTile = clickedTile as EmptyTile;
                if (emptyTile.MouseClick(e, out int direction))
                {
                    QuestionOrBaseWordTile questionTileInterface;
                    if (Tile.TupleToBeFilled.Question != "")
                        questionTileInterface = emptyTile.ToQuestionTile(grid, Tile.TupleToBeFilled.Question, direction);
                    else
                        questionTileInterface = emptyTile.ToBaseWordTile(grid, direction);

                    FillAnswer(questionTileInterface, Tile.TupleToBeFilled);
                }
            }

            RepaintFlaggedTiles();
        }

        // Methods that call DetermineCandidateSubtiles()
        private void InsertTupleBTN_Click(object sender, EventArgs e)
        {
            // Check whether to use tuplesListBox or questionAnswerPanel
            if (UI_mode == UI_mode_enum.normal)
            {
                // Use tuplesListBox
                if (tuplesListBox.SelectedItem != null)
                {
                    string selectedItem = tuplesListBox.SelectedItem.ToString();
                    string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                    (string Question, string Answer) tuple = (array[0], array[1]);
                    DetermineCandidateSubtiles(tuple, highlightCandidates: false, baseWord: false);
                }
            }
            else
            {
                // Use questionAnswerPanel
                (string Question, string Answer) tuple = (QuestionTBox.Text, AnswerTBox.Text);
                DetermineCandidateSubtiles(tuple, highlightCandidates: false, baseWord: false);
            }
        }
        private void InsertBaseWordBTN_Click(object sender, EventArgs e)
        {
            (string Question, string Answer) tuple = ("", baseWordTBox.Text);
            DetermineCandidateSubtiles(tuple, highlightCandidates: false, baseWord: true);
        }
        private void TuplesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (tuplesListBox.SelectedItem != null)
            {
                string selectedItem = tuplesListBox.SelectedItem.ToString();
                string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                (string Question, string Answer) tuple = (array[0], array[1]);
                DetermineCandidateSubtiles(tuple, highlightCandidates: false, baseWord: false);
            }
        }
        private void ShowMatchesBTN_Click(object sender, EventArgs e)
        {
            // Check whether to use tuplesListBox or questionAnswerPanel
            if (UI_mode == UI_mode_enum.normal)
            {
                // Use tuplesListBox
                if (tuplesListBox.SelectedItem != null)
                {
                    string selectedItem = tuplesListBox.SelectedItem.ToString();
                    string[] array = selectedItem.Split(new string[] { " <---> " }, StringSplitOptions.None);
                    (string Question, string Answer) tuple = (array[0], array[1]);
                    DetermineCandidateSubtiles(tuple, highlightCandidates: true, baseWord: false);
                }
            }
            else
            {
                // Use questionAnswerPanel
                (string Question, string Answer) tuple = (QuestionTBox.Text, AnswerTBox.Text);
                DetermineCandidateSubtiles(tuple, highlightCandidates: true, baseWord: false);
            }
        }
        private void ShowMatchesBaseWordBTN_Click(object sender, EventArgs e)
        {
            string baseWord = baseWordTBox.Text;
            if (baseWord != "")
            {
                (string Question, string Answer) tuple = ("", baseWord);
                DetermineCandidateSubtiles(tuple, highlightCandidates: true, baseWord: true);
            }
            else
                MessageBox.Show("Hilfswort darf nicht leer sein");
        }

        // Handle UI changes
        private void BaseWordTBox_TextChanged(object sender, EventArgs e)
        {
            TextBox senderTBox = sender as TextBox;
            if (senderTBox.Text != "")
            {
                InsertBaseWordBTN.Enabled = true;
                showMatchesBaseWordBTN.Enabled = true;
            }
            else
            {
                InsertBaseWordBTN.Enabled = false;
                showMatchesBaseWordBTN.Enabled = false;
            }
        }

        private void QuestionTBox_Or_AnswerTBox_TextChanged(object sender, EventArgs e)
        {
            if (QuestionTBox.Text != "" && AnswerTBox.Text != "")
            {
                InsertTupleBTN.Enabled = true;
                showMatchesBTN.Enabled = true;
            }
            else
            {
                InsertTupleBTN.Enabled = false;
                showMatchesBTN.Enabled = false;
            }
        }

        private void TableMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTuplesListBox();
        }
    }
}