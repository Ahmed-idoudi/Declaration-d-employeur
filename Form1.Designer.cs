using System;

namespace DeclarationEmployeurApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem declarationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nouveauToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guide1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guide2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guide3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidePdf1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidePdf2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidePdf3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guidePdf4ToolStripMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtNomSociete = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExercice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbActe = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAnnexe7 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe6 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe5 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe4 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe3 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe2 = new System.Windows.Forms.CheckBox();
            this.chkAnnexe1 = new System.Windows.Forms.CheckBox();
            this.chkDeclarationPrincipale = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFichierSalaires = new System.Windows.Forms.TextBox();
            this.btnParcourirSalaires = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFichierRetenues = new System.Windows.Forms.TextBox();
            this.btnParcourirRetenues = new System.Windows.Forms.Button();
            this.btnGenerer = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMat = new System.Windows.Forms.TextBox();
            this.txtActiv = new System.Windows.Forms.TextBox();
            this.txtAdr = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPo = new System.Windows.Forms.TextBox();
            this.txtNum = new System.Windows.Forms.TextBox();
            this.txtRue = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.declarationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nouveauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guide1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guide2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guide3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNomSociete
            // 
            this.txtNomSociete.Location = new System.Drawing.Point(147, 22);
            this.txtNomSociete.Margin = new System.Windows.Forms.Padding(4);
            this.txtNomSociete.Name = "txtNomSociete";
            this.txtNomSociete.Size = new System.Drawing.Size(265, 27);
            this.txtNomSociete.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nom de la société:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 396);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Exercice:";
            // 
            // txtExercice
            // 
            this.txtExercice.Location = new System.Drawing.Point(151, 392);
            this.txtExercice.Margin = new System.Windows.Forms.Padding(4);
            this.txtExercice.Name = "txtExercice";
            this.txtExercice.Size = new System.Drawing.Size(132, 27);
            this.txtExercice.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 426);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Acte:";
            // 
            // cmbActe
            // 
            this.cmbActe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActe.FormattingEnabled = true;
            this.cmbActe.Items.AddRange(new object[] {
            "0 - Spontané",
            "1 - Régularisation",
            "2 - Redressement"});
            this.cmbActe.Location = new System.Drawing.Point(151, 422);
            this.cmbActe.Margin = new System.Windows.Forms.Padding(4);
            this.cmbActe.Name = "cmbActe";
            this.cmbActe.Size = new System.Drawing.Size(199, 28);
            this.cmbActe.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAnnexe7);
            this.groupBox1.Controls.Add(this.chkAnnexe6);
            this.groupBox1.Controls.Add(this.chkAnnexe5);
            this.groupBox1.Controls.Add(this.chkAnnexe4);
            this.groupBox1.Controls.Add(this.chkAnnexe3);
            this.groupBox1.Controls.Add(this.chkAnnexe2);
            this.groupBox1.Controls.Add(this.chkAnnexe1);
            this.groupBox1.Controls.Add(this.chkDeclarationPrincipale);
            this.groupBox1.Location = new System.Drawing.Point(20, 564);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(467, 246);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fichiers à générer";
            // 
            // chkAnnexe7
            // 
            this.chkAnnexe7.AutoSize = true;
            this.chkAnnexe7.Location = new System.Drawing.Point(240, 197);
            this.chkAnnexe7.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe7.Name = "chkAnnexe7";
            this.chkAnnexe7.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe7.TabIndex = 7;
            this.chkAnnexe7.Text = "Annexe 7";
            this.chkAnnexe7.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe6
            // 
            this.chkAnnexe6.AutoSize = true;
            this.chkAnnexe6.Location = new System.Drawing.Point(240, 160);
            this.chkAnnexe6.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe6.Name = "chkAnnexe6";
            this.chkAnnexe6.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe6.TabIndex = 6;
            this.chkAnnexe6.Text = "Annexe 6";
            this.chkAnnexe6.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe5
            // 
            this.chkAnnexe5.AutoSize = true;
            this.chkAnnexe5.Location = new System.Drawing.Point(240, 123);
            this.chkAnnexe5.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe5.Name = "chkAnnexe5";
            this.chkAnnexe5.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe5.TabIndex = 5;
            this.chkAnnexe5.Text = "Annexe 5";
            this.chkAnnexe5.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe4
            // 
            this.chkAnnexe4.AutoSize = true;
            this.chkAnnexe4.Location = new System.Drawing.Point(240, 86);
            this.chkAnnexe4.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe4.Name = "chkAnnexe4";
            this.chkAnnexe4.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe4.TabIndex = 4;
            this.chkAnnexe4.Text = "Annexe 4";
            this.chkAnnexe4.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe3
            // 
            this.chkAnnexe3.AutoSize = true;
            this.chkAnnexe3.Location = new System.Drawing.Point(240, 49);
            this.chkAnnexe3.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe3.Name = "chkAnnexe3";
            this.chkAnnexe3.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe3.TabIndex = 3;
            this.chkAnnexe3.Text = "Annexe 3";
            this.chkAnnexe3.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe2
            // 
            this.chkAnnexe2.AutoSize = true;
            this.chkAnnexe2.Location = new System.Drawing.Point(27, 160);
            this.chkAnnexe2.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe2.Name = "chkAnnexe2";
            this.chkAnnexe2.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe2.TabIndex = 2;
            this.chkAnnexe2.Text = "Annexe 2";
            this.chkAnnexe2.UseVisualStyleBackColor = true;
            // 
            // chkAnnexe1
            // 
            this.chkAnnexe1.AutoSize = true;
            this.chkAnnexe1.Location = new System.Drawing.Point(27, 123);
            this.chkAnnexe1.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnexe1.Name = "chkAnnexe1";
            this.chkAnnexe1.Size = new System.Drawing.Size(92, 24);
            this.chkAnnexe1.TabIndex = 1;
            this.chkAnnexe1.Text = "Annexe 1";
            this.chkAnnexe1.UseVisualStyleBackColor = true;
            // 
            // chkDeclarationPrincipale
            // 
            this.chkDeclarationPrincipale.AutoSize = true;
            this.chkDeclarationPrincipale.Checked = true;
            this.chkDeclarationPrincipale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeclarationPrincipale.Location = new System.Drawing.Point(27, 49);
            this.chkDeclarationPrincipale.Margin = new System.Windows.Forms.Padding(4);
            this.chkDeclarationPrincipale.Name = "chkDeclarationPrincipale";
            this.chkDeclarationPrincipale.Size = new System.Drawing.Size(178, 24);
            this.chkDeclarationPrincipale.TabIndex = 0;
            this.chkDeclarationPrincipale.Text = "Déclaration principale";
            this.chkDeclarationPrincipale.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 468);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Fichier Salaires:";
            // 
            // txtFichierSalaires
            // 
            this.txtFichierSalaires.Location = new System.Drawing.Point(151, 462);
            this.txtFichierSalaires.Margin = new System.Windows.Forms.Padding(4);
            this.txtFichierSalaires.Name = "txtFichierSalaires";
            this.txtFichierSalaires.Size = new System.Drawing.Size(265, 27);
            this.txtFichierSalaires.TabIndex = 9;
            // 
            // btnParcourirSalaires
            // 
            this.btnParcourirSalaires.Location = new System.Drawing.Point(431, 468);
            this.btnParcourirSalaires.Margin = new System.Windows.Forms.Padding(4);
            this.btnParcourirSalaires.Name = "btnParcourirSalaires";
            this.btnParcourirSalaires.Size = new System.Drawing.Size(40, 28);
            this.btnParcourirSalaires.TabIndex = 10;
            this.btnParcourirSalaires.Text = "...";
            this.btnParcourirSalaires.UseVisualStyleBackColor = true;
            this.btnParcourirSalaires.Click += new System.EventHandler(this.btnParcourirSalaires_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 513);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Fichier Retenues:";
            // 
            // txtFichierRetenues
            // 
            this.txtFichierRetenues.Location = new System.Drawing.Point(151, 513);
            this.txtFichierRetenues.Margin = new System.Windows.Forms.Padding(4);
            this.txtFichierRetenues.Name = "txtFichierRetenues";
            this.txtFichierRetenues.Size = new System.Drawing.Size(265, 27);
            this.txtFichierRetenues.TabIndex = 12;
            // 
            // btnParcourirRetenues
            // 
            this.btnParcourirRetenues.Location = new System.Drawing.Point(431, 513);
            this.btnParcourirRetenues.Margin = new System.Windows.Forms.Padding(4);
            this.btnParcourirRetenues.Name = "btnParcourirRetenues";
            this.btnParcourirRetenues.Size = new System.Drawing.Size(40, 28);
            this.btnParcourirRetenues.TabIndex = 13;
            this.btnParcourirRetenues.Text = "...";
            this.btnParcourirRetenues.UseVisualStyleBackColor = true;
            this.btnParcourirRetenues.Click += new System.EventHandler(this.btnParcourirRetenues_Click);
            // 
            // btnGenerer
            // 
            this.btnGenerer.Location = new System.Drawing.Point(1243, 727);
            this.btnGenerer.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerer.Name = "btnGenerer";
            this.btnGenerer.Size = new System.Drawing.Size(133, 37);
            this.btnGenerer.TabIndex = 14;
            this.btnGenerer.Text = "Générer";
            this.btnGenerer.UseVisualStyleBackColor = true;
            this.btnGenerer.Click += new System.EventHandler(this.btnGenerer_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 20);
            this.label6.TabIndex = 15;
            this.label6.Text = "Matricule:";
            // 
            // txtMat
            // 
            this.txtMat.Location = new System.Drawing.Point(147, 57);
            this.txtMat.Name = "txtMat";
            this.txtMat.Size = new System.Drawing.Size(265, 27);
            this.txtMat.TabIndex = 16;
            // 
            // txtActiv
            // 
            this.txtActiv.Location = new System.Drawing.Point(147, 90);
            this.txtActiv.Name = "txtActiv";
            this.txtActiv.Size = new System.Drawing.Size(265, 27);
            this.txtActiv.TabIndex = 17;
            // 
            // txtAdr
            // 
            this.txtAdr.Location = new System.Drawing.Point(147, 124);
            this.txtAdr.Name = "txtAdr";
            this.txtAdr.Size = new System.Drawing.Size(265, 27);
            this.txtAdr.TabIndex = 18;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPo);
            this.groupBox2.Controls.Add(this.txtNum);
            this.groupBox2.Controls.Add(this.txtRue);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtNomSociete);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtAdr);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtActiv);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtMat);
            this.groupBox2.Location = new System.Drawing.Point(20, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(479, 307);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Société";
            // 
            // txtPo
            // 
            this.txtPo.Location = new System.Drawing.Point(303, 207);
            this.txtPo.Margin = new System.Windows.Forms.Padding(4);
            this.txtPo.Name = "txtPo";
            this.txtPo.Size = new System.Drawing.Size(109, 27);
            this.txtPo.TabIndex = 20;
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(94, 204);
            this.txtNum.Margin = new System.Windows.Forms.Padding(4);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(95, 27);
            this.txtNum.TabIndex = 21;
            // 
            // txtRue
            // 
            this.txtRue.Location = new System.Drawing.Point(147, 164);
            this.txtRue.Margin = new System.Windows.Forms.Padding(4);
            this.txtRue.Name = "txtRue";
            this.txtRue.Size = new System.Drawing.Size(265, 27);
            this.txtRue.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(206, 207);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(90, 20);
            this.label11.TabIndex = 24;
            this.label11.Text = "Code Postal:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 207);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 20);
            this.label10.TabIndex = 23;
            this.label10.Text = "Numéro:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 164);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 20);
            this.label9.TabIndex = 22;
            this.label9.Text = "Rue";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "Adresse:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 20);
            this.label7.TabIndex = 20;
            this.label7.Text = "Activité:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.declarationToolStripMenuItem,
            this.guideToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(20, 20);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1377, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // declarationToolStripMenuItem
            // 
            this.declarationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nouveauToolStripMenuItem,
            this.ouvrirToolStripMenuItem});
            this.declarationToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.declarationToolStripMenuItem.Name = "declarationToolStripMenuItem";
            this.declarationToolStripMenuItem.Size = new System.Drawing.Size(100, 24);
            this.declarationToolStripMenuItem.Text = "Déclaration";
            // 
            // nouveauToolStripMenuItem
            // 
            this.nouveauToolStripMenuItem.Name = "nouveauToolStripMenuItem";
            this.nouveauToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.nouveauToolStripMenuItem.Text = "Nouveau";
            this.nouveauToolStripMenuItem.Click += new System.EventHandler(this.nouveauToolStripMenuItem_Click);
            // 
            // ouvrirToolStripMenuItem
            // 
            this.ouvrirToolStripMenuItem.Name = "ouvrirToolStripMenuItem";
            this.ouvrirToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.ouvrirToolStripMenuItem.Text = "Ouvrir";
            this.ouvrirToolStripMenuItem.Click += new System.EventHandler(this.ouvrirToolStripMenuItem_Click);
            // 
            // guideToolStripMenuItem
            // 
            this.guideToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.guide1ToolStripMenuItem,
            this.guide2ToolStripMenuItem,
            this.guide3ToolStripMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            this.guidePdf1ToolStripMenuItem,
            this.guidePdf2ToolStripMenuItem,
            this.guidePdf3ToolStripMenuItem,
            this.guidePdf4ToolStripMenuItem});
            this.guideToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.guideToolStripMenuItem.Name = "guideToolStripMenuItem";
            this.guideToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.guideToolStripMenuItem.Text = "Guide";

            // Nouveaux éléments pour les PDF
            this.guidePdf1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf1ToolStripMenuItem.Name = "guidePdf1ToolStripMenuItem";
            this.guidePdf1ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guidePdf1ToolStripMenuItem.Text = "CDC2019";
            this.guidePdf1ToolStripMenuItem.Click += new System.EventHandler(this.guidePdf1ToolStripMenuItem_Click);

            this.guidePdf2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf2ToolStripMenuItem.Name = "guidePdf2ToolStripMenuItem";
            this.guidePdf2ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guidePdf2ToolStripMenuItem.Text = "CDC2020";
            this.guidePdf2ToolStripMenuItem.Click += new System.EventHandler(this.guidePdf2ToolStripMenuItem_Click);

            this.guidePdf3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf3ToolStripMenuItem.Name = "guidePdf3ToolStripMenuItem";
            this.guidePdf3ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guidePdf3ToolStripMenuItem.Text = "CDC2021-2022";
            this.guidePdf3ToolStripMenuItem.Click += new System.EventHandler(this.guidePdf3ToolStripMenuItem_Click);

            this.guidePdf4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidePdf4ToolStripMenuItem.Name = "guidePdf4ToolStripMenuItem";
            this.guidePdf4ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guidePdf4ToolStripMenuItem.Text = "CDC2023-2024";
            this.guidePdf4ToolStripMenuItem.Click += new System.EventHandler(this.guidePdf4ToolStripMenuItem_Click);
            // 
            // guide1ToolStripMenuItem
            // 
            this.guide1ToolStripMenuItem.Name = "guide1ToolStripMenuItem";
            this.guide1ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guide1ToolStripMenuItem.Text = "Exemple fichier des salaires";
            this.guide1ToolStripMenuItem.Click += new System.EventHandler(this.guide1ToolStripMenuItem_Click);
            // 
            // guide2ToolStripMenuItem
            // 
            this.guide2ToolStripMenuItem.Name = "guide2ToolStripMenuItem";
            this.guide2ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guide2ToolStripMenuItem.Text = "Exemple fichier des Retenues";
            this.guide2ToolStripMenuItem.Click += new System.EventHandler(this.guide2ToolStripMenuItem_Click);
            // 
            // guide3ToolStripMenuItem
            // 
            this.guide3ToolStripMenuItem.Name = "guide3ToolStripMenuItem";
            this.guide3ToolStripMenuItem.Size = new System.Drawing.Size(285, 26);
            this.guide3ToolStripMenuItem.Text = "Type de Retenue 2023";
            this.guide3ToolStripMenuItem.Click += new System.EventHandler(this.guide3ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1417, 834);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnGenerer);
            this.Controls.Add(this.btnParcourirRetenues);
            this.Controls.Add(this.txtFichierRetenues);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnParcourirSalaires);
            this.Controls.Add(this.txtFichierSalaires);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbActe);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExercice);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Générateur de Déclarations Employeur";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //private void groupBox2_Enter(object sender, EventArgs e)
        //{
           // throw new NotImplementedException();
//}

        private System.Windows.Forms.TextBox txtNomSociete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExercice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbActe;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAnnexe7;
        private System.Windows.Forms.CheckBox chkAnnexe6;
        private System.Windows.Forms.CheckBox chkAnnexe5;
        private System.Windows.Forms.CheckBox chkAnnexe4;
        private System.Windows.Forms.CheckBox chkAnnexe3;
        private System.Windows.Forms.CheckBox chkAnnexe2;
        private System.Windows.Forms.CheckBox chkAnnexe1;
        private System.Windows.Forms.CheckBox chkDeclarationPrincipale;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFichierSalaires;
        private System.Windows.Forms.Button btnParcourirSalaires;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFichierRetenues;
        private System.Windows.Forms.Button btnParcourirRetenues;
        private System.Windows.Forms.Button btnGenerer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMat;
        private System.Windows.Forms.TextBox txtActiv;
        private System.Windows.Forms.TextBox txtAdr;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPo;
        private System.Windows.Forms.TextBox txtNum;
        private System.Windows.Forms.TextBox txtRue;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
    }
}

