
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
            lblInfo = new System.Windows.Forms.Label();
            btnGenerate = new System.Windows.Forms.Button();
            btnRestore = new System.Windows.Forms.Button();
            btnRestoreScript = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new System.Drawing.Point(10, 19);
            lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new System.Drawing.Size(541, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Genere un respaldo, restaure desde un archivo .bak o cargue un script .sql con datos de demostración";
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new System.Drawing.Point(8, 91);
            btnGenerate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new System.Drawing.Size(175, 32);
            btnGenerate.TabIndex = 1;
            btnGenerate.Text = "Generar respaldo";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // btnRestore
            // 
            btnRestore.Location = new System.Drawing.Point(191, 91);
            btnRestore.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new System.Drawing.Size(175, 32);
            btnRestore.TabIndex = 2;
            btnRestore.Text = "Restaurar respaldo";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += btnRestore_Click;
            // 
            // btnRestoreScript
            // 
            btnRestoreScript.Location = new System.Drawing.Point(374, 91);
            btnRestoreScript.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRestoreScript.Name = "btnRestoreScript";
            btnRestoreScript.Size = new System.Drawing.Size(175, 32);
            btnRestoreScript.TabIndex = 3;
            btnRestoreScript.Text = "Restaurar desde script";
            btnRestoreScript.UseVisualStyleBackColor = true;
            btnRestoreScript.Click += btnRestoreScript_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnRestoreScript);
            groupBox1.Controls.Add(btnGenerate);
            groupBox1.Controls.Add(lblInfo);
            groupBox1.Controls.Add(btnRestore);
            groupBox1.Location = new System.Drawing.Point(13, 12);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(561, 196);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "☺";
            // 
            // BackupForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(594, 231);
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "BackupForm";
            Text = "Respaldo y Restauración";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnRestoreScript;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
