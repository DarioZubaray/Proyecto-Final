using System.Drawing;
using System.Windows.Forms;

namespace TrabajoFinal_DarioZubaray
{
    public class TestReportsForm : Form
    {
        public TestReportsForm()
        {
            this.Text = "Reportes";
            this.Size = new Size(400, 300);

            var lbl = new Label
            {
                Text = "Formulario de Reportes\n(Prueba de funcionalidad)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }
}
