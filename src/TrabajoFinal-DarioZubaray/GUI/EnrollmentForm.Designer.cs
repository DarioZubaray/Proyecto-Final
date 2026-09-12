
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
            this.lblAvailableCourses = new System.Windows.Forms.Label();
            this.dgvAvailableCourses = new System.Windows.Forms.DataGridView();
            this.btnEnroll = new System.Windows.Forms.Button();
            this.lblMyEnrollments = new System.Windows.Forms.Label();
            this.dgvMyEnrollments = new System.Windows.Forms.DataGridView();
            this.btnUnenroll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableCourses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyEnrollments)).BeginInit();
            this.SuspendLayout();
            //
            // lblAvailableCourses
            //
            this.lblAvailableCourses.AutoSize = true;
            this.lblAvailableCourses.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAvailableCourses.Location = new System.Drawing.Point(12, 9);
            this.lblAvailableCourses.Name = "lblAvailableCourses";
            this.lblAvailableCourses.Size = new System.Drawing.Size(119, 13);
            this.lblAvailableCourses.TabIndex = 0;
            this.lblAvailableCourses.Text = "Cursos Disponibles:";
            //
            // dgvAvailableCourses
            //
            this.dgvAvailableCourses.AllowUserToAddRows = false;
            this.dgvAvailableCourses.AllowUserToDeleteRows = false;
            this.dgvAvailableCourses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAvailableCourses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvailableCourses.Location = new System.Drawing.Point(12, 25);
            this.dgvAvailableCourses.Name = "dgvAvailableCourses";
            this.dgvAvailableCourses.ReadOnly = true;
            this.dgvAvailableCourses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAvailableCourses.Size = new System.Drawing.Size(826, 180);
            this.dgvAvailableCourses.TabIndex = 1;
            //
            // btnEnroll
            //
            this.btnEnroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnroll.Location = new System.Drawing.Point(12, 211);
            this.btnEnroll.Name = "btnEnroll";
            this.btnEnroll.Size = new System.Drawing.Size(120, 28);
            this.btnEnroll.TabIndex = 2;
            this.btnEnroll.Text = "Inscribirse";
            this.btnEnroll.UseVisualStyleBackColor = true;
            this.btnEnroll.Click += new System.EventHandler(this.btnEnroll_Click);
            //
            // lblMyEnrollments
            //
            this.lblMyEnrollments.AutoSize = true;
            this.lblMyEnrollments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMyEnrollments.Location = new System.Drawing.Point(12, 255);
            this.lblMyEnrollments.Name = "lblMyEnrollments";
            this.lblMyEnrollments.Size = new System.Drawing.Size(115, 13);
            this.lblMyEnrollments.TabIndex = 3;
            this.lblMyEnrollments.Text = "Mis Inscripciones:";
            //
            // dgvMyEnrollments
            //
            this.dgvMyEnrollments.AllowUserToAddRows = false;
            this.dgvMyEnrollments.AllowUserToDeleteRows = false;
            this.dgvMyEnrollments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMyEnrollments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMyEnrollments.Location = new System.Drawing.Point(12, 271);
            this.dgvMyEnrollments.Name = "dgvMyEnrollments";
            this.dgvMyEnrollments.ReadOnly = true;
            this.dgvMyEnrollments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMyEnrollments.Size = new System.Drawing.Size(826, 180);
            this.dgvMyEnrollments.TabIndex = 4;
            //
            // btnUnenroll
            //
            this.btnUnenroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUnenroll.Location = new System.Drawing.Point(12, 457);
            this.btnUnenroll.Name = "btnUnenroll";
            this.btnUnenroll.Size = new System.Drawing.Size(120, 28);
            this.btnUnenroll.TabIndex = 5;
            this.btnUnenroll.Text = "Desuscribirse";
            this.btnUnenroll.UseVisualStyleBackColor = true;
            this.btnUnenroll.Click += new System.EventHandler(this.btnUnenroll_Click);
            //
            // EnrollmentForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 500);
            this.Controls.Add(this.lblAvailableCourses);
            this.Controls.Add(this.dgvAvailableCourses);
            this.Controls.Add(this.btnEnroll);
            this.Controls.Add(this.lblMyEnrollments);
            this.Controls.Add(this.dgvMyEnrollments);
            this.Controls.Add(this.btnUnenroll);
            this.Name = "EnrollmentForm";
            this.Text = "Mis Inscripciones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableCourses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMyEnrollments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAvailableCourses;
        private System.Windows.Forms.DataGridView dgvAvailableCourses;
        private System.Windows.Forms.Button btnEnroll;
        private System.Windows.Forms.Label lblMyEnrollments;
        private System.Windows.Forms.DataGridView dgvMyEnrollments;
        private System.Windows.Forms.Button btnUnenroll;
    }
}
