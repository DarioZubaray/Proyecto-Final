using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public class ActivityHistoryForm : Form
    {
        #region Propiedades
        private readonly UserBE _user;
        private readonly IActivityBLL _activityBLL;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _pageSize = 10;

        private DataGridView dgvActivities;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPageInfo;
        private ComboBox cbPageSize;
        private Label lblPageSize;
        #endregion

        #region Constructor
        public ActivityHistoryForm(UserBE user)
        {
            _user = user;
            _activityBLL = ServiceLocatorBLL.CreateActivityBLL();

            InitializeControls();
            ApplyResources();
            LoadPage();
        }
        #endregion

        #region UI
        private void InitializeControls()
        {
            this.Text = "Historial de Actividad";
            this.Size = new Size(760, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(640, 400);

            dgvActivities = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                MultiSelect = false
            };

            dgvActivities.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDate",
                DataPropertyName = "CreatedAt",
                Width = 150,
                ReadOnly = true
            });
            dgvActivities.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAction",
                DataPropertyName = "Action",
                Width = 160,
                ReadOnly = true
            });
            dgvActivities.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colForm",
                DataPropertyName = "FormName",
                Width = 200,
                ReadOnly = true
            });
            dgvActivities.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDescription",
                DataPropertyName = "Description",
                Width = 220,
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            btnPrev = new Button { Text = "‹ Anterior", Width = 90 };
            btnPrev.Click += (s, e) => ChangePage(_currentPage - 1);

            btnNext = new Button { Text = "Siguiente ›", Width = 90 };
            btnNext.Click += (s, e) => ChangePage(_currentPage + 1);

            cbPageSize = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 60 };
            cbPageSize.Items.AddRange(new object[] { 10, 25, 50 });
            cbPageSize.SelectedItem = 10;

            lblPageSize = new Label { Text = "Tamaño de página:", AutoSize = true };

            lblPageInfo = new Label
            {
                Text = "Página 1 de 1",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var pageBar = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Padding = new Padding(6)
            };
            pageBar.ColumnCount = 5;
            pageBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pageBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pageBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pageBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pageBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            pageBar.Controls.Add(btnPrev, 0, 0);
            pageBar.Controls.Add(lblPageInfo, 1, 0);
            pageBar.Controls.Add(new Panel(), 2, 0);
            pageBar.Controls.Add(lblPageSize, 3, 0);
            pageBar.Controls.Add(cbPageSize, 4, 0);

            this.Controls.Add(dgvActivities);
            this.Controls.Add(pageBar);

            cbPageSize.SelectedIndexChanged += (s, e) =>
            {
                _pageSize = (int)cbPageSize.SelectedItem;
                _currentPage = 1;
                LoadPage();
            };
        }

        private void ApplyResources()
        {
            this.Text = Resources.ActivityHistory_Title;
            dgvActivities.Columns["colDate"].HeaderText = Resources.ActivityHistory_ColumnDate;
            dgvActivities.Columns["colAction"].HeaderText = Resources.ActivityHistory_ColumnAction;
            dgvActivities.Columns["colForm"].HeaderText = Resources.ActivityHistory_ColumnForm;
            dgvActivities.Columns["colDescription"].HeaderText = Resources.ActivityHistory_ColumnDescription;
            btnPrev.Text = Resources.ActivityHistory_Prev;
            btnNext.Text = Resources.ActivityHistory_Next;
            lblPageSize.Text = Resources.ActivityHistory_PageSize;
            UpdatePaginationLabels();
        }
        #endregion

        #region Data
        private void LoadPage()
        {
            if (_user == null)
            {
                return;
            }

            _totalPages = _activityBLL.TotalPages(_user.Id, _pageSize);
            if (_totalPages < 1) _totalPages = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;

            List<ActivityLogBE> logs = _activityBLL.GetByUserPaginated(_user.Id, _currentPage, _pageSize);

            dgvActivities.DataSource = null;
            dgvActivities.DataSource = logs;

            foreach (DataGridViewRow row in dgvActivities.Rows)
            {
                var log = (ActivityLogBE)row.DataBoundItem;
                if (log.CreatedAt != null)
                {
                    row.Cells["colDate"].Value = log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                }
                row.Cells["colAction"].Value = TranslateAction(log.Action);
            }

            UpdatePaginationLabels();
        }

        private string TranslateAction(string action)
        {
            switch (action)
            {
                case "LOGIN":
                    return Resources.ActivityHistory_ActionLogin;
                case "LOGOUT":
                    return Resources.ActivityHistory_ActionLogout;
                case "FORM_ACCESS":
                    return Resources.ActivityHistory_ActionFormAccess;
                default:
                    return action;
            }
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > _totalPages)
            {
                return;
            }
            _currentPage = page;
            LoadPage();
        }

        private void UpdatePaginationLabels()
        {
            if (lblPageInfo != null)
            {
                lblPageInfo.Text = string.Format(Resources.ActivityHistory_PageInfo, _currentPage, _totalPages);
            }
        }
        #endregion
    }
}
