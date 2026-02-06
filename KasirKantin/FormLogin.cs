using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace KasirKantin
{
    public partial class FormLogin : Form
    {
        SqlConnection conn = new SqlConnection(
    ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString);
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Username dan password wajib diisi");
                return;
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM [User] WHERE Username=@u AND Password=@p",
                conn);

            cmd.Parameters.AddWithValue("@u", txtUsername.Text);
            cmd.Parameters.AddWithValue("@p", txtPassword.Text);

            conn.Open();
            int hasil = (int)cmd.ExecuteScalar();
            conn.Close();

            if (hasil > 0)
            {
                Global.Username = txtUsername.Text;

                MessageBox.Show("Login berhasil");

                FormDashboard fd = new FormDashboard();
                fd.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username atau password salah");
            }
        }

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
