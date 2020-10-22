namespace MiniKreuzwortraetsel
{
    partial class NewCollectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.NewCollectionNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ErrorLBL = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(140, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Erstellen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CreateTable);
            // 
            // NewCollectionNameTextBox
            // 
            this.NewCollectionNameTextBox.Location = new System.Drawing.Point(12, 24);
            this.NewCollectionNameTextBox.Name = "NewCollectionNameTextBox";
            this.NewCollectionNameTextBox.Size = new System.Drawing.Size(122, 20);
            this.NewCollectionNameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name der Sammlung:";
            // 
            // ErrorLBL
            // 
            this.ErrorLBL.AutoSize = true;
            this.ErrorLBL.ForeColor = System.Drawing.Color.Red;
            this.ErrorLBL.Location = new System.Drawing.Point(12, 50);
            this.ErrorLBL.Name = "ErrorLBL";
            this.ErrorLBL.Size = new System.Drawing.Size(0, 13);
            this.ErrorLBL.TabIndex = 3;
            // 
            // NewCollectionForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 72);
            this.Controls.Add(this.ErrorLBL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewCollectionNameTextBox);
            this.Controls.Add(this.button1);
            this.KeyPreview = true;
            this.Name = "NewCollectionForm";
            this.Text = "Neue Sammlung erstellen";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NewCollectionForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox NewCollectionNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ErrorLBL;
    }
}