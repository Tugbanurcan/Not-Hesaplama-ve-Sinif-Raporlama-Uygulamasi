﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sinav_takip_uygulamasi
{
    public partial class Rapor : Form
    {
        SinavTakipSinifi sinav = new SinavTakipSinifi();
        public Rapor()
        {
            InitializeComponent();
        }

        private void btnBasariSirasi_Click(object sender, EventArgs e)
        {
            sinav.BasariSirasiGetir(dataGridView1);

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // "Durum" sütununun ismi veritabanındaki sütunla birebir eşleşmeli
            string columnName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;

            if (columnName == "Durum" && e.Value != null)
            {
                string durum = e.Value.ToString();

                if (durum.Equals("Geçti", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (durum.Equals("Kaldı", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.White;
                }
            }

        }

        private void Rapor_Load(object sender, EventArgs e)
        {
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            var (gecenSayisi, kalanSayisi) = sinav.GecenKalanSayilari();

            int toplam = gecenSayisi + kalanSayisi;
            if (toplam == 0)
            {
                MessageBox.Show("Hiç öğrenci bulunamadı.");
                return;
            }

            int gecenYuzde = (int)((double)gecenSayisi / toplam * 100);
            int kalanYuzde = 100 - gecenYuzde;

            progressGecen.Value = gecenYuzde;
            progressKalan.Value = kalanYuzde;

            lblGecen.Text = $"Geçen: %{gecenYuzde} ({gecenSayisi} kişi)";
            lblKalan.Text = $"Kalan: %{kalanYuzde} ({kalanSayisi} kişi)";

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            OgretmenSayfasi tablo_sayfasi = new OgretmenSayfasi();
            tablo_sayfasi.Show();
            this.Hide();
        }
    }
}
