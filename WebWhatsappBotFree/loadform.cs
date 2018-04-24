using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//esrefyildirici.com
namespace WebWhatsappBotFree
{
    public partial class loadform : Form
    {
        public loadform()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0)
            {

                this.Opacity += 0.1;
            }
            else
            {
                timer1.Enabled = false;
                Form aaa = new Anaform();
                aaa.Show();
                this.Hide();
            }
        }
    }
}
