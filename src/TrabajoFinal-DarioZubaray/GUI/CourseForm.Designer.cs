
namespace TrabajoFinal_DarioZubaray
{
    partial class CourseForm
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
            lblName = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            lblDescription = new System.Windows.Forms.Label();
            txtDescription = new System.Windows.Forms.TextBox();
            lblStartDate = new System.Windows.Forms.Label();
            dtpStartDate = new System.Windows.Forms.DateTimePicker();
            lblEndDate = new System.Windows.Forms.Label();
            dtpEndDate = new System.Windows.Forms.DateTimePicker();
            lblClassroom = new System.Windows.Forms.Label();
            cbClassroom = new System.Windows.Forms.ComboBox();
            lblStartTime = new System.Windows.Forms.Label();
            mtbStartTime = new System.Windows.Forms.MaskedTextBox();
            lblEndTime = new System.Windows.Forms.Label();
            mtbEndTime = new System.Windows.Forms.MaskedTextBox();
            lblTeachers = new System.Windows.Forms.Label();
            lstAvailableTeachers = new System.Windows.Forms.ListBox();
            lstAssignedTeachers = new System.Windows.Forms.ListBox();
            btnAddTeacher = new System.Windows.Forms.Button();
            btnRemoveTeacher = new System.Windows.Forms.Button();
            lblAvailable = new System.Windows.Forms.Label();
            lblAssigned = new System.Windows.Forms.Label();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnToggleActive = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(14, 17);
            lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(54, 15);
            lblName.TabIndex = 0;
            lblName.Text = "Nombre:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(152, 14);
            txtName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtName.MaxLength = 200;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(349, 23);
            txtName.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new System.Drawing.Point(14, 52);
            lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new System.Drawing.Size(72, 15);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Descripción:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new System.Drawing.Point(152, 48);
            txtDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDescription.MaxLength = 500;
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new System.Drawing.Size(349, 57);
            txtDescription.TabIndex = 3;
            // 
            // lblStartDate
            // 
            lblStartDate.AutoSize = true;
            lblStartDate.Location = new System.Drawing.Point(14, 121);
            lblStartDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStartDate.Name = "lblStartDate";
            lblStartDate.Size = new System.Drawing.Size(73, 15);
            lblStartDate.TabIndex = 4;
            lblStartDate.Text = "Fecha Inicio:";
            // 
            // dtpStartDate
            // 
            dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpStartDate.Location = new System.Drawing.Point(152, 118);
            dtpStartDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new System.Drawing.Size(174, 23);
            dtpStartDate.TabIndex = 5;
            // 
            // lblEndDate
            // 
            lblEndDate.AutoSize = true;
            lblEndDate.Location = new System.Drawing.Point(14, 156);
            lblEndDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEndDate.Name = "lblEndDate";
            lblEndDate.Size = new System.Drawing.Size(60, 15);
            lblEndDate.TabIndex = 6;
            lblEndDate.Text = "Fecha Fin:";
            // 
            // dtpEndDate
            // 
            dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpEndDate.Location = new System.Drawing.Point(152, 152);
            dtpEndDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new System.Drawing.Size(174, 23);
            dtpEndDate.TabIndex = 7;
            // 
            // lblClassroom
            // 
            lblClassroom.AutoSize = true;
            lblClassroom.Location = new System.Drawing.Point(14, 190);
            lblClassroom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblClassroom.Name = "lblClassroom";
            lblClassroom.Size = new System.Drawing.Size(34, 15);
            lblClassroom.TabIndex = 8;
            lblClassroom.Text = "Aula:";
            // 
            // cbClassroom
            // 
            cbClassroom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbClassroom.Location = new System.Drawing.Point(152, 187);
            cbClassroom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbClassroom.Name = "cbClassroom";
            cbClassroom.Size = new System.Drawing.Size(233, 23);
            cbClassroom.TabIndex = 9;
            // 
            // lblStartTime
            // 
            lblStartTime.AutoSize = true;
            lblStartTime.Location = new System.Drawing.Point(14, 229);
            lblStartTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStartTime.Name = "lblStartTime";
            lblStartTime.Size = new System.Drawing.Size(68, 15);
            lblStartTime.TabIndex = 12;
            lblStartTime.Text = "Hora Inicio:";
            // 
            // mtbStartTime
            // 
            mtbStartTime.Location = new System.Drawing.Point(152, 225);
            mtbStartTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mtbStartTime.Mask = "00:00";
            mtbStartTime.Name = "mtbStartTime";
            mtbStartTime.Size = new System.Drawing.Size(69, 23);
            mtbStartTime.TabIndex = 13;
            // 
            // lblEndTime
            // 
            lblEndTime.AutoSize = true;
            lblEndTime.Location = new System.Drawing.Point(14, 263);
            lblEndTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEndTime.Name = "lblEndTime";
            lblEndTime.Size = new System.Drawing.Size(55, 15);
            lblEndTime.TabIndex = 14;
            lblEndTime.Text = "Hora Fin:";
            // 
            // mtbEndTime
            // 
            mtbEndTime.Location = new System.Drawing.Point(152, 260);
            mtbEndTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mtbEndTime.Mask = "00:00";
            mtbEndTime.Name = "mtbEndTime";
            mtbEndTime.Size = new System.Drawing.Size(69, 23);
            mtbEndTime.TabIndex = 15;
            // 
            // lblTeachers
            // 
            lblTeachers.AutoSize = true;
            lblTeachers.Location = new System.Drawing.Point(14, 304);
            lblTeachers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTeachers.Name = "lblTeachers";
            lblTeachers.Size = new System.Drawing.Size(59, 15);
            lblTeachers.TabIndex = 16;
            lblTeachers.Text = "Docentes:";
            // 
            // lstAvailableTeachers
            // 
            lstAvailableTeachers.FormattingEnabled = true;
            lstAvailableTeachers.Location = new System.Drawing.Point(152, 327);
            lstAvailableTeachers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstAvailableTeachers.Name = "lstAvailableTeachers";
            lstAvailableTeachers.Size = new System.Drawing.Size(174, 109);
            lstAvailableTeachers.TabIndex = 18;
            // 
            // lstAssignedTeachers
            // 
            lstAssignedTeachers.FormattingEnabled = true;
            lstAssignedTeachers.Location = new System.Drawing.Point(467, 327);
            lstAssignedTeachers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstAssignedTeachers.Name = "lstAssignedTeachers";
            lstAssignedTeachers.Size = new System.Drawing.Size(174, 109);
            lstAssignedTeachers.TabIndex = 22;
            // 
            // btnAddTeacher
            // 
            btnAddTeacher.Location = new System.Drawing.Point(338, 361);
            btnAddTeacher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddTeacher.Name = "btnAddTeacher";
            btnAddTeacher.Size = new System.Drawing.Size(35, 27);
            btnAddTeacher.TabIndex = 19;
            btnAddTeacher.Text = ">";
            btnAddTeacher.UseVisualStyleBackColor = true;
            btnAddTeacher.Click += btnAddTeacher_Click;
            // 
            // btnRemoveTeacher
            // 
            btnRemoveTeacher.Location = new System.Drawing.Point(338, 396);
            btnRemoveTeacher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRemoveTeacher.Name = "btnRemoveTeacher";
            btnRemoveTeacher.Size = new System.Drawing.Size(35, 27);
            btnRemoveTeacher.TabIndex = 20;
            btnRemoveTeacher.Text = "<";
            btnRemoveTeacher.UseVisualStyleBackColor = true;
            btnRemoveTeacher.Click += btnRemoveTeacher_Click;
            // 
            // lblAvailable
            // 
            lblAvailable.AutoSize = true;
            lblAvailable.Location = new System.Drawing.Point(14, 327);
            lblAvailable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAvailable.Name = "lblAvailable";
            lblAvailable.Size = new System.Drawing.Size(71, 15);
            lblAvailable.TabIndex = 17;
            lblAvailable.Text = "Disponibles:";
            // 
            // lblAssigned
            // 
            lblAssigned.AutoSize = true;
            lblAssigned.Location = new System.Drawing.Point(385, 327);
            lblAssigned.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAssigned.Name = "lblAssigned";
            lblAssigned.Size = new System.Drawing.Size(65, 15);
            lblAssigned.TabIndex = 21;
            lblAssigned.Text = "Asignados:";
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(385, 454);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 27);
            btnSave.TabIndex = 23;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(479, 454);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 27);
            btnCancel.TabIndex = 24;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnToggleActive
            // 
            btnToggleActive.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            btnToggleActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnToggleActive.ForeColor = System.Drawing.Color.White;
            btnToggleActive.Location = new System.Drawing.Point(14, 454);
            btnToggleActive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnToggleActive.Name = "btnToggleActive";
            btnToggleActive.Size = new System.Drawing.Size(105, 27);
            btnToggleActive.TabIndex = 25;
            btnToggleActive.Text = "Inactivar";
            btnToggleActive.UseVisualStyleBackColor = false;
            btnToggleActive.Click += btnToggleActive_Click;
            // 
            // CourseForm
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(658, 497);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblDescription);
            Controls.Add(txtDescription);
            Controls.Add(lblStartDate);
            Controls.Add(dtpStartDate);
            Controls.Add(lblEndDate);
            Controls.Add(dtpEndDate);
            Controls.Add(lblClassroom);
            Controls.Add(cbClassroom);
            Controls.Add(lblStartTime);
            Controls.Add(mtbStartTime);
            Controls.Add(lblEndTime);
            Controls.Add(mtbEndTime);
            Controls.Add(lblTeachers);
            Controls.Add(lblAvailable);
            Controls.Add(lstAvailableTeachers);
            Controls.Add(btnAddTeacher);
            Controls.Add(btnRemoveTeacher);
            Controls.Add(lblAssigned);
            Controls.Add(lstAssignedTeachers);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(btnToggleActive);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CourseForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Curso";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblClassroom;
        private System.Windows.Forms.ComboBox cbClassroom;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.MaskedTextBox mtbStartTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.MaskedTextBox mtbEndTime;
        private System.Windows.Forms.Label lblTeachers;
        private System.Windows.Forms.ListBox lstAvailableTeachers;
        private System.Windows.Forms.ListBox lstAssignedTeachers;
        private System.Windows.Forms.Button btnAddTeacher;
        private System.Windows.Forms.Button btnRemoveTeacher;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.Label lblAssigned;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnToggleActive;
    }
}
