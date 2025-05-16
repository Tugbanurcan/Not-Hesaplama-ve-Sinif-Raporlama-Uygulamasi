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


namespace sinav_takip_uygulamasi
{
    public partial class OgrenciKayitOl : Form
    {
        SinavTakipSinifi sinavTakip = new SinavTakipSinifi();
        public OgrenciKayitOl()
        {
            InitializeComponent();
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {

            string numara = txtNumara.Text.Trim();
            string ad = txtAd.Text.Trim();
            string soyad = txtSoyad.Text.Trim();
            string sifre = txtSifre.Text;

            if (numara == "" || ad == "" || soyad == "" || sifre == "")
            {
                MessageBox.Show("Tüm alanları doldurunuz.");
                return;
            }

            try
            {
                using (SqlConnection conn = sinavTakip.Baglanti())
                {
                    conn.Open();
                    string sql = "INSERT INTO Ogrenciler (Numara, Ad, Soyad, Sifre) VALUES (@Numara, @Ad, @Soyad, @Sifre)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Numara", numara);
                    cmd.Parameters.AddWithValue("@Ad", ad);
                    cmd.Parameters.AddWithValue("@Soyad", soyad);
                    cmd.Parameters.AddWithValue("@Sifre", sifre);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kayıt başarılı.");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            OgrenciAdminPaneli ogrPaneliSayfasi = new OgrenciAdminPaneli();
            ogrPaneliSayfasi.Show();
            this.Hide();
        }
    }
}

