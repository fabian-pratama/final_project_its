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
    public partial class FormTransaksi : Form
    {
        SqlConnection conn = new SqlConnection(
    ConfigurationManager.ConnectionStrings["koneksi"].ConnectionString);

        public FormTransaksi()
        {
            InitializeComponent();
        }

        private void labelSubTotal_Click(object sender, EventArgs e)
        {

        }

        void LoadMenu()
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Id, NamaMenu, Harga, Stok FROM Menu WHERE Stok > 0", conn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbMenu.DataSource = dt;
            cmbMenu.DisplayMember = "NamaMenu";
            cmbMenu.ValueMember = "Id";
            cmbMenu.SelectedIndex = -1;
        }

        void SetupKeranjang()
        {
            dgvKeranjang.Columns.Clear();

            dgvKeranjang.Columns.Add("IdMenu", "IdMenu");
            dgvKeranjang.Columns.Add("NamaMenu", "Menu");
            dgvKeranjang.Columns.Add("Harga", "Harga");
            dgvKeranjang.Columns.Add("Jumlah", "Jumlah");
            dgvKeranjang.Columns.Add("Subtotal", "Subtotal");

            dgvKeranjang.Columns["IdMenu"].Visible = false;
        }

        void HitungTotal()
        {
            int total = 0;
            foreach (DataGridViewRow row in dgvKeranjang.Rows)
            {
                total += Convert.ToInt32(row.Cells["Subtotal"].Value);
            }

            lblTotal.Text = "Total: Rp " + total.ToString("N0");
        }

        private void FormTransaksi_Load(object sender, EventArgs e)
        {
            LoadMenu();
            SetupKeranjang();
            HitungTotal();
        }

        private void cmbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMenu.SelectedIndex == -1) return;

            DataRowView row = (DataRowView)cmbMenu.SelectedItem;

            txtHarga.Text = row["Harga"].ToString();
            txtStok.Text = row["Stok"].ToString();
        }

        private void txtJumlah_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtJumlah.Text, out int jumlah) &&
        int.TryParse(txtHarga.Text, out int harga))
            {
                txtSubtotal.Text = (jumlah * harga).ToString();
            }
            else
            {
                txtSubtotal.Clear();
            }
        }

        void ResetItem()
        {
            cmbMenu.SelectedIndex = -1;
            txtHarga.Clear();
            txtStok.Clear();
            txtJumlah.Clear();
            txtSubtotal.Clear();
        }


        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (cmbMenu.SelectedIndex == -1 ||
        !int.TryParse(txtJumlah.Text, out int jumlah))
            {
                MessageBox.Show("Input tidak valid");
                return;
            }

            int stok = int.Parse(txtStok.Text);
            if (jumlah > stok)
            {
                MessageBox.Show("Jumlah melebihi stok");
                return;
            }

            dgvKeranjang.Rows.Add(
                cmbMenu.SelectedValue,
                cmbMenu.Text,
                txtHarga.Text,
                jumlah,
                txtSubtotal.Text
            );

            HitungTotal();
            ResetItem();
        }

        private void btnSimpanTransaksi_Click(object sender, EventArgs e)
        {
            if (dgvKeranjang.Rows.Count == 0)
            {
                MessageBox.Show("Keranjang kosong");
                return;
            }

            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                // Simpan transaksi
                SqlCommand cmdTransaksi = new SqlCommand(
                    "INSERT INTO Transaksi (TanggalWaktu, TotalHarga) OUTPUT INSERTED.Id VALUES (GETDATE(), @total)",
                    conn, trans);

                int totalHarga = dgvKeranjang.Rows
                    .Cast<DataGridViewRow>()
                    .Sum(r => Convert.ToInt32(r.Cells["Subtotal"].Value));

                cmdTransaksi.Parameters.AddWithValue("@total", totalHarga);
                int transaksiId = (int)cmdTransaksi.ExecuteScalar();

                // Simpan detail + update stok
                foreach (DataGridViewRow row in dgvKeranjang.Rows)
                {
                    SqlCommand cmdDetail = new SqlCommand(
                        @"INSERT INTO DetailTransaksi
                  (TransaksiId, MenuId, Jumlah, HargaSaatItu, Subtotal)
                  VALUES (@tid, @mid, @jml, @harga, @sub)", conn, trans);

                    cmdDetail.Parameters.AddWithValue("@tid", transaksiId);
                    cmdDetail.Parameters.AddWithValue("@mid", row.Cells["IdMenu"].Value);
                    cmdDetail.Parameters.AddWithValue("@jml", row.Cells["Jumlah"].Value);
                    cmdDetail.Parameters.AddWithValue("@harga", row.Cells["Harga"].Value);
                    cmdDetail.Parameters.AddWithValue("@sub", row.Cells["Subtotal"].Value);
                    cmdDetail.ExecuteNonQuery();

                    SqlCommand cmdStok = new SqlCommand(
                        "UPDATE Menu SET Stok = Stok - @jml WHERE Id=@id", conn, trans);
                    cmdStok.Parameters.AddWithValue("@jml", row.Cells["Jumlah"].Value);
                    cmdStok.Parameters.AddWithValue("@id", row.Cells["IdMenu"].Value);
                    cmdStok.ExecuteNonQuery();
                }

                trans.Commit();
                MessageBox.Show("Transaksi berhasil disimpan");

                dgvKeranjang.Rows.Clear();
                HitungTotal();
                LoadMenu();
            }
            catch
            {
                trans.Rollback();
                MessageBox.Show("Gagal menyimpan transaksi");
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
