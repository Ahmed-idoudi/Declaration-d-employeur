using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DeclarationEmployeurApp
{
    public class DatabaseManager
    {
        private string connectionString;
        private static DatabaseManager instance;

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DatabaseManager();
                return instance;
            }
        }

        private DatabaseManager()
        {
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeclarationEmployeur",
                "declarations.db");

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            connectionString = $"Data Source={dbPath};Version=3;";

            // Create database and table if they don't exist
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeclarationEmployeur",
                "declarations.db")))
            {
                SQLiteConnection.CreateFile(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DeclarationEmployeur",
                    "declarations.db"));
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Declarations (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            NomSociete TEXT,
                            Matricule TEXT,
                            MF TEXT,
                            Exercice TEXT,
                            DateCreation TEXT,
                            Activite TEXT,
                            Adresse TEXT,
                            Rue TEXT,
                            Numero TEXT,
                            CodePostal TEXT,
                            Acte TEXT
                        )";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveDeclaration(DataRow row)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Declarations (
                            NomSociete, Matricule, MF, Exercice, DateCreation,
                            Activite, Adresse, Rue, Numero, CodePostal, Acte
                        ) VALUES (
                            @NomSociete, @Matricule, @MF, @Exercice, @DateCreation,
                            @Activite, @Adresse, @Rue, @Numero, @CodePostal, @Acte
                        )";

                    command.Parameters.AddWithValue("@NomSociete", row["NomSociete"]);
                    command.Parameters.AddWithValue("@Matricule", row["Matricule"]);
                    command.Parameters.AddWithValue("@MF", row["MF"]);
                    command.Parameters.AddWithValue("@Exercice", row["Exercice"]);
                    command.Parameters.AddWithValue("@DateCreation", row["DateCreation"]);
                    command.Parameters.AddWithValue("@Activite", row["Activite"]);
                    command.Parameters.AddWithValue("@Adresse", row["Adresse"]);
                    command.Parameters.AddWithValue("@Rue", row["Rue"]);
                    command.Parameters.AddWithValue("@Numero", row["Numero"]);
                    command.Parameters.AddWithValue("@CodePostal", row["CodePostal"]);
                    command.Parameters.AddWithValue("@Acte", row["Acte"]);

                    command.ExecuteNonQuery();
                }
            }
        }

        public DataTable LoadDeclarations()
        {
            DataTable dt = new DataTable("Declarations");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NomSociete", typeof(string));
            dt.Columns.Add("Matricule", typeof(string));
            dt.Columns.Add("MF", typeof(string));
            dt.Columns.Add("Exercice", typeof(string));
            dt.Columns.Add("DateCreation", typeof(DateTime));
            dt.Columns.Add("Activite", typeof(string));
            dt.Columns.Add("Adresse", typeof(string));
            dt.Columns.Add("Rue", typeof(string));
            dt.Columns.Add("Numero", typeof(string));
            dt.Columns.Add("CodePostal", typeof(string));
            dt.Columns.Add("Acte", typeof(string));

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Declarations", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["ID"] = reader["ID"];
                            row["NomSociete"] = reader["NomSociete"];
                            row["Matricule"] = reader["Matricule"];
                            row["MF"] = reader["MF"];
                            row["Exercice"] = reader["Exercice"];
                            row["DateCreation"] = DateTime.Parse(reader["DateCreation"].ToString());
                            row["Activite"] = reader["Activite"];
                            row["Adresse"] = reader["Adresse"];
                            row["Rue"] = reader["Rue"];
                            row["Numero"] = reader["Numero"];
                            row["CodePostal"] = reader["CodePostal"];
                            row["Acte"] = reader["Acte"];
                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            return dt;
        }

        public void DeleteSociete(string nomSociete)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM Declarations WHERE NomSociete = @NomSociete";
                    command.Parameters.AddWithValue("@NomSociete", nomSociete);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
} 