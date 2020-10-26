using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

/// <summary>
/// Make instance of this form and call it using ShowDialog();
/// Pressing the button in this form saves the text in the text boxes in the public field userInputs, so it's accessible in the mainForm,
/// All labels are public aswell, so they can be changed from the mainForm without creating a new instance. 
/// </summary>

namespace MiniKreuzwortraetsel
{
    public partial class TextDialogForm : Form
    {
        public Label[] labels;
        TextBox[] textBoxes;
        public string[] userInputs;
        public TextDialogForm(int numberOfTextBoxes, string title, string[] messages, string buttonText, string errorMsg)
        {
            InitializeComponent();
            Text = title;


            // Create textBoxes and labels
            textBoxes = new TextBox[numberOfTextBoxes];
            userInputs = new string[numberOfTextBoxes];
            labels = new Label[numberOfTextBoxes];
            for (int i = 0; i < numberOfTextBoxes; i++)
            {
                textBoxes[i] = new TextBox();
                labels[i] = new Label {
                    Text = messages[i],
                    Location = new Point(12, i * (5 + 19 + 26) + 12),
                };
                textBoxes[i].Location = new Point(labels[i].Location.X, labels[i].Location.Y + labels[i].Height);
                
                Controls.Add(textBoxes[i]);
                Controls.Add(labels[i]);
            }

            button1.Text = buttonText;
            errorLBL.Text = errorMsg;

            Height = numberOfTextBoxes * (textBoxes[0].Height + labels[0].Height) + errorLBL.Height + 39 + 12 + 12;
            ActiveControl = textBoxes[0];
        }
        /// <summary>
        /// Save textBox contents in userInputs array and clear all textboxes
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                userInputs[i] = textBoxes[i].Text;
                textBoxes[i].Clear();
            }
        }

        // Escape closes window
        private void RichTextDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
