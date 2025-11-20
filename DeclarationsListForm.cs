using System;
using System.Data;
using System.Windows.Forms;

namespace DeclarationEmployeurApp
{
    public partial class DeclarationsListForm : Form
    {
        public DataRow SelectedDeclaration { get; private set; }

        public DeclarationsListForm(DataTable declarations)
        {
            InitializeComponent();

            // Wire up events
            this.dgvDeclarations.CellDoubleClick += new DataGridViewCellEventHandler(dgvDeclarations_CellDoubleClick);
            this.btnSelect.Click += new EventHandler(btnSelect_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);

            InitializeGrid(declarations);
        }

        private void InitializeGrid(DataTable declarations)
        {
            if (declarations == null || declarations.Rows.Count == 0)
            {
                MessageBox.Show("Aucune déclaration n'est disponible.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dgvDeclarations.DataSource = declarations;
            
            // Configure columns
            if (dgvDeclarations.Columns.Contains("ID"))
                dgvDeclarations.Columns["ID"].Visible = false;
            
            if (dgvDeclarations.Columns.Contains("NomSociete"))
                dgvDeclarations.Columns["NomSociete"].HeaderText = "Nom de la Société";
            
            if (dgvDeclarations.Columns.Contains("Matricule"))
                dgvDeclarations.Columns["Matricule"].HeaderText = "Matricule";
            
            if (dgvDeclarations.Columns.Contains("MF"))
                dgvDeclarations.Columns["MF"].HeaderText = "MF";
            
            if (dgvDeclarations.Columns.Contains("Exercice"))
                dgvDeclarations.Columns["Exercice"].HeaderText = "Exercice";
            
            if (dgvDeclarations.Columns.Contains("DateCreation"))
            {
                dgvDeclarations.Columns["DateCreation"].HeaderText = "Date de Création";
                dgvDeclarations.Columns["DateCreation"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }

            // Adjust columns width
            dgvDeclarations.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void dgvDeclarations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectCurrentRow();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectCurrentRow();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SelectCurrentRow()
        {
            if (dgvDeclarations.CurrentRow != null)
            {
                SelectedDeclaration = ((DataRowView)dgvDeclarations.CurrentRow.DataBoundItem).Row;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
} 