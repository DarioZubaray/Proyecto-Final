using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DAL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ConnectionConfigForm : Form
    {
        #region Propiedades
        private const string DefaultServer = @"localhost\SQLEXPRESS";
        private const string DefaultDatabase = "DarioZubaray_TF";
        #endregion

        #region Constructor
        public ConnectionConfigForm()
        {
            InitializeComponent();
            LoadCurrentConfiguration();
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Mensaje informativo que se muestra arriba del formulario
        /// (p. ej. el error DB-001 que disparó la apertura).
        /// </summary>
        public void SetInitialMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                lblIntro.Text = message;
                lblIntro.ForeColor = System.Drawing.Color.IndianRed;
            }
        }

        private void LoadCurrentConfiguration()
        {
            string server;
            string database;
            ConnectionConfigDAL.GetCurrent(out server, out database);

            if (string.IsNullOrWhiteSpace(server))
            {
                server = DefaultServer;
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                database = DefaultDatabase;
            }

            cboServer.Items.Clear();
            AddUnique(cboServer, server);
            AddUnique(cboServer, DefaultServer);
            AddUnique(cboServer, @".\SQLEXPRESS");
            AddUnique(cboServer, "(local)");
            AddUnique(cboServer, "localhost");
            cboServer.Text = server;

            cboDatabase.Items.Clear();
            AddUnique(cboDatabase, database);
            AddUnique(cboDatabase, DefaultDatabase);
            cboDatabase.Text = database;
        }

        private static void AddUnique(ComboBox combo, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !combo.Items.Contains(value))
            {
                combo.Items.Add(value);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(cboServer.Text))
            {
                ShowStatus("Debe indicar el servidor SQL Server.", true);
                cboServer.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cboDatabase.Text))
            {
                ShowStatus("Debe indicar el nombre de la base de datos.", true);
                cboDatabase.Focus();
                return false;
            }

            return true;
        }

        private string BuildConnectionString()
        {
            return ConnectionConfigDAL.Build(cboServer.Text, cboDatabase.Text);
        }

        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isError
                ? System.Drawing.Color.IndianRed
                : System.Drawing.Color.SeaGreen;
        }

        private void ShowWarningStatus(string message)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = System.Drawing.Color.DarkOrange;
        }

        private void RefreshDatabaseList()
        {
            string current = cboDatabase.Text;
            List<string> databases = ConnectionConfigDAL.ListDatabases(cboServer.Text.Trim());

            cboDatabase.Items.Clear();

            foreach (string database in databases)
            {
                cboDatabase.Items.Add(database);
            }

            if (!string.IsNullOrWhiteSpace(current))
            {
                cboDatabase.Text = current;
            }
        }

        private string ScriptsDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sql");
        }

        private void RunBootstrap()
        {
            if (!ValidateFields())
            {
                return;
            }

            string server = cboServer.Text.Trim();
            string database = cboDatabase.Text.Trim();

            try
            {
                Cursor = Cursors.WaitCursor;
                btnCreate.Enabled = false;
                btnTest.Enabled = false;

                string masterError = ConnectionConfigDAL.TestError(
                    ConnectionConfigDAL.Build(server, "master"));

                if (masterError != null)
                {
                    ShowStatus("No se pudo conectar al servidor: " + masterError, true);
                    return;
                }

                var masterAccess = new AccessDAL(ConnectionConfigDAL.Build(server, "master"));
                int databaseExists = masterAccess.ReadScalar(
                    "SELECT CASE WHEN DB_ID(@db) IS NULL THEN 0 ELSE 1 END",
                    new[] { new Microsoft.Data.SqlClient.SqlParameter("@db", database) });

                if (databaseExists == 1)
                {
                    var access = new AccessDAL(BuildConnectionString());
                    int tables = access.ReadScalar(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                        "WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo'");

                    if (tables > 0)
                    {
                        ShowStatus(
                            "La base de datos '" + database + "' ya existe con su esquema. No se modificó.",
                            false);
                        return;
                    }
                }

                var runner = new SqlScriptRunner(BuildConnectionString());
                int applied = runner.RunDirectory(ScriptsDirectory());

                ShowStatus(
                    "Base de datos preparada correctamente (" + applied + " scripts aplicados).",
                    false);
                RefreshDatabaseList();
            }
            catch (Exception ex)
            {
                ShowStatus("Error al preparar la base de datos: " + ex.Message, true);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnCreate.Enabled = true;
                btnTest.Enabled = true;
            }
        }
        #endregion

        #region Eventos
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            string connectionString = BuildConnectionString();
            string error = ConnectionConfigDAL.TestError(connectionString);

            if (error != null)
            {
                ShowStatus("Error al conectar: " + error, true);
                return;
            }

            RefreshDatabaseList();

            bool databaseExists = false;
            foreach (string item in cboDatabase.Items)
            {
                if (string.Equals(item, cboDatabase.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    databaseExists = true;
                    break;
                }
            }

            if (databaseExists)
            {
                ShowStatus("Conexión correcta. La base de datos está disponible.", false);
            }
            else
            {
                ShowWarningStatus(
                    "Servidor accesible. La base '" + cboDatabase.Text.Trim() +
                    "' no existe todavía: puede crearla con «Crear base de datos».");
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            RunBootstrap();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            string connectionString = BuildConnectionString();
            string error = ConnectionConfigDAL.TestError(connectionString);

            if (error != null)
            {
                DialogResult decision = MessageBox.Show(
                    this,
                    "No se pudo conectar con la configuración indicada:\n\n" + error +
                    "\n\n¿Desea guardar la configuración de todos modos?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (decision != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                ConnectionConfigDAL.Save(cboServer.Text.Trim(), cboDatabase.Text.Trim());
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowStatus("No se pudo guardar: " + ex.Message, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion
    }
}
