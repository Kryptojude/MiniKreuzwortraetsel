using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    public partial class EditCollectionForm : Form
    {
        Form mainForm;
        public EditCollectionForm(Form mainForm, string mode)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            if (mode == "edit")
            {
                Text = "Sammlung bearbeiten";
            }
            else
            {
                Text = "Neue Sammlung erstellen";
            }
        }

        private void EditCollectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Enabled = true;
        }
    }
}
