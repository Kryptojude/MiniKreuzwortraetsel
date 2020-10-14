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
            this.UIPanel = new System.Windows.Forms.Panel();
            this.errorMessageLBL = new System.Windows.Forms.Label();
            this.popupLBL = new System.Windows.Forms.Label();
            this.UIPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GenerateCrosswordBTN
            // 
            this.GenerateCrosswordBTN.Location = new System.Drawing.Point(70, 122);
            this.GenerateCrosswordBTN.Margin = new System.Windows.Forms.Padding(4);
            this.GenerateCrosswordBTN.Name = "GenerateCrosswordBTN";
            this.GenerateCrosswordBTN.Size = new System.Drawing.Size(135, 58);
            this.GenerateCrosswordBTN.TabIndex = 0;
            this.GenerateCrosswordBTN.Text = "Mach Kreuzworträtsel!";
            this.GenerateCrosswordBTN.UseVisualStyleBackColor = true;
            this.GenerateCrosswordBTN.Click += new System.EventHandler(this.ReadBaseWord);
            // 
            // baseWordTB
            // 
            this.baseWordTB.Location = new System.Drawing.Point(63, 65);
            this.baseWordTB.Margin = new System.Windows.Forms.Padding(4);
            this.baseWordTB.Name = "baseWordTB";
            this.baseWordTB.Size = new System.Drawing.Size(148, 26);
            this.baseWordTB.TabIndex = 1;
            this.baseWordTB.Text = "Elektron";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lösungswort:";
            // 
            // UIPanel
            // 
            this.UIPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UIPanel.Controls.Add(this.errorMessageLBL);
            this.UIPanel.Controls.Add(this.label1);
            this.UIPanel.Controls.Add(this.GenerateCrosswordBTN);
            this.UIPanel.Controls.Add(this.baseWordTB);
            this.UIPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.UIPanel.Location = new System.Drawing.Point(931, 0);
            this.UIPanel.Margin = new System.Windows.Forms.Padding(4);
            this.UIPanel.Name = "UIPanel";
            this.UIPanel.Size = new System.Drawing.Size(269, 658);
            this.UIPanel.TabIndex = 3;
            // 
            // errorMessageLBL
            // 
            this.errorMessageLBL.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorMessageLBL.ForeColor = System.Drawing.Color.Red;
            this.errorMessageLBL.Location = new System.Drawing.Point(11, 95);
            this.errorMessageLBL.Name = "errorMessageLBL";
            this.errorMessageLBL.Size = new System.Drawing.Size(256, 23);
            this.errorMessageLBL.TabIndex = 3;
            this.errorMessageLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // popupLBL
            // 
            this.popupLBL.AutoSize = true;
            this.popupLBL.Location = new System.Drawing.Point(504, 373);
            this.popupLBL.Name = "popupLBL";
            this.popupLBL.Size = new System.Drawing.Size(0, 19);
            this.popupLBL.TabIndex = 4;
            this.popupLBL.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 658);
            this.Controls.Add(this.popupLBL);
            this.Controls.Add(this.UIPanel);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.UIPanel.ResumeLayout(false);
            this.UIPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GenerateCrosswordBTN;
        private System.Windows.Forms.TextBox baseWordTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel UIPanel;
        private System.Windows.Forms.Label errorMessageLBL;
        private System.Windows.Forms.Label popupLBL;
    }
}

