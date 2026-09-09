
namespace TrabajoFinal_DarioZubaray
{
    partial class CursoForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.lblAula = new System.Windows.Forms.Label();
            this.cbAula = new System.Windows.Forms.ComboBox();
            this.lblDocentes = new System.Windows.Forms.Label();
            this.lstDocentesDisponibles = new System.Windows.Forms.ListBox();
            this.lstDocentesAsignados = new System.Windows.Forms.ListBox();
            this.btnAddDocente = new System.Windows.Forms.Button();
            this.btnRemoveDocente = new System.Windows.Forms.Button();
            this.lblDisponibles = new System.Windows.Forms.Label();
            this.lblAsignados = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(12, 15);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(47, 13);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(120, 12);
            this.txtNombre.MaxLength = 200;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(300, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(12, 45);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(66, 13);
            this.lblDescripcion.TabIndex = 2;
            this.lblDescripcion.Text = "Descripción:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(120, 42);
            this.txtDescripcion.MaxLength = 500;
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(300, 50);
            this.txtDescripcion.TabIndex = 3;
            // 
            // lblFechaInicio
            // 
            this.lblFechaInicio.AutoSize = true;
            this.lblFechaInicio.Location = new System.Drawing.Point(12, 105);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(75, 13);
            this.lblFechaInicio.TabIndex = 4;
            this.lblFechaInicio.Text = "Fecha Inicio:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(120, 102);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(150, 20);
            this.dtpFechaInicio.TabIndex = 5;
            // 
            // lblFechaFin
            // 
            this.lblFechaFin.AutoSize = true;
            this.lblFechaFin.Location = new System.Drawing.Point(12, 135);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(62, 13);
            this.lblFechaFin.TabIndex = 6;
            this.lblFechaFin.Text = "Fecha Fin:";
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFin.Location = new System.Drawing.Point(120, 132);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(150, 20);
            this.dtpFechaFin.TabIndex = 7;
            // 
            // lblAula
            // 
            this.lblAula.AutoSize = true;
            this.lblAula.Location = new System.Drawing.Point(12, 165);
            this.lblAula.Name = "lblAula";
            this.lblAula.Size = new System.Drawing.Size(34, 13);
            this.lblAula.TabIndex = 8;
            this.lblAula.Text = "Aula:";
            // 
            // cbAula
            // 
            this.cbAula.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAula.Location = new System.Drawing.Point(120, 162);
            this.cbAula.Name = "cbAula";
            this.cbAula.Size = new System.Drawing.Size(200, 21);
            this.cbAula.TabIndex = 9;
            // 
            // lblDocentes
            // 
            this.lblDocentes.AutoSize = true;
            this.lblDocentes.Location = new System.Drawing.Point(12, 200);
            this.lblDocentes.Name = "lblDocentes";
            this.lblDocentes.Size = new System.Drawing.Size(56, 13);
            this.lblDocentes.TabIndex = 10;
            this.lblDocentes.Text = "Docentes:";
            // 
            // lblDisponibles
            // 
            this.lblDisponibles.AutoSize = true;
            this.lblDisponibles.Location = new System.Drawing.Point(12, 220);
            this.lblDisponibles.Name = "lblDisponibles";
            this.lblDisponibles.Size = new System.Drawing.Size(67, 13);
            this.lblDisponibles.TabIndex = 11;
            this.lblDisponibles.Text = "Disponibles:";
            // 
            // lstDocentesDisponibles
            // 
            this.lstDocentesDisponibles.FormattingEnabled = true;
            this.lstDocentesDisponibles.Location = new System.Drawing.Point(120, 220);
            this.lstDocentesDisponibles.Name = "lstDocentesDisponibles";
            this.lstDocentesDisponibles.Size = new System.Drawing.Size(150, 95);
            this.lstDocentesDisponibles.TabIndex = 12;
            // 
            // btnAddDocente
            // 
            this.btnAddDocente.Location = new System.Drawing.Point(280, 250);
            this.btnAddDocente.Name = "btnAddDocente";
            this.btnAddDocente.Size = new System.Drawing.Size(30, 23);
            this.btnAddDocente.TabIndex = 13;
            this.btnAddDocente.Text = ">";
            this.btnAddDocente.UseVisualStyleBackColor = true;
            this.btnAddDocente.Click += new System.EventHandler(this.btnAddDocente_Click);
            // 
            // btnRemoveDocente
            // 
            this.btnRemoveDocente.Location = new System.Drawing.Point(280, 280);
            this.btnRemoveDocente.Name = "btnRemoveDocente";
            this.btnRemoveDocente.Size = new System.Drawing.Size(30, 23);
            this.btnRemoveDocente.TabIndex = 14;
            this.btnRemoveDocente.Text = "<";
            this.btnRemoveDocente.UseVisualStyleBackColor = true;
            this.btnRemoveDocente.Click += new System.EventHandler(this.btnRemoveDocente_Click);
            // 
            // lblAsignados
            // 
            this.lblAsignados.AutoSize = true;
            this.lblAsignados.Location = new System.Drawing.Point(320, 220);
            this.lblAsignados.Name = "lblAsignados";
            this.lblAsignados.Size = new System.Drawing.Size(62, 13);
            this.lblAsignados.TabIndex = 15;
            this.lblAsignados.Text = "Asignados:";
            // 
            // lstDocentesAsignados
            // 
            this.lstDocentesAsignados.FormattingEnabled = true;
            this.lstDocentesAsignados.Location = new System.Drawing.Point(390, 220);
            this.lstDocentesAsignados.Name = "lstDocentesAsignados";
            this.lstDocentesAsignados.Size = new System.Drawing.Size(150, 95);
            this.lstDocentesAsignados.TabIndex = 16;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(320, 330);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(401, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CursoForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(554, 366);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.lblFechaInicio);
            this.Controls.Add(this.dtpFechaInicio);
            this.Controls.Add(this.lblFechaFin);
            this.Controls.Add(this.dtpFechaFin);
            this.Controls.Add(this.lblAula);
            this.Controls.Add(this.cbAula);
            this.Controls.Add(this.lblDocentes);
            this.Controls.Add(this.lblDisponibles);
            this.Controls.Add(this.lstDocentesDisponibles);
            this.Controls.Add(this.btnAddDocente);
            this.Controls.Add(this.btnRemoveDocente);
            this.Controls.Add(this.lblAsignados);
            this.Controls.Add(this.lstDocentesAsignados);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CursoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Curso";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private System.Windows.Forms.Label lblAula;
        private System.Windows.Forms.ComboBox cbAula;
        private System.Windows.Forms.Label lblDocentes;
        private System.Windows.Forms.ListBox lstDocentesDisponibles;
        private System.Windows.Forms.ListBox lstDocentesAsignados;
        private System.Windows.Forms.Button btnAddDocente;
        private System.Windows.Forms.Button btnRemoveDocente;
        private System.Windows.Forms.Label lblDisponibles;
        private System.Windows.Forms.Label lblAsignados;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
