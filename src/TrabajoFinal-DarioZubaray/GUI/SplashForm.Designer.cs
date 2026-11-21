
namespace TrabajoFinal_DarioZubaray
{
    partial class SplashForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Aplicativo Académico";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblStatus
            //
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(20, 74);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 20);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Verificando conexión...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // progress
            //
            this.progress.Location = new System.Drawing.Point(20, 104);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(400, 18);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress.MarqueeAnimationSpeed = 30;
            this.progress.TabIndex = 2;
            //
            // SplashForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 140);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aplicativo Académico";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progress;
    }
}
