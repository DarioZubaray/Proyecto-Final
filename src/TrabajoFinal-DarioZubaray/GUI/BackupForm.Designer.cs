
namespace TrabajoFinal_DarioZubaray
{
    partial class BackupForm
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblInfo);
            this.panelTop.Controls.Add(this.btnGenerate);
            this.panelTop.Controls.Add(this.btnRestore);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(584, 80);
            this.panelTop.TabIndex = 0;
            //
            // lblInfo
            //
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 17);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(340, 13);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Genere un respaldo de la base de datos o restaure desde un archivo .bak";
            //
            // btnGenerate
            //
            this.btnGenerate.Location = new System.Drawing.Point(15, 42);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(150, 28);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generar respaldo";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // btnRestore
            //
            this.btnRestore.Location = new System.Drawing.Point(180, 42);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(150, 28);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restaurar respaldo";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            //
            // BackupForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 261);
            this.Controls.Add(this.panelTop);
            this.Name = "BackupForm";
            this.Text = "Respaldo y Restauración";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnRestore;
    }
}
