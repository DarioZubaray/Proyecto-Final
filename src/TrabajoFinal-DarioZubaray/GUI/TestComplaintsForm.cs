using System.Drawing;
using System.Windows.Forms;

namespace TrabajoFinal_DarioZubaray
{
    public class TestComplaintsForm : Form
    {
        public TestComplaintsForm()
        {
            this.Text = "Quejas";
            this.Size = new Size(400, 300);

            var lbl = new Label
            {
                Text = "Formulario de Quejas\n(Prueba de funcionalidad)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }
}
