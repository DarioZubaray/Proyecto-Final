
namespace TrabajoFinal_DarioZubaray
{
    partial class EnrollmentForm
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
            this.lblCursosDisponibles = new System.Windows.Forms.Label();
            this.dgvCursosDisponibles = new System.Windows.Forms.DataGridView();
            this.btnInscribirse = new System.Windows.Forms.Button();
            this.lblMisInscripciones = new System.Windows.Forms.Label();
            this.dgvMisInscripciones = new System.Windows.Forms.DataGridView();
            this.btnDesinscribirse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCursosDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMisInscripciones)).BeginInit();
            this.SuspendLayout();
            //
            // lblCursosDisponibles
            //
            this.lblCursosDisponibles.AutoSize = true;
            this.lblCursosDisponibles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCursosDisponibles.Location = new System.Drawing.Point(12, 9);
            this.lblCursosDisponibles.Name = "lblCursosDisponibles";
            this.lblCursosDisponibles.Size = new System.Drawing.Size(119, 13);
            this.lblCursosDisponibles.TabIndex = 0;
            this.lblCursosDisponibles.Text = "Cursos Disponibles:";
            //
            // dgvCursosDisponibles
            //
            this.dgvCursosDisponibles.AllowUserToAddRows = false;
            this.dgvCursosDisponibles.AllowUserToDeleteRows = false;
            this.dgvCursosDisponibles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCursosDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCursosDisponibles.Location = new System.Drawing.Point(12, 25);
            this.dgvCursosDisponibles.Name = "dgvCursosDisponibles";
            this.dgvCursosDisponibles.ReadOnly = true;
            this.dgvCursosDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCursosDisponibles.Size = new System.Drawing.Size(826, 180);
            this.dgvCursosDisponibles.TabIndex = 1;
            //
            // btnInscribirse
            //
            this.btnInscribirse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInscribirse.Location = new System.Drawing.Point(12, 211);
            this.btnInscribirse.Name = "btnInscribirse";
            this.btnInscribirse.Size = new System.Drawing.Size(120, 28);
            this.btnInscribirse.TabIndex = 2;
            this.btnInscribirse.Text = "Inscribirse";
            this.btnInscribirse.UseVisualStyleBackColor = true;
            this.btnInscribirse.Click += new System.EventHandler(this.btnInscribirse_Click);
            //
            // lblMisInscripciones
            //
            this.lblMisInscripciones.AutoSize = true;
            this.lblMisInscripciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMisInscripciones.Location = new System.Drawing.Point(12, 255);
            this.lblMisInscripciones.Name = "lblMisInscripciones";
            this.lblMisInscripciones.Size = new System.Drawing.Size(115, 13);
            this.lblMisInscripciones.TabIndex = 3;
            this.lblMisInscripciones.Text = "Mis Inscripciones:";
            //
            // dgvMisInscripciones
            //
            this.dgvMisInscripciones.AllowUserToAddRows = false;
            this.dgvMisInscripciones.AllowUserToDeleteRows = false;
            this.dgvMisInscripciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMisInscripciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMisInscripciones.Location = new System.Drawing.Point(12, 271);
            this.dgvMisInscripciones.Name = "dgvMisInscripciones";
            this.dgvMisInscripciones.ReadOnly = true;
            this.dgvMisInscripciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMisInscripciones.Size = new System.Drawing.Size(826, 180);
            this.dgvMisInscripciones.TabIndex = 4;
            //
            // btnDesinscribirse
            //
            this.btnDesinscribirse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDesinscribirse.Location = new System.Drawing.Point(12, 457);
            this.btnDesinscribirse.Name = "btnDesinscribirse";
            this.btnDesinscribirse.Size = new System.Drawing.Size(120, 28);
            this.btnDesinscribirse.TabIndex = 5;
            this.btnDesinscribirse.Text = "Desuscribirse";
            this.btnDesinscribirse.UseVisualStyleBackColor = true;
            this.btnDesinscribirse.Click += new System.EventHandler(this.btnDesinscribirse_Click);
            //
            // InscripcionForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 500);
            this.Controls.Add(this.lblCursosDisponibles);
            this.Controls.Add(this.dgvCursosDisponibles);
            this.Controls.Add(this.btnInscribirse);
            this.Controls.Add(this.lblMisInscripciones);
            this.Controls.Add(this.dgvMisInscripciones);
            this.Controls.Add(this.btnDesinscribirse);
            this.Name = "InscripcionForm";
            this.Text = "Mis Inscripciones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCursosDisponibles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMisInscripciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCursosDisponibles;
        private System.Windows.Forms.DataGridView dgvCursosDisponibles;
        private System.Windows.Forms.Button btnInscribirse;
        private System.Windows.Forms.Label lblMisInscripciones;
        private System.Windows.Forms.DataGridView dgvMisInscripciones;
        private System.Windows.Forms.Button btnDesinscribirse;
    }
}
