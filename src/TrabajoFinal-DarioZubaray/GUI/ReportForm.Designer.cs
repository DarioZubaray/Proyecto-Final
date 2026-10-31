
namespace TrabajoFinal_DarioZubaray
{
    partial class ReportForm
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
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lblCourse = new System.Windows.Forms.Label();
            this.cboCourse = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblType);
            this.panelTop.Controls.Add(this.cboType);
            this.panelTop.Controls.Add(this.lblCourse);
            this.panelTop.Controls.Add(this.cboCourse);
            this.panelTop.Controls.Add(this.btnGenerate);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(850, 50);
            this.panelTop.TabIndex = 0;
            //
            // lblType
            //
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 17);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(80, 13);
            this.lblType.TabIndex = 0;
            this.lblType.Text = "Tipo de informe:";
            //
            // cboType
            //
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Location = new System.Drawing.Point(100, 14);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(180, 21);
            this.cboType.TabIndex = 1;
            //
            // lblCourse
            //
            this.lblCourse.AutoSize = true;
            this.lblCourse.Location = new System.Drawing.Point(300, 17);
            this.lblCourse.Name = "lblCourse";
            this.lblCourse.Size = new System.Drawing.Size(40, 13);
            this.lblCourse.TabIndex = 2;
            this.lblCourse.Text = "Curso:";
            //
            // cboCourse
            //
            this.cboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourse.Location = new System.Drawing.Point(345, 14);
            this.cboCourse.Name = "cboCourse";
            this.cboCourse.Size = new System.Drawing.Size(250, 21);
            this.cboCourse.TabIndex = 3;
            //
            // btnGenerate
            //
            this.btnGenerate.Location = new System.Drawing.Point(610, 12);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(110, 23);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generar";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // dgvReport
            //
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.Location = new System.Drawing.Point(0, 50);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.Size = new System.Drawing.Size(850, 400);
            this.dgvReport.TabIndex = 1;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnExport);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 450);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(850, 50);
            this.panelBottom.TabIndex = 2;
            //
            // btnExport
            //
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(730, 12);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 23);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Exportar";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // ReportForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 500);
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "ReportForm";
            this.Text = "Informes Personalizados";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.ComboBox cboCourse;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.DataGridView dgvReport;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnExport;
    }
}
