
namespace TrabajoFinal_DarioZubaray
{
    partial class EnrollmentManagementForm
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
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panelEnroll = new System.Windows.Forms.Panel();
            this.lblStudent = new System.Windows.Forms.Label();
            this.cboStudent = new System.Windows.Forms.ComboBox();
            this.lblCourse = new System.Windows.Forms.Label();
            this.cboCourse = new System.Windows.Forms.ComboBox();
            this.btnEnroll = new System.Windows.Forms.Button();
            this.dgvEnrollments = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            this.panelEnroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEnrollments)).BeginInit();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblSearch);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Controls.Add(this.btnSearch);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(850, 40);
            this.panelTop.TabIndex = 0;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 12);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(95, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Buscar alumno/curso:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(130, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(340, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Buscar";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // panelEnroll
            //
            this.panelEnroll.Controls.Add(this.lblStudent);
            this.panelEnroll.Controls.Add(this.cboStudent);
            this.panelEnroll.Controls.Add(this.lblCourse);
            this.panelEnroll.Controls.Add(this.cboCourse);
            this.panelEnroll.Controls.Add(this.btnEnroll);
            this.panelEnroll.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEnroll.Location = new System.Drawing.Point(0, 40);
            this.panelEnroll.Name = "panelEnroll";
            this.panelEnroll.Size = new System.Drawing.Size(850, 40);
            this.panelEnroll.TabIndex = 1;
            //
            // lblStudent
            //
            this.lblStudent.AutoSize = true;
            this.lblStudent.Location = new System.Drawing.Point(12, 12);
            this.lblStudent.Name = "lblStudent";
            this.lblStudent.Size = new System.Drawing.Size(100, 13);
            this.lblStudent.TabIndex = 0;
            this.lblStudent.Text = "Alumno a inscribir:";
            //
            // cboStudent
            //
            this.cboStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudent.Location = new System.Drawing.Point(130, 9);
            this.cboStudent.Name = "cboStudent";
            this.cboStudent.Size = new System.Drawing.Size(180, 21);
            this.cboStudent.TabIndex = 1;
            //
            // lblCourse
            //
            this.lblCourse.AutoSize = true;
            this.lblCourse.Location = new System.Drawing.Point(325, 12);
            this.lblCourse.Name = "lblCourse";
            this.lblCourse.Size = new System.Drawing.Size(41, 13);
            this.lblCourse.TabIndex = 2;
            this.lblCourse.Text = "Curso:";
            //
            // cboCourse
            //
            this.cboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourse.Location = new System.Drawing.Point(372, 9);
            this.cboCourse.Name = "cboCourse";
            this.cboCourse.Size = new System.Drawing.Size(220, 21);
            this.cboCourse.TabIndex = 3;
            //
            // btnEnroll
            //
            this.btnEnroll.Location = new System.Drawing.Point(602, 7);
            this.btnEnroll.Name = "btnEnroll";
            this.btnEnroll.Size = new System.Drawing.Size(130, 23);
            this.btnEnroll.TabIndex = 4;
            this.btnEnroll.Text = "Inscribir alumno";
            this.btnEnroll.UseVisualStyleBackColor = true;
            this.btnEnroll.Click += new System.EventHandler(this.btnEnroll_Click);
            //
            // dgvEnrollments
            //
            this.dgvEnrollments.AllowUserToAddRows = false;
            this.dgvEnrollments.AllowUserToDeleteRows = false;
            this.dgvEnrollments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEnrollments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEnrollments.Location = new System.Drawing.Point(0, 80);
            this.dgvEnrollments.Name = "dgvEnrollments";
            this.dgvEnrollments.ReadOnly = true;
            this.dgvEnrollments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEnrollments.Size = new System.Drawing.Size(850, 420);
            this.dgvEnrollments.TabIndex = 2;
            //
            // InscripcionManagementForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 500);
            this.Controls.Add(this.dgvEnrollments);
            this.Controls.Add(this.panelEnroll);
            this.Controls.Add(this.panelTop);
            this.Name = "InscripcionManagementForm";
            this.Text = "Gestión de Inscripciones";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelEnroll.ResumeLayout(false);
            this.panelEnroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEnrollments)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel panelEnroll;
        private System.Windows.Forms.Label lblStudent;
        private System.Windows.Forms.ComboBox cboStudent;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.ComboBox cboCourse;
        private System.Windows.Forms.Button btnEnroll;
        private System.Windows.Forms.DataGridView dgvEnrollments;
    }
}
