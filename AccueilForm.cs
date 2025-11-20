using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeclarationEmployeurApp
{
    public partial class AccueilForm : Form
    {
        private Form1 mainForm;

        public AccueilForm()
        {
            InitializeComponent();
            
            // Configuration plein écran
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9F);
            this.Padding = new Padding(20);
            this.Text = "Accueil - Déclarations Employeur";

            // Ajouter bouton de fermeture
            Button btnClose = new Button
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
            btnClose.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnClose);

            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int centerX = (screenWidth - 600) / 2; // 600 est la largeur de la zone de contenu

            // Logo ou Icône (à remplacer par votre logo)
            Label lblLogo = new Label
            {
                Text = "📊",
                Font = new Font("Segoe UI", 48F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                Location = new Point(centerX, screenHeight / 6)
            };
            this.Controls.Add(lblLogo);

            // Titre principal
            Label lblTitle = new Label
            {
                Text = "Bienvenue dans l'Application de Déclarations Employeur",
                Font = new Font("Segoe UI", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblTitle.Location = new Point((screenWidth - lblTitle.PreferredWidth) / 2, screenHeight / 4);
            this.Controls.Add(lblTitle);

            // Description
            Label lblDescription = new Label
            {
                Text = "Gérez facilement vos déclarations employeur avec notre application intuitive.",
                Font = new Font("Segoe UI", 16F),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblDescription.Location = new Point((screenWidth - lblDescription.PreferredWidth) / 2, lblTitle.Bottom + 30);
            this.Controls.Add(lblDescription);

            // Conteneur pour les boutons
            Panel buttonPanel = new Panel
            {
                Width = 600,
                Height = 300,
                Location = new Point(centerX, lblDescription.Bottom + 50)
            };
            this.Controls.Add(buttonPanel);

            // Boutons avec style moderne
            Button btnNewDeclaration = CreateStyledButton("Nouvelle Déclaration", 0, 0);
            btnNewDeclaration.Width = 600;
            btnNewDeclaration.Click += (s, e) => { OpenMainForm(true); };
            buttonPanel.Controls.Add(btnNewDeclaration);

            Button btnOpenDeclaration = CreateStyledButton("Ouvrir une Déclaration", 0, 90);
            btnOpenDeclaration.Width = 600;
            btnOpenDeclaration.Click += (s, e) => { OpenMainForm(false); };
            buttonPanel.Controls.Add(btnOpenDeclaration);

            Button btnGuide = CreateStyledButton("Consulter le Guide", 0, 180);
            btnGuide.Width = 600;
            btnGuide.Click += (s, e) => 
            { 
                if (mainForm == null) mainForm = new Form1();
                mainForm.OpenGuide(); 
            };
            buttonPanel.Controls.Add(btnGuide);

            // Footer
            Label lblFooter = new Label
            {
                Text = "© 2024 DeclarationEmployeurApp - Tous droits réservés",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(128, 128, 128),
                AutoSize = true
            };
            lblFooter.Location = new Point((screenWidth - lblFooter.PreferredWidth) / 2, screenHeight - 50);
            this.Controls.Add(lblFooter);
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 16F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                Size = new Size(250, 70),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Ajouter des effets de survol
            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(0, 102, 184);
                btn.FlatAppearance.BorderColor = Color.White;
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.FromArgb(0, 122, 204);
                btn.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
            };

            return btn;
        }

        private void OpenMainForm(bool isNew)
        {
            if (mainForm == null || mainForm.IsDisposed)
            {
                mainForm = new Form1();
            }

            if (isNew)
            {
                mainForm.ClearAllFields();
            }
            else
            {
                mainForm.OpenExistingDeclaration();
            }

            mainForm.Show();
            this.Hide();
        }
    }
} 