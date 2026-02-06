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
    public partial class FormMenu : Form
    {
        SqlConnection conn = new SqlConnection(
    ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString);

        int idMenu = 0;

        public FormMenu()
        {
            InitializeComponent();
        }

        void LoadKategori()
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Id, NamaKategori FROM Kategori", conn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbKategori.DataSource = dt;
            cmbKategori.DisplayMember = "NamaKategori";
            cmbKategori.ValueMember = "Id";
        }

        void LoadMenu()
        {
            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT m.Id, m.NamaMenu, m.Harga, m.Stok, k.NamaKategori
          FROM Menu m
          JOIN Kategori k ON m.KategoriId = k.Id", conn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvMenu.DataSource = dt;

            dgvMenu.Columns[0].HeaderText = "ID";
            dgvMenu.Columns[1].HeaderText = "Nama Menu";
            dgvMenu.Columns[2].HeaderText = "Harga";
            dgvMenu.Columns[3].HeaderText = "Stok";
            dgvMenu.Columns[4].HeaderText = "Kategori";
        }

        void ResetForm()
        {
            txtNamaMenu.Clear();
            txtHarga.Clear();
            txtStok.Clear();
            cmbKategori.SelectedIndex = -1;

            idMenu = 0;
            btnSimpan.Enabled = true;
            btnUpdate.Enabled = false;
            btnHapus.Enabled = false;
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            LoadKategori();
            LoadMenu();
            ResetForm();
        }

        bool Validasi()
        {
            if (txtNamaMenu.Text.Trim() == "")
            {
                MessageBox.Show("Nama menu tidak boleh kosong");
                return false;
            }

            if (!int.TryParse(txtHarga.Text, out _))
            {
                MessageBox.Show("Harga harus angka");
                return false;
            }

            if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
            {
                MessageBox.Show("Stok minimal 0");
                return false;
            }

            if (cmbKategori.SelectedIndex == -1)
            {
                MessageBox.Show("Kategori wajib dipilih");
                return false;
            }

            return true;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Menu 
          (NamaMenu, Harga, Stok, KategoriId)
          VALUES (@nama, @harga, @stok, @kategori)", conn);

            cmd.Parameters.AddWithValue("@nama", txtNamaMenu.Text);
            cmd.Parameters.AddWithValue("@harga", txtHarga.Text);
            cmd.Parameters.AddWithValue("@stok", txtStok.Text);
            cmd.Parameters.AddWithValue("@kategori", cmbKategori.SelectedValue);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Menu berhasil disimpan");

            LoadMenu();
            ResetForm();
        }

        private void dgvMenu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                idMenu = Convert.ToInt32(dgvMenu.Rows[e.RowIndex].Cells[0].Value);
                txtNamaMenu.Text = dgvMenu.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtHarga.Text = dgvMenu.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtStok.Text = dgvMenu.Rows[e.RowIndex].Cells[3].Value.ToString();

                cmbKategori.Text = dgvMenu.Rows[e.RowIndex].Cells[4].Value.ToString();

                btnSimpan.Enabled = false;
                btnUpdate.Enabled = true;
                btnHapus.Enabled = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;

            SqlCommand cmd = new SqlCommand(
                @"UPDATE Menu SET 
          NamaMenu=@nama,
          Harga=@harga,
          Stok=@stok,
          KategoriId=@kategori
          WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@nama", txtNamaMenu.Text);
            cmd.Parameters.AddWithValue("@harga", txtHarga.Text);
            cmd.Parameters.AddWithValue("@stok", txtStok.Text);
            cmd.Parameters.AddWithValue("@kategori", cmbKategori.SelectedValue);
            cmd.Parameters.AddWithValue("@id", idMenu);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Menu berhasil diupdate");

            LoadMenu();
            ResetForm();
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
        "Yakin ingin menghapus menu?",
        "Konfirmasi",
        MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Menu WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", idMenu);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Menu berhasil dihapus");

                LoadMenu();
                ResetForm();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
