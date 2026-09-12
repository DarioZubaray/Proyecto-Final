using System;
using System.Windows.Forms;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ClassroomForm : Form
    {
        #region Propiedades
        private readonly IClassroomBLL _aulaBLL;
        private readonly ClassroomBE _aula;
        private readonly bool _isNewAula;
        #endregion

        #region Constructor
        public ClassroomForm(string theme)
        {
            InitializeComponent();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _isNewAula = true;
            _aula = new ClassroomBE();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public ClassroomForm(ClassroomBE aula, string theme)
        {
            InitializeComponent();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _isNewAula = false;
            _aula = aula;
            LoadAulaData();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (nudCapacidad.Value <= 0)
            {
                MessageBox.Show("La capacidad debe ser mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudCapacidad.Focus();
                return false;
            }

            return true;
        }

        private void MapAulaFromUI()
        {
            _aula.Nombre = txtNombre.Text.Trim();
            _aula.Capacidad = (int)nudCapacidad.Value;
            _aula.IsActive = true;

            if (_isNewAula)
            {
                _aula.CreatedAt = DateTime.Now;
            }

            _aula.LastUpdate = DateTime.Now;
        }

        private bool SaveAula()
        {
            return _aulaBLL.Save(_aula);
        }

        private void LoadAulaData()
        {
            txtNombre.Text = _aula.Nombre;
            nudCapacidad.Value = _aula.Capacidad;
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            MapAulaFromUI();

            if (SaveAula())
            {
                MessageBox.Show(_isNewAula ? "Aula creada exitosamente." : "Aula modificada exitosamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
