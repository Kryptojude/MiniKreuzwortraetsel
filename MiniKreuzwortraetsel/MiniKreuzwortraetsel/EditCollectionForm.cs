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
        public EditCollectionForm(Form mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void EditCollectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Enabled = true;
        }
    }
}
