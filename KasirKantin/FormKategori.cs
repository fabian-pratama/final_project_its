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


namespace KasirKantin
{
    public partial class FormKategori : Form
    {
        SqlConnection conn = new SqlConnection(
    System.Configuration.ConfigurationManager
    .ConnectionStrings["koneksi"].ConnectionString);

        int idKategori = 0;

        public FormKategori()
        {
            InitializeComponent();
        }

        void LoadKategori()
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Id, NamaKategori FROM Kategori", conn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvKategori.DataSource = dt;
            dgvKategori.Columns[0].HeaderText = "ID";
            dgvKategori.Columns[1].HeaderText = "Nama Kategori";
        }

        void ResetForm()
        {
            txtNamaKategori.Clear();
            idKategori = 0;

            btnTambah.Enabled = true;
            btnUpdate.Enabled = false;
            btnHapus.Enabled = false;
        }

        private void FormKategori_Load(object sender, EventArgs e)
        {
            LoadKategori();
            ResetForm();
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (txtNamaKategori.Text.Trim() == "")
            {
                MessageBox.Show("Nama kategori tidak boleh kosong");
                return;
            }

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Kategori (NamaKategori) VALUES (@nama)", conn);
            cmd.Parameters.AddWithValue("@nama", txtNamaKategori.Text);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Kategori berhasil ditambahkan");

            LoadKategori();
            ResetForm();
        }

        private void dgvKategori_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                idKategori = Convert.ToInt32(
                    dgvKategori.Rows[e.RowIndex].Cells[0].Value);

                txtNamaKategori.Text =
                    dgvKategori.Rows[e.RowIndex].Cells[1].Value.ToString();

                btnTambah.Enabled = false;
                btnUpdate.Enabled = true;
                btnHapus.Enabled = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNamaKategori.Text.Trim() == "")
            {
                MessageBox.Show("Nama kategori tidak boleh kosong");
                return;
            }

            SqlCommand cmd = new SqlCommand(
                "UPDATE Kategori SET NamaKategori=@nama WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@nama", txtNamaKategori.Text);
            cmd.Parameters.AddWithValue("@id", idKategori);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Kategori berhasil diupdate");

            LoadKategori();
            ResetForm();
        }

        bool KategoriDipakai()
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Menu WHERE KategoriId=@id", conn);
            cmd.Parameters.AddWithValue("@id", idKategori);

            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            return count > 0;
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (KategoriDipakai())
            {
                MessageBox.Show("Kategori sedang digunakan oleh menu");
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Yakin ingin menghapus kategori?",
                "Konfirmasi",
                MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Kategori WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", idKategori);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Kategori berhasil dihapus");

                LoadKategori();
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
