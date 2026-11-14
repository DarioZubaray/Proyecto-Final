
namespace TrabajoFinal_DarioZubaray
{
    partial class ChangePasswordForm
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            btnChange = new System.Windows.Forms.Button();
            txtConfirmPassword = new System.Windows.Forms.TextBox();
            lblConfirmPassword = new System.Windows.Forms.Label();
            txtNewPassword = new System.Windows.Forms.TextBox();
            lblNewPassword = new System.Windows.Forms.Label();
            txtCurrentPassword = new System.Windows.Forms.TextBox();
            lblCurrentPassword = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnChange);
            groupBox1.Controls.Add(txtConfirmPassword);
            groupBox1.Controls.Add(lblConfirmPassword);
            groupBox1.Controls.Add(txtNewPassword);
            groupBox1.Controls.Add(lblNewPassword);
            groupBox1.Controls.Add(txtCurrentPassword);
            groupBox1.Controls.Add(lblCurrentPassword);
            groupBox1.Location = new System.Drawing.Point(14, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(420, 208);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "☺";
            // 
            // btnChange
            // 
            btnChange.Location = new System.Drawing.Point(163, 150);
            btnChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnChange.Name = "btnChange";
            btnChange.Size = new System.Drawing.Size(88, 27);
            btnChange.TabIndex = 6;
            btnChange.Text = "Cambiar";
            btnChange.UseVisualStyleBackColor = true;
            btnChange.Click += btnChange_Click;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new System.Drawing.Point(163, 106);
            txtConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PasswordChar = '*';
            txtConfirmPassword.Size = new System.Drawing.Size(233, 23);
            txtConfirmPassword.TabIndex = 5;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new System.Drawing.Point(21, 110);
            lblConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new System.Drawing.Size(122, 15);
            lblConfirmPassword.TabIndex = 4;
            lblConfirmPassword.Text = "Confirmar contraseña";
            // 
            // txtNewPassword
            // 
            txtNewPassword.Location = new System.Drawing.Point(163, 68);
            txtNewPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '*';
            txtNewPassword.Size = new System.Drawing.Size(233, 23);
            txtNewPassword.TabIndex = 3;
            // 
            // lblNewPassword
            // 
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new System.Drawing.Point(21, 72);
            lblNewPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new System.Drawing.Size(102, 15);
            lblNewPassword.TabIndex = 2;
            lblNewPassword.Text = "Nueva contraseña";
            // 
            // txtCurrentPassword
            // 
            txtCurrentPassword.Location = new System.Drawing.Point(163, 30);
            txtCurrentPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCurrentPassword.Name = "txtCurrentPassword";
            txtCurrentPassword.PasswordChar = '*';
            txtCurrentPassword.Size = new System.Drawing.Size(233, 23);
            txtCurrentPassword.TabIndex = 1;
            // 
            // lblCurrentPassword
            // 
            lblCurrentPassword.AutoSize = true;
            lblCurrentPassword.Location = new System.Drawing.Point(21, 33);
            lblCurrentPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCurrentPassword.Name = "lblCurrentPassword";
            lblCurrentPassword.Size = new System.Drawing.Size(102, 15);
            lblCurrentPassword.TabIndex = 0;
            lblCurrentPassword.Text = "Contraseña actual";
            // 
            // ChangePasswordForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(449, 231);
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangePasswordForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Cambiar Contraseña";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCurrentPassword;
        private System.Windows.Forms.TextBox txtCurrentPassword;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnChange;
    }
}
