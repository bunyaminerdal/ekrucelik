using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace BECELIK18
{
    public partial class BCMAIN : Form
    {
        public BCMAIN()
        {

            // single instance app yani program açıkken tekrar açılmaz.
            Mutex muTex = new Mutex(false, "SINGLE_INSTANCE_APP_MUTEX");
            if (muTex.WaitOne()==false)
            {
                muTex.Close();
                muTex.Dispose();                
            }
            InitializeComponent();
            
            
            
            


            // birlikte aç komutu için programı .bec uzantısıyla açıyor.
            //TODO: *.bec uzantı için kontrol koymalıyım
            //TODO: .bec için setup sırasında registry değiştirsin

            string[] cla = Environment.GetCommandLineArgs();
            if (cla.Length > 1) 
            {
                Dosyadanac(cla[1]);
            }
            
            else
            {
                Pnlkapatac(pnl_main);
                Tabpageackapa(_tabpage_anaekran);
                Surumnotu();
            }           
        }
        private void BCMAIN_Load(object sender, EventArgs e)
        {            
                if (Screen.PrimaryScreen.Bounds.Width < 1360)
                {
                    MessageBox.Show("EKRAN ÖLÇÜLERİNİZ YETERLİ DEĞİL!");
                    this.Close();
                //EKRAN 1360 dan küçükse kapanacak
                }
                
            
            try
            {
                // TODO: Bu kod satırı 'becelikDataSet.KARYUKU' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
                this.karyukuTableAdapter1.Fill(this.ekrudbDataSet1.KARYUKU);
                Karyukuliste = this.ekrudbDataSet1.KARYUKU.ToList();
            }
            catch (Exception)
            {

                throw;
            }

            //bilgisayarın ondalık ayracını çekip ona göre replace ediyoruz
            char a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            
            if ( a==',')
            {
                separator1 = ',';
                separator2 = '.';
            }
            else
            {
                separator1 = '.';
                separator2 = ',';
            }

            listbx_projebilesenleri.SelectedIndex = 0;
            List<string> sehirlist = new List<string>();
            List<string> sehirlist1 = new List<string>();
            for (int i = 0; i < Karyukuliste.Count; i++)
            {
                sehirlist.Add(Karyukuliste[i].sehir);
            }
            sehirlist1 = sehirlist.Distinct().ToList();
            for (int i = 0; i < sehirlist1.Count; i++)
            {
                comboBox_sehir.Items.Add(sehirlist1[i]);
            }
        }
        #region Genel değişkenler
        public char separator1=',';
        public char separator2='.';
        public string projeadi;
        public int karyüklemesayisi = 0;
        public int ruzgaryuklemesayisi = 0;
        public int toplambilesensayisi = 0;
        public int depremyuklemesayisi = 0;
        public int birlesikprofilhesabisayisi = 0;
        public int asikhesabisayisi = 0;
        public int bagalinplcivsayisi = 0;
        public int bagalinplkaysayisi = 0;
        public int ekyericivatasayisi = 0;
        public int ekyerikaynaksayisi = 0;
        public int yarmaliborusayisi = 0;
        public int mesnethesabisayisi = 0;        
        Karyuklemesi KarYuklemesi = new Karyuklemesi();
        public List<ekrudbDataSet.KARYUKURow> Karyukuliste = new List<ekrudbDataSet.KARYUKURow>();
        public string AktifBilesen;
        #endregion
        private void Pnlkapatac(Panel p) {
            foreach (Panel asd in toolStripContainer1.ContentPanel.Controls.OfType<Panel>())
            {
                asd.Hide();
                asd.Enabled = false;
            }
            p.Show();
            p.Enabled = true;
            p.Dock = DockStyle.Left;
            _pnl_tabpage_container.Show();
            _pnl_tabpage_container.Enabled = true;
        } //panel açıp kapatma
        private void Tabpageackapa(TabPage p) {
            foreach (TabPage page in tabControl1.Controls.OfType<TabPage>())
            {

                tabControl1.Controls.Clear();

                tabControl1.Refresh();
            }
            tabControl1.TabPages.Add(p);


        } 
        private void Tabpageackapa(TabPage p, TabPage p2)
        {
            foreach (TabPage page in tabControl1.Controls.OfType<TabPage>())
            {

                tabControl1.Controls.Clear();

                tabControl1.Refresh();
            }
            tabControl1.TabPages.Add(p);
            tabControl1.TabPages.Add(p2);

        }        
        private void btn_yeniprojeolustur_Click_1(object sender, EventArgs e)
        {
            Pnlkapatac(pnl_yeniproje);
            Tabpageackapa(_tabpage_Projegenel);
        }
        private void btn_projekaydet_Click_1(object sender, EventArgs e)
        {
            if (tbx_projeadi.Text == "" || tbx_projeadi.Text.Trim() == "")
            {
                //Yeniprojeolustur();
                //Saydir("PROJENİZ BAŞARIYLA OLUŞTURULDU!");
                //status bardaki yazı 3 saniye sonra kapansın kodu


                //ERROR PROVIDER KULLANILABİLİR.
                //errorProvider1.SetError(tbx_projeadi, "PROJE ADI GİRİNİZ!");
                Saydir("PROJE ADI GİRİNİZ!");
            }
            else
            {
                if (lbl_kayityeri.Text != "")
                {
                    Projekaydet();

                }
                else
                {
                    Saydir("KAYIT YERİ SEÇİNİZ!");
                }

            }
        }
        private void Projekaydet() {
            /* KAYDEY */
            //Kaydet mantığı çok yanlış oldu.


            /* filedelete */
            // Kayıt yeri seçerken mevcut dosyayı seçersen üstüne yazıyor.
            //Bunu genel olarak nasıl halledeceğimi bilmiyorum
            // genel çözüm olarak ini dosyası şeklinde kaydetmek yerine 
            // bi database e kaydetmem lazım sanki.
            //temp dosya oluşturmak en mantıklısı gibi
            //File.Delete(lbl_kayityeri.Text.ToString());
            string p = lbl_kayityeri.Text;
            
            if (listbx_projebilesenleri.Items[0].ToString() != "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
            {

                if (tbx_projeadi.Text != projeadi)
                {
                    projeadi = tbx_projeadi.Text;
                    this.Text = "EKRU-Çelik v18" + "-  Proje adi: " + projeadi;
                }                
                Iniyeni.WriteValue("GENEL", "Proje adi", projeadi, p);
                Iniyeni.WriteValue("GENEL", "toplambilesen", toplambilesensayisi.ToString(),p);
                for (int i = 0; i < listbx_projebilesenleri.Items.Count; i++)
                {
                    Iniyeni.WriteValue("bilesen-" + i, "bilesen", listbx_projebilesenleri.Items[i].ToString().Split('-')[0],p);
                    Iniyeni.WriteValue("bilesen-" + i, "yuklemesirasi", listbx_projebilesenleri.Items[i].ToString().Split('-')[1],p); 
                }
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("PROJENİZ BAŞARIYLA KAYDEDİLDİ-1!");
            }
            else
            {

                if (tbx_projeadi.Text != projeadi)
                {
                    projeadi = tbx_projeadi.Text;
                    Iniyeni.WriteValue("GENEL", "Proje adi", projeadi, p);
                    Iniyeni.WriteValue("GENEL", "toplambilesen", toplambilesensayisi.ToString(), p);
                    this.Text = "EKRU-Çelik v18" + "-  Proje adi: " + projeadi;
                    
                    //status bardaki yazı 3 saniye sonra kapansın kodu
                    Saydir("PROJENİZ BAŞARIYLA KAYDEDİLDİ-2!");
                }
                else {
                    projeadi = tbx_projeadi.Text;
                    Iniyeni.WriteValue("GENEL", "Proje adi", projeadi, p);
                    Iniyeni.WriteValue("GENEL", "toplambilesen", toplambilesensayisi.ToString(), p);
                    this.Text = "EKRU-Çelik v18" + "-  Proje adi: " + projeadi;
                    
                    Saydir("Buraya nasıl geldim :S");

                }

            }


        }
        
        private void btn_kayityeri_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog kayitDialog = new SaveFileDialog();
            kayitDialog.Title = "PROJE KAYDET";
            kayitDialog.Filter = "BEÇelik PROJE|*.bec";
            kayitDialog.DefaultExt = ".bec";
            kayitDialog.AddExtension = true;
            kayitDialog.OverwritePrompt = true;




            if (kayitDialog.ShowDialog() == DialogResult.OK)
            {

                lbl_kayityeri.Text = kayitDialog.FileName;

            }
        }
        private void btn_dosyadanprojeac_Click_1(object sender, EventArgs e)
        {
            Dosyadanac();
        }
        private void ProjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pnl_yeniproje.Enabled == false)
            {
                if (projeadi != null)
                {
                    Pnlkapatac(pnl_yeniproje);
                    Tabpageackapa(_tabpage_Projegenel);
                    _lbl_header.Text = "Proje Genel";
                    
                    //_lbl_header.Location = new Point((this.Location.X + this.Width), _lbl_header.Location.Y);

                }
                else
                {
                    //status bardaki yazı 3 saniye sonra kapansın kodu
                    Saydir("Geçerli bir proje bulunamadı!");
                }
            }
        }
        private void YeniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {
                DialogResult result = MessageBox.Show("AÇIK OLAN PROJEYİ KAYDETMEK İSTER MİSİNİZ?", "UYARI", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    Projekaydet();
                    karyüklemesayisi = 0;
                    ruzgaryuklemesayisi = 0;
                    toplambilesensayisi = 0;
                    depremyuklemesayisi = 0;
                    birlesikprofilhesabisayisi = 0;
                    asikhesabisayisi = 0;
                    bagalinplcivsayisi = 0;
                    bagalinplkaysayisi = 0;
                    ekyericivatasayisi = 0;
                    ekyerikaynaksayisi = 0;
                    yarmaliborusayisi = 0;
                    mesnethesabisayisi = 0;
                    tbx_projeadi.Text = "";
                    lbl_kayityeri.Text = "";
                    listbx_projebilesenleri.Items.Clear();
                    listbx_projebilesenleri.Items.Add("PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR");
                    Pnlkapatac(pnl_yeniproje);
                    Tabpageackapa(_tabpage_Projegenel);
                    projeadi = null;
                }
                else if (result == DialogResult.No)
                {
                    karyüklemesayisi = 0;
                    ruzgaryuklemesayisi = 0;
                    toplambilesensayisi = 0;
                    depremyuklemesayisi = 0;
                    birlesikprofilhesabisayisi = 0;
                    asikhesabisayisi = 0;
                    bagalinplcivsayisi = 0;
                    bagalinplkaysayisi = 0;
                    ekyericivatasayisi = 0;
                    ekyerikaynaksayisi = 0;
                    yarmaliborusayisi = 0;
                    mesnethesabisayisi = 0;
                    tbx_projeadi.Text = "";
                    lbl_kayityeri.Text = "";
                    listbx_projebilesenleri.Items.Clear();
                    listbx_projebilesenleri.Items.Add("PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR");
                    Pnlkapatac(pnl_yeniproje);
                    Tabpageackapa(_tabpage_Projegenel);
                    projeadi = null;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }

            }
            else
            {
                Pnlkapatac(pnl_yeniproje);
                Tabpageackapa(_tabpage_Projegenel);
            }

        }
        private void Saydir(string s) {

            toolStripStatusLabel1.Text = s;
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            DateTime dt = DateTime.Now.AddHours(0).AddMinutes(0).AddSeconds(4);
            tmr.Start();
            tmr.Tick += (asd, a) => {
                TimeSpan diff = dt.Subtract(DateTime.Now); if (dt < DateTime.Now)
                {
                    tmr.Stop();
                    toolStripStatusLabel1.Text = "";
                }
            };

        }        
        private void Surumnotu() {
            
            try
            {
                string p= AppDomain.CurrentDomain.BaseDirectory.ToString();
                p = p + "surumnotlari.txt";
                int lokasyon = 20;
                int notesayisi = Int32.Parse(Iniyeni.ReadValue("SURUMNOTLARI", "SURUMNOTUSAYISI", p));
                
                    for (int i =0; i < notesayisi; i++)
                    {
                        string a = Iniyeni.ReadValue("note-" + i, "versiyon",p);
                        string b = Iniyeni.ReadValue("note-" + i, "yayınlanmaTarihi",p);

                        int notsayisi = Int32.Parse(Iniyeni.ReadValue("note-" + i, "notsayisi",p));
                        GroupBox gb = new GroupBox();
                        pnl_Surumnotlari.Controls.Add(gb);

                        gb.Text = "Sürüm notu - Versiyon: " + a + " - Tarih: " + b;
                        gb.Location = new Point(10, lokasyon);
                        
                        gb.Width = _tabpage_anaekran.Width - 50;
                        gb.Height = 25 + notsayisi * 20;

                        for (int i1 = 0; i1 < notsayisi; i1++)
                        {
                        string nottakiyazi;
                            Label lbl = new Label();
                            gb.Controls.Add(lbl);
                        nottakiyazi ="- "+ Iniyeni.ReadValue("note-" + i, "not-" + i1, p);                        
                        lbl.Size =new Size(nottakiyazi.Length*2,lbl.Height);
                        lbl.Text = nottakiyazi;
                        lbl.AutoSize = true;
                        lbl.Location = new Point(20, lbl.Height *( i1+1));
                            lbl.Font = new Font(lbl.Font, FontStyle.Regular);
                        
                    }
                        lokasyon = gb.Location.Y + (40 + notsayisi * lbl_catiacisi.Height);
                    }
                
                }
            catch (Exception)
            {
                Saydir("Sürüm notlarına ulaşılamadı, internet bağlantınızı kontrol ediniz.");
                return;
            }
            
        } //sürüm notları        
        private void Listbx_projebilesenleri_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listbx_projebilesenleri.Items[0].ToString() != "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
            {
                string p = lbl_kayityeri.Text;

                string projeadikontrol = Iniyeni.ReadValue("GENEL", "Proje adi",p);
                
                if (projeadi == projeadikontrol)
                {
                    string bilesentürü;
                    bilesentürü = MyToString(listbx_projebilesenleri.SelectedItem).Split('-')[0];
                    int bilesensayisi = listbx_projebilesenleri.SelectedIndex;
                    AktifBilesen = "bilesen-" + bilesensayisi;
                    if (bilesentürü == Iniyeni.ReadValue("bilesen-" + bilesensayisi, "bilesen",p) )
                    {
                        _lbl_header.Text = MyToString(listbx_projebilesenleri.SelectedItem);
                        switch (bilesentürü)
                        {
                            case "Kar Yüklemesi":
                                Pnlkapatac(pnl_karyuku);
                                Tabpageackapa(_tabpage_karyuku1, _tabpage_karyuku2);
                                break;
                            case "Rüzgar Yüklemesi":
                                Pnlkapatac(pnl_ruzgaryuku);
                                Tabpageackapa(_tabpage_ruzgaryuku1);
                                break;
                            case "Deprem Yüklemesi":
                                Pnlkapatac(Pnl_DepYuk);
                                Tabpageackapa(_tabpage_depremyuku1);
                                break;
                            case "Aşık Hesabı":
                                Pnlkapatac(Pnl_AsikHesabi);
                                break;
                            case "Bağlantı Alın Plakası Civatalı":
                                Pnlkapatac(Pnl_BagAlinCiv);
                                break;
                            case "Bağlantı Alın Plakası Kaynaklı":
                                Pnlkapatac(Pnl_BagAlinKay);
                                break;
                            case "Bağlantı Ekyeri Civatalı":
                                Pnlkapatac(Pnl_BagEkyerCiv);
                                break;
                            case "Bağlantı Ekyeri Kaynaklı":
                                Pnlkapatac(Pnl_BagEkyeriKay);
                                break;
                            case "Bağlantı Yarmalı Boru/Kutu":
                                Pnlkapatac(Pnl_BagYarmali);
                                break;
                            case "Profil Hesabı Birleşik Etki":
                                Pnlkapatac(Pnl_Blesiketki);
                                break;
                            case "Mesnet Hesabı":
                                Pnlkapatac(Pnl_Mesnethesabi);
                                break;


                            default:
                                break;
                        }


                    }
                    else
                    {
                        Saydir("PROJEYİ KAYDETMELİSİNİZ!");
                    }
                }
            }
            else
            {
                Saydir("BİLEŞEN BULUNAMADI!");
            }
        }
        private void HakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pnlkapatac(pnl_hakkinda);
            Tabpageackapa(_tabpage_anaekran);
            _lbl_header.Text = "Hakkında";
            //_lbl_header.Location = new Point(this.Location.X + this.Width, _lbl_header.Location.Y);
        }
        private void linkLabel_ekru_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ekruproje.com");
        }
        private void AçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {
                DialogResult result = MessageBox.Show("AÇIK OLAN PROJEYİ KAYDETMEK İSTER MİSİNİZ?", "UYARI", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {

                    //FİXME!: açık projeyi kaydetmeliyiz.

                    Projekaydet();
                    Dosyadanac();
                }
                else if (result == DialogResult.No)
                {
                    Dosyadanac();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                Dosyadanac();
            }
        }
        private void Dosyadanac() {

            OpenFileDialog dosyadanac = new OpenFileDialog();
            dosyadanac.Title = "PROJE AÇ";
            dosyadanac.Filter = "BEÇelik PROJE|*.bec";
            dosyadanac.DefaultExt = ".bec";
            dosyadanac.RestoreDirectory = true;
            if (dosyadanac.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string dosyayolu = dosyadanac.FileName;
            lbl_kayityeri.Text = dosyayolu;
            string p = lbl_kayityeri.Text;
            
            string projeadikontrol =Iniyeni.ReadValue("GENEL", "Proje adi",p);
            toplambilesensayisi = Int32.Parse(Iniyeni.ReadValue("GENEL", "toplambilesen", p)); 
            projeadi = projeadikontrol;
            tbx_projeadi.Text = projeadi;
            listbx_projebilesenleri.Items.Clear();
            if (toplambilesensayisi != 0)
            {
                for (int i = 0; i < toplambilesensayisi; i++)
                {
                    string a =Iniyeni.ReadValue("bilesen-" + i, "bilesen",p);
                    string b =Iniyeni.ReadValue("bilesen-" + i, "yuklemesirasi",p);
                    switch (a)
                    {
                        case "Kar Yüklemesi":
                            karyüklemesayisi = int.Parse(b);
                            break;
                        case "Rüzgar Yüklemesi":
                            ruzgaryuklemesayisi = int.Parse(b);
                            break;
                        case "Deprem Yüklemesi":
                            depremyuklemesayisi = int.Parse(b);
                            break;
                        case "Aşık Hesabı":
                            asikhesabisayisi = int.Parse(b);
                            break;
                        case "Bağlantı Alın Plakası Civatalı":
                            bagalinplcivsayisi = int.Parse(b);
                            break;
                        case "Bağlantı Alın Plakası Kaynaklı":
                            bagalinplkaysayisi = int.Parse(b);
                            break;
                        case "Bağlantı Ekyeri Civatalı":
                            ekyericivatasayisi = int.Parse(b);
                            break;
                        case "Bağlantı Ekyeri Kaynaklı":
                            ekyerikaynaksayisi = int.Parse(b);
                            break;
                        case "Bağlantı Yarmalı Boru/Kutu":
                            yarmaliborusayisi = int.Parse(b);
                            break;
                        case "Profil Hesabı Birleşik Etki":
                            birlesikprofilhesabisayisi = int.Parse(b);
                            break;
                        case "Mesnet Hesabı":
                            mesnethesabisayisi = int.Parse(b);
                            break;


                        default:
                            break;
                    }

                    listbx_projebilesenleri.Items.Add(a + "-" + b);


                }
            }
            else
            {

                listbx_projebilesenleri.Items.Add("PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR");
            }
            this.Text = "EKRU-Çelik v18" + "-  Proje adi: " + projeadi;
            
            Pnlkapatac(pnl_yeniproje);
            Tabpageackapa(_tabpage_Projegenel);
            listbx_projebilesenleri.SelectedIndex = 0;
        }
        private void Dosyadanac(string dosya)
        {

           
            string dosyayolu = dosya;
            lbl_kayityeri.Text = dosyayolu;
            string p = lbl_kayityeri.Text;

            string projeadikontrol = Iniyeni.ReadValue("GENEL", "Proje adi", p);
            toplambilesensayisi = Int32.Parse(Iniyeni.ReadValue("GENEL", "toplambilesen", p));
            projeadi = projeadikontrol;
            tbx_projeadi.Text = projeadi;
            listbx_projebilesenleri.Items.Clear();
            if (toplambilesensayisi != 0)
            {
                for (int i = 0; i < toplambilesensayisi; i++)
                {
                    string a = Iniyeni.ReadValue("bilesen-" + i, "bilesen", p);
                    string b = Iniyeni.ReadValue("bilesen-" + i, "yuklemesirasi", p);
                    switch (a)
                    {
                        case "Kar Yüklemesi":
                            karyüklemesayisi = int.Parse(b);
                            break;
                        case "Rüzgar Yüklemesi":
                            ruzgaryuklemesayisi = int.Parse(b);
                            break;
                        case "Deprem Yüklemesi":
                            depremyuklemesayisi = int.Parse(b);
                            break;
                        case "Aşık Hesabı":
                            asikhesabisayisi = int.Parse(b);
                            break;
                        case "Bağlantı Alın Plakası Civatalı":
                            bagalinplcivsayisi = int.Parse(b);
                            break;
                        case "Bağlantı Alın Plakası Kaynaklı":
                            bagalinplkaysayisi = int.Parse(b);
                            break;
                        case "Bağlantı Ekyeri Civatalı":
                            ekyericivatasayisi = int.Parse(b);
                            break;
                        case "Bağlantı Ekyeri Kaynaklı":
                            ekyerikaynaksayisi = int.Parse(b);
                            break;
                        case "Bağlantı Yarmalı Boru/Kutu":
                            yarmaliborusayisi = int.Parse(b);
                            break;
                        case "Profil Hesabı Birleşik Etki":
                            birlesikprofilhesabisayisi = int.Parse(b);
                            break;
                        case "Mesnet Hesabı":
                            mesnethesabisayisi = int.Parse(b);
                            break;


                        default:
                            break;
                    }

                    listbx_projebilesenleri.Items.Add(a + "-" + b);


                }
            }
            else
            {

                listbx_projebilesenleri.Items.Add("PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR");
            }
            this.Text = "EKRU-Çelik v18" + "-  Proje adi: " + projeadi;

            Pnlkapatac(pnl_yeniproje);
            Tabpageackapa(_tabpage_Projegenel);
        }
        private void KarYüklemesiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                karyüklemesayisi += 1;
                listbx_projebilesenleri.Items.Add("Kar Yüklemesi-" + karyüklemesayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void RüzgarYüklemesiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                ruzgaryuklemesayisi += 1;
                listbx_projebilesenleri.Items.Add("Rüzgar Yüklemesi-" + ruzgaryuklemesayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void DepremYüklemesiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                depremyuklemesayisi += 1;
                listbx_projebilesenleri.Items.Add("Deprem Yüklemesi-" + depremyuklemesayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void BirleşikEtkiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                birlesikprofilhesabisayisi += 1;
                listbx_projebilesenleri.Items.Add("Profil Hesabı Birleşik Etki-" + birlesikprofilhesabisayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void AşıkHesabıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                asikhesabisayisi += 1;
                listbx_projebilesenleri.Items.Add("Aşık Hesabı-" + asikhesabisayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void CivatalıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                bagalinplcivsayisi += 1;
                listbx_projebilesenleri.Items.Add("Bağlantı Alın Plakası Civatalı-" + bagalinplcivsayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void KaynaklıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                bagalinplkaysayisi += 1;
                listbx_projebilesenleri.Items.Add("Bağlantı Alın Plakası Kaynaklı-" + bagalinplkaysayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void CivatalıToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                ekyericivatasayisi += 1;
                listbx_projebilesenleri.Items.Add("Bağlantı Ekyeri Civatalı-" + ekyericivatasayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void KaynaklıToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                ekyerikaynaksayisi += 1;
                listbx_projebilesenleri.Items.Add("Bağlantı Ekyeri Kaynaklı-" + ekyerikaynaksayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void YarmalıBoruKutuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                yarmaliborusayisi += 1;
                listbx_projebilesenleri.Items.Add("Bağlantı Yarmalı Boru/Kutu-" + yarmaliborusayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void MesnetHesabıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projeadi != null)
            {

                try
                {
                    if (listbx_projebilesenleri.Items[0].ToString() == "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
                    {
                        listbx_projebilesenleri.Items.Clear();
                    }
                }
                catch (Exception)
                {
                    Saydir("PROJENİZ BOZULMUŞ!");
                    return;
                }

                mesnethesabisayisi += 1;
                listbx_projebilesenleri.Items.Add("Mesnet Hesabı-" + mesnethesabisayisi);
                toplambilesensayisi += 1;

            }
            else
            {
                //status bardaki yazı 3 saniye sonra kapansın kodu
                Saydir("Geçerli bir proje bulunamadı!");
            }
        }
        private void pnl_yeniproje_EnabledChanged_1(object sender, EventArgs e)
        {
            if (pnl_yeniproje.Enabled == false)
            {

                ekleToolStripMenuItem.Enabled = false;
            }
            else
            {
                _lbl_header.Text = "Proje Genel";
                //_lbl_header.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                ekleToolStripMenuItem.Enabled = true;
            }
        }
        private void pnl_main_EnabledChanged_1(object sender, EventArgs e)
        {
            if (pnl_main.Enabled == true)
            {
                _lbl_header.Text = "Ana Sayfa";
                //_lbl_header.Location = new Point(this.Location.X + this.Width, _lbl_header.Location.Y);
                projeToolStripMenuItem.Enabled = false;
            }
            else
            {
                projeToolStripMenuItem.Enabled = true;
            }
        }
        private void ÇıkToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Close();
        }        
        private void BCMAIN_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (projeadi != null)
            //{
            //    DialogResult result = MessageBox.Show("AÇIK OLAN PROJEYİ KAYDETMEK İSTER MİSİNİZ?", "PROGRAM KAPATILIYOR", MessageBoxButtons.YesNoCancel);
            //    if (result == DialogResult.Yes)
            //    {

            //        //FİXME: açık projeyi kaydetmeliyiz.
            //        Projekaydet();
            //        e.Cancel = false;
            //    }
            //    else if (result == DialogResult.No)
            //    {
            //        e.Cancel = false;
            //    }
            //    else if (result == DialogResult.Cancel)
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //else
            //{
            //    e.Cancel = false;
            //}
        } // PROGRAM KAPATILIRKEN KAYDEDİLSİN Mİ DİYE SORMAK İÇİN
        private void Btn_bilesensil_Click(object sender, EventArgs e)
        {
            if (listbx_projebilesenleri.Items[0].ToString() != "PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR")
            {
                string p = lbl_kayityeri.Text;
                if (listbx_projebilesenleri.Items.Count==int.Parse(Iniyeni.ReadValue("GENEL", "toplambilesen", p)))
                {
                    if (listbx_projebilesenleri.SelectedIndex>-1)
                    {
                        DialogResult result = MessageBox.Show(listbx_projebilesenleri.SelectedItem.ToString() + "  SİLİNSİN Mİ?", "UYARI", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {

                            int silinecek = listbx_projebilesenleri.SelectedIndex;
                            Iniyeni.DeleteSection("bilesen-" + listbx_projebilesenleri.SelectedIndex.ToString(), p);

                            string str = File.ReadAllText(p);
                            if (toplambilesensayisi != silinecek)
                            {
                                for (int i = 3; i < toplambilesensayisi; i++)
                                {
                                    str = str.Replace("[bilesen-" + i.ToString(), "[bilesen-" + (i - 1).ToString());
                                    File.WriteAllText(p, str);
                                }
                            }


                            listbx_projebilesenleri.Items.RemoveAt(silinecek);

                            toplambilesensayisi -= 1;
                            
                            Iniyeni.WriteValue("GENEL", "toplambilesen", toplambilesensayisi.ToString(), p);
                            if (toplambilesensayisi == 0)
                            {
                                listbx_projebilesenleri.Items.Add("PROJENİZDE HİÇBİR BİLEŞEN BULUNMAMAKTADIR");
                            }
                        }
                        else if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Saydir("KAYDEDİLMEMİŞ BİLEŞENLER VAR.");
                    }
                }
                else
                {
                    Saydir("SİLMEK İSTEDİĞİNİZ BİLEŞENİ SEÇİNİZ");
                }
                   
                
            }
            else
            {
                Saydir("ÖNCELİKLE PROJENİZE BİLEŞEN EKLEMELİSİNİZ.");
            }
            } 
        private static string MyToString(object o)
        {
            if (o == DBNull.Value || o == null)
                return "";

            return o.ToString();
        } //özel string yapma fonksiyonu.
        
        #region KAR YÜKLEMESİ
        private void pnl_karyuku_EnabledChanged_1(object sender, EventArgs e)
        {
            if (pnl_karyuku.Enabled == true)
            {
                string p = lbl_kayityeri.Text;
                
                if (Iniyeni.ReadValue(AktifBilesen, "SehirID",p) != "")
                {
                    KarYuklemesi.SehirID = int.Parse(Iniyeni.ReadValue(AktifBilesen, "SehirID",p));
                    comboBox_sehir.Text = Karyukuliste[KarYuklemesi.SehirID - 1].sehir;
                    comboBox_ilce.Text = Karyukuliste[KarYuklemesi.SehirID - 1].ilce;
                    lbl_sehirID.Text = MyToString(KarYuklemesi.SehirID);
                }
                else
                {
                    comboBox_sehir.Text = Karyukuliste[KarYuklemesi.SehirID - 1].sehir;
                    comboBox_ilce.Text = Karyukuliste[KarYuklemesi.SehirID - 1].ilce;
                    lbl_sehirID.Text = MyToString(KarYuklemesi.SehirID);
                }
                if (Iniyeni.ReadValue(AktifBilesen, "Rakım",p) != "")
                {
                    KarYuklemesi.Rakim = float.Parse(Iniyeni.ReadValue(AktifBilesen, "Rakım",p).Replace(separator2,separator1));
                    tbx_rakim.Text = MyToString(KarYuklemesi.Rakim);
                }
                else
                {
                    tbx_rakim.Text = MyToString(KarYuklemesi.Rakim);
                }
                if (Iniyeni.ReadValue(AktifBilesen, "ZeminKarYükü",p) != "")
                {
                    KarYuklemesi.ZeminKaryuku = float.Parse(Iniyeni.ReadValue(AktifBilesen, "ZeminKarYükü",p).Replace(separator2, separator1));
                    tbx_zeminkaryuku.Text = MyToString(KarYuklemesi.ZeminKaryuku);
                }
                else
                {
                    tbx_zeminkaryuku.Text = MyToString(KarYuklemesi.ZeminKaryuku);
                }
                if (Iniyeni.ReadValue(AktifBilesen, "CeMaruzKalmaKatsayısı",p) != "")
                {
                    KarYuklemesi.Ce_maruzkalmakat = int.Parse(Iniyeni.ReadValue(AktifBilesen, "CeMaruzKalmaKatsayısı",p).Replace(separator2, separator1));
                    combobox_Ce.SelectedIndex = KarYuklemesi.Ce_maruzkalmakat;
                }
                else
                {
                    combobox_Ce.SelectedIndex = KarYuklemesi.Ce_maruzkalmakat;
                }
                if (Iniyeni.ReadValue(AktifBilesen, "CtIsıKatsayısı",p) != "")
                {
                    KarYuklemesi.Ct_isikatsayisi = float.Parse(Iniyeni.ReadValue(AktifBilesen, "CtIsıKatsayısı",p).Replace(separator2, separator1));
                    tbx_ct_Isikatsayisi.Text = MyToString(KarYuklemesi.Ct_isikatsayisi);
                }
                else
                {
                    tbx_ct_Isikatsayisi.Text = MyToString(KarYuklemesi.Ct_isikatsayisi);
                }
                if (Iniyeni.ReadValue(AktifBilesen, "CatisekliSayisi",p) != "")
                {
                    KarYuklemesi.catisekli = int.Parse(Iniyeni.ReadValue(AktifBilesen, "CatisekliSayisi",p));
                    comboBox_catisekil.SelectedIndex = KarYuklemesi.catisekli;
                }
                else
                {
                    comboBox_catisekil.SelectedIndex = KarYuklemesi.catisekli;
                }
                if (Iniyeni.ReadValue(AktifBilesen, "Catiacisi1",p) != "")
                {
                    KarYuklemesi.catiacisi1 = float.Parse(Iniyeni.ReadValue(AktifBilesen, "Catiacisi1",p).Replace(separator2, separator1));
                    tbx_catiacisi1.Text = MyToString(KarYuklemesi.catiacisi1);
                }
                else
                {
                    tbx_catiacisi1.Text = MyToString(KarYuklemesi.catiacisi1);
                }
                if (Iniyeni.ReadValue(AktifBilesen, "Catiacisi2",p) != "")
                {
                    KarYuklemesi.catiacisi2 = float.Parse(Iniyeni.ReadValue(AktifBilesen, "Catiacisi2",p).Replace(separator2, separator1));
                    tbx_catiacisi2.Text = MyToString(KarYuklemesi.catiacisi2);
                }
                else
                {
                    tbx_catiacisi2.Text = MyToString(KarYuklemesi.catiacisi2);
                }


            }
        }
        private void ComboBox_sehir_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_ilce.Items.Clear();
            for (int i = 0; i < Karyukuliste.Count; i++)
            {

                if (MyToString(comboBox_sehir.Text) == MyToString(Karyukuliste[i].sehir))
                {
                    comboBox_ilce.Items.Add(Karyukuliste[i].ilce);
                }
            }
            
        }
        private void ComboBox_ilce_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Karyukuliste.Count; i++)
            {

                if (MyToString(comboBox_ilce.Text) == MyToString(Karyukuliste[i].ilce))
                {
                    lbl_sehirID.Text = MyToString(Karyukuliste[i].ID);
                    lbl_il_ilce.Text = Karyukuliste[i].sehir + "/" + Karyukuliste[i].ilce;
                    lbl_Kybolgesi.Text = Karyukuliste[i].KAR;
                    Skhesapla();
                }
            }
        }
        private void Tbx_rakim_TextChanged(object sender, EventArgs e)
        {
            lbl_rakim.Text = tbx_rakim.Text.Replace(separator2,separator1);
            label29.Location = new Point(lbl_rakim.Location.X + lbl_rakim.Width+10, label29.Location.Y);
            Skhesapla();
        }
        private void Tbx_zeminkaryuku_TextChanged(object sender, EventArgs e)
        {
            lbl_zeminkaryuku.Text = tbx_zeminkaryuku.Text.Replace(separator2,separator1);
            label32.Location = new Point(lbl_zeminkaryuku.Location.X + lbl_zeminkaryuku.Width+10, label32.Location.Y);
            Karyukuhesapla(e);
            
        }
        private void Combobox_Ce_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_Ce_maruzkalmakat.Text = combobox_Ce.Text.Replace(separator2,separator1);
            Karyukuhesapla(e);

        }
        private void Tbx_ct_Isikatsayisi_TextChanged(object sender, EventArgs e)
        {
            lbl_CtIsıKat.Text = tbx_ct_Isikatsayisi.Text;
            Karyukuhesapla(e);
            
        }
        private void ComboBox_catisekil_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Çatı şekillerini 0 dan başlayarak combobox ın index i ne göre isimlendirdim
            lbl_catisekli.Text = comboBox_catisekil.Text + " için şekil katsayısı";
            lbl_catisekli.Location = new Point(_tabpage_karyuku1.Width/2 - lbl_catisekli.Width / 2, lbl_catisekli.Location.Y);
            lbl_catiseklisayisi.Text = MyToString(comboBox_catisekil.SelectedIndex);
            picbox_catisekli_0.Hide();
            picbox_catisekli_1.Hide();
            switch (comboBox_catisekil.SelectedIndex)
            {
                case 0: //Tek yöne eğimli çatı                    
                    picbox_catisekli_0.Show();
                    tbx_catiacisi2.Enabled = false;
                    tbx_catiegimi2.Enabled = false;
                    pnl_karyuku_aci1.Show();
                    pnl_karyuku_aci2.Hide();
                    break;
                case 1: //Çift yöne eğimli çatı
                    picbox_catisekli_1.Show();
                    tbx_catiacisi2.Enabled = true;
                    tbx_catiegimi2.Enabled = true;
                    pnl_karyuku_aci2.Show();
                    pnl_karyuku_aci1.Hide();
                    break;
                case 2: //Çok yöne eğimli çatı(testere)
                    Saydir(" Çok yöne eğimli çatı(testere) ŞUAN AKTİF DEĞİL, SÜRÜM NOTLARINI TAKİP EDİNİZ!");
                    break;
                case 3: //Silindirik Çatı
                    Saydir("Silindirik Çatı ŞUAN AKTİF DEĞİL, SÜRÜM NOTLARINI TAKİP EDİNİZ!");
                    break;
                case 4: //Daha yüksek yapıya bitişik çatı
                    Saydir("Daha yüksek yapıya bitişik çatı ŞUAN AKTİF DEĞİL, SÜRÜM NOTLARINI TAKİP EDİNİZ!");
                    break;
                case 5: //Kapı üstü sundurma
                    Saydir("Kapı üstü sundurma ŞUAN AKTİF DEĞİL, SÜRÜM NOTLARINI TAKİP EDİNİZ!");
                    break;              

                default:
                    break;
            }
            
        }        
        private void Tbx_catiegimi1_TextChanged(object sender, EventArgs e)
        {
            if (tbx_catiacisi1.Text !="" && tbx_ct_Isikatsayisi.Text != "" && tbx_zeminkaryuku.Text != "")
            {
                lbl_catiacisi.Text = tbx_catiacisi1.Text;
                lbl_catiacisi1.Text = tbx_catiacisi1.Text;
                label44.Location = new Point(lbl_catiacisi.Location.X + lbl_catiacisi.Width+5, label44.Location.Y);
                label45.Location = new Point(lbl_catiacisi1.Location.X + lbl_catiacisi1.Width+5, label45.Location.Y);
                Nu1hesapla(float.Parse(lbl_catiacisi.Text));
                lbl_nu1.Text = MyToString(KarYuklemesi.catisekilkatsayisi1);
                lbl_nu1_1.Text = MyToString(KarYuklemesi.catisekilkatsayisi1);                
                lbl_karyuku_S.Text =lbl_karyuku_S1.Text= MyToString(MyToString((float)Math.Round((float.Parse(lbl_zeminkaryuku.Text) * float.Parse(lbl_CtIsıKat.Text) * float.Parse(lbl_Ce_maruzkalmakat.Text) * float.Parse(lbl_nu1_1.Text)), 3)));
                label43.Location = new Point(lbl_karyuku_S.Location.X + lbl_karyuku_S.Width+5, label43.Location.Y);
                label46.Location = new Point(lbl_karyuku_S1.Location.X + lbl_karyuku_S1.Width+5, label46.Location.Y);
                
                if (tbx_catiacisi2.Text=="")
                {
                    tbx_catiacisi2.Text = lbl_catiacisi1.Text;
                }
                if (tbx_catiacisi1.Focused|| (tbx_catiacisi1.Focused == false && tbx_catiegimi1.Focused == false))
                {
                    float aci1 = float.Parse(tbx_catiacisi1.Text);

                    double egim1 = Math.Round(Math.Tan((aci1 * Math.PI) / 180) * 100, 2);

                    tbx_catiegimi1.Text = MyToString(egim1);
                }
                
            }
            
        }
        private void tbx_catiegimi1_TextChanged_1(object sender, EventArgs e)
        {
            if (tbx_catiegimi1.Text == "")
            {
                tbx_catiacisi1.Text = "0";

            }
            if (tbx_catiegimi1.Focused && tbx_catiegimi1.Text != "")
            {
                float egim2 = float.Parse(tbx_catiegimi1.Text);

                double aci2 = Math.Round((Math.Atan(egim2 / 100)) * 180 / Math.PI, 2);

                tbx_catiacisi1.Text = MyToString(aci2);
            }
        }
        private void Tbx_catiegimi2_TextChanged(object sender, EventArgs e)
        {
            if (tbx_catiacisi2.Text!=""&&tbx_ct_Isikatsayisi.Text!=""&&tbx_zeminkaryuku.Text!="")
            {
                lbl_catiacisi2.Text = tbx_catiacisi2.Text;
                label47.Location = new Point(lbl_catiacisi2.Location.X + lbl_catiacisi2.Width+5, label47.Location.Y);
                Nu1hesapla(float.Parse(lbl_catiacisi2.Text));
                lbl_nu1_2.Text = MyToString(KarYuklemesi.catisekilkatsayisi1);                
                lbl_karyuku_S2.Text = MyToString((float)Math.Round((double)(float.Parse(lbl_zeminkaryuku.Text) * float.Parse(lbl_CtIsıKat.Text) * float.Parse(lbl_Ce_maruzkalmakat.Text) * float.Parse(lbl_nu1_2.Text)), 3));
                label49.Location = new Point(lbl_karyuku_S2.Location.X + lbl_karyuku_S2.Width+5, label49.Location.Y);

                if (tbx_catiacisi2.Focused || (tbx_catiacisi2.Focused == false && tbx_catiegimi2.Focused == false))
                {
                    float aci1 = float.Parse(tbx_catiacisi2.Text);

                    double egim1 = Math.Round(Math.Tan((aci1 * Math.PI) / 180) * 100, 2);

                    tbx_catiegimi2.Text = MyToString(egim1);
                }
            }
         }
        private void tbx_catiegimi2_TextChanged_1(object sender, EventArgs e)
        {
            if (tbx_catiegimi2.Text == "")
            {
                tbx_catiacisi2.Text = "0";

            }
            if (tbx_catiegimi2.Focused&& tbx_catiegimi2.Text!="")
            {
                float egim2 = float.Parse(tbx_catiegimi2.Text);

                double aci2 = Math.Round((Math.Atan(egim2 / 100)) * 180 / Math.PI, 2);

                tbx_catiacisi2.Text = MyToString(aci2);
            }
        }
        private void Nu1hesapla(float aci1) {
            if (aci1<=30)
            {
                KarYuklemesi.catisekilkatsayisi1 = 0.8f;
            }
            else if (aci1<60)
            {
               KarYuklemesi.catisekilkatsayisi1 = (float) Math.Round((double)0.8f *(60-aci1)/30,3);
            }
            else
            {
                KarYuklemesi.catisekilkatsayisi1 = 0;
            }
            
        }
        private void Karyukuhesapla(EventArgs easd) {

            Tbx_catiegimi1_TextChanged(this, easd);
            Tbx_catiegimi2_TextChanged(this, easd);

        }
        private void Skhesapla() {
            if (lbl_il_ilce.Text!=""&&lbl_rakim.Text!="")
            {
                switch (lbl_Kybolgesi.Text)
                {
                    case "I":
                        if (float.Parse(lbl_rakim.Text)<=200)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 300)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 400)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 500)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 600)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 700)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 800)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 900)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1000)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1500)
                        {
                            tbx_zeminkaryuku.Text = "0,88";
                        }
                        else if (float.Parse(lbl_rakim.Text) >1500)
                        {
                            tbx_zeminkaryuku.Text = "0,92";
                        }


                        break;
                    case "II":
                        if (float.Parse(lbl_rakim.Text) <= 200)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 300)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 400)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 500)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 600)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 700)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 800)
                        {
                            tbx_zeminkaryuku.Text = "0,85";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 900)
                        {
                            tbx_zeminkaryuku.Text = "0,95";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1000)
                        {
                            tbx_zeminkaryuku.Text = "1,05";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,155";
                        }
                        else if (float.Parse(lbl_rakim.Text) > 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,2075";
                        }
                        break;
                    case "III":
                        if (float.Parse(lbl_rakim.Text) <= 200)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 300)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 400)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 500)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 600)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 700)
                        {
                            tbx_zeminkaryuku.Text = "0,85";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 800)
                        {
                            tbx_zeminkaryuku.Text = "1,25";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 900)
                        {
                            tbx_zeminkaryuku.Text = "1,30";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1000)
                        {
                            tbx_zeminkaryuku.Text = "1,35";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,485";
                        }
                        else if (float.Parse(lbl_rakim.Text) > 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,5525";
                        }
                        break;
                    case "IV":
                        if (float.Parse(lbl_rakim.Text) <= 200)
                        {
                            tbx_zeminkaryuku.Text = "0,75";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 300)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 400)
                        {
                            tbx_zeminkaryuku.Text = "0,80";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 500)
                        {
                            tbx_zeminkaryuku.Text = "0,85";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 600)
                        {
                            tbx_zeminkaryuku.Text = "0,90";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 700)
                        {
                            tbx_zeminkaryuku.Text = "0,95";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 800)
                        {
                            tbx_zeminkaryuku.Text = "1,40";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 900)
                        {
                            tbx_zeminkaryuku.Text = "1,50";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1000)
                        {
                            tbx_zeminkaryuku.Text = "1,60";
                        }
                        else if (float.Parse(lbl_rakim.Text) <= 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,76";
                        }
                        else if (float.Parse(lbl_rakim.Text) > 1500)
                        {
                            tbx_zeminkaryuku.Text = "1,84";
                        }
                        break;

                    default:
                        break;
                }
            }

        }       
        private void Tbx_catiegimi1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }
            

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }

        }
        private void Tbx_catiegimi2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void Tbx_ct_Isikatsayisi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void Tbx_zeminkaryuku_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void Tbx_rakim_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void tbx_catiegimi1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void tbx_catiegimi2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == separator2)
            {
                e.KeyChar = separator1;
            }


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == separator1) && ((sender as TextBox).Text.IndexOf(separator1) > -1))
            {
                e.Handled = true;
            }
        }
        private void Btn_KarYuku_Kaydet_Click_1(object sender, EventArgs e)
        {
            if (tbx_rakim.Text != "" && tbx_zeminkaryuku.Text != "" && tbx_ct_Isikatsayisi.Text != "" && tbx_catiacisi1.Text != "" && tbx_catiacisi2.Text != "")
            {
                string p = lbl_kayityeri.Text;
                
                KarYuklemesi.SehirID = int.Parse(lbl_sehirID.Text);
               Iniyeni.WriteValue(AktifBilesen, "SehirID", MyToString(KarYuklemesi.SehirID),p);

                KarYuklemesi.Rakim = float.Parse(lbl_rakim.Text);
                Iniyeni.WriteValue(AktifBilesen, "Rakım", MyToString(KarYuklemesi.Rakim),p);

                KarYuklemesi.ZeminKaryuku = float.Parse(lbl_zeminkaryuku.Text);
                Iniyeni.WriteValue(AktifBilesen, "ZeminKarYükü", MyToString(KarYuklemesi.ZeminKaryuku),p);

                KarYuklemesi.Ce_maruzkalmakat = combobox_Ce.SelectedIndex;
                Iniyeni.WriteValue(AktifBilesen, "CeMaruzKalmaKatsayısı", MyToString(KarYuklemesi.Ce_maruzkalmakat),p);

                KarYuklemesi.Ct_isikatsayisi = float.Parse(lbl_CtIsıKat.Text);
                Iniyeni.WriteValue(AktifBilesen, "CtIsıKatsayısı", MyToString(KarYuklemesi.Ct_isikatsayisi),p);

                KarYuklemesi.catisekli = comboBox_catisekil.SelectedIndex;
                Iniyeni.WriteValue(AktifBilesen, "CatisekliSayisi", MyToString(KarYuklemesi.catisekli),p);

                KarYuklemesi.catiacisi1 = float.Parse(lbl_catiacisi.Text);
                Iniyeni.WriteValue(AktifBilesen, "Catiacisi1", MyToString(KarYuklemesi.catiacisi1),p);

                KarYuklemesi.catiacisi2 = float.Parse(lbl_catiacisi2.Text);
                Iniyeni.WriteValue(AktifBilesen, "Catiacisi2", MyToString(KarYuklemesi.catiacisi2),p);

                Saydir(" BAŞARIYLA KAYDEDİLMİŞTİR!");
            }
            else
            {
                Saydir("boş alanlar var!");
            }
        }
        //print tabpage         
        Bitmap MemoryImage;
        Bitmap MemoryImage1;
        private int sayfasayisi=0;
        public int Netlik = 600; //pdf ve jpeg kalitesini arttırmak için
        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            sayfasayisi = sayfasayisi - 1;
            if (sayfasayisi<0)
            {
                //sayfa sayısının 1 eksiği printPreviewDialog1 dan 2. kez döngüye girdiğinde döngü dursun diye
                sayfasayisi = 1;
            }
            
            if (sayfasayisi == 0)
            {
                e.Graphics.DrawImage(MemoryImage1, _tabpage_karyuku2.Location.X+40, _tabpage_karyuku2.Location.Y+10 , _tabpage_karyuku2.Width-80, _tabpage_karyuku2.Height - 60);
                e.HasMorePages = false; 
            }
            else
            {
                e.Graphics.DrawImage(MemoryImage, _tabpage_karyuku1.Location.X+40, _tabpage_karyuku1.Location.Y+10 , _tabpage_karyuku1.Width-80, _tabpage_karyuku1.Height - 60);
                e.HasMorePages = true;
            } 
        }        
        private void btn_jpg_karyuku_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "JPEG Image (.jpeg)|*.jpeg";
            sf.CheckFileExists = false;
            sf.OverwritePrompt = false;
            sf.CreatePrompt = false;


            if (sf.ShowDialog() == DialogResult.OK)
            {
                string path = sf.FileName.ToString().Split('.')[0];

                Tabpageackapa(_tabpage_karyuku1);
                MemoryImage = new Bitmap(_tabpage_karyuku1.Width, _tabpage_karyuku1.Height);
                _tabpage_karyuku1.DrawToBitmap(MemoryImage, new Rectangle(0, 0, _tabpage_karyuku1.Width * Netlik, _tabpage_karyuku1.Height * Netlik));
                Tabpageackapa(_tabpage_karyuku2);
                MemoryImage1 = new Bitmap(_tabpage_karyuku2.Width, _tabpage_karyuku2.Height);
                _tabpage_karyuku2.DrawToBitmap(MemoryImage1, new Rectangle(0, 0, _tabpage_karyuku2.Width * Netlik, _tabpage_karyuku2.Height * Netlik));
                Tabpageackapa(_tabpage_karyuku1, _tabpage_karyuku2);
                if (File.Exists(path + "1.jpeg") || File.Exists(path + "2.jpeg"))
                {
                    DialogResult result = MessageBox.Show("MEVCUT JPGLER ÜZERİNE KAYDEDİLSİN Mİ?", "UYARI", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        MemoryImage.Save(path + "1.jpeg");
                        MemoryImage1.Save(path + "2.jpeg");
                    }
                    else if (result == DialogResult.No)
                    {
                        return;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                MemoryImage.Save(path + "1.jpeg");
                MemoryImage1.Save(path + "2.jpeg");

            }
        }        
        private void btn_prnt_karyuku_Click_1(object sender, EventArgs e)
        {
            sayfasayisi = 2;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                Tabpageackapa(_tabpage_karyuku1);

                MemoryImage = new Bitmap(_tabpage_karyuku1.Width, _tabpage_karyuku1.Height);
                
                //_tabpage_karyuku1.DrawToBitmap(MemoryImage, _tabpage_karyuku1.Bounds);
                _tabpage_karyuku1.DrawToBitmap(MemoryImage,new Rectangle(0,0, _tabpage_karyuku1.Width * Netlik, _tabpage_karyuku1.Height * Netlik));
                //tabcntrl_main.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right);

                Tabpageackapa(_tabpage_karyuku2);
                MemoryImage1 = new Bitmap(_tabpage_karyuku2.Width, _tabpage_karyuku2.Height);
                _tabpage_karyuku2.DrawToBitmap(MemoryImage1, new Rectangle(0, 0, _tabpage_karyuku2.Width * Netlik, _tabpage_karyuku2.Height * Netlik));

                Tabpageackapa(_tabpage_karyuku1, _tabpage_karyuku2);
                PaperSize paperSize = new PaperSize("papersize", 850, 1200);
                printDocument1.DefaultPageSettings.PaperSize = paperSize;
                printPreviewDialog1.PrintPreviewControl.Columns = 2;
                printPreviewDialog1.ShowDialog();
            }
        }
        #endregion
        //TODO: ekran ölçüsüne göre ölçeklendir

    }
}
    




