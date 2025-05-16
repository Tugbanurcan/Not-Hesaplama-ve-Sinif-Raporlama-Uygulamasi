using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;


namespace sinav_takip_uygulamasi
{
    internal class SinavTakipSinifi
    {
        public string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=NotHesaplamaSistemi;Integrated Security=True;";


        public SqlConnection Baglanti()
        {
            return new SqlConnection(connectionString);
        }

        public bool CheckOgrenciLogin(string numara, string sifre)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Ogrenciler WHERE Numara = @numara AND Sifre = @sifre";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@numara", numara);
                    cmd.Parameters.AddWithValue("@sifre", sifre);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool OgretmenGirisKontrol(string kullaniciAdi, string sifre)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Ogretmenler WHERE KullaniciAdi = @kullaniciAdi AND Sifre = @sifre";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@sifre", sifre);
                    con.Open();
                    int result = (int)cmd.ExecuteScalar();
                    return result > 0;
                }
            }
        }







        public void OgrenciNotlariniYukle(DataGridView dataGridView)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                o.Numara AS [Numara],
                o.Ad AS [Ad], 
                o.Soyad AS [Soyad], 
                n.Vize, 
                n.Final, 
                n.Ortalama, 
                n.Durum
            FROM 
                Ogrenciler o
            LEFT JOIN 
                Notlar n ON o.OgrenciID = n.OgrenciID
            ORDER BY o.Ad
        ";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView.DataSource = dt;

                // Görsel ayarlar
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue;
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dataGridView.EnableHeadersVisualStyles = false;
                dataGridView.ColumnHeadersHeight = 30;
                dataGridView.RowTemplate.Height = 28;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.Width = 120;
                }
            }
        }



        public void NotEkle(int ogrenciId, int? vize, int? final)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Notlar (OgrenciID, Vize, Final, Ortalama, Durum)
            VALUES (@ogrenciId, @vize, @final, NULL, NULL)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    cmd.Parameters.AddWithValue("@vize", (object)vize ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@final", (object)final ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void NotGuncelle(int ogrenciId, int? vize, int? final)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Güncellenecek sütunları dinamik oluştur
                List<string> setClauses = new List<string>();
                if (vize.HasValue)
                    setClauses.Add("Vize = @vize");
                if (final.HasValue)
                    setClauses.Add("Final = @final");

                if (setClauses.Count == 0)
                    return; // Güncellenecek veri yok

                string query = $"UPDATE Notlar SET {string.Join(", ", setClauses)} WHERE OgrenciID = @ogrenciId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                    if (vize.HasValue)
                        cmd.Parameters.AddWithValue("@vize", vize.Value);
                    if (final.HasValue)
                        cmd.Parameters.AddWithValue("@final", final.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /*public void OgrenciSil(int ogrenciId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Öncelikle Notlar tablosundaki öğrencinin notlarını sil
                string deleteNotlarQuery = "DELETE FROM Notlar WHERE OgrenciID = @ogrenciId";
                using (SqlCommand cmdNotlar = new SqlCommand(deleteNotlarQuery, con))
                {
                    cmdNotlar.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    cmdNotlar.ExecuteNonQuery();
                }

                // Sonra Öğrenci kaydını sil
                string deleteOgrenciQuery = "DELETE FROM Ogrenciler WHERE OgrenciID = @ogrenciId";
                using (SqlCommand cmdOgrenci = new SqlCommand(deleteOgrenciQuery, con))
                {
                    cmdOgrenci.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    cmdOgrenci.ExecuteNonQuery();
                }
            }
        }*/




        /* void OgrenciSil(int ogrenciId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string deleteOgrenciQuery = "DELETE FROM Ogrenciler WHERE OgrenciID = @ogrenciId";

                using (SqlCommand cmd = new SqlCommand(deleteOgrenciQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Öğrenci başarıyla silindi ve arşive taşındı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Silinecek öğrenci bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }*/


        public void OgrenciSil(int ogrenciId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Önce Notlar tablosundaki öğrencinin notlarını çekelim
                    string selectNotlarQuery = "SELECT NotID, Vize, Final, Ortalama, Durum FROM Notlar WHERE OgrenciID = @ogrenciId";
                    SqlCommand cmdSelectNotlar = new SqlCommand(selectNotlarQuery, con, transaction);
                    cmdSelectNotlar.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                    DataTable notlarTable = new DataTable();
                    using (SqlDataReader reader = cmdSelectNotlar.ExecuteReader())
                    {
                        notlarTable.Load(reader);
                    }

                    // Arşive ekleme
                    foreach (DataRow row in notlarTable.Rows)
                    {
                        string insertArsivQuery = @"
                    INSERT INTO ArsivNotlar (OgrenciID, Vize, Final, Ortalama, Durum, SilinmeTarihi)
                    VALUES (@OgrenciID, @Vize, @Final, @Ortalama, @Durum, @SilinmeTarihi)";

                        using (SqlCommand cmdInsertArsiv = new SqlCommand(insertArsivQuery, con, transaction))
                        {
                            cmdInsertArsiv.Parameters.AddWithValue("@OgrenciID", ogrenciId);
                            cmdInsertArsiv.Parameters.AddWithValue("@Vize", row["Vize"] == DBNull.Value ? (object)DBNull.Value : row["Vize"]);
                            cmdInsertArsiv.Parameters.AddWithValue("@Final", row["Final"] == DBNull.Value ? (object)DBNull.Value : row["Final"]);
                            cmdInsertArsiv.Parameters.AddWithValue("@Ortalama", row["Ortalama"] == DBNull.Value ? (object)DBNull.Value : row["Ortalama"]);
                            cmdInsertArsiv.Parameters.AddWithValue("@Durum", row["Durum"] == DBNull.Value ? (object)DBNull.Value : row["Durum"]);
                            cmdInsertArsiv.Parameters.AddWithValue("@SilinmeTarihi", DateTime.Now);
                            cmdInsertArsiv.ExecuteNonQuery();
                        }
                    }

                    // Notlar tablosundan öğrenci notlarını sil
                    string deleteNotlarQuery = "DELETE FROM Notlar WHERE OgrenciID = @ogrenciId";
                    using (SqlCommand cmdDeleteNotlar = new SqlCommand(deleteNotlarQuery, con, transaction))
                    {
                        cmdDeleteNotlar.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                        cmdDeleteNotlar.ExecuteNonQuery();
                    }

                    // Öğrenciyi sil
                    string deleteOgrenciQuery = "DELETE FROM Ogrenciler WHERE OgrenciID = @ogrenciId";
                    using (SqlCommand cmdDeleteOgrenci = new SqlCommand(deleteOgrenciQuery, con, transaction))
                    {
                        cmdDeleteOgrenci.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                        int rowsAffected = cmdDeleteOgrenci.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            MessageBox.Show("Öğrenci başarıyla silindi ve arşive taşındı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            transaction.Rollback();
                            MessageBox.Show("Silinecek öğrenci bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        public void OgrenciAra(DataGridView dataGridView, string arama)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string[] parcalar = arama.Split(' ');
                string adParca = parcalar.Length > 0 ? parcalar[0] : "";
                string soyadParca = parcalar.Length > 1 ? parcalar[1] : "";

                string query = @"
        SELECT 
            o.Numara AS [Numara],
            o.Ad AS [Ad],
            o.Soyad AS [Soyad],
            n.Vize,
            n.Final,
            n.Ortalama,
            n.Durum
        FROM 
            Ogrenciler o
        LEFT JOIN 
            Notlar n ON o.OgrenciID = n.OgrenciID
        WHERE 
            (o.Numara LIKE @arama OR 
             (o.Ad LIKE @adParca AND o.Soyad LIKE @soyadParca))";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@arama", $"%{arama}%");
                cmd.Parameters.AddWithValue("@adParca", $"%{adParca}%");
                cmd.Parameters.AddWithValue("@soyadParca", $"%{soyadParca}%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView.DataSource = dt;
            }
        }


        public void VizeyeGirmeyenler(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT o.Numara, o.Ad, o.Soyad, n.Vize, n.Final
                    FROM Ogrenciler o
                    LEFT JOIN Notlar n ON o.OgrenciID = n.OgrenciID
                    WHERE n.Vize IS NULL";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Vizeye girmeyen öğrenci bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dgv.DataSource = dt;
            }
        }


        public void FinaleGirmeyenler(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT o.Numara, o.Ad, o.Soyad, n.Vize, n.Final
                    FROM Ogrenciler o
                    LEFT JOIN Notlar n ON o.OgrenciID = n.OgrenciID
                    WHERE n.Final IS NULL";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Finale girmeyen öğrenci bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dgv.DataSource = dt;
            }
        }

        public void HicSinavaGirmeyenler(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT o.Numara, o.Ad, o.Soyad, n.Vize, n.Final
                    FROM Ogrenciler o
                    LEFT JOIN Notlar n ON o.OgrenciID = n.OgrenciID
                    WHERE n.Vize IS NULL AND n.Final IS NULL";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Hiçbir sınava girmeyen öğrenci bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dgv.DataSource = dt;
            }
        }

        /*public void ArsivOgrencileriGetir(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                a.NotID,
                o.Numara,
                o.Ad,
                o.Soyad,
                a.Vize,
                a.Final,
                a.Ortalama,
                a.Durum,
                a.SilinmeTarihi
            FROM ArsivNotlar a
            LEFT JOIN Ogrenciler o ON a.OgrenciID = o.OgrenciID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                // Görsel ayarlar
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkRed;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersHeight = 30;
                dgv.RowTemplate.Height = 28;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }*/



        /*public void ArsivOgrencileriGetir(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"SELECT 
                            o.Numara, 
                            o.Ad, 
                            o.Soyad, 
                            an.Vize, 
                            an.Final, 
                            an.Ortalama, 
                            an.Durum, 
                            an.SilinmeTarihi
                         FROM ArsivNotlar an
                         LEFT JOIN Ogrenciler o ON an.OgrenciID = o.OgrenciID";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
        }*/
        public void ArsivOgrencileriGetir(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"SELECT 
                            
                            ao.Ad, 
                            ao.Soyad, 
                            an.Vize, 
                            an.Final, 
                            an.Ortalama, 
                            an.Durum, 
                            an.SilinmeTarihi
                         FROM ArsivNotlar an
                         LEFT JOIN ArsivOgrenciler ao ON an.OgrenciID = ao.OgrenciID";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
        }

        public void BasariSirasiGetir(DataGridView dgv)
        {
             

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT 
                o.Numara, 
                o.Ad, 
                o.Soyad, 
                n.Ortalama,
                n.Durum
            FROM Ogrenciler o
            INNER JOIN Notlar n ON o.OgrenciID = n.OgrenciID
            ORDER BY n.Ortalama DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }

          
        }

        /*public (int gecenSayisi, int kalanSayisi) GecenKalanSayilari()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT 
                SUM(CASE WHEN Ortalama >= 50 THEN 1 ELSE 0 END) AS Gecen,
                SUM(CASE WHEN Ortalama < 50 THEN 1 ELSE 0 END) AS Kalan
            FROM Notlar";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int gecen = reader["Gecen"] != DBNull.Value ? Convert.ToInt32(reader["Gecen"]) : 0;
                            int kalan = reader["Kalan"] != DBNull.Value ? Convert.ToInt32(reader["Kalan"]) : 0;
                            return (gecen, kalan);
                        }
                        return (0, 0);
                    }
                }
            }
        }*/

        public (int gecenSayisi, int kalanSayisi) GecenKalanSayilari()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
        SELECT 
            SUM(CASE WHEN Durum = 'Geçti' THEN 1 ELSE 0 END) AS Gecen,
            SUM(CASE WHEN Durum = 'Kaldı' THEN 1 ELSE 0 END) AS Kalan
        FROM Notlar";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int gecen = reader["Gecen"] != DBNull.Value ? Convert.ToInt32(reader["Gecen"]) : 0;
                            int kalan = reader["Kalan"] != DBNull.Value ? Convert.ToInt32(reader["Kalan"]) : 0;
                            return (gecen, kalan);
                        }
                        return (0, 0);
                    }
                }
            }
        }




        // SinavTakipSinifi sınıfına ekle:
        public int GetOgrenciID(string numara, string sifre)
        {
            int ogrenciID = -1;
            string query = "SELECT OgrenciID FROM Ogrenciler WHERE Numara = @numara AND Sifre = @sifre";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@numara", numara);
                    cmd.Parameters.AddWithValue("@sifre", sifre);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        ogrenciID = Convert.ToInt32(result);
                }
            }

            return ogrenciID;
        }

        public DataTable GetOgrenciNotlariByID(int ogrenciID)
        {
            DataTable dt = new DataTable();

            string query = "SELECT Vize, Final, Ortalama,  Durum FROM Notlar WHERE OgrenciID = @ogrenciID";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ogrenciID", ogrenciID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }



















    }
}
