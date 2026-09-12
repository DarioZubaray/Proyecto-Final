
namespace TrabajoFinal_DarioZubaray
{
    partial class AttendanceForm
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblCurso = new System.Windows.Forms.Label();
            this.cboCurso = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvAsistencia = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencia)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblCurso);
            this.panelTop.Controls.Add(this.cboCurso);
            this.panelTop.Controls.Add(this.lblFecha);
            this.panelTop.Controls.Add(this.dtpFecha);
            this.panelTop.Controls.Add(this.btnBuscar);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(850, 50);
            this.panelTop.TabIndex = 0;
            //
            // lblCurso
            //
            this.lblCurso.AutoSize = true;
            this.lblCurso.Location = new System.Drawing.Point(12, 17);
            this.lblCurso.Name = "lblCurso";
            this.lblCurso.Size = new System.Drawing.Size(40, 13);
            this.lblCurso.TabIndex = 0;
            this.lblCurso.Text = "Curso:";
            //
            // cboCurso
            //
            this.cboCurso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCurso.Location = new System.Drawing.Point(60, 14);
            this.cboCurso.Name = "cboCurso";
            this.cboCurso.Size = new System.Drawing.Size(250, 21);
            this.cboCurso.TabIndex = 1;
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(330, 17);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(40, 13);
            this.lblFecha.TabIndex = 2;
            this.lblFecha.Text = "Fecha:";
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(375, 14);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(120, 20);
            this.dtpFecha.TabIndex = 3;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(510, 12);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 4;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvAsistencia
            //
            this.dgvAsistencia.AllowUserToAddRows = false;
            this.dgvAsistencia.AllowUserToDeleteRows = false;
            this.dgvAsistencia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsistencia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAsistencia.Location = new System.Drawing.Point(0, 50);
            this.dgvAsistencia.Name = "dgvAsistencia";
            this.dgvAsistencia.Size = new System.Drawing.Size(850, 400);
            this.dgvAsistencia.TabIndex = 1;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnGuardar);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 450);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(850, 50);
            this.panelBottom.TabIndex = 2;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(730, 12);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 23);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "Guardar Asistencia";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // AsistenciaForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 500);
            this.Controls.Add(this.dgvAsistencia);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "AsistenciaForm";
            this.Text = "Registrar Asistencia";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencia)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblCurso;
        private System.Windows.Forms.ComboBox cboCurso;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvAsistencia;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnGuardar;
    }
}
