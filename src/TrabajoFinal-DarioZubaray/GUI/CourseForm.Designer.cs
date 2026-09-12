
namespace TrabajoFinal_DarioZubaray
{
    partial class CourseForm
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
            lblNombre = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            lblDescripcion = new System.Windows.Forms.Label();
            txtDescripcion = new System.Windows.Forms.TextBox();
            lblFechaInicio = new System.Windows.Forms.Label();
            dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            lblFechaFin = new System.Windows.Forms.Label();
            dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            lblAula = new System.Windows.Forms.Label();
            cbAula = new System.Windows.Forms.ComboBox();
            lblHoraInicio = new System.Windows.Forms.Label();
            mtbHoraInicio = new System.Windows.Forms.MaskedTextBox();
            lblHoraFin = new System.Windows.Forms.Label();
            mtbHoraFin = new System.Windows.Forms.MaskedTextBox();
            lblDocentes = new System.Windows.Forms.Label();
            lstDocentesDisponibles = new System.Windows.Forms.ListBox();
            lstDocentesAsignados = new System.Windows.Forms.ListBox();
            btnAddDocente = new System.Windows.Forms.Button();
            btnRemoveDocente = new System.Windows.Forms.Button();
            lblDisponibles = new System.Windows.Forms.Label();
            lblAsignados = new System.Windows.Forms.Label();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnInactivar = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new System.Drawing.Point(14, 17);
            lblNombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(54, 15);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new System.Drawing.Point(152, 14);
            txtNombre.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNombre.MaxLength = 200;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(349, 23);
            txtNombre.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new System.Drawing.Point(14, 52);
            lblDescripcion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new System.Drawing.Size(72, 15);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Descripción:";
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new System.Drawing.Point(152, 48);
            txtDescripcion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDescripcion.MaxLength = 500;
            txtDescripcion.Multiline = true;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new System.Drawing.Size(349, 57);
            txtDescripcion.TabIndex = 3;
            // 
            // lblFechaInicio
            // 
            lblFechaInicio.AutoSize = true;
            lblFechaInicio.Location = new System.Drawing.Point(14, 121);
            lblFechaInicio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFechaInicio.Name = "lblFechaInicio";
            lblFechaInicio.Size = new System.Drawing.Size(73, 15);
            lblFechaInicio.TabIndex = 4;
            lblFechaInicio.Text = "Fecha Inicio:";
            // 
            // dtpFechaInicio
            // 
            dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpFechaInicio.Location = new System.Drawing.Point(152, 118);
            dtpFechaInicio.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtpFechaInicio.Name = "dtpFechaInicio";
            dtpFechaInicio.Size = new System.Drawing.Size(174, 23);
            dtpFechaInicio.TabIndex = 5;
            // 
            // lblFechaFin
            // 
            lblFechaFin.AutoSize = true;
            lblFechaFin.Location = new System.Drawing.Point(14, 156);
            lblFechaFin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFechaFin.Name = "lblFechaFin";
            lblFechaFin.Size = new System.Drawing.Size(60, 15);
            lblFechaFin.TabIndex = 6;
            lblFechaFin.Text = "Fecha Fin:";
            // 
            // dtpFechaFin
            // 
            dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpFechaFin.Location = new System.Drawing.Point(152, 152);
            dtpFechaFin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtpFechaFin.Name = "dtpFechaFin";
            dtpFechaFin.Size = new System.Drawing.Size(174, 23);
            dtpFechaFin.TabIndex = 7;
            // 
            // lblAula
            // 
            lblAula.AutoSize = true;
            lblAula.Location = new System.Drawing.Point(14, 190);
            lblAula.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAula.Name = "lblAula";
            lblAula.Size = new System.Drawing.Size(34, 15);
            lblAula.TabIndex = 8;
            lblAula.Text = "Aula:";
            // 
            // cbAula
            // 
            cbAula.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAula.Location = new System.Drawing.Point(152, 187);
            cbAula.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbAula.Name = "cbAula";
            cbAula.Size = new System.Drawing.Size(233, 23);
            cbAula.TabIndex = 9;
            // 
            // lblHoraInicio
            // 
            lblHoraInicio.AutoSize = true;
            lblHoraInicio.Location = new System.Drawing.Point(14, 229);
            lblHoraInicio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblHoraInicio.Name = "lblHoraInicio";
            lblHoraInicio.Size = new System.Drawing.Size(68, 15);
            lblHoraInicio.TabIndex = 12;
            lblHoraInicio.Text = "Hora Inicio:";
            // 
            // mtbHoraInicio
            // 
            mtbHoraInicio.Location = new System.Drawing.Point(152, 225);
            mtbHoraInicio.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mtbHoraInicio.Mask = "00:00";
            mtbHoraInicio.Name = "mtbHoraInicio";
            mtbHoraInicio.Size = new System.Drawing.Size(69, 23);
            mtbHoraInicio.TabIndex = 13;
            // 
            // lblHoraFin
            // 
            lblHoraFin.AutoSize = true;
            lblHoraFin.Location = new System.Drawing.Point(14, 263);
            lblHoraFin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblHoraFin.Name = "lblHoraFin";
            lblHoraFin.Size = new System.Drawing.Size(55, 15);
            lblHoraFin.TabIndex = 14;
            lblHoraFin.Text = "Hora Fin:";
            // 
            // mtbHoraFin
            // 
            mtbHoraFin.Location = new System.Drawing.Point(152, 260);
            mtbHoraFin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mtbHoraFin.Mask = "00:00";
            mtbHoraFin.Name = "mtbHoraFin";
            mtbHoraFin.Size = new System.Drawing.Size(69, 23);
            mtbHoraFin.TabIndex = 15;
            // 
            // lblDocentes
            // 
            lblDocentes.AutoSize = true;
            lblDocentes.Location = new System.Drawing.Point(14, 304);
            lblDocentes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDocentes.Name = "lblDocentes";
            lblDocentes.Size = new System.Drawing.Size(59, 15);
            lblDocentes.TabIndex = 16;
            lblDocentes.Text = "Docentes:";
            // 
            // lstDocentesDisponibles
            // 
            lstDocentesDisponibles.FormattingEnabled = true;
            lstDocentesDisponibles.Location = new System.Drawing.Point(152, 327);
            lstDocentesDisponibles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstDocentesDisponibles.Name = "lstDocentesDisponibles";
            lstDocentesDisponibles.Size = new System.Drawing.Size(174, 109);
            lstDocentesDisponibles.TabIndex = 18;
            // 
            // lstDocentesAsignados
            // 
            lstDocentesAsignados.FormattingEnabled = true;
            lstDocentesAsignados.Location = new System.Drawing.Point(467, 327);
            lstDocentesAsignados.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstDocentesAsignados.Name = "lstDocentesAsignados";
            lstDocentesAsignados.Size = new System.Drawing.Size(174, 109);
            lstDocentesAsignados.TabIndex = 22;
            // 
            // btnAddDocente
            // 
            btnAddDocente.Location = new System.Drawing.Point(338, 361);
            btnAddDocente.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddDocente.Name = "btnAddDocente";
            btnAddDocente.Size = new System.Drawing.Size(35, 27);
            btnAddDocente.TabIndex = 19;
            btnAddDocente.Text = ">";
            btnAddDocente.UseVisualStyleBackColor = true;
            btnAddDocente.Click += btnAddDocente_Click;
            // 
            // btnRemoveDocente
            // 
            btnRemoveDocente.Location = new System.Drawing.Point(338, 396);
            btnRemoveDocente.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRemoveDocente.Name = "btnRemoveDocente";
            btnRemoveDocente.Size = new System.Drawing.Size(35, 27);
            btnRemoveDocente.TabIndex = 20;
            btnRemoveDocente.Text = "<";
            btnRemoveDocente.UseVisualStyleBackColor = true;
            btnRemoveDocente.Click += btnRemoveDocente_Click;
            // 
            // lblDisponibles
            // 
            lblDisponibles.AutoSize = true;
            lblDisponibles.Location = new System.Drawing.Point(14, 327);
            lblDisponibles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDisponibles.Name = "lblDisponibles";
            lblDisponibles.Size = new System.Drawing.Size(71, 15);
            lblDisponibles.TabIndex = 17;
            lblDisponibles.Text = "Disponibles:";
            // 
            // lblAsignados
            // 
            lblAsignados.AutoSize = true;
            lblAsignados.Location = new System.Drawing.Point(385, 327);
            lblAsignados.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAsignados.Name = "lblAsignados";
            lblAsignados.Size = new System.Drawing.Size(65, 15);
            lblAsignados.TabIndex = 21;
            lblAsignados.Text = "Asignados:";
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(385, 454);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 27);
            btnSave.TabIndex = 23;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(479, 454);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 24;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnInactivar
            // 
            btnInactivar.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            btnInactivar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnInactivar.ForeColor = System.Drawing.Color.White;
            btnInactivar.Location = new System.Drawing.Point(14, 454);
            btnInactivar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnInactivar.Name = "btnInactivar";
            btnInactivar.Size = new System.Drawing.Size(105, 27);
            btnInactivar.TabIndex = 25;
            btnInactivar.Text = "Inactivar";
            btnInactivar.UseVisualStyleBackColor = false;
            btnInactivar.Click += btnInactivar_Click;
            // 
            // CursoForm
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(658, 497);
            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblFechaInicio);
            Controls.Add(dtpFechaInicio);
            Controls.Add(lblFechaFin);
            Controls.Add(dtpFechaFin);
            Controls.Add(lblAula);
            Controls.Add(cbAula);
            Controls.Add(lblHoraInicio);
            Controls.Add(mtbHoraInicio);
            Controls.Add(lblHoraFin);
            Controls.Add(mtbHoraFin);
            Controls.Add(lblDocentes);
            Controls.Add(lblDisponibles);
            Controls.Add(lstDocentesDisponibles);
            Controls.Add(btnAddDocente);
            Controls.Add(btnRemoveDocente);
            Controls.Add(lblAsignados);
            Controls.Add(lstDocentesAsignados);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(btnInactivar);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CursoForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Curso";
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.Label lblHoraInicio;
        private System.Windows.Forms.MaskedTextBox mtbHoraInicio;
        private System.Windows.Forms.Label lblHoraFin;
        private System.Windows.Forms.MaskedTextBox mtbHoraFin;
        private System.Windows.Forms.Label lblDocentes;
        private System.Windows.Forms.ListBox lstDocentesDisponibles;
        private System.Windows.Forms.ListBox lstDocentesAsignados;
        private System.Windows.Forms.Button btnAddDocente;
        private System.Windows.Forms.Button btnRemoveDocente;
        private System.Windows.Forms.Label lblDisponibles;
        private System.Windows.Forms.Label lblAsignados;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnInactivar;
    }
}
