namespace DeclarationEmployeurApp
{
    partial class DeclarationsListForm
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
            this.components = new System.ComponentModel.Container();
            this.dgvDeclarations = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvDeclarations)).BeginInit();
            this.SuspendLayout();

            // Form properties
            this.Text = "Liste des Déclarations";
            this.Size = new System.Drawing.Size(800, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Padding = new System.Windows.Forms.Padding(20);

            // DataGridView
            this.dgvDeclarations.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvDeclarations.Location = new System.Drawing.Point(20, 20);
            this.dgvDeclarations.Size = new System.Drawing.Size(760, 400);
            this.dgvDeclarations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDeclarations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDeclarations.MultiSelect = false;
            this.dgvDeclarations.BackgroundColor = System.Drawing.Color.White;
            this.dgvDeclarations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDeclarations.RowHeadersVisible = false;
            this.dgvDeclarations.AllowUserToAddRows = false;
            this.dgvDeclarations.AllowUserToDeleteRows = false;
            this.dgvDeclarations.ReadOnly = true;
            this.dgvDeclarations.Name = "dgvDeclarations";
            this.dgvDeclarations.TabIndex = 0;

            // Select Button
            this.btnSelect.Text = "Sélectionner";
            this.btnSelect.Size = new System.Drawing.Size(120, 35);
            this.btnSelect.Location = new System.Drawing.Point(540, 430);
            this.btnSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSelect.ForeColor = System.Drawing.Color.White;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.TabIndex = 1;

            // Cancel Button
            this.btnCancel.Text = "Annuler";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.Location = new System.Drawing.Point(660, 430);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 2;

            // Add controls to form
            this.Controls.Add(this.dgvDeclarations);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnCancel);

            ((System.ComponentModel.ISupportInitialize)(this.dgvDeclarations)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        protected internal System.Windows.Forms.DataGridView dgvDeclarations;
        protected internal System.Windows.Forms.Button btnSelect;
        protected internal System.Windows.Forms.Button btnCancel;
    }
} 