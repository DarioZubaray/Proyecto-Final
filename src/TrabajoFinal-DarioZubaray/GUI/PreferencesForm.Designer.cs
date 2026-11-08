
namespace TrabajoFinal_DarioZubaray
{
    partial class PreferencesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new System.Windows.Forms.GroupBox();
            lblTheme = new System.Windows.Forms.Label();
            cbTheme = new System.Windows.Forms.ComboBox();
            btnSave = new System.Windows.Forms.Button();
            cbLanguage = new System.Windows.Forms.ComboBox();
            lblLanguage = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblTheme);
            groupBox1.Controls.Add(cbTheme);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(cbLanguage);
            groupBox1.Controls.Add(lblLanguage);
            groupBox1.Location = new System.Drawing.Point(14, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(420, 196);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "☺";
            // 
            // lblTheme
            // 
            lblTheme.AutoSize = true;
            lblTheme.Location = new System.Drawing.Point(21, 87);
            lblTheme.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTheme.Name = "lblTheme";
            lblTheme.Size = new System.Drawing.Size(35, 15);
            lblTheme.TabIndex = 2;
            lblTheme.Text = "Tema";
            // 
            // cbTheme
            // 
            cbTheme.FormattingEnabled = true;
            cbTheme.Location = new System.Drawing.Point(163, 83);
            cbTheme.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbTheme.Name = "cbTheme";
            cbTheme.Size = new System.Drawing.Size(233, 23);
            cbTheme.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(163, 153);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 27);
            btnSave.TabIndex = 4;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // cbLanguage
            // 
            cbLanguage.FormattingEnabled = true;
            cbLanguage.Location = new System.Drawing.Point(163, 30);
            cbLanguage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbLanguage.Name = "cbLanguage";
            cbLanguage.Size = new System.Drawing.Size(233, 23);
            cbLanguage.TabIndex = 1;
            // 
            // lblLanguage
            // 
            lblLanguage.AutoSize = true;
            lblLanguage.Location = new System.Drawing.Point(21, 33);
            lblLanguage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Size = new System.Drawing.Size(44, 15);
            lblLanguage.TabIndex = 0;
            lblLanguage.Text = "Idioma";
            // 
            // PreferencesForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(449, 231);
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PreferencesForm";
            Text = "Preferences";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cbTheme;
    }
}