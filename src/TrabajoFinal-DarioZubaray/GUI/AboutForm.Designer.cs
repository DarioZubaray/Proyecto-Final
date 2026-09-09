
namespace TrabajoFinal_DarioZubaray
{
    partial class AboutForm
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
            lblAppName = new System.Windows.Forms.Label();
            lblVersion = new System.Windows.Forms.Label();
            lblCopyright = new System.Windows.Forms.Label();
            lblRepositoryLabel = new System.Windows.Forms.Label();
            linkRepository = new System.Windows.Forms.LinkLabel();
            btnClose = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblAppName
            // 
            lblAppName.AutoSize = true;
            lblAppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblAppName.Location = new System.Drawing.Point(111, 23);
            lblAppName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new System.Drawing.Size(237, 25);
            lblAppName.TabIndex = 0;
            lblAppName.Text = "Aplicativo Académico";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new System.Drawing.Point(30, 75);
            lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(55, 15);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Versión X";
            // 
            // lblCopyright
            // 
            lblCopyright.AutoSize = true;
            lblCopyright.Location = new System.Drawing.Point(30, 104);
            lblCopyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new System.Drawing.Size(178, 15);
            lblCopyright.TabIndex = 2;
            lblCopyright.Text = "Copyright © 2026 Darío Zubaray";
            // 
            // lblRepositoryLabel
            // 
            lblRepositoryLabel.AutoSize = true;
            lblRepositoryLabel.Location = new System.Drawing.Point(30, 144);
            lblRepositoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblRepositoryLabel.Name = "lblRepositoryLabel";
            lblRepositoryLabel.Size = new System.Drawing.Size(70, 15);
            lblRepositoryLabel.TabIndex = 3;
            lblRepositoryLabel.Text = "Repositorio:";
            // 
            // linkRepository
            // 
            linkRepository.AutoSize = true;
            linkRepository.Location = new System.Drawing.Point(121, 144);
            linkRepository.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkRepository.Name = "linkRepository";
            linkRepository.Size = new System.Drawing.Size(227, 15);
            linkRepository.TabIndex = 4;
            linkRepository.TabStop = true;
            linkRepository.Text = "github.com/DarioZubaray/Proyecto-Final";
            linkRepository.LinkClicked += linkRepository_LinkClicked;
            // 
            // btnClose
            // 
            btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnClose.Location = new System.Drawing.Point(177, 188);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(88, 27);
            btnClose.TabIndex = 5;
            btnClose.Text = "Cerrar";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(460, 227);
            Controls.Add(btnClose);
            Controls.Add(linkRepository);
            Controls.Add(lblRepositoryLabel);
            Controls.Add(lblCopyright);
            Controls.Add(lblVersion);
            Controls.Add(lblAppName);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Acerca de";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblRepositoryLabel;
        private System.Windows.Forms.LinkLabel linkRepository;
        private System.Windows.Forms.Button btnClose;
    }
}
