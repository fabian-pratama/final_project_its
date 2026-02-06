using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KasirKantin
{
    public partial class FormDashboard : Form
    {
        public FormDashboard()
        {
            InitializeComponent();
        }

        private void btnKategori_Click(object sender, EventArgs e)
        {
            FormKategori fk = new FormKategori();
            fk.ShowDialog();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            FormMenu fm = new FormMenu();
            fm.ShowDialog();
        }

        private void btnTransaksi_Click(object sender, EventArgs e)
        {
            FormTransaksi ft = new FormTransaksi();
            ft.ShowDialog();
        }

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
        "Yakin ingin keluar aplikasi?",
        "Konfirmasi",
        MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            lblSapaan.Text = "Halo, " + Global.Username + "!";
        }
    }
}
