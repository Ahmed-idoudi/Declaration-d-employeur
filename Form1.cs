using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DeclarationEmployeurApp
{
    public partial class Form1 : Form
    {
        private DataTable declarationsTable;
        private string declarationsFilePath;
        private Label lblSociete;
        private ListBox lstSocietes;
        private Button btnAjouterSociete;
        private Button btnViewSalaires;
        private Button btnViewRetenues;
        private Button btnDeleteSociete;

        public Form1()
        {
            InitializeComponent();
            
            // Configuration plein écran
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            // Ajouter bouton de fermeture
            Button btnClose = CreateCloseButton();
            this.Controls.Add(btnClose);

            InitializeDeclarationsTable();
            LoadSavedDeclarations();
            
            // Set default value for cmbActe
            cmbActe.SelectedIndex = 0;

            // Modifier le menu
            ModifyMainMenu();

            // Initialiser les nouveaux contrôles
            InitializeSocieteControls();

            // Initialiser les boutons de visualisation
            InitializeViewButtons();

            // Initialiser la validation des champs
            InitializeValidation();

            // Cacher groupBox2 initialement
            groupBox2.Visible = false;

            // Adapter le design pour le plein écran
            AdaptLayoutForFullscreen();
        }

        private void InitializeValidation()
        {
            // Ajouter le placeholder pour le champ Matricule
            string placeholderText = "MF-CléMF-Cat-Numsecondaire";
            txtMat.Text = placeholderText;
            txtMat.ForeColor = System.Drawing.Color.Gray;

            // Ajouter la validation de longueur pour le matricule (maximum 13 caractères)
            txtMat.MaxLength = 13;
            txtMat.TextChanged += (s, e) =>
            {
                if (txtMat.Text.Length > 13)
                {
                    txtMat.Text = txtMat.Text.Substring(0, 13);
                    txtMat.SelectionStart = txtMat.Text.Length;
                }
            };

            txtMat.Enter += (s, e) =>
            {
                if (txtMat.Text == placeholderText)
                {
                    txtMat.Text = "";
                    txtMat.ForeColor = System.Drawing.Color.Black;
                }
            };

            txtMat.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtMat.Text))
                {
                    txtMat.Text = placeholderText;
                    txtMat.ForeColor = System.Drawing.Color.Gray;
                }
            };

            // Ajouter la validation pour le champ Numéro
            txtNum.MaxLength = 4;
            txtNum.TextChanged += (s, e) =>
            {
                if (txtNum.Text.Length > 4)
                {
                    txtNum.Text = txtNum.Text.Substring(0, 4);
                    txtNum.SelectionStart = txtNum.Text.Length;
                }
            };

            // Ajouter la validation pour le champ Code Postal
            txtPo.MaxLength = 4;
            txtPo.TextChanged += (s, e) =>
            {
                if (txtPo.Text.Length > 4)
                {
                    txtPo.Text = txtPo.Text.Substring(0, 4);
                    txtPo.SelectionStart = txtPo.Text.Length;
                }
            };
        }

        private Button CreateCloseButton()
        {
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
            
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "Voulez-vous vraiment quitter l'application ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            
            // Ajouter des effets de survol
            btnClose.MouseEnter += (s, e) =>
            {
                btnClose.BackColor = Color.FromArgb(232, 17, 35);  // Rouge Microsoft
            };
            btnClose.MouseLeave += (s, e) =>
            {
                btnClose.BackColor = Color.FromArgb(0, 122, 204);  // Bleu normal
            };

            return btnClose;
        }

        private void InitializeSocieteControls()
        {
            // Label Société
            lblSociete = new Label
            {
                Text = "Sociétés",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true
            };
            this.Controls.Add(lblSociete);

            // ListBox Sociétés
            lstSocietes = new ListBox
            {
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.One
            };
            lstSocietes.SelectedIndexChanged += LstSocietes_SelectedIndexChanged;
            this.Controls.Add(lstSocietes);

            // Bouton Ajouter Société
            btnAjouterSociete = new Button
            {
                Text = "Nouvelle Société",
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAjouterSociete.Click += BtnAjouterSociete_Click;
            this.Controls.Add(btnAjouterSociete);

            // Bouton Supprimer Société
            btnDeleteSociete = new Button
            {
                Text = "🗑️ Supprimer",
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69), // Rouge
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDeleteSociete.FlatAppearance.BorderSize = 0;
            btnDeleteSociete.Click += BtnDeleteSociete_Click;
            this.Controls.Add(btnDeleteSociete);

            // Remplir la liste des sociétés
            RefreshSocietesList();
        }

        private void RefreshSocietesList()
        {
            lstSocietes.Items.Clear();
            var societes = declarationsTable.AsEnumerable()
                .Select(row => row["NomSociete"].ToString())
                .Distinct()
                .OrderBy(nom => nom);
            foreach (var societe in societes)
            {
                lstSocietes.Items.Add(societe);
            }
        }

        private void LstSocietes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteSociete.Enabled = lstSocietes.SelectedItem != null;
            
            if (lstSocietes.SelectedItem != null)
            {
                string selectedSociete = lstSocietes.SelectedItem.ToString();
                var declaration = declarationsTable.AsEnumerable()
                    .FirstOrDefault(row => row["NomSociete"].ToString() == selectedSociete);
                
                if (declaration != null)
                {
                    LoadDeclarationData(declaration);
                }
            }
        }

        private void BtnAjouterSociete_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            ClearAllFields();
            txtNomSociete.Focus();
        }

        private void BtnDeleteSociete_Click(object sender, EventArgs e)
        {
            if (lstSocietes.SelectedItem != null)
            {
                string selectedSociete = lstSocietes.SelectedItem.ToString();
                
                // Demander confirmation
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la société '{selectedSociete}' ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Supprimer de la base de données
                        DatabaseManager.Instance.DeleteSociete(selectedSociete);

                        // Supprimer du DataTable
                        var rowsToDelete = declarationsTable.AsEnumerable()
                            .Where(row => row["NomSociete"].ToString() == selectedSociete)
                            .ToList();

                        foreach (var row in rowsToDelete)
                        {
                            declarationsTable.Rows.Remove(row);
                        }

                        // Sauvegarder dans le fichier XML
                        SaveDeclarationsToFile();

                        // Rafraîchir la liste
                        RefreshSocietesList();
                        ClearAllFields();
                        
                        MessageBox.Show("La société a été supprimée avec succès.", "Succès", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", 
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void InitializeViewButtons()
        {
            // Bouton pour visualiser le fichier salaires
            btnViewSalaires = new Button
            {
                Text = "👁 Voir Tableau",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Default
            };
            btnViewSalaires.FlatAppearance.BorderSize = 0;
            btnViewSalaires.Click += BtnViewSalaires_Click;
            this.Controls.Add(btnViewSalaires);

            // Bouton pour visualiser le fichier retenues
            btnViewRetenues = new Button
            {
                Text = "👁 Voir Tableau",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Default
            };
            btnViewRetenues.FlatAppearance.BorderSize = 0;
            btnViewRetenues.Click += BtnViewRetenues_Click;
            this.Controls.Add(btnViewRetenues);
        }

        private void BtnViewSalaires_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtFichierSalaires.Text) && File.Exists(txtFichierSalaires.Text))
                {
                    DataTable dtSalaires = ReadExcelFile(txtFichierSalaires.Text);
                    var viewerForm = new ExcelViewerForm(dtSalaires, "Fichier Salaires");
                    viewerForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du fichier salaires : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewRetenues_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtFichierRetenues.Text) && File.Exists(txtFichierRetenues.Text))
                {
                    DataTable dtRetenues = ReadExcelFile(txtFichierRetenues.Text);
                    var viewerForm = new ExcelViewerForm(dtRetenues, "Fichier Retenues");
                    viewerForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du fichier retenues : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdaptLayoutForFullscreen()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int padding = 20;
            int menuHeight = menuStrip1.Height;

            // Section Sociétés (panneau gauche)
            int leftPanelWidth = 300;
            lblSociete.Location = new Point(padding, menuHeight + padding);
            
            lstSocietes.Location = new Point(padding, lblSociete.Bottom + 10);
            lstSocietes.Size = new Size(leftPanelWidth, screenHeight - menuHeight - 200); // Ajuster la hauteur
            lstSocietes.BackColor = Color.White;
            lstSocietes.BorderStyle = BorderStyle.None;

            // Boutons sous la liste
            btnAjouterSociete.Location = new Point(padding, lstSocietes.Bottom + 10);
            btnAjouterSociete.Size = new Size((leftPanelWidth - padding) / 2, 40);

            btnDeleteSociete.Location = new Point(btnAjouterSociete.Right + padding, lstSocietes.Bottom + 10);
            btnDeleteSociete.Size = new Size((leftPanelWidth - padding) / 2, 40);

            // Panneau principal (droite)
            int mainPanelX = leftPanelWidth + (2 * padding);
            int mainPanelWidth = screenWidth - mainPanelX - padding;
            
            // GroupBox1 (Annexes)
            groupBox1.Location = new Point(mainPanelX, menuHeight + padding);
            groupBox1.Size = new Size((mainPanelWidth - padding) / 2, 300);
            groupBox1.BackColor = Color.White;

            // GroupBox2 (Informations société) - Augmenter la hauteur
            groupBox2.Location = new Point(groupBox1.Right + padding, menuHeight + padding);
            groupBox2.Size = new Size((mainPanelWidth - padding) / 2, 300);
            groupBox2.BackColor = Color.White;

            // Section Exercice et Acte
            int bottomSectionY = groupBox1.Bottom + padding;
            int controlWidth = (mainPanelWidth - (3 * padding)) / 2;

            // Labels
            label2.Location = new Point(mainPanelX, bottomSectionY);
            label2.Font = new Font("Segoe UI", 10F);
            
            label3.Location = new Point(mainPanelX + controlWidth + padding, bottomSectionY);
            label3.Font = new Font("Segoe UI", 10F);

            // Controls
            txtExercice.Location = new Point(mainPanelX, label2.Bottom + 5);
            txtExercice.Size = new Size(controlWidth, 30);
            txtExercice.Font = new Font("Segoe UI", 10F);

            cmbActe.Location = new Point(mainPanelX + controlWidth + padding, label3.Bottom + 5);
            cmbActe.Size = new Size(controlWidth, 30);
            cmbActe.Font = new Font("Segoe UI", 10F);

            // Section Fichiers
            int filesSectionY = txtExercice.Bottom + (2 * padding);

            // Labels fichiers
            label4.Location = new Point(mainPanelX, filesSectionY);
            label4.Font = new Font("Segoe UI", 10F);
            
            label5.Location = new Point(mainPanelX, filesSectionY + 60);
            label5.Font = new Font("Segoe UI", 10F);

            // Contrôles fichiers
            int fileControlWidth = mainPanelWidth - 220; // Réduire la largeur pour faire place aux boutons
            txtFichierSalaires.Location = new Point(mainPanelX, label4.Bottom + 5);
            txtFichierSalaires.Size = new Size(fileControlWidth, 30);
            txtFichierSalaires.Font = new Font("Segoe UI", 10F);

            txtFichierRetenues.Location = new Point(mainPanelX, label5.Bottom + 5);
            txtFichierRetenues.Size = new Size(fileControlWidth, 30);
            txtFichierRetenues.Font = new Font("Segoe UI", 10F);

            // Boutons Parcourir
            btnParcourirSalaires.Location = new Point(txtFichierSalaires.Right + padding, txtFichierSalaires.Top);
            btnParcourirSalaires.Size = new Size(80, 30);
            btnParcourirSalaires.FlatStyle = FlatStyle.Flat;
            btnParcourirSalaires.BackColor = Color.FromArgb(0, 122, 204);
            btnParcourirSalaires.ForeColor = Color.White;
            btnParcourirSalaires.Font = new Font("Segoe UI", 9F);

            btnParcourirRetenues.Location = new Point(txtFichierRetenues.Right + padding, txtFichierRetenues.Top);
            btnParcourirRetenues.Size = new Size(80, 30);
            btnParcourirRetenues.FlatStyle = FlatStyle.Flat;
            btnParcourirRetenues.BackColor = Color.FromArgb(0, 122, 204);
            btnParcourirRetenues.ForeColor = Color.White;
            btnParcourirRetenues.Font = new Font("Segoe UI", 9F);

            // Boutons de visualisation
            btnViewSalaires.Location = new Point(btnParcourirSalaires.Right + padding, txtFichierSalaires.Top);
            btnViewSalaires.Size = new Size(100, 30);

            btnViewRetenues.Location = new Point(btnParcourirRetenues.Right + padding, txtFichierRetenues.Top);
            btnViewRetenues.Size = new Size(100, 30);

            // Bouton Générer
            btnGenerer.Location = new Point(mainPanelX + (mainPanelWidth - 300) / 2, txtFichierRetenues.Bottom + (2 * padding));
            btnGenerer.Size = new Size(300, 50);
            btnGenerer.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGenerer.FlatStyle = FlatStyle.Flat;
            btnGenerer.BackColor = Color.FromArgb(0, 122, 204);
            btnGenerer.ForeColor = Color.White;

            // Ajuster les contrôles dans groupBox2
            if (groupBox2.Visible)
            {
                int labelWidth = 120;
                int margin = 20;
                int textBoxWidth = groupBox2.Width - labelWidth - (3 * margin);
                int rowHeight = 35;  // Augmenter la hauteur des lignes
                int rowSpacing = 15;  // Augmenter l'espacement
                int startY = 40;  // Augmenter l'espace en haut

                // Ajuster les champs principaux avec plus d'espace
                AdjustControlPair(txtNomSociete, "lblNomSociete", labelWidth, textBoxWidth, margin, startY);
                AdjustControlPair(txtMat, "lblMat", labelWidth, textBoxWidth, margin, startY + (rowHeight + rowSpacing));
                //AdjustControlPair(txtMf, "lblMf", labelWidth, textBoxWidth, margin, startY + 2 * (rowHeight + rowSpacing));
                AdjustControlPair(txtActiv, "lblActiv", labelWidth, textBoxWidth, margin, startY + 3 * (rowHeight + rowSpacing));
                AdjustControlPair(txtAdr, "lblAdr", labelWidth, textBoxWidth, margin, startY + 4 * (rowHeight + rowSpacing));
                AdjustControlPair(txtRue, "lblRue", labelWidth, textBoxWidth, margin, startY + 5 * (rowHeight + rowSpacing));

                // Ajuster spécialement Numéro et Code Postal sur la même ligne
                int lastRowY = startY + 6 * (rowHeight + rowSpacing);
                
                // Label et TextBox pour Numéro
                var lblNum = groupBox2.Controls.Find("lblNum", true).FirstOrDefault();
                if (lblNum != null)
                {
                    lblNum.Location = new Point(margin, lastRowY);
                    lblNum.Size = new Size(labelWidth, 25);
                    lblNum.Font = new Font("Segoe UI", 10F);
                }

                // Ajuster la taille du champ Numéro
                int smallFieldWidth = (textBoxWidth - margin) / 2;
                txtNum.Location = new Point(margin + labelWidth, lastRowY);
                txtNum.Size = new Size(smallFieldWidth, 30);
                txtNum.Font = new Font("Segoe UI", 10F);

                // Label et TextBox pour Code Postal
                var lblPo = groupBox2.Controls.Find("lblPo", true).FirstOrDefault();
                if (lblPo != null)
                {
                    lblPo.Location = new Point(margin + labelWidth + smallFieldWidth + margin, lastRowY);
                    lblPo.Size = new Size(labelWidth, 25);
                    lblPo.Font = new Font("Segoe UI", 10F);
                    lblPo.Text = "Code Postal:";
                }

                // Ajuster la taille du champ Code Postal
                txtPo.Location = new Point(margin + labelWidth + smallFieldWidth + margin + labelWidth, lastRowY);
                txtPo.Size = new Size(smallFieldWidth, 30);
                txtPo.Font = new Font("Segoe UI", 10F);
            }

            // Appliquer des styles cohérents aux groupBox
            ApplyGroupBoxStyle(groupBox1);
            ApplyGroupBoxStyle(groupBox2);
        }

        private void AdjustControlPair(Control textBox, string labelName, int labelWidth, int textBoxWidth, int margin, int top)
        {
            var label = groupBox2.Controls.Find(labelName, true).FirstOrDefault();
            if (label != null)
            {
                label.Location = new Point(margin, top);
                label.Size = new Size(labelWidth, 20);
                label.Font = new Font("Segoe UI", 10F);
            }

            textBox.Location = new Point(margin + labelWidth, top);
            textBox.Size = new Size(textBoxWidth, 25);
            textBox.Font = new Font("Segoe UI", 10F);
        }

        private void ApplyGroupBoxStyle(GroupBox groupBox)
        {
            groupBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox.ForeColor = Color.FromArgb(0, 122, 204);
            groupBox.BackColor = Color.White;
            groupBox.Padding = new Padding(10);

            // Appliquer un style aux labels dans le groupBox
            foreach (Control control in groupBox.Controls)
            {
                if (control is Label)
                {
                    control.ForeColor = Color.FromArgb(0, 122, 204);
                    control.Font = new Font("Segoe UI", 10F);
                }
                else if (control is TextBox)
                {
                    control.Font = new Font("Segoe UI", 10F);
                    ((TextBox)control).BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        private void ModifyMainMenu()
        {
            // Supprimer l'ancien menu "Déclaration"
            menuStrip1.Items.Remove(declarationToolStripMenuItem);

            // Créer les nouveaux éléments du menu avec style
            ToolStripMenuItem accueilMenuItem = CreateStyledMenuItem("Accueil");
            accueilMenuItem.Click += (s, e) => { ShowAccueilForm(); };

            ToolStripMenuItem ouvrirMenuItem = CreateStyledMenuItem("Ouvrir Déclaration");
            ouvrirMenuItem.Click += ouvrirToolStripMenuItem_Click;

            ToolStripMenuItem ajouterMenuItem = CreateStyledMenuItem("Ajouter Société");
            ajouterMenuItem.Click += nouveauToolStripMenuItem_Click;

            // Ajouter les nouveaux éléments au début du menu
            menuStrip1.Items.Insert(0, accueilMenuItem);
            menuStrip1.Items.Insert(1, ouvrirMenuItem);
            menuStrip1.Items.Insert(2, ajouterMenuItem);

            // Ajuster le style du menu
            menuStrip1.BackColor = Color.FromArgb(0, 122, 204);
            menuStrip1.ForeColor = Color.White;
            menuStrip1.Font = new Font("Segoe UI", 10F);
            menuStrip1.Padding = new Padding(10, 5, 0, 5);
        }

        private ToolStripMenuItem CreateStyledMenuItem(string text)
        {
            return new ToolStripMenuItem
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Padding = new Padding(10, 5, 10, 5)
            };
        }

        private void ShowAccueilForm()
        {
            AccueilForm accueilForm = new AccueilForm();
            accueilForm.Show();
            this.Hide();
        }

        public void OpenExistingDeclaration()
        {
            ouvrirToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        public void OpenGuide()
        {
            guide1ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void InitializeDeclarationsTable()
        {
            declarationsTable = new DataTable("Declarations");
            declarationsTable.Columns.Add("ID", typeof(int));
            declarationsTable.Columns.Add("NomSociete", typeof(string));
            declarationsTable.Columns.Add("Matricule", typeof(string));
            declarationsTable.Columns.Add("MF", typeof(string));
            declarationsTable.Columns.Add("Exercice", typeof(string));
            declarationsTable.Columns.Add("DateCreation", typeof(DateTime));
            declarationsTable.Columns.Add("Activite", typeof(string));
            declarationsTable.Columns.Add("Adresse", typeof(string));
            declarationsTable.Columns.Add("Rue", typeof(string));
            declarationsTable.Columns.Add("Numero", typeof(string));
            declarationsTable.Columns.Add("CodePostal", typeof(string));
            declarationsTable.Columns.Add("Acte", typeof(string));

            // Set the declarations file path
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeclarationEmployeur");
            Directory.CreateDirectory(appDataPath);
            declarationsFilePath = Path.Combine(appDataPath, "declarations.xml");
        }

        private void LoadSavedDeclarations()
        {
            try
            {
                // Load from SQLite
                declarationsTable = DatabaseManager.Instance.LoadDeclarations();

                // If SQLite is empty, try loading from XML for backward compatibility
                if (declarationsTable.Rows.Count == 0 && File.Exists(declarationsFilePath))
                {
                    DataSet ds = new DataSet("DeclarationsEmployeur");
                    ds.ReadXml(declarationsFilePath);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Columns.Count > 0)
                    {
                        declarationsTable = ds.Tables[0];
                        
                        // Migrate data from XML to SQLite
                        foreach (DataRow row in declarationsTable.Rows)
                        {
                            DatabaseManager.Instance.SaveDeclaration(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des déclarations: {ex.Message}");
                
                // Initialize empty table if loading fails
                InitializeDeclarationsTable();
            }
        }

        private void SaveDeclarationsToFile()
        {
            try
            {
                // Create a new DataSet and add a copy of the table
                DataSet ds = new DataSet("DeclarationsEmployeur");
                
                // Create a copy of the table with the same structure
                DataTable tableCopy = declarationsTable.Clone();
                
                // Copy the data
                foreach (DataRow row in declarationsTable.Rows)
                {
                    tableCopy.ImportRow(row);
                }
                
                ds.Tables.Add(tableCopy);
                
                // Save with schema
                ds.WriteXml(declarationsFilePath, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde des déclarations: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nouveauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Afficher groupBox2 et vider les champs
            groupBox2.Visible = true;
            ClearAllFields();
            txtNomSociete.Focus();
        }

        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create and show the declarations list form
            using (var declarationsForm = new DeclarationsListForm(declarationsTable))
            {
                if (declarationsForm.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected declaration data
                    DataRow selectedRow = declarationsForm.SelectedDeclaration;
                    if (selectedRow != null)
                    {
                        LoadDeclarationData(selectedRow);
                    }
                }
            }
        }

        private void guide1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExcelGuide("Exemple fichier des salaires.xlsx");
        }

        private void guide2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExcelGuide("Exemple fichier des Retenues.xlsx");
        }

        private void guide3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExcelGuide("Type de Retenue 2023.xlsx");
        }

        private void guidePdf1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPdfGuide("CDC2019.pdf");
        }

        private void guidePdf2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPdfGuide("CDC2020.pdf");
        }

        private void guidePdf3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPdfGuide("CDC2021-2022.pdf");
        }

        private void guidePdf4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPdfGuide("CDC2023-2024.pdf");
        }

        private void OpenPdfGuide(string fileName)
        {
            try
            {
                string guidePath = System.IO.Path.Combine(Application.StartupPath, "cdc", fileName);
                if (System.IO.File.Exists(guidePath))
                {
                    Process.Start(guidePath);
                }
                else
                {
                    MessageBox.Show($"Le fichier guide {fileName} n'existe pas dans le dossier cdc.", "Erreur", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du guide PDF: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenExcelGuide(string fileName)
        {
            try
            {
                string guidePath = System.IO.Path.Combine(Application.StartupPath, "cdc", fileName);
                if (System.IO.File.Exists(guidePath))
                {
                    Process.Start(guidePath);
                }
                else
                {
                    MessageBox.Show($"Le fichier guide {fileName} n'existe pas dans le dossier cdc.", "Erreur", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du guide: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveDeclaration()
        {
            try
            {
                DataRow newRow = declarationsTable.NewRow();
                newRow["ID"] = declarationsTable.Rows.Count + 1;
                newRow["NomSociete"] = txtNomSociete.Text;
                newRow["Matricule"] = txtMat.Text;
                newRow["MF"] = "";  // Champ vide car supprimé
                newRow["Exercice"] = txtExercice.Text;
                newRow["DateCreation"] = DateTime.Now;
                newRow["Activite"] = txtActiv.Text;
                newRow["Adresse"] = txtAdr.Text;
                newRow["Rue"] = txtRue.Text;
                newRow["Numero"] = txtNum.Text;
                newRow["CodePostal"] = txtPo.Text;
                newRow["Acte"] = cmbActe.Text;

                // Save to DataTable
                declarationsTable.Rows.Add(newRow);

                // Save to SQLite
                DatabaseManager.Instance.SaveDeclaration(newRow);

                // Save to XML for backup
                SaveDeclarationsToFile();

                // Refresh the list of companies
                RefreshSocietesList();

                // Hide groupBox2 after saving
                groupBox2.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde de la déclaration: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDeclarationData(DataRow row)
        {
            try
            {
                txtNomSociete.Text = row["NomSociete"].ToString();
                txtMat.Text = row["Matricule"].ToString();
                txtExercice.Text = row["Exercice"].ToString();
                txtActiv.Text = row["Activite"].ToString();
                txtAdr.Text = row["Adresse"].ToString();
                txtRue.Text = row["Rue"].ToString();
                txtNum.Text = row["Numero"].ToString();
                txtPo.Text = row["CodePostal"].ToString();
                cmbActe.Text = row["Acte"].ToString();

                // Show groupBox2 when loading data
                groupBox2.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la déclaration: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearAllFields()
        {
            txtNomSociete.Text = "";
            txtMat.Text = "";
            txtActiv.Text = "";
            txtAdr.Text = "";
            txtRue.Text = "";
            txtNum.Text = "";
            txtPo.Text = "";
            txtExercice.Text = "";
            cmbActe.SelectedIndex = -1;
            txtFichierSalaires.Text = "";
            txtFichierRetenues.Text = "";
            
            // Reset checkboxes
            chkDeclarationPrincipale.Checked = true;
            chkAnnexe1.Checked = false;
            chkAnnexe2.Checked = false;
            chkAnnexe3.Checked = false;
            chkAnnexe4.Checked = false;
            chkAnnexe5.Checked = false;
            chkAnnexe6.Checked = false;
            chkAnnexe7.Checked = false;

            // Hide groupBox2 when clearing fields
            if (!groupBox2.Visible)
            {
                groupBox2.Visible = false;
            }

            // Désactiver les boutons de visualisation
            if (btnViewSalaires != null)
            {
                btnViewSalaires.Enabled = false;
                btnViewSalaires.Cursor = Cursors.Default;
                btnViewSalaires.BackColor = Color.Gray;
            }
            if (btnViewRetenues != null)
            {
                btnViewRetenues.Enabled = false;
                btnViewRetenues.Cursor = Cursors.Default;
                btnViewRetenues.BackColor = Color.Gray;
            }
        }

        private void btnGenerer_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation des entrées
                if (string.IsNullOrWhiteSpace(txtNomSociete.Text) ||
                    string.IsNullOrWhiteSpace(txtExercice.Text) ||
                    string.IsNullOrWhiteSpace(cmbActe.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
                    return;
                }

                // Validation de l'exercice
                if (!int.TryParse(txtExercice.Text, out int exercice) || exercice < 2019 || exercice > 2024)
                {
                    MessageBox.Show("L'exercice doit être compris entre 2019 et 2024.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFichierSalaires.Text) ||
                    !File.Exists(txtFichierSalaires.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un fichier salaires valide.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFichierRetenues.Text) ||
                    !File.Exists(txtFichierRetenues.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un fichier retenues valide.");
                    return;
                }

                // Création du dossier de sortie
                string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "DeclarationsEmployeur");
                Directory.CreateDirectory(outputDir);

                // Lecture des fichiers Excel
                DataTable dtSalaires = ReadExcelFile(txtFichierSalaires.Text);
                DataTable dtRetenues = ReadExcelFile(txtFichierRetenues.Text);

                // Validation des colonnes avec gestion flexible des noms
                ValidateDataTableColumns(dtSalaires,
                    new[] { "Type Identifiant", "Identifiant", "Nom & Prénom", "Revenu Imposable", "Retenues Opérées" });

                ValidateDataTableColumns(dtRetenues,
                    new[] { "Type Identifiant", "Identifiant", "Base", "Retenue" });

                // Génération des fichiers sélectionnés
                if (chkDeclarationPrincipale.Checked)
                {
                    GenerateDECEMP(dtSalaires, dtRetenues, outputDir);
                }

                if (chkAnnexe1.Checked)
                {
                    GenerateANXEMP(1, dtSalaires, outputDir);
                }

                if (chkAnnexe2.Checked)
                {
                    GenerateANXEMP(2, dtRetenues, outputDir);
                }

                if (chkAnnexe3.Checked)
                {
                    GenerateANXEMP(3, dtRetenues, outputDir);
                }

                if (chkAnnexe4.Checked)
                {
                    GenerateANXEMP(4, dtRetenues, outputDir);
                }

                if (chkAnnexe5.Checked)
                {
                    GenerateANXEMP(5, dtRetenues, outputDir);
                }

                if (chkAnnexe6.Checked)
                {
                    GenerateANXEMP(6, dtRetenues, outputDir);
                }

                if (chkAnnexe7.Checked)
                {
                    GenerateANXEMP(7, dtRetenues, outputDir);
                }

                // Save the declaration data
                SaveDeclaration();

                MessageBox.Show($"Génération terminée avec succès!\nFichiers enregistrés dans : {outputDir}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération : {GetFullErrorMessage(ex)}");
            }
        }

        private string GetFullErrorMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return $"{ex.Message}\n\nDétails : {ex.InnerException.Message}";
            }
            return ex.Message;
        }

        private DataTable ReadExcelFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            FilterColumn = (rowReader, columnIndex) =>
                            {
                                var header = rowReader[columnIndex]?.ToString()?
                                    .Trim()
                                    .Replace("\u00A0", " ")
                                    .Replace("\uFEFF", "");
                                return !string.IsNullOrEmpty(header);
                            }
                        }
                    });

                    if (result.Tables.Count == 0)
                        throw new Exception("Aucune feuille trouvée dans le fichier Excel");

                    // Nettoyage des noms de colonnes
                    foreach (DataColumn column in result.Tables[0].Columns)
                    {
                        column.ColumnName = column.ColumnName.Trim()
                            .Replace("\u00A0", " ")
                            .Replace("\uFEFF", "");
                    }

                    return result.Tables[0];
                }
            }
        }

        private void ValidateDataTableColumns(DataTable dt, string[] requiredColumns)
        {
            var missingColumns = new List<string>();
            var availableColumns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            foreach (var requiredCol in requiredColumns)
            {
                if (!availableColumns.Any(c => NormalizeColumnName(c).Contains(NormalizeColumnName(requiredCol))))
                {
                    missingColumns.Add(requiredCol);
                }
            }

            if (missingColumns.Any())
            {
                throw new Exception($"Colonnes requises manquantes : {string.Join(", ", missingColumns)}\n" +
                                  $"Colonnes disponibles : {string.Join(", ", availableColumns)}");
            }
        }

        private string NormalizeColumnName(string columnName)
        {
            return columnName.Split('(')[0]
                           .Trim()
                           .ToLower()
                           .Replace("/", "")
                           .Replace("\\", "")
                           .Replace(" ", "");
        }

        private string GetColumnValue(DataRow row, string columnName)
        {
            var matchingColumn = row.Table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => NormalizeColumnName(c.ColumnName) == NormalizeColumnName(columnName));

            if (matchingColumn != null)
            {
                return row[matchingColumn.ColumnName]?.ToString() ?? string.Empty;
            }

            throw new Exception($"Colonne '{columnName}' non trouvée. Colonnes disponibles: {string.Join(", ", row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
        }

        private decimal CalculateTotal(DataTable dt, string columnName)
        {
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                string value = GetColumnValue(row, columnName);
                if (decimal.TryParse(value, out decimal numericValue))
                {
                    total += numericValue;
                }
            }
            return total;
        }

        private string FormatOrder(object value, int length)
        {
            decimal number = 0;
            if (value != null && decimal.TryParse(value.ToString(), out number))
            {
                long millimes = (long)(number * 1);
                return millimes.ToString().PadLeft(length, '0');
            }
            return "0".PadLeft(length, '0');
        }

        private string FormatNumber(object value, int length)
        {
            decimal number = 0;
            if (value != null && decimal.TryParse(value.ToString(), out number))
            {
                long millimes = (long)(number * 1000);
                return millimes.ToString().PadLeft(length, '0');
            }
            return "0".PadLeft(length, '0');
        }

        private string FormatText(string text, int length)
        {
            if (string.IsNullOrEmpty(text)) return "".PadRight(length);
            if (text.Length > length) return text.Substring(0, length);
            return text.PadRight(length);
        }

        private string FormatDate(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out DateTime date))
            {
                return date.ToString("ddMMyyyy");
            }
            return "01012019"; // Date par défaut
        }

        private void btnParcourirSalaires_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Tous les fichiers (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFichierSalaires.Text = openFileDialog.FileName;
                btnViewSalaires.Enabled = true;
                btnViewSalaires.Cursor = Cursors.Hand;
                btnViewSalaires.BackColor = Color.FromArgb(0, 122, 204);
            }
        }

        private void btnParcourirRetenues_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Tous les fichiers (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFichierRetenues.Text = openFileDialog.FileName;
                btnViewRetenues.Enabled = true;
                btnViewRetenues.Cursor = Cursors.Hand;
                btnViewRetenues.BackColor = Color.FromArgb(0, 122, 204);
            }
        }

        private void GenerateDECEMP(DataTable dts, DataTable dtr, string outputDir)
        {
            if (txtExercice.Text == "2019") { 
            try
        {
            string fileName = $"DECEMP_{txtExercice.Text.Substring(2)}.txt";
            string filePath = Path.Combine(outputDir, fileName);

                // Initialize annexe flags
                string a1 = "1", a2 = "1", a3 = "1", a4 = "1", a5 = "1", a6 = "1", a7 = "1";

                // Set flags based on checked annexes
                if (chkAnnexe1.Checked) a1 = "0";
                if (chkAnnexe2.Checked) a2 = "0";
                if (chkAnnexe3.Checked) a3 = "0";
                if (chkAnnexe4.Checked) a4 = "0";
                if (chkAnnexe5.Checked) a5 = "0";
                if (chkAnnexe6.Checked) a6 = "0";
                if (chkAnnexe7.Checked) a7 = "0";

                // Get the last rows safely
                DataRow lastSalaryRow = dts.Rows.Count > 0 ? dts.Rows[dts.Rows.Count - 1] : null;
                DataRow lastRetentionRow = dtr.Rows.Count > 0 ? dtr.Rows[dtr.Rows.Count - 1] : null;

                // Calculate totals 
                string totalSalaires = GetColumnValue(lastSalaryRow, "Revenu Imposable");
                string totalRetenues = GetColumnValue(lastSalaryRow, "Retenues Opérées");
                string totalCSS = GetColumnValue(lastSalaryRow, "CSS");
                string totalRetentionsHonoraires = GetColumnValue(lastRetentionRow, "Retenue");
                string totalBaseHonoraires = GetColumnValue(lastRetentionRow, "Base");

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
            {
                // Enregistrement d'en-tête DECEMP00
                    sw.WriteLine("000" + txtMat.Text + txtExercice.Text + a1 + a2 + a3 + a4 + a5 + a6 + a7 + "".PadRight(12));

                    // Write salary information
                    sw.WriteLine("010" + FormatNumber(totalSalaires, 15) + "00000" + FormatNumber(totalRetenues, 15));

                    // Write retentions 20%
                    if (lastSalaryRow != null)
                    {
                        sw.WriteLine("170" + FormatNumber(GetColumnValue(lastSalaryRow, "Retenues 20%"), 15) + "00000" + "000000000000000");
                    }
                    else
                    {
                        sw.WriteLine("170" + "000000000000000" + "00000" + "000000000000000");
                    }

                    // Write CSS
                    sw.WriteLine("300" + "".PadRight(20) + FormatNumber(totalCSS, 15));

                    // Write honoraires information
                    sw.WriteLine("021" + FormatNumber(totalBaseHonoraires, 15) + "01500" + FormatNumber(totalRetentionsHonoraires, 15));

                    // Write other lines with default values
                    sw.WriteLine("023" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("025" + "000000000000000" + "00250" + "000000000000000");
                    sw.WriteLine("030" + "000000000000000" + "00500" + "000000000000000");
                    sw.WriteLine("180" + "000000000000000" + "00250" + "000000000000000");
                    sw.WriteLine("040" + "000000000000000" + "00500" + "000000000000000");
                    sw.WriteLine("260" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("060" + "000000000000000" + "02000" + "000000000000000");
                    sw.WriteLine("071" + "000000000000000" + "02000" + "000000000000000");
                    sw.WriteLine("073" + "000000000000000" + "02000" + "000000000000000");
                    sw.WriteLine("080" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("241" + "000000000000000" + "01000" + "000000000000000");
                    sw.WriteLine("242" + "000000000000000" + "01000" + "000000000000000");
                    sw.WriteLine("091" + "000000000000000" + "02000" + "000000000000000");
                    sw.WriteLine("093" + "000000000000000" + "02000" + "000000000000000");
                    sw.WriteLine("100" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("110" + "000000000000000" + "01000" + "000000000000000");
                    sw.WriteLine("121" + "000000000000000" + "00250" + "000000000000000");
                    sw.WriteLine("122" + "000000000000000" + "00250" + "000000000000000");
                    sw.WriteLine("123" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("131" + "000000000000000" + "00050" + "000000000000000");
                    sw.WriteLine("132" + "000000000000000" + "00150" + "000000000000000");
                    sw.WriteLine("140" + "000000000000000" + "02500" + "000000000000000");
                    sw.WriteLine("150" + "000000000000000" + "10000" + "000000000000000");
                    sw.WriteLine("160" + "000000000000000" + "00000" + "000000000000000");
                    sw.WriteLine("270" + "000000000000000" + "00000" + "000000000000000");
                    sw.WriteLine("200" + "000000000000000" + "00100" + "000000000000000");
                    sw.WriteLine("191" + "000000000000000" + "01000" + "000000000000000");
                    sw.WriteLine("192" + "000000000000000" + "02500" + "000000000000000");
                    sw.WriteLine("051" + "000000000000000" + "01500" + "000000000000000");
                    sw.WriteLine("220" + "000000000000000" + "02500" + "000000000000000");
                    sw.WriteLine("250" + "000000000000000" + "00150" + "000000000000000");
                    sw.WriteLine("280" + "000000000000000" + "02500" + "000000000000000");
                    sw.WriteLine("290" + "000000000000000" + "00300" + "000000000000000");

                    // Write the final line with total retentions
                    decimal totalAllRetentions = Convert.ToDecimal(totalRetenues) + Convert.ToDecimal(totalCSS) + Convert.ToDecimal(totalRetentionsHonoraires);
                    sw.WriteLine("999" + "".PadRight(20)+ FormatNumber(totalAllRetentions, 15));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du fichier DECEMP : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            }
            else if(txtExercice.Text == "2020")
            {
                try
                {
                    string fileName = $"DECEMP_{txtExercice.Text.Substring(2)}.txt";
                    string filePath = Path.Combine(outputDir, fileName);

                    // Initialize annexe flags
                    string a1 = "1", a2 = "1", a3 = "1", a4 = "1", a5 = "1", a6 = "1", a7 = "1";

                    // Set flags based on checked annexes
                    if (chkAnnexe1.Checked) a1 = "0";
                    if (chkAnnexe2.Checked) a2 = "0";
                    if (chkAnnexe3.Checked) a3 = "0";
                    if (chkAnnexe4.Checked) a4 = "0";
                    if (chkAnnexe5.Checked) a5 = "0";
                    if (chkAnnexe6.Checked) a6 = "0";
                    if (chkAnnexe7.Checked) a7 = "0";

                    // Get the last rows safely
                    DataRow lastSalaryRow = dts.Rows.Count > 0 ? dts.Rows[dts.Rows.Count - 1] : null;
                    DataRow lastRetentionRow = dtr.Rows.Count > 0 ? dtr.Rows[dtr.Rows.Count - 1] : null;

                    // Calculate totals 
                    string totalSalaires = GetColumnValue(lastSalaryRow, "Revenu Imposable");
                    string totalRetenues = GetColumnValue(lastSalaryRow, "Retenues Opérées");
                    string totalCSS = GetColumnValue(lastSalaryRow, "CSS");
                    string totalCPE2020 = GetColumnValue(lastSalaryRow, "CPE2020");
                    string totalRetentionsHonoraires = GetColumnValue(lastRetentionRow, "Retenue");
                    string totalBaseHonoraires = GetColumnValue(lastRetentionRow, "Base");

                    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
                    {
                        // Enregistrement d'en-tête DECEMP00
                        sw.WriteLine("000" + txtMat.Text + txtExercice.Text + a1 + a2 + a3 + a4 + a5 + a6 + a7 + "".PadRight(12));

                        // Write salary information
                        sw.WriteLine("010" + FormatNumber(totalSalaires, 15) + "00000" + FormatNumber(totalRetenues, 15));

                        // Write retentions 20%
                        if (lastSalaryRow != null)
                        {
                            sw.WriteLine("170" + FormatNumber(GetColumnValue(lastSalaryRow, "Retenues 20%"), 15) + "00000" + "000000000000000");
                        }
                        else
                        {
                            sw.WriteLine("170" + "000000000000000" + "00000" + "000000000000000");
                        }

                        // Write CSS
                        sw.WriteLine("300" + "".PadRight(20) + FormatNumber(totalCSS, 15));
                        sw.WriteLine("400" + "".PadRight(20) + FormatNumber(totalCPE2020, 15));
                        // Write honoraires information
                        sw.WriteLine("021" + FormatNumber(totalBaseHonoraires, 15) + "01500" + FormatNumber(totalRetentionsHonoraires, 15));

                        // Write other lines with default values
                        sw.WriteLine("023" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("025" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("030" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("180" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("040" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("260" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("060" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("071" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("073" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("080" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("241" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("242" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("091" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("093" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("100" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("110" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("121" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("122" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("123" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("131" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("132" + "000000000000000" + "00150" + "000000000000000");
                        sw.WriteLine("140" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("150" + "000000000000000" + "10000" + "000000000000000");
                        sw.WriteLine("160" + "000000000000000" + "00000" + "000000000000000");
                        sw.WriteLine("270" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("271" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("200" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("191" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("192" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("051" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("220" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("250" + "000000000000000" + "00150" + "000000000000000");
                        sw.WriteLine("280" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("290" + "000000000000000" + "00300" + "000000000000000");

                        // Write the final line with total retentions
                        decimal totalAllRetentions = Convert.ToDecimal(totalRetenues) + Convert.ToDecimal(totalCSS) + Convert.ToDecimal(totalRetentionsHonoraires);
                        sw.WriteLine("999" + "".PadRight(20) + FormatNumber(totalAllRetentions, 15));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la génération du fichier DECEMP : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (txtExercice.Text == "2021" || txtExercice.Text == "2022")
            {
                try
                {
                    string fileName = $"DECEMP_{txtExercice.Text.Substring(2)}.txt";
                    string filePath = Path.Combine(outputDir, fileName);

                    // Initialize annexe flags
                    string a1 = "1", a2 = "1", a3 = "1", a4 = "1", a5 = "1", a6 = "1", a7 = "1";

                    // Set flags based on checked annexes
                    if (chkAnnexe1.Checked) a1 = "0";
                    if (chkAnnexe2.Checked) a2 = "0";
                    if (chkAnnexe3.Checked) a3 = "0";
                    if (chkAnnexe4.Checked) a4 = "0";
                    if (chkAnnexe5.Checked) a5 = "0";
                    if (chkAnnexe6.Checked) a6 = "0";
                    if (chkAnnexe7.Checked) a7 = "0";

                    // Get the last rows safely
                    DataRow lastSalaryRow = dts.Rows.Count > 0 ? dts.Rows[dts.Rows.Count - 1] : null;
                    DataRow lastRetentionRow = dtr.Rows.Count > 0 ? dtr.Rows[dtr.Rows.Count - 1] : null;

                    // Calculate totals 
                    string totalSalaires = GetColumnValue(lastSalaryRow, "Revenu Imposable");
                    string totalRetenues = GetColumnValue(lastSalaryRow, "Retenues Opérées");
                    string totalCSS = GetColumnValue(lastSalaryRow, "CSS");
                    string totalCPE2020 = GetColumnValue(lastSalaryRow, "CPE2020");
                    string totalRetentionsHonoraires = GetColumnValue(lastRetentionRow, "Retenue");
                    string totalBaseHonoraires = GetColumnValue(lastRetentionRow, "Base");

                    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
                    {
                        // Enregistrement d'en-tête DECEMP00
                        sw.WriteLine("000" + txtMat.Text + txtExercice.Text + a1 + a2 + a3 + a4 + a5 + a6 + a7 + "".PadRight(12));

                        // Write salary information
                        sw.WriteLine("010" + FormatNumber(totalSalaires, 15) + "00000" + FormatNumber(totalRetenues, 15));

                        // Write retentions 20%
                        if (lastSalaryRow != null)
                        {
                            sw.WriteLine("170" + FormatNumber(GetColumnValue(lastSalaryRow, "Retenues 20%"), 15) + "00000" + "000000000000000");
                        }
                        else
                        {
                            sw.WriteLine("170" + "000000000000000" + "00000" + "000000000000000");
                        }

                        // Write CSS
                        sw.WriteLine("300" + "".PadRight(20) + FormatNumber(totalCSS, 15));
                        // Write honoraires information
                        sw.WriteLine("021" + FormatNumber(totalBaseHonoraires, 15) + "01000" + FormatNumber(totalRetentionsHonoraires, 15));

                        // Write other lines with default values
                        sw.WriteLine("022" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("023" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("024" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("027" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("030" + "000000000000000" + "00300" + "000000000000000");
                        sw.WriteLine("400" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("040" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("260" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("060" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("071" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("072" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("073" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("074" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("241" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("242" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("091" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("092" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("093" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("094" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("100" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("110" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("121" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("122" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("123" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("132" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("133" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("134" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("135" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("140" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("150" + "000000000000000" + "10000" + "000000000000000");
                        sw.WriteLine("160" + "000000000000000" + "00000" + "000000000000000");
                        sw.WriteLine("270" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("271" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("200" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("191" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("192" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("051" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("052" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("220" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("250" + "000000000000000" + "00150" + "000000000000000");
                        sw.WriteLine("251" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("280" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("290" + "000000000000000" + "00300" + "000000000000000");

                        // Write the final line with total retentions
                        decimal totalAllRetentions = Convert.ToDecimal(totalRetenues) + Convert.ToDecimal(totalCSS) + Convert.ToDecimal(totalRetentionsHonoraires);
                        sw.WriteLine("999" + "".PadRight(20) + FormatNumber(totalAllRetentions, 15));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la génération du fichier DECEMP : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (txtExercice.Text == "2023" || txtExercice.Text == "2024")
            {
                try
                {
                    string fileName = $"DECEMP_{txtExercice.Text.Substring(2)}.txt";
                    string filePath = Path.Combine(outputDir, fileName);

                    // Initialize annexe flags
                    string a1 = "1", a2 = "1", a3 = "1", a4 = "1", a5 = "1", a6 = "1", a7 = "1";

                    // Set flags based on checked annexes
                    if (chkAnnexe1.Checked) a1 = "0";
                    if (chkAnnexe2.Checked) a2 = "0";
                    if (chkAnnexe3.Checked) a3 = "0";
                    if (chkAnnexe4.Checked) a4 = "0";
                    if (chkAnnexe5.Checked) a5 = "0";
                    if (chkAnnexe6.Checked) a6 = "0";
                    if (chkAnnexe7.Checked) a7 = "0";

                    // Get the last rows safely
                    DataRow lastSalaryRow = dts.Rows.Count > 0 ? dts.Rows[dts.Rows.Count - 1] : null;
                    DataRow lastRetentionRow = dtr.Rows.Count > 0 ? dtr.Rows[dtr.Rows.Count - 1] : null;

                    // Calculate totals 
                    string totalSalaires = GetColumnValue(lastSalaryRow, "Revenu Imposable");
                    string totalRetenues = GetColumnValue(lastSalaryRow, "Retenues Opérées");
                    string totalCSS = GetColumnValue(lastSalaryRow, "CSS");
                    string totalCPE2020 = GetColumnValue(lastSalaryRow, "CPE2020");
                    string totalRetentionsHonoraires = GetColumnValue(lastRetentionRow, "Retenue");
                    string totalBaseHonoraires = GetColumnValue(lastRetentionRow, "Base");

                    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
                    {
                        // Enregistrement d'en-tête DECEMP00
                        sw.WriteLine("000" + txtMat.Text + txtExercice.Text + a1 + a2 + a3 + a4 + a5 + a6 + a7 + "".PadRight(12));

                        // Write salary information
                        sw.WriteLine("010" + FormatNumber(totalSalaires, 15) + "00000" + FormatNumber(totalRetenues, 15));

                        // Write retentions 20%
                        if (lastSalaryRow != null)
                        {
                            sw.WriteLine("170" + FormatNumber(GetColumnValue(lastSalaryRow, "Retenues 20%"), 15) + "00000" + "000000000000000");
                        }
                        else
                        {
                            sw.WriteLine("170" + "000000000000000" + "00000" + "000000000000000");
                        }

                        // Write CSS
                        sw.WriteLine("300" + "".PadRight(20) + FormatNumber(totalCSS, 15));
                        // Write honoraires information
                        sw.WriteLine("021" + FormatNumber(totalBaseHonoraires, 15) + "01000" + FormatNumber(totalRetentionsHonoraires, 15));

                        // Write other lines with default values
                        sw.WriteLine("022" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("023" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("024" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("027" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("030" + "000000000000000" + "00300" + "000000000000000");
                        sw.WriteLine("400" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("040" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("260" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("060" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("071" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("072" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("073" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("074" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("241" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("242" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("091" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("092" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("093" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("094" + "000000000000000" + "02000" + "000000000000000");
                        sw.WriteLine("100" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("110" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("121" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("122" + "000000000000000" + "00250" + "000000000000000");
                        sw.WriteLine("123" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("132" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("133" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("134" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("135" + "000000000000000" + "00050" + "000000000000000");
                        sw.WriteLine("140" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("150" + "000000000000000" + "10000" + "000000000000000");
                        sw.WriteLine("160" + "000000000000000" + "00000" + "000000000000000");
                        sw.WriteLine("270" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("271" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("200" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("500" + "000000000000000" + "00500" + "000000000000000");
                        sw.WriteLine("191" + "000000000000000" + "01000" + "000000000000000");
                        sw.WriteLine("192" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("051" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("052" + "000000000000000" + "01500" + "000000000000000");
                        sw.WriteLine("220" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("250" + "000000000000000" + "00150" + "000000000000000");
                        sw.WriteLine("251" + "000000000000000" + "00100" + "000000000000000");
                        sw.WriteLine("280" + "000000000000000" + "02500" + "000000000000000");
                        sw.WriteLine("290" + "000000000000000" + "00300" + "000000000000000");

                        // Write the final line with total retentions
                        decimal totalAllRetentions = Convert.ToDecimal(totalRetenues) + Convert.ToDecimal(totalCSS) + Convert.ToDecimal(totalRetentionsHonoraires);
                        sw.WriteLine("999" + "".PadRight(20) + FormatNumber(totalAllRetentions, 15));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la génération du fichier DECEMP : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GenerateANXEMP(int annexeNumber, DataTable dt, string outputDir)
        {
            string fileName = $"ANXEMP_{annexeNumber}_{txtExercice.Text.Substring(2)}_1.txt";
            string filePath = Path.Combine(outputDir, fileName);
            int totalBaseAmount = 0;
            int totalRetentionAmount = 0;
            int totalNetAmount = 0;

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
            {
                int i = 0;
                int limit = dt.Rows.Count - 2;
                //cdc 2019
                if (txtExercice.Text == "2019")
                {
                    if (annexeNumber >= 2 && annexeNumber <= 7)
                {
                    // En-tête commun pour toutes les annexes sauf l'ann 1
                    sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                        FormatText(cmbActe.Text, 1) +
                        FormatOrder(dt.Rows.Count, 6) +
                        FormatText(txtNomSociete.Text, 40) +
                        FormatText(txtActiv.Text, 40) +
                        FormatText(txtAdr.Text, 40) +
                        FormatText(txtRue.Text, 72) +
                        FormatText(txtNum.Text, 4) +
                        FormatText(txtPo.Text, 4) +
                        "".PadRight(171));
                }

                

                if (annexeNumber == 1)
                {
                    // En-tête pour l'annexe 1
                    sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                        FormatText(cmbActe.Text, 1) +
                        FormatOrder(limit, 6) +
                        FormatText(txtNomSociete.Text, 40) +
                        FormatText(txtActiv.Text, 40) +
                        FormatText(txtAdr.Text, 40) +
                        FormatText(txtRue.Text, 72) +
                        FormatText(txtNum.Text, 4) +
                        FormatText(txtPo.Text, 4) +
                        "".PadRight(171));
                    // Code pour l'annexe 1 
                    for (int rowIndex = 0; rowIndex < limit; rowIndex++)
                    {
                        i++;
                        DataRow row = dt.Rows[rowIndex];
                        string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            FormatOrder(i, 6) +
                            "2" +
                            FormatText(GetColumnValue(row, "Identifiant"), 13) +
                            FormatText(GetColumnValue(row, "Nom & Prénom"), 40) +
                            FormatText(GetColumnValue(row, "Emploi Occupé"), 40) +
                            FormatText(GetColumnValue(row, "Adresse"), 120) +
                            GetColumnValue(row, "Situation Familiale") +
                            "0" +
                            GetColumnValue(row, "Nbr Enfants") +
                            FormatDate(GetColumnValue(row, "Date Début")) +
                            FormatDate(GetColumnValue(row, "Date fin")) +
                            GetColumnValue(row, "Durée (jours)") +
                            FormatNumber(GetColumnValue(row, "Revenu Imposable"), 15) +
                            FormatNumber(GetColumnValue(row, "Av. Nature"), 15) +
                            FormatNumber(GetColumnValue(row, "Rev. Brut Imposable"), 15) +
                            FormatNumber(GetColumnValue(row, "Revenus Réinvestis"), 15) +
                            FormatNumber(GetColumnValue(row, "Retenues Opérées"), 15) +
                            FormatNumber(GetColumnValue(row, "Retenues 20%"), 15) +
                            FormatNumber(GetColumnValue(row, "CSS"), 15) +
                            FormatNumber(GetColumnValue(row, "Net Servi"), 15) +
                            "".PadRight(19);
                        sw.WriteLine(enregistrement);
                    }

                    sw.WriteLine("T1" + txtMat.Text + txtExercice.Text +
                        "".PadRight(242) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenu Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Av. Nature"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Rev. Brut Imposable"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenus Réinvestis"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues Opérées"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues 20%"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "CSS"), 15) +
                        FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Net Servi"), 15) +
                        "".PadRight(19));
                }
                else if (annexeNumber == 2)
                {
                    // Code spécifique pour l'annexe 2
                    for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                    {
                        i++;
                        DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                    "0" +
                                    "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +


                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                    FormatNumber(currentNetAmount, 15) + "".PadRight(10);
                        sw.WriteLine(enregistrement);
                    }

                        // ANFIN
                    sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                        "000000000000000" +
                            FormatNumber(totalBaseAmount, 15) +
                        "000000000000000" +
                        "000000000000000" +
                        "000000000000000" +
                        "000000000000000" +
                        "000000000000000" +
                        "000000000000000" +
                        
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) + "".PadRight(10));



                    }
                    else if (annexeNumber == 3)
                    {
                        // Code spécifique pour l'annexe 3
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) +
                                "".PadRight(86);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                        "".PadRight(86));


                    }
                    else if (annexeNumber == 4)
                    {
                        // Code spécifique pour l'annexe 4
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                
                                "000000000000000" +
                                "000000000000000" +

                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15);
                            "".PadRight(4);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) + 
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +

                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15));



                    }
                    else if (annexeNumber == 5)
                    {
                        // Code spécifique pour l'annexe 5
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(currentNetAmount, 15) +
                            "".PadRight(26);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(26));




                    }
                    else if (annexeNumber == 6)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(48);

                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(48));



                    }
                    else if (annexeNumber == 7)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "01" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15)+
                            "".PadRight(114);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            FormatNumber(totalBaseAmount, 15) +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(115));



                    }
                }
                //cdc 2020
                if (txtExercice.Text == "2020")
                {
                    if (annexeNumber >= 2 && annexeNumber <= 7)
                    {
                        // En-tête commun pour toutes les annexes sauf l'ann 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(dt.Rows.Count, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(177));
                    }



                    if (annexeNumber == 1)
                    {
                        // En-tête pour l'annexe 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(limit, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(177));
                        // Code pour l'annexe 1 
                    for (int rowIndex = 0; rowIndex < limit; rowIndex++)
                    {
                        i++;
                        DataRow row = dt.Rows[rowIndex];
                        string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            FormatOrder(i, 6) +
                                "2" +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Nom & Prénom"), 40) +
                                FormatText(GetColumnValue(row, "Emploi Occupé"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                GetColumnValue(row, "Situation Familiale") +
                                "0" +
                                GetColumnValue(row, "Nbr Enfants") +
                                FormatDate(GetColumnValue(row, "Date Début")) +
                                FormatDate(GetColumnValue(row, "Date fin")) +
                                GetColumnValue(row, "Durée (jours)") +
                                FormatNumber(GetColumnValue(row, "Revenu Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Av. Nature"), 15) +
                                FormatNumber(GetColumnValue(row, "Rev. Brut Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Revenus Réinvestis"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues Opérées"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues 20%"), 15) +
                                FormatNumber(GetColumnValue(row, "CSS"), 15) +
                                FormatNumber(GetColumnValue(row, "CPE2020"), 15) +
                                FormatNumber(GetColumnValue(row, "Net Servi"), 15) +
                                "".PadRight(10);
                            sw.WriteLine(enregistrement);
                        }

                        sw.WriteLine("T1" + txtMat.Text + txtExercice.Text +
                            "".PadRight(242) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenu Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Av. Nature"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Rev. Brut Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenus Réinvestis"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues Opérées"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues 20%"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "CSS"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "CPE2020"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Net Servi"), 15) +
                            "".PadRight(10));
                    }
                    else if (annexeNumber == 2)
                    {
                        // Code spécifique pour l'annexe 2
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                            FormatText(GetColumnValue(row, "Identifiant"), 13) +
                            FormatText(GetColumnValue(row, "Raison"), 40) +
                            FormatText(GetColumnValue(row, "Activité"), 40) +
                            FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "000000000000000" +
                            FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "0" +
                                "000000000000000" +
                            FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15);
                            sw.WriteLine(enregistrement);
                    }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                "".PadRight(221) +
                                "000000000000000" +
                                FormatNumber(totalBaseAmount, 15) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" + " " +
                                "000000000000000" +

                                FormatNumber(totalRetentionAmount, 15) +
                                FormatNumber(totalNetAmount, 15) );



                    }
                    else if (annexeNumber == 3)
                    {
                        // Code spécifique pour l'annexe 3
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                
                                "000000000000000" +
                                //FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                
                                "000000000000000" +
                                "000000000000000" +
                               
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) +
                                "".PadRight(92);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                        "".PadRight(92));


                    }
                    else if (annexeNumber == 4)
                    {
                        // Code spécifique pour l'annexe 4
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "0"+
                              
                                "000000000000000" +
                                "000000000000000" +

                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15)+
                            "".PadRight(5);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "0"+
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                        "".PadRight(5));



                    }
                    else if (annexeNumber == 5)
                    {
                        // Code spécifique pour l'annexe 5
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(currentNetAmount, 15) +
                        "".PadRight(32);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(32));




                    }
                    else if (annexeNumber == 6)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(47);

                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(47));



                    }
                    else if (annexeNumber == 7)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "01" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15)+
                            "".PadRight(120);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            FormatNumber(totalBaseAmount, 15) +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(121));



                    }
                }
                //cdc 2021-2022
                if (txtExercice.Text == "2021"|| txtExercice.Text == "2022")
                {
                    if (annexeNumber >= 2 && annexeNumber <= 7)
                    {
                        // En-tête commun pour toutes les annexes sauf l'ann 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(dt.Rows.Count, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(171));
                    }



                    if (annexeNumber == 1)
                    {
                        // En-tête pour l'annexe 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(limit, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(171));
                        // Code pour l'annexe 1 
                        for (int rowIndex = 0; rowIndex < limit; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                "2" +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Nom & Prénom"), 40) +
                                FormatText(GetColumnValue(row, "Emploi Occupé"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                GetColumnValue(row, "Situation Familiale") +
                                "0" +
                                GetColumnValue(row, "Nbr Enfants") +
                                FormatDate(GetColumnValue(row, "Date Début")) +
                                FormatDate(GetColumnValue(row, "Date fin")) +
                                GetColumnValue(row, "Durée (jours)") +
                                FormatNumber(GetColumnValue(row, "Revenu Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Av. Nature"), 15) +
                                FormatNumber(GetColumnValue(row, "Rev. Brut Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Revenus Réinvestis"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues Opérées"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues 20%"), 15) +
                                FormatNumber(GetColumnValue(row, "CSS"), 15) +
                                FormatNumber(GetColumnValue(row, "Net Servi"), 15) +
                                "".PadRight(19);
                            sw.WriteLine(enregistrement);
                        }

                        sw.WriteLine("T1" + txtMat.Text + txtExercice.Text +
                        "".PadRight(242) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenu Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Av. Nature"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Rev. Brut Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenus Réinvestis"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues Opérées"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues 20%"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "CSS"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Net Servi"), 15) +
                            "".PadRight(19));
                    }
                    else if (annexeNumber == 2)
                    {
                        // Code spécifique pour l'annexe 2
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) + "".PadRight(10);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "000000000000000" +
                            FormatNumber(totalBaseAmount, 15) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) + "".PadRight(10));



                    }
                    else if (annexeNumber == 3)
                    {
                        // Code spécifique pour l'annexe 3
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15)+
                                "".PadRight(86);
                        sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                        "".PadRight(86));


                    }
                    else if (annexeNumber == 4)
                    {
                        // Code spécifique pour l'annexe 4
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                               
                                "000000000000000" +
                                "000000000000000" +

                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15);
                            "".PadRight(4);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) 
                        );



                    }
                    else if (annexeNumber == 5)
                    {
                        // Code spécifique pour l'annexe 5
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(currentNetAmount, 15) +
                        "".PadRight(26);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(26));




                    }
                    else if (annexeNumber == 6)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0"+
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(40);

                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "".PadRight(40));



                    }
                    else if (annexeNumber == 7)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "01" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15)+
                            "".PadRight(114);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            FormatNumber(totalBaseAmount, 15) +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(115));



                    }
                }
                //cdc 2023
                if (txtExercice.Text == "2023" || txtExercice.Text == "2024")
                {
                    if (annexeNumber >= 2 && annexeNumber <= 7)
                    {
                        // En-tête commun pour toutes les annexes sauf l'ann 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(dt.Rows.Count, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(171));
                    }



                    if (annexeNumber == 1)
                    {
                        // En-tête pour l'annexe 1
                        sw.WriteLine("E" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text + "An" + FormatOrder(annexeNumber, 1) +
                            FormatText(cmbActe.Text, 1) +
                            FormatOrder(limit, 6) +
                            FormatText(txtNomSociete.Text, 40) +
                            FormatText(txtActiv.Text, 40) +
                            FormatText(txtAdr.Text, 40) +
                            FormatText(txtRue.Text, 72) +
                            FormatText(txtNum.Text, 4) +
                            FormatText(txtPo.Text, 4) +
                            "".PadRight(171));
                        // Code pour l'annexe 1 
                        for (int rowIndex = 0; rowIndex < limit; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                "2" +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Nom & Prénom"), 40) +
                                FormatText(GetColumnValue(row, "Emploi Occupé"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                GetColumnValue(row, "Situation Familiale") +
                                "0" +
                                GetColumnValue(row, "Nbr Enfants") +
                                FormatDate(GetColumnValue(row, "Date Début")) +
                                FormatDate(GetColumnValue(row, "Date fin")) +
                                GetColumnValue(row, "Durée (jours)") +
                                FormatNumber(GetColumnValue(row, "Revenu Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Av. Nature"), 15) +
                                FormatNumber(GetColumnValue(row, "Rev. Brut Imposable"), 15) +
                                FormatNumber(GetColumnValue(row, "Revenus Réinvestis"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues Opérées"), 15) +
                                FormatNumber(GetColumnValue(row, "Retenues 20%"), 15) +
                                FormatNumber(GetColumnValue(row, "CSS"), 15) +
                                FormatNumber(GetColumnValue(row, "Net Servi"), 15) +
                                "".PadRight(19);
                            sw.WriteLine(enregistrement);
                        }

                        sw.WriteLine("T1" + txtMat.Text + txtExercice.Text +
                            "".PadRight(242) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenu Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Av. Nature"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Rev. Brut Imposable"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Revenus Réinvestis"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues Opérées"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Retenues 20%"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "CSS"), 15) +
                            FormatNumber(GetColumnValue(dt.Rows[dt.Rows.Count - 1], "Net Servi"), 15) +
                            "".PadRight(19));
                    }
                    else if (annexeNumber == 2)
                    {
                        // Code spécifique pour l'annexe 2
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) + "".PadRight(10);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "000000000000000" +
                            FormatNumber(totalBaseAmount, 15) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) + "".PadRight(10));



                    }
                    else if (annexeNumber == 3)
                    {
                        // Code spécifique pour l'annexe 3
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) +
                                "".PadRight(86);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                        "".PadRight(86));


                    }
                    else if (annexeNumber == 4)
                    {
                        // Code spécifique pour l'annexe 4
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "0" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                "00000" +
                                "000000000000000" +
                                
                                "000000000000000" +
                                "000000000000000" +

                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15);
                            "".PadRight(4);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            "00000" +
                            "000000000000000" +
                            
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) 
                        );



                    }
                    else if (annexeNumber == 5)
                    {
                        // Code spécifique pour l'annexe 5
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber(currentNetAmount, 15) +
                            "".PadRight(26);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            "000000000000000" +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(26));




                    }
                    else if (annexeNumber == 6)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber("0", 15) +
                                "".PadRight(11);

                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(220) +
                            "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                "000000000000000" +
                                FormatNumber("0",15) +
                                "".PadRight(11));



                    }
                    else if (annexeNumber == 7)
                    {
                        // Code spécifique pour l'annexe 7
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            i++;
                            DataRow row = dt.Rows[rowIndex];
                            int currentNetAmount = int.Parse(GetColumnValue(row, "Base")) - int.Parse(GetColumnValue(row, "Retenue"));
                            totalBaseAmount += int.Parse(GetColumnValue(row, "Base"));
                            totalRetentionAmount += int.Parse(GetColumnValue(row, "Retenue"));
                            totalNetAmount += currentNetAmount;
                            string enregistrement = "L" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                                FormatOrder(i, 6) +
                                GetColumnValue(row, "Type identifiant") +
                                FormatText(GetColumnValue(row, "Identifiant"), 13) +
                                FormatText(GetColumnValue(row, "Raison"), 40) +
                                FormatText(GetColumnValue(row, "Activité"), 40) +
                                FormatText(GetColumnValue(row, "Adresse"), 120) +
                                "01" +
                                FormatNumber(GetColumnValue(row, "Base"), 15) + // Base imposable
                                FormatNumber(GetColumnValue(row, "Retenue"), 15) + // Retenue
                                FormatNumber(currentNetAmount, 15) +
                            "".PadRight(114);
                            sw.WriteLine(enregistrement);
                        }

                        // ANFIN
                        sw.WriteLine("T" + FormatOrder(annexeNumber, 1) + txtMat.Text + txtExercice.Text +
                            "".PadRight(221) +
                            FormatNumber(totalBaseAmount, 15) +
                            FormatNumber(totalRetentionAmount, 15) +
                            FormatNumber(totalNetAmount, 15) +
                            "".PadRight(115));



                    }
                }




            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
