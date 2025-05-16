using System;
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
    public partial class OgrenciSayfasi : Form
    {
        public int OgrenciID { get; set; }
        private SinavTakipSinifi sinavtakip;

        public OgrenciSayfasi()
        {
            InitializeComponent();

            sinavtakip = new SinavTakipSinifi();
            this.Load += OgrenciSayfasi_Load;



        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void OgrenciSayfasi_Load(object sender, EventArgs e)
        {

            if (OgrenciID > 0)
            {
                DataTable dt = sinavtakip.GetOgrenciNotlariByID(OgrenciID);
                dataGridView1.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Geçersiz öğrenci ID!");
            }


        }
    }
}
