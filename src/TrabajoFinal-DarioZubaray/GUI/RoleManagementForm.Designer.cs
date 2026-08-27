namespace TrabajoFinal_DarioZubaray
{
    partial class RoleManagementForm
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
            this.lblRoleName = new System.Windows.Forms.Label();
            this.txtRoleName = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.lbRoles = new System.Windows.Forms.ListBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lbAvailable = new System.Windows.Forms.ListBox();
            this.lbAssigned = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblAvailable = new System.Windows.Forms.Label();
            this.lblAssigned = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // panelTop
            this.panelTop.Controls.Add(this.lblRoleName);
            this.panelTop.Controls.Add(this.txtRoleName);
            this.panelTop.Controls.Add(this.btnCreate);
            this.panelTop.Controls.Add(this.btnDelete);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            this.panelTop.Size = new System.Drawing.Size(750, 50);

            // lblRoleName
            this.lblRoleName.AutoSize = true;
            this.lblRoleName.Location = new System.Drawing.Point(13, 16);
            this.lblRoleName.Name = "lblRoleName";
            this.lblRoleName.Size = new System.Drawing.Size(32, 13);
            this.lblRoleName.Text = "Rol:";

            // txtRoleName
            this.txtRoleName.Location = new System.Drawing.Point(55, 13);
            this.txtRoleName.Name = "txtRoleName";
            this.txtRoleName.Size = new System.Drawing.Size(200, 20);

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(270, 11);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(100, 25);
            this.btnCreate.Text = "Crear Rol";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(380, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 25);
            this.btnDelete.Text = "Eliminar";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // panelCenter
            this.panelCenter.Controls.Add(this.lbRoles);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Padding = new System.Windows.Forms.Padding(10);
            this.panelCenter.Size = new System.Drawing.Size(750, 200);

            // lbRoles
            this.lbRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRoles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lbRoles.FormattingEnabled = true;
            this.lbRoles.ItemHeight = 17;
            this.lbRoles.Name = "lbRoles";
            this.lbRoles.SelectedIndexChanged += new System.EventHandler(this.lbRoles_SelectedIndexChanged);

            // panelBottom
            this.panelBottom.Controls.Add(this.lbAvailable);
            this.panelBottom.Controls.Add(this.lbAssigned);
            this.panelBottom.Controls.Add(this.btnAdd);
            this.panelBottom.Controls.Add(this.btnRemove);
            this.panelBottom.Controls.Add(this.lblAvailable);
            this.panelBottom.Controls.Add(this.lblAssigned);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(10);
            this.panelBottom.Size = new System.Drawing.Size(750, 250);

            // lblAvailable
            this.lblAvailable.AutoSize = true;
            this.lblAvailable.Location = new System.Drawing.Point(13, 10);
            this.lblAvailable.Name = "lblAvailable";
            this.lblAvailable.Text = "Disponibles";

            // lbAvailable
            this.lbAvailable.FormattingEnabled = true;
            this.lbAvailable.Location = new System.Drawing.Point(16, 28);
            this.lbAvailable.Name = "lbAvailable";
            this.lbAvailable.Size = new System.Drawing.Size(280, 212);

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(310, 100);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.Text = ">>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // btnRemove
            this.btnRemove.Location = new System.Drawing.Point(310, 140);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 30);
            this.btnRemove.Text = "<<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);

            // lblAssigned
            this.lblAssigned.AutoSize = true;
            this.lblAssigned.Location = new System.Drawing.Point(420, 10);
            this.lblAssigned.Name = "lblAssigned";
            this.lblAssigned.Text = "Asignados";

            // lbAssigned
            this.lbAssigned.FormattingEnabled = true;
            this.lbAssigned.Location = new System.Drawing.Point(423, 28);
            this.lbAssigned.Name = "lbAssigned";
            this.lbAssigned.Size = new System.Drawing.Size(280, 212);
            this.lbAssigned.SelectedIndexChanged += new System.EventHandler(this.lbAssigned_SelectedIndexChanged);

            // RoleManagementForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 500);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "RoleManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Roles";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelCenter.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblRoleName;
        private System.Windows.Forms.TextBox txtRoleName;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.ListBox lbRoles;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.ListBox lbAvailable;
        private System.Windows.Forms.ListBox lbAssigned;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Label lblAssigned;
    }
}
