
namespace TrabajoFinal_DarioZubaray
{
    partial class LoginForm
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
            lblUser = new System.Windows.Forms.Label();
            txtUser = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            lblDbStatus = new System.Windows.Forms.Label();
            btnRetry = new System.Windows.Forms.Button();
            lblMessage = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            btnLogin = new System.Windows.Forms.Button();
            lblPass = new System.Windows.Forms.Label();
            txtPass = new System.Windows.Forms.TextBox();
            lblDeveloper = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblUser.Location = new System.Drawing.Point(5, 15);
            lblUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUser.Name = "lblUser";
            lblUser.Size = new System.Drawing.Size(57, 17);
            lblUser.TabIndex = 0;
            lblUser.Text = "Usuario";
            // 
            // txtUser
            // 
            txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtUser.Location = new System.Drawing.Point(121, 15);
            txtUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtUser.Name = "txtUser";
            txtUser.Size = new System.Drawing.Size(174, 23);
            txtUser.TabIndex = 1;
            txtUser.KeyDown += txtUser_KeyDown;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblDbStatus);
            groupBox1.Controls.Add(btnRetry);
            groupBox1.Controls.Add(lblMessage);
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(lblDeveloper);
            groupBox1.Location = new System.Drawing.Point(14, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(583, 346);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "☻";
            // 
            // lblDbStatus
            // 
            lblDbStatus.AutoSize = true;
            lblDbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            lblDbStatus.ForeColor = System.Drawing.Color.SeaGreen;
            lblDbStatus.Location = new System.Drawing.Point(8, 330);
            lblDbStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDbStatus.Name = "lblDbStatus";
            lblDbStatus.Size = new System.Drawing.Size(100, 13);
            lblDbStatus.TabIndex = 8;
            lblDbStatus.Text = "Servidor conectado";
            // 
            // btnRetry
            // 
            btnRetry.Location = new System.Drawing.Point(20, 300);
            btnRetry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRetry.Name = "btnRetry";
            btnRetry.Size = new System.Drawing.Size(71, 27);
            btnRetry.TabIndex = 9;
            btnRetry.Text = "Reintentar";
            btnRetry.UseVisualStyleBackColor = true;
            btnRetry.Visible = false;
            btnRetry.Click += btnRetry_Click;
            // 
            // lblMessage
            // 
            lblMessage.ForeColor = System.Drawing.Color.IndianRed;
            lblMessage.Location = new System.Drawing.Point(0, 218);
            lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(583, 20);
            lblMessage.TabIndex = 7;
            lblMessage.Text = "Usuario o Contraseña no válidos!";
            lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblMessage.Visible = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(txtUser);
            panel1.Controls.Add(lblUser);
            panel1.Controls.Add(btnLogin);
            panel1.Controls.Add(lblPass);
            panel1.Controls.Add(txtPass);
            panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            panel1.Location = new System.Drawing.Point(137, 99);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(306, 115);
            panel1.TabIndex = 6;
            // 
            // btnLogin
            // 
            btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnLogin.Location = new System.Drawing.Point(180, 84);
            btnLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(117, 27);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Iniciar Sesión";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblPass.Location = new System.Drawing.Point(5, 50);
            lblPass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPass.Name = "lblPass";
            lblPass.Size = new System.Drawing.Size(81, 17);
            lblPass.TabIndex = 2;
            lblPass.Text = "Contraseña";
            // 
            // txtPass
            // 
            txtPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtPass.Location = new System.Drawing.Point(121, 50);
            txtPass.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtPass.Name = "txtPass";
            txtPass.PasswordChar = '*';
            txtPass.Size = new System.Drawing.Size(174, 23);
            txtPass.TabIndex = 3;
            txtPass.KeyDown += txtPass_KeyDown;
            // 
            // lblDeveloper
            // 
            lblDeveloper.AutoSize = true;
            lblDeveloper.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            lblDeveloper.Location = new System.Drawing.Point(429, 330);
            lblDeveloper.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDeveloper.Name = "lblDeveloper";
            lblDeveloper.Size = new System.Drawing.Size(154, 13);
            lblDeveloper.TabIndex = 5;
            lblDeveloper.Text = "desarrollado por Darío Zubaray";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(617, 370);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Aplicativo Académico";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblDeveloper;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblDbStatus;
        private System.Windows.Forms.Button btnRetry;
    }
}