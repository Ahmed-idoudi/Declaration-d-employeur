using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DeclarationEmployeurApp
{
    public partial class ExcelViewerForm : Form
    {
        private DataGridView dataGridView;
        private Button btnClose;
        private Label lblTitle;

        public ExcelViewerForm(DataTable data, string title)
        {
            InitializeComponent();
            
            // Configuration de la forme
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Titre
            lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            // Bouton de fermeture
            btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                Size = new Size(40, 40),
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 60, 20),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 40, 
                              Screen.PrimaryScreen.WorkingArea.Height - 100),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F)
            };

            // Style du DataGridView
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;

            // Style alterné des lignes
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;

            // Remplir les données
            dataGridView.DataSource = data;

            this.Controls.Add(dataGridView);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Name = "ExcelViewerForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }
    }
} 