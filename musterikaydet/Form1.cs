using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace musterikaydet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OleDbConnection baglan = new OleDbConnection("provider=microsoft.jet.oledb.4.0; data source = bilgiler.mdb");
        void guncelle()
        {
            OleDbDataAdapter adaptor = new OleDbDataAdapter("select * from tablo1",baglan);
            DataTable tablo = new DataTable();
            adaptor.Fill(tablo);
            dataGridView1.DataSource = tablo;
            dataGridView1.ClearSelection();
        }
        void temizle()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            comboBox1.SelectedIndex = -1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("FİLTRE DEĞİŞİM");
            comboBox1.Items.Add("CİHAZ MONTAJ");
            guncelle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.SelectedIndex != -1 && aylik != false || yillik != false)
            {
                OleDbCommand ekle = new OleDbCommand("insert into tablo1 values(@id,@ad,@soyad,@adres,@telnno,@islem,@ilktar,@sontar,@tutar)",baglan);
                ekle.Parameters.AddWithValue("@id",textBox4.Text.Substring(textBox4.Text.Length-6).ToString());
                ekle.Parameters.AddWithValue("@ad",textBox1.Text);
                ekle.Parameters.AddWithValue("@soyad" ,textBox2.Text);
                ekle.Parameters.AddWithValue("@adres", textBox3.Text);
                ekle.Parameters.AddWithValue("@telno", textBox4.Text);
                ekle.Parameters.AddWithValue("@islem" ,comboBox1.SelectedItem.ToString());
                ekle.Parameters.AddWithValue("@ilktar",DateTime.Now.Date.ToShortDateString());
                if (aylik == true)
                {
                    ekle.Parameters.AddWithValue("@sontar", DateTime.Now.AddMonths(6).ToShortDateString());
                }
                else if (yillik == true)
                {
                    ekle.Parameters.AddWithValue("@sontar", DateTime.Now.AddMonths(12).ToShortDateString());
                }
                
                ekle.Parameters.AddWithValue("@tutar" ,textBox5.Text);
                baglan.Open();
                ekle.ExecuteNonQuery();
                baglan.Close();
                guncelle();
                temizle();
            }
            else if (textBox1.Text == "" && textBox2.Text == "" && textBox3.Text == "" && textBox4.Text == "")
            {
                MessageBox.Show("Tüm müşteri bilgilerini girdiğinizden emin olun!","DİKKAT!");
            }else if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("İşlem türü seçiniz!","DİKKAT");
            }
            else if (yillik == false && aylik == false)
            {
                MessageBox.Show("Değişim süresi seçiniz!","DİKKAT");
            }
        }

        bool kontrol = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            kontrol = false;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (checkBox1.Checked == true)
                {
                    kontrol = true;
                }
            }
            else if(dataGridView1.SelectedRows.Count <= 0)
            {
                checkBox1.Checked = false;
                MessageBox.Show("Kayıt seçiniz.");
            }
            
        }

        bool aylik = false;
        bool yillik = false;
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            yillik = false;
            if (checkBox3.Checked == true)
            {
                checkBox2.Checked = false;
                yillik = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            aylik = false;
            if (checkBox2.Checked == true)
            {
                checkBox3.Checked = false;
                aylik = true;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && checkBox1.Checked == true)
            {
                string secilen = dataGridView1.SelectedCells[0].Value.ToString();
                OleDbCommand upda = new OleDbCommand("update tablo1 set TUTAR = @tutr where MUSTERINO='" + secilen + "'", baglan);
                upda.Parameters.AddWithValue("@tutr", "");
                baglan.Open();
                upda.ExecuteNonQuery();
                baglan.Close();
                guncelle();
                temizle();
            }
            else if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Ödemesi Yapılan Kaydı Seçiniz!", "DİKKAT!");
            }
            else if (checkBox1.Checked == false)
            {
                MessageBox.Show("Ödeme Durumunu Seçiniz!", "DİKKAT!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult sor = MessageBox.Show(dataGridView1.SelectedCells[1].Value.ToString() + " " + dataGridView1.SelectedCells[2].Value.ToString() + " müşterisine ait kaydı silmek istiyor musunuz?", "DİKKAT!", MessageBoxButtons.YesNo);
                if (sor == System.Windows.Forms.DialogResult.Yes)
                {
                    string secilen = dataGridView1.SelectedCells[0].Value.ToString();
                    OleDbCommand sil = new OleDbCommand("delete from tablo1 where MUSTERINO='" + secilen + "'", baglan);
                    baglan.Open();
                    sil.ExecuteNonQuery();
                    baglan.Close();
                    guncelle();
                    temizle();
                }
            }
            else
                MessageBox.Show("Silinecek müşteriyi seçiniz!","DİKKAT");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult sor = MessageBox.Show("Yedeklensin mi?","YEDEK",MessageBoxButtons.YesNo);
            if(sor == System.Windows.Forms.DialogResult.Yes)
            {
                File.Copy(".\\bilgiler.mdb", "yedek.mdb", true);
                MessageBox.Show("VERİTABANI BAŞARIYLA YÜKLENDİ");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow satirlar in dataGridView1.Rows)
                {
                    if ((satirlar.Index % 2) == 0)
                    {
                        satirlar.DefaultCellStyle.BackColor = Color.Bisque;
                    }
                    else
                    {
                        satirlar.DefaultCellStyle.BackColor = Color.Azure;
                    }
                }
            }
        }
    }
}
