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

namespace MiniKreuzwortraetsel
{
    public partial class NewCollectionForm : Form
    {
        Form1 mainForm;
        public NewCollectionForm(Form1 mainForm)
        {
            InitializeComponent();

            this.mainForm = mainForm;
        }

        private void CreateTable(object sender, EventArgs e)
        {
            string newCollectionName = NewCollectionNameTextBox.Text;
            if (newCollectionName != "")
            {
                // Check if name is available
                bool available = true;
                foreach (var item in MySqlQueries.SHOW_TABLES())
                {
                    if (item == newCollectionName)
                        available = false;
                }

                if (available)
                {
                    MySqlQueries.CREATE_TABLE(newCollectionName);
                    CloseWindow();
                }
                else
                    ErrorLBL.Text = "Existiert bereits!";
            }
            else
                ErrorLBL.Text = "Name eingeben!";
        }

        // Escape closes window
        private void NewCollectionForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                CloseWindow();
        }

        private void CloseWindow()
        {
            Close();
            mainForm.Enabled = true;
            mainForm.UpdateTableMenu();
        }
    }
}
