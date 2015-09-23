using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ensamblador_HCS12
{
    public partial class frmSplash : Form
    {
        public frmSplash(int segundos)
        {
            InitializeComponent();
            timer1.Interval = segundos * 1000;    // pasamos de segundos a milisegundos

            if (!timer1.Enabled)
                timer1.Enabled = true;    // Activamos el Timer si no esta Enabled (Activado)
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();     // Se para el timer.
            this.Close();      // Cerramos el formulario.
        }
    }
}
