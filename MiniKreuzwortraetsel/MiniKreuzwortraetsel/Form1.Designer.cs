﻿using System;

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
            this.baseWordTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UIPanel = new System.Windows.Forms.Panel();
            this.deleteTupleBTN = new System.Windows.Forms.Button();
            this.deleteCollectionBTN = new System.Windows.Forms.Button();
            this.newTupleBTN = new System.Windows.Forms.Button();
            this.insertTupleBTN = new System.Windows.Forms.Button();
            this.newCollectionBTN = new System.Windows.Forms.Button();
            this.baseWordBTN = new System.Windows.Forms.Button();
            this.tuplesListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableMenu = new System.Windows.Forms.ComboBox();
            this.exportBTN = new System.Windows.Forms.Button();
            this.NoDBPanel = new System.Windows.Forms.Panel();
            this.NoDBInsertTupleBTN = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NoDBAnswerTB = new System.Windows.Forms.TextBox();
            this.NoDBQuestionTB = new System.Windows.Forms.TextBox();
            this.popupLBL = new System.Windows.Forms.Label();
            this.gridPB = new System.Windows.Forms.PictureBox();
            this.NoDBErrorLBL = new System.Windows.Forms.Label();
            this.NoDBExportBTN = new System.Windows.Forms.Button();
            this.UIPanel.SuspendLayout();
            this.NoDBPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPB)).BeginInit();
            this.SuspendLayout();
            // 
            // baseWordTB
            // 
            this.baseWordTB.Location = new System.Drawing.Point(9, 31);
            this.baseWordTB.Margin = new System.Windows.Forms.Padding(4);
            this.baseWordTB.Name = "baseWordTB";
            this.baseWordTB.Size = new System.Drawing.Size(148, 26);
            this.baseWordTB.TabIndex = 1;
            this.baseWordTB.Text = "Elektron";
            this.baseWordTB.TextChanged += new System.EventHandler(this.BaseWordTB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hilfswort:";
            // 
            // UIPanel
            // 
            this.UIPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UIPanel.Controls.Add(this.deleteTupleBTN);
            this.UIPanel.Controls.Add(this.deleteCollectionBTN);
            this.UIPanel.Controls.Add(this.newTupleBTN);
            this.UIPanel.Controls.Add(this.insertTupleBTN);
            this.UIPanel.Controls.Add(this.newCollectionBTN);
            this.UIPanel.Controls.Add(this.baseWordBTN);
            this.UIPanel.Controls.Add(this.tuplesListBox);
            this.UIPanel.Controls.Add(this.label2);
            this.UIPanel.Controls.Add(this.tableMenu);
            this.UIPanel.Controls.Add(this.exportBTN);
            this.UIPanel.Controls.Add(this.label1);
            this.UIPanel.Controls.Add(this.baseWordTB);
            this.UIPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.UIPanel.Location = new System.Drawing.Point(762, 0);
            this.UIPanel.Margin = new System.Windows.Forms.Padding(4);
            this.UIPanel.Name = "UIPanel";
            this.UIPanel.Size = new System.Drawing.Size(438, 658);
            this.UIPanel.TabIndex = 3;
            // 
            // deleteTupleBTN
            // 
            this.deleteTupleBTN.Enabled = false;
            this.deleteTupleBTN.Location = new System.Drawing.Point(356, 457);
            this.deleteTupleBTN.Name = "deleteTupleBTN";
            this.deleteTupleBTN.Size = new System.Drawing.Size(71, 48);
            this.deleteTupleBTN.TabIndex = 10;
            this.deleteTupleBTN.Text = "Eintrag löschen";
            this.deleteTupleBTN.UseVisualStyleBackColor = true;
            this.deleteTupleBTN.Click += new System.EventHandler(this.DeleteTupleBTN_Click);
            // 
            // deleteCollectionBTN
            // 
            this.deleteCollectionBTN.Enabled = false;
            this.deleteCollectionBTN.Location = new System.Drawing.Point(281, 118);
            this.deleteCollectionBTN.Name = "deleteCollectionBTN";
            this.deleteCollectionBTN.Size = new System.Drawing.Size(146, 28);
            this.deleteCollectionBTN.TabIndex = 9;
            this.deleteCollectionBTN.Text = "Sammlung löschen";
            this.deleteCollectionBTN.UseVisualStyleBackColor = true;
            this.deleteCollectionBTN.Click += new System.EventHandler(this.DeleteCollectionBTN_Click);
            // 
            // newTupleBTN
            // 
            this.newTupleBTN.Enabled = false;
            this.newTupleBTN.Location = new System.Drawing.Point(266, 457);
            this.newTupleBTN.Name = "newTupleBTN";
            this.newTupleBTN.Size = new System.Drawing.Size(73, 48);
            this.newTupleBTN.TabIndex = 5;
            this.newTupleBTN.Text = "Neuer Eintrag";
            this.newTupleBTN.UseVisualStyleBackColor = true;
            this.newTupleBTN.Click += new System.EventHandler(this.NewTupleBTN_Click);
            // 
            // insertTupleBTN
            // 
            this.insertTupleBTN.Enabled = false;
            this.insertTupleBTN.Location = new System.Drawing.Point(15, 457);
            this.insertTupleBTN.Name = "insertTupleBTN";
            this.insertTupleBTN.Size = new System.Drawing.Size(111, 48);
            this.insertTupleBTN.TabIndex = 5;
            this.insertTupleBTN.Text = "Auswahl einfügen";
            this.insertTupleBTN.UseVisualStyleBackColor = true;
            this.insertTupleBTN.Click += new System.EventHandler(this.InsertTupleBTN_Click);
            // 
            // newCollectionBTN
            // 
            this.newCollectionBTN.Location = new System.Drawing.Point(146, 117);
            this.newCollectionBTN.Name = "newCollectionBTN";
            this.newCollectionBTN.Size = new System.Drawing.Size(129, 29);
            this.newCollectionBTN.TabIndex = 5;
            this.newCollectionBTN.Text = "Neue Sammlung";
            this.newCollectionBTN.UseVisualStyleBackColor = true;
            this.newCollectionBTN.Click += new System.EventHandler(this.NewCollectionBTN_Click);
            // 
            // baseWordBTN
            // 
            this.baseWordBTN.Location = new System.Drawing.Point(164, 31);
            this.baseWordBTN.Name = "baseWordBTN";
            this.baseWordBTN.Size = new System.Drawing.Size(97, 26);
            this.baseWordBTN.TabIndex = 8;
            this.baseWordBTN.Text = "Einfügen";
            this.baseWordBTN.UseVisualStyleBackColor = true;
            this.baseWordBTN.Click += new System.EventHandler(this.BaseWordBTN_Click);
            // 
            // tuplesListBox
            // 
            this.tuplesListBox.FormattingEnabled = true;
            this.tuplesListBox.ItemHeight = 19;
            this.tuplesListBox.Location = new System.Drawing.Point(10, 153);
            this.tuplesListBox.Name = "tuplesListBox";
            this.tuplesListBox.Size = new System.Drawing.Size(418, 289);
            this.tuplesListBox.TabIndex = 7;
            this.tuplesListBox.DoubleClick += new System.EventHandler(this.TuplesListBox_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sammlung auswählen:";
            // 
            // tableMenu
            // 
            this.tableMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableMenu.FormattingEnabled = true;
            this.tableMenu.Location = new System.Drawing.Point(11, 117);
            this.tableMenu.Name = "tableMenu";
            this.tableMenu.Size = new System.Drawing.Size(129, 27);
            this.tableMenu.TabIndex = 5;
            this.tableMenu.SelectedIndexChanged += new System.EventHandler(this.UpdateTuples);
            // 
            // exportBTN
            // 
            this.exportBTN.Location = new System.Drawing.Point(64, 590);
            this.exportBTN.Name = "exportBTN";
            this.exportBTN.Size = new System.Drawing.Size(135, 54);
            this.exportBTN.TabIndex = 4;
            this.exportBTN.Text = "Kreuzworträtsel exportieren";
            this.exportBTN.UseVisualStyleBackColor = true;
            this.exportBTN.Click += new System.EventHandler(this.ExportToHTML);
            // 
            // NoDBPanel
            // 
            this.NoDBPanel.Controls.Add(this.NoDBExportBTN);
            this.NoDBPanel.Controls.Add(this.NoDBErrorLBL);
            this.NoDBPanel.Controls.Add(this.NoDBInsertTupleBTN);
            this.NoDBPanel.Controls.Add(this.label4);
            this.NoDBPanel.Controls.Add(this.label3);
            this.NoDBPanel.Controls.Add(this.NoDBAnswerTB);
            this.NoDBPanel.Controls.Add(this.NoDBQuestionTB);
            this.NoDBPanel.Location = new System.Drawing.Point(291, 0);
            this.NoDBPanel.Name = "NoDBPanel";
            this.NoDBPanel.Size = new System.Drawing.Size(437, 656);
            this.NoDBPanel.TabIndex = 6;
            this.NoDBPanel.Visible = false;
            // 
            // NoDBInsertTupleBTN
            // 
            this.NoDBInsertTupleBTN.Location = new System.Drawing.Point(20, 198);
            this.NoDBInsertTupleBTN.Name = "NoDBInsertTupleBTN";
            this.NoDBInsertTupleBTN.Size = new System.Drawing.Size(81, 28);
            this.NoDBInsertTupleBTN.TabIndex = 4;
            this.NoDBInsertTupleBTN.Text = "Einfügen";
            this.NoDBInsertTupleBTN.UseVisualStyleBackColor = true;
            this.NoDBInsertTupleBTN.Click += new System.EventHandler(this.NoDBInsertTupleBTN_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Antwort eingeben: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Frage eingeben: ";
            // 
            // NoDBAnswerTB
            // 
            this.NoDBAnswerTB.Location = new System.Drawing.Point(21, 154);
            this.NoDBAnswerTB.Name = "NoDBAnswerTB";
            this.NoDBAnswerTB.Size = new System.Drawing.Size(248, 26);
            this.NoDBAnswerTB.TabIndex = 1;
            // 
            // NoDBQuestionTB
            // 
            this.NoDBQuestionTB.Location = new System.Drawing.Point(20, 96);
            this.NoDBQuestionTB.Name = "NoDBQuestionTB";
            this.NoDBQuestionTB.Size = new System.Drawing.Size(249, 26);
            this.NoDBQuestionTB.TabIndex = 0;
            // 
            // popupLBL
            // 
            this.popupLBL.AutoSize = true;
            this.popupLBL.BackColor = System.Drawing.SystemColors.Control;
            this.popupLBL.Location = new System.Drawing.Point(0, 0);
            this.popupLBL.Name = "popupLBL";
            this.popupLBL.Size = new System.Drawing.Size(0, 19);
            this.popupLBL.TabIndex = 4;
            this.popupLBL.Visible = false;
            // 
            // gridPB
            // 
            this.gridPB.Location = new System.Drawing.Point(0, -2);
            this.gridPB.Name = "gridPB";
            this.gridPB.Size = new System.Drawing.Size(755, 658);
            this.gridPB.TabIndex = 5;
            this.gridPB.TabStop = false;
            this.gridPB.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPB_Paint);
            this.gridPB.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GridPB_MouseClick);
            this.gridPB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GridPB_MouseMove);
            // 
            // NoDBErrorLBL
            // 
            this.NoDBErrorLBL.AutoSize = true;
            this.NoDBErrorLBL.ForeColor = System.Drawing.Color.Red;
            this.NoDBErrorLBL.Location = new System.Drawing.Point(20, 19);
            this.NoDBErrorLBL.Name = "NoDBErrorLBL";
            this.NoDBErrorLBL.Size = new System.Drawing.Size(399, 38);
            this.NoDBErrorLBL.TabIndex = 5;
            this.NoDBErrorLBL.Text = "Verbindung zur Datenbank konnte nicht aufgebaut werden.\r\nReduzierte Benutzeroberf" +
    "läche: ";
            // 
            // NoDBExportBTN
            // 
            this.NoDBExportBTN.Location = new System.Drawing.Point(20, 250);
            this.NoDBExportBTN.Name = "NoDBExportBTN";
            this.NoDBExportBTN.Size = new System.Drawing.Size(135, 54);
            this.NoDBExportBTN.TabIndex = 11;
            this.NoDBExportBTN.Text = "Kreuzworträtsel exportieren";
            this.NoDBExportBTN.UseVisualStyleBackColor = true;
            this.NoDBExportBTN.Click += new System.EventHandler(this.ExportToHTML);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 658);
            this.Controls.Add(this.NoDBPanel);
            this.Controls.Add(this.popupLBL);
            this.Controls.Add(this.gridPB);
            this.Controls.Add(this.UIPanel);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "KreuzworträtselMacher";
            this.UIPanel.ResumeLayout(false);
            this.UIPanel.PerformLayout();
            this.NoDBPanel.ResumeLayout(false);
            this.NoDBPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox baseWordTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel UIPanel;
        private System.Windows.Forms.Label popupLBL;
        private System.Windows.Forms.Button exportBTN;
        private System.Windows.Forms.ComboBox tableMenu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox tuplesListBox;
        private System.Windows.Forms.Button newTupleBTN;
        private System.Windows.Forms.Button insertTupleBTN;
        private System.Windows.Forms.Button newCollectionBTN;
        private System.Windows.Forms.Button baseWordBTN;
        private System.Windows.Forms.Button deleteCollectionBTN;
        private System.Windows.Forms.Button deleteTupleBTN;
        private System.Windows.Forms.PictureBox gridPB;
        private System.Windows.Forms.Panel NoDBPanel;
        private System.Windows.Forms.Button NoDBInsertTupleBTN;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NoDBAnswerTB;
        private System.Windows.Forms.TextBox NoDBQuestionTB;
        private System.Windows.Forms.Label NoDBErrorLBL;
        private System.Windows.Forms.Button NoDBExportBTN;
    }
}

