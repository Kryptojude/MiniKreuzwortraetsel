namespace MiniKreuzwortraetsel
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.GenerateCrosswordBTN = new System.Windows.Forms.Button();
            this.baseWordTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // GenerateCrosswordBTN
            // 
            this.GenerateCrosswordBTN.Location = new System.Drawing.Point(364, 255);
            this.GenerateCrosswordBTN.Name = "GenerateCrosswordBTN";
            this.GenerateCrosswordBTN.Size = new System.Drawing.Size(90, 40);
            this.GenerateCrosswordBTN.TabIndex = 0;
            this.GenerateCrosswordBTN.Text = "Kreuzworträtsel machen!";
            this.GenerateCrosswordBTN.UseVisualStyleBackColor = true;
            this.GenerateCrosswordBTN.Click += new System.EventHandler(this.GenerateCrossword);
            // 
            // baseWordTB
            // 
            this.baseWordTB.Location = new System.Drawing.Point(354, 209);
            this.baseWordTB.Name = "baseWordTB";
            this.baseWordTB.Size = new System.Drawing.Size(100, 20);
            this.baseWordTB.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(370, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lösungswort:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.baseWordTB);
            this.Controls.Add(this.GenerateCrosswordBTN);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GenerateCrosswordBTN;
        private System.Windows.Forms.TextBox baseWordTB;
        private System.Windows.Forms.Label label1;
    }
}

