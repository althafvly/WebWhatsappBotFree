using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Management;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using Gecko.DOM;
using Gecko.Events;
using Gecko;
using System.Data.SQLite;
using OpenQA.Selenium;

namespace WebWhatsappBotFree
{
   
    public partial class Anaform : Form
    {


        static bool kapatilmasin = true;

        static bool islemisonlandir = false;
        static string selectgrupid = "";
        static string aktifkisisayisi = "";
        static string selectmesajid = "";
        static string selectmesaj = "";
        static string islemidsii = "";

        static string ad = "";
        static string ikinciAd = "";
        static string soyad = "";
        static string cepNo = "";

        static List<Kisi> liste_kisi = new List<Kisi>();
        static List<Mesajlar> mesajlarlistesi = new List<Mesajlar>();

        public Anaform()
        {
            InitializeComponent();
            try
            {
                Xpcom.Initialize("Firefox");
                Gecko.GeckoPreferences.User["general.useragent.override"] = "Mozilla / 5.0(Windows NT 10.0; WOW64 ; rv:54.0) Gecko/20100101 Firefox/54.0";
                Gecko.GeckoPreferences.User["intl.accept_languages"] = "EN-us";
            }
            catch { MessageBox.Show("Firefox Folder Corrupted or Deleted! \nPlease try to run as Administrator", "Error code: 1", MessageBoxButtons.OK,MessageBoxIcon.Error);Application.Exit(); }
        }

        private void AcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;

        }

        private void KapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kapatilmasin = false;
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void Raporlarbtn_Click(object sender, EventArgs e)
        {
            Gizle();
            RAPORLAR.Visible = true;

        }
        public static bool DnsTest()
        {
            try
            {
#pragma warning disable CS0618 // Type or member old
                IPHostEntry ipHe = Dns.GetHostByName("www.google.com");
#pragma warning restore CS0618 // Type or member old
                return false;
            }
            catch
            {
                return true;
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (DnsTest() == false)
            {
                panelinternetyok.Visible = false;
            }
            else
            {
                panelinternetyok.Visible = true;
            }
        }

        private void Yenigonderimbaslatbtn_Click(object sender, EventArgs e)
        {
            Gizle();
            grupolusturvesecpaneli.Visible = true;

        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Anaform_VisibleChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void Anaform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (kapatilmasin == true)
            {
                this.Visible = false;
                e.Cancel = true;
            }
            else
            {
                notifyIcon1.Visible = false;
                Application.Exit();
            }
        }
        private void Gizle()
        {
            foreach (Control item in this.Controls)
            {
                if (item is Panel)
                {
                    Panel pnls = (Panel)item;
                    pnls.Visible = false;
                }
            }
        }
        private void Anaform_Load(object sender, EventArgs e)
        {

            if (System.IO.File.Exists("wwbf.sqlite") == false)
            {
               SQLiteConnection.CreateFile("MyDatabase.sqlite");


                LDB.sqlcalistir("CREATE TABLE tbl_gruplar (id integer PRIMARY KEY , grupadi varchar(20))");
                LDB.sqlcalistir("CREATE TABLE tbl_islemler (id integer PRIMARY KEY, grupid int, mesajid int, basarili int DEFAULT 0, basarisiz int DEFAULT 0, siradaki int DEFAULT 0, baslangictarihi Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)");
                LDB.sqlcalistir("CREATE TABLE tbl_log (id integer PRIMARY KEY, islemid int, numaraid int,durum varchar(11), tarih Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)");
                LDB.sqlcalistir("CREATE TABLE tbl_mesajlar (id integer PRIMARY KEY, mesaj varchar(255), baslik varchar(255))");
                LDB.sqlcalistir("CREATE TABLE tbl_numaralar (id integer PRIMARY KEY, grup_id int, adi varchar(55), soyadi varchar(55),numara varchar(20), kullan int default 1)");
            }

            Gizle();
            foreach (Control item in this.Controls)
            {
                if (item is Panel)
                {
                    Panel pnls = (Panel)item;
                    pnls.Dock = DockStyle.Fill;
                }
            }
            anasayfa.Visible = true;
        }

        private void Yenigrupaditextboxpaneli_Click(object sender, EventArgs e)
        {
            yenigrupaditextBox.Focus();
        }

        private void Yenigrupolusturbtnpaneli_Click(object sender, EventArgs e)
        {
            string yenigrupadi = yenigrupaditextBox.Text.Trim();
            if (yenigrupadi.Length < 4)
            {
                MessageBox.Show("Cannot create group name less than 4 characters", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else if (LDB.sorgula("select count(grupadi) from tbl_gruplar where grupadi='" + yenigrupadi + "' ") == "0")
            {

                if (LDB.sqlcalistir("INSERT INTO tbl_gruplar (grupadi) VALUES('" + yenigrupadi + "') ") == true)
                {
                    Gruplariyenile();
                    yenigrupaditextBox.ResetText();
                    yenigrupaditextBox.Focus();
                }
                else
                {
                    MessageBox.Show(yenigrupadi + " Could not add group name please check your database!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }

            }
            else
            {
                MessageBox.Show(yenigrupadi + " you have already used the group name to avoid confusion, change the name of the new group!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        
    }

        private void GrulardataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0)
            {

                string grupidsi = grulardataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                string grupadi = grulardataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                DialogResult dialogResult = MessageBox.Show(grupadi + " Deleting a group means deleting all numbers and history reports.. \n \n Do you still want to delete ? ", "Warning!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    LDB.sqlcalistir("Delete  tbl_log.* FROM (tbl_gruplar INNER JOIN tbl_islemler ON tbl_gruplar.id = tbl_islemler.grupid) INNER JOIN tbl_log ON tbl_islemler.id = tbl_log.islemid WHERE tbl_gruplar.id =" + grupidsi);
                    LDB.sqlcalistir("Delete  tbl_numaralar.* FROM tbl_numaralar INNER JOIN tbl_gruplar ON tbl_numaralar.grup_id = tbl_gruplar.id WHEREtbl_gruplar.id =" + grupidsi);
                    LDB.sqlcalistir("Delete from tbl_islemler  where grupid=" + grupidsi);
                    LDB.sqlcalistir("Delete from tbl_gruplar  where id=" + grupidsi);

                    Gruplariyenile();
                }
            }
            
        }

        private void GrulardataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in grulardataGridView.SelectedRows)
                {
                    grupsecildidevambtn.BackgroundImage = null;
                    selectgrupid = row.Cells[0].Value.ToString();
                }
            }
            catch
            {
                selectgrupid = "0"; grupsecildidevambtn.BackgroundImage = WebWhatsappBotFree.Properties.Resources.devamedemiyorum;
            }
            
        }
        public void Gruplariyenile()
        {

            grupsecildidevambtn.BackgroundImage = Properties.Resources.devamedemiyorum;
            selectgrupid = "0";
            grulardataGridView.Rows.Clear();
            DataTable cevap = LDB.goruntule("select * from tbl_gruplar ");
            if (cevap.Rows.Count >= 1)
            {
                int sira = 0;
                foreach (DataRow rowwws in cevap.Rows)
                {
                    sira++;

                    grulardataGridView.Rows.Add(new object[] { Convert.ToString(rowwws["id"].ToString()), sira.ToString(), Convert.ToString(rowwws["grupadi"].ToString()), null });
                }
                grupsecildidevambtn.BackgroundImage = null;
            }

        }
        private void Grupolusturvesecpaneli_VisibleChanged(object sender, EventArgs e)
        {
            if (grupolusturvesecpaneli.Visible == true)
            {
                Gruplariyenile();
            }
        }

        private void Grupsecildidevambtn_Click(object sender, EventArgs e)
        {
            if (grupsecildidevambtn.BackgroundImage == null)
            {
                Gizle();
                numaraduzenlepaneli.Visible = true;
            }
            else
            {
                MessageBox.Show("You cannot continue without the selected Group, if you have not created any groups, create and select a group", "Warning!");
            }
        }

        private void Btnanasayfayadn_Click(object sender, EventArgs e)
        {
            Gizle();
            anasayfa.Visible = true;
        }

        private void Numaralariptalanasayfabtn_Click(object sender, EventArgs e)
        {
            Gizle();
            anasayfa.Visible = true;
        }

        private void Numaralardengeridonbtn_Click(object sender, EventArgs e)
        {
            Gizle();
            grupolusturvesecpaneli.Visible = true;
        }
        public void Numaradatalariniyenile(string sorguny)
        {
            yuklenenkisisayisibildirir.Text = "";
            dataGridView1.Visible = true;
            dataGridView1.Rows.Clear();
            aktifkisisayisi = "0";
            DataTable cevap = LDB.goruntule(sorguny);

            if (cevap.Rows.Count >= 1)
            {
                foreach (DataRow rowwws in cevap.Rows)
                {
                    if (rowwws["kullan"].ToString() == "1")
                    {
                        dataGridView1.Rows.Add(new object[] { Convert.ToString(rowwws["id"].ToString()), Convert.ToString(rowwws["adi"].ToString()), Convert.ToString(rowwws["soyadi"].ToString()), Convert.ToString(rowwws["numara"].ToString()), (Bitmap)WebWhatsappBotFree.Properties.Resources.yesil, rowwws["kullan"] });
                    }
                    else if (rowwws["kullan"].ToString() == "0")
                    {
                        dataGridView1.Rows.Add(new object[] { Convert.ToString(rowwws["id"].ToString()), Convert.ToString(rowwws["adi"].ToString()), Convert.ToString(rowwws["soyadi"].ToString()), Convert.ToString(rowwws["numara"].ToString()), (Bitmap)WebWhatsappBotFree.Properties.Resources.kirmizi, rowwws["kullan"] });
                    }
                }

                aktifkisisayisi = LDB.sorgula("select count(*) from tbl_numaralar where grup_id = " + selectgrupid + " and kullan=1 ");
            }
            if (selectgrupid != "0" && dataGridView1.RowCount >= 1 && Convert.ToInt32(aktifkisisayisi) >= 1)
            {
                numaralarduzenlendidevametbtn.BackgroundImage = null;
            }
            else
            {
                numaralarduzenlendidevametbtn.BackgroundImage = WebWhatsappBotFree.Properties.Resources.devamedemiyorum;
            }
        }
        private void Excelveyavcfdennumarayuklebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Excel File (* .xlsx, * .xls) | * .xlsx; * .xls | Vcf File (*. vcf) | * .vcf; | All Files (*. *) | *. *";
            file.FilterIndex = 1;
            file.RestoreDirectory = true;
            file.CheckFileExists = false;
            file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            file.Title = "Select Excel File ..";
            file.Multiselect = false;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    string DosyaYolu = file.FileName;
                    FileInfo ff = new FileInfo(DosyaYolu);
                    string DosyaUzantisi = ff.Extension;
                    if (DosyaUzantisi == ".xls" || DosyaUzantisi == ".xlsx")
                    {
                        try
                        {

                            excelveyavcfdennumarayuklebtn.Visible = false;
                            numaralardengeridonbtn.Visible = false;
                            numaralariptalanasayfabtn.Visible = false;
                            numaralarduzenlendidevametbtn.Enabled = false;
                            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
                            xla.Visible = false;
                            xla.DisplayAlerts = false;
                            xla.ScreenUpdating = false;
                            xla.UserControl = false;
                            xla.Interactive = false;

                            Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Open(file.FileName.ToString());
                            Microsoft.Office.Interop.Excel.Worksheet ws = wb.Worksheets[1];
                            int columns = ws.UsedRange.Columns.Count;
                            int rows = ws.UsedRange.Rows.Count;
                            for (int i = 1; i <= rows; i++)
                            {
                                Application.DoEvents();
                                string telefonn = "";
                                foreach (char c in Convert.ToString(ws.Cells[i, 3].Value))
                                {
                                    Application.DoEvents();
                                    if (char.IsDigit(c))
                                    {
                                        Application.DoEvents();
                                        telefonn += c.ToString();
                                        Application.DoEvents();
                                    }
                                    Application.DoEvents();
                                }
                                if (telefonn[0].ToString() == "0")
                                {
                                    telefonn = "9" + telefonn;
                                }
                                else if (telefonn[0].ToString() != "9")
                                {
                                    telefonn = "90" + telefonn;
                                }

                                Application.DoEvents();
                                LDB.sqlcalistir("INSERT INTO tbl_numaralar (grup_id,adi,soyadi,numara) VALUES(" + selectgrupid + ", '" + Convert.ToString(ws.Cells[i, 1].Value) + "','" + Convert.ToString(ws.Cells[i, 2].Value) + "','" + telefonn + "' ) ");
                                Application.DoEvents();
                                yuklenenkisisayisibildirir.Text = i.ToString();
                                Application.DoEvents();
                            }
                            wb.Close(false);
                            xla.Quit();
                            Exceliramdensil(ws);
                            Exceliramdensil(wb);
                            Exceliramdensil(xla);
                            MessageBox.Show("Downloading is done");
                        }
                        catch (Exception hataaciklamasi)
                        {
                            MessageBox.Show("Failed to load numbers from excel n " + hataaciklamasi.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {

                            Numaradatalariniyenile("select * from tbl_numaralar where grup_id = " + selectgrupid);
                            excelveyavcfdennumarayuklebtn.Visible = true;
                            numaralardengeridonbtn.Visible = true;
                            numaralariptalanasayfabtn.Visible = true;
                            numaralarduzenlendidevametbtn.Enabled = true;
                        }
                    }
                    else if (DosyaUzantisi == ".vcf")
                    {
                        Vcfoku(DosyaYolu);
                    }
                }
                catch
                {
                }
            }
        }
        private void Vcfoku(string dosyayolu)
        {
            try
            {
                StreamReader dosyaOku = File.OpenText(dosyayolu);
                string okunanSatir;
                okunanSatir = dosyaOku.ReadLine();
                while (okunanSatir != null)
                {
                    string[] satir = okunanSatir.Split(':');
                    if (satir[0] == "N") Nparcala(satir[1]);
                    if (satir[0].StartsWith("TEL")) TELParcala(satir[1]);
                    okunanSatir = dosyaOku.ReadLine();
                }
                dosyaOku.Close();
                VcflisteGuncelle();

            }
            catch { MessageBox.Show("Failed to load numbers from vcf", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        }
        static void Nparcala(string Nbaslayan)
        {
            try
            {
                string[] satirN = Nbaslayan.Split(';');
                soyad = satirN[0]; ad = satirN[1]; ikinciAd = satirN[2];
            }
            catch { }
        }

        static void TELParcala(string TELbaslayan)
        {
            try
            {
                string tellll = "";
                foreach (char c in TELbaslayan)
                {
                    if (char.IsDigit(c))
                    {
                        tellll += c.ToString();
                    }
                }
                if (tellll[0].ToString() == "0")
                {
                    tellll = "9" + tellll;
                }
                else if (tellll[0].ToString() != "9")
                {
                    tellll = "90" + tellll;
                }
                cepNo = tellll;
                liste_kisi.Add(new Kisi(ad, ikinciAd, soyad, cepNo));
            }
            catch { }
        }

        void VcflisteGuncelle()
        {
            try
            {
                excelveyavcfdennumarayuklebtn.Visible = false;
                numaralardengeridonbtn.Visible = false;
                numaralariptalanasayfabtn.Visible = false;
                numaralarduzenlendidevametbtn.Enabled = false;
                for (int i = 0; i < liste_kisi.Count; i++)
                {

                    Application.DoEvents();
                    LDB.sqlcalistir("INSERT INTO tbl_numaralar (grup_id,adi,soyadi,numara) VALUES(" + selectgrupid + ", '" + liste_kisi[i].Ad + " " + liste_kisi[i].IkıncıAd + "','" + liste_kisi[i].Soyad + "','" + liste_kisi[i].CepNo + "' ) ");
                    Application.DoEvents();
                    this.Text = i.ToString();
                    Application.DoEvents();
                }

                Numaradatalariniyenile("select * from tbl_numaralar where grup_id = " + selectgrupid);
                excelveyavcfdennumarayuklebtn.Visible = true;
                numaralardengeridonbtn.Visible = true;
                numaralariptalanasayfabtn.Visible = true;
                numaralarduzenlendidevametbtn.Enabled = true;
                MessageBox.Show("Downloading is done");
            }
            catch { MessageBox.Show("Failed to load numbers from vcf", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        }
        public static void Exceliramdensil(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        private void Numaralarduzenlendidevametbtn_Click(object sender, EventArgs e)
        {
            if (numaralarduzenlendidevametbtn.BackgroundImage == null)
            {
                Gizle();
                mesajolusturyadasecpaneli.Visible = true;
            }
            else
            {
                MessageBox.Show("No active numbers in the selected group, you cannot continue without adding numbers to this active group", "Warning!");
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                string idsi = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() == "1")
                {

                    LDB.sqlcalistir("Update tbl_numaralar set kullan=0 where id=" + idsi);
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = WebWhatsappBotFree.Properties.Resources.kirmizi;
                    dataGridView1.Rows[e.RowIndex].Cells[5].Value = 0;
                }
                else if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() == "0")
                {

                    LDB.sqlcalistir("Update tbl_numaralar set kullan=1 where id=" + idsi);
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = (Bitmap)WebWhatsappBotFree.Properties.Resources.yesil;
                    dataGridView1.Rows[e.RowIndex].Cells[5].Value = 1;
                }

                aktifkisisayisi = LDB.sorgula("select count(*) from tbl_numaralar where grup_id = " + selectgrupid + " and kullan=1 ");
                if (selectgrupid != "0" && dataGridView1.RowCount >= 1 && Convert.ToInt32(aktifkisisayisi) >= 1)
                {
                    numaralarduzenlendidevametbtn.BackgroundImage = null;
                }
                else
                {
                    numaralarduzenlendidevametbtn.BackgroundImage = WebWhatsappBotFree.Properties.Resources.devamedemiyorum;
                }
            }
        }

        private void Numaraduzenlepaneli_VisibleChanged(object sender, EventArgs e)
        {
            if (numaraduzenlepaneli.Visible == true)
            {
                Numaradatalariniyenile("select * from tbl_numaralar where grup_id = " + selectgrupid);
            }
        }

        private void Mesajdananasayfayabtn_Click(object sender, EventArgs e)
        {
            Gizle();
            anasayfa.Visible = true;
        }

        private void Mesajdannumaralarabtn_Click(object sender, EventArgs e)
        {
            Gizle();
            numaraduzenlepaneli.Visible = true;
        }

        private void Mesajidasectikdevambtn_Click(object sender, EventArgs e)
        {
            if (mesajidasectikdevambtn.BackgroundImage == null)
            {
                islemloglabel.Text = "";
                nsICookieManager CookieMan;
                CookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
                CookieMan = Xpcom.QueryInterface<nsICookieManager>(CookieMan);
                CookieMan.RemoveAll();
                MessageBox.Show("Starting new post");
                Gizle();
                barkoduokutpnl.Visible = true;
                Wpbarkodvemesaj();
            }
        }
        private bool Yukleniyor()
        {
            try
            {
                geckoWebBrowser1.Document.GetElementById("initial_startup").ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private Image Base64ToImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = Image.FromStream(ms, true);
                return image;
            }
        }
        private void Wpkontroll()
        {
            bool ooooooo = true;
            Application.DoEvents();
            panelbarkodkenari.Visible = false;
            Application.DoEvents();
            while (ooooooo)
            {

                Application.DoEvents();
                if (islemisonlandir == true)
                {
                    return;
                }
                Application.DoEvents();
                while (DnsTest())
                {
                    Application.DoEvents();
                }
                while (Yukleniyor())
                {
                    Application.DoEvents();
                }
                Application.DoEvents();
                System.Threading.Thread.Sleep(200);
                try
                {
                    Application.DoEvents();
                    GeckoElementCollection tagsCollection = geckoWebBrowser1.Document.GetElementsByTagName("img");
                    Application.DoEvents();
                    foreach (GeckoElement currentTag in tagsCollection)
                    {
                        Application.DoEvents();
                        if (currentTag.GetAttribute("alt").Equals("Scan me!"))
                        {
                            string srcsi = currentTag.GetAttribute("src").ToString();
                            srcsi = srcsi.Substring(22);
                            pictureBoxbarkod.Image = Base64ToImage(srcsi);
                            Application.DoEvents();
                            panelbarkodkenari.Visible = true;
                            Application.DoEvents();
                            sondurum.Text = "You need to have the Web Whatsapp barcode read from your phone";
                            Application.DoEvents();
                            barkoduokutpnl.Visible = true;
                            Application.DoEvents();
                            gonderimlerbaslatildipanel.Visible = false;
                            Application.DoEvents();
                        }
                    }
                }
                catch
                {
                    Application.DoEvents();
                    panelbarkodkenari.Visible = true;
                    Application.DoEvents();
                    try
                    {
                        Application.DoEvents();
                        geckoWebBrowser1.Document.GetElementById("side").ToString();
                        Gizle();
                        gonderimlerbaslatildipanel.Visible = true;
                        Application.DoEvents();
                        ooooooo = false;
                        Application.DoEvents();
                    }
                    catch { Application.DoEvents(); ooooooo = true; Application.DoEvents(); }
                    Application.DoEvents();
                }
                Application.DoEvents();
            }
            Application.DoEvents();
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }
        public void Wpbarkodvemesaj()
        {
            int gecerliiii = 0;
            int gecersizzz = 0;

            islemisonlandir = false;
            geckoWebBrowser1.Navigate("https://web.whatsapp.com/");
            Application.DoEvents();
            sondurum.Text = "Check your phone's Internet connection and get ready.";
            Wpkontroll();
            Application.DoEvents();
                Application.DoEvents();
                LDB.sqlcalistir("INSERT INTO tbl_islemler (grupid,mesajid,siradaki) VALUES(" + selectgrupid + ", " + selectmesajid + "," + aktifkisisayisi + " ) ");
                Application.DoEvents();
                islemidsii = LDB.sorgula("SELECT id FROM tbl_islemler where grupid=" + selectgrupid + " and mesajid=" + selectmesajid + " and siradaki=" + aktifkisisayisi + "  ORDER BY id DESC LIMIT 1");
                Application.DoEvents();
            Application.DoEvents();
            siradakinumaralabel.Text = Convert.ToInt32(Convert.ToInt32(aktifkisisayisi) - (gecerliiii + gecersizzz)).ToString();
            gecerlileti.Text = "0";
            Application.DoEvents();
            foreach (DataGridViewRow rowp in dataGridView1.Rows)
            {

                Application.DoEvents();
                if (rowp.Cells[5].Value.ToString() == "1")
                {

                    Wpkontroll();
                    Application.DoEvents();
                    geckoWebBrowser1.Navigate("https://web.whatsapp.com/send?phone=+" + rowp.Cells[3].Value.ToString().Trim() + "&text=" + selectmesaj);
                    int[] oddArray = new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21 };
                    foreach (int num in oddArray)
                    {
                        SendKeys.SendWait("{ENTER}");
                        Application.DoEvents();
                        SendKeys.SendWait("{~}");
                        Application.DoEvents();
                    }
                    Application.DoEvents();
                    islemloglabel.Text = rowp.Cells[3].Value.ToString().Trim() + " Trying to text";
                    Application.DoEvents();
                    Wpkontroll();
                    Application.DoEvents();
                    try
                    {
                        Application.DoEvents();
                        geckoWebBrowser1.Document.GetElementsByClassName("popup-contents")[0].LastChild.ToString();
                        Application.DoEvents();
                        gecersizzz++;
                        gecersizileti.Text = gecersizzz.ToString();
                        siradakinumaralabel.Text = Convert.ToInt32(Convert.ToInt32(aktifkisisayisi) - (gecerliiii + gecersizzz)).ToString();
                        Application.DoEvents();
                        LDB.sqlcalistir("INSERT INTO tbl_log (islemid,numaraid,durum) VALUES(" + islemidsii + ", " + rowp.Cells[0].Value.ToString().Trim() + ", 'başarısız' ) ");
                        Application.DoEvents();
                    }
                    catch
                    {
                        try
                        {
                            Application.DoEvents();
                            GeckoElementCollection tagsCollection = geckoWebBrowser1.Document.GetElementsByTagName("button");
                            foreach (GeckoElement currentTag in tagsCollection)
                            {
                                if (currentTag.GetAttribute("id").Equals(""))
                                {
                                    Application.DoEvents();
                                    ((GeckoHtmlElement)currentTag).Click();
                                  Application.DoEvents();
                                    ((GeckoHtmlElement)currentTag).Click();
                                    gecerliiii++;
                                    siradakinumaralabel.Text = Convert.ToInt32(Convert.ToInt32(aktifkisisayisi) - (gecerliiii + gecersizzz)).ToString();
                                    gecerlileti.Text = gecerliiii.ToString();
                                    Application.DoEvents();
                                    LDB.sqlcalistir("INSERT INTO tbl_log (islemid,numaraid,durum) VALUES(" + islemidsii + ", " + rowp.Cells[0].Value.ToString().Trim() + ", 'başarılı') ");
                                    Application.DoEvents();
                                    Thread.Sleep(3000);
                                    Application.DoEvents();
                                }
                            }
                            Application.DoEvents();
                        }
                        catch
                        {

                        }
                    }
                    Application.DoEvents();
                    LDB.sqlcalistir("Update tbl_islemler set siradaki=" + siradakinumaralabel.Text.ToString().Trim() + ", basarisiz=" + gecersizileti.Text.ToString().Trim() + ", basarili=" + gecerlileti.Text.ToString().Trim() + " where id=" + islemidsii);
                    Application.DoEvents();

                }
            }
            MessageBox.Show(gecerlileti.Text.ToString().Trim() + " successful posts", "Congratulations Action Complete");
            islemloglabel.Text = "";
            Gizle();
            RAPORLAR.Visible = true;
        }
       
        private void Panel3_Click(object sender, EventArgs e)
        {
            yenimesajtextbox.Focus();
        }

        private void Panel4_Click(object sender, EventArgs e)
        {
            yenimesajsablonaditextBox.Focus();
        }

        private void Mesajsablonuolusturbtn_Click(object sender, EventArgs e)
        {
            string yenimesajj = yenimesajtextbox.Text.Trim();
            string yenimesajsablonadi = yenimesajsablonaditextBox.Text.Trim();
            if (yenimesajj.Length < 3)
            {
                MessageBox.Show("Messages less than 3 characters cannot be created", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else if (yenimesajsablonadi.Length < 3)
            {
                MessageBox.Show("Template name less than 3 characters cannot be created", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else if (LDB.sorgula("select count(baslik) from tbl_mesajlar where baslik='" + yenimesajsablonadi + "' ") == "0")
            {


                if (LDB.sqlcalistir("INSERT INTO tbl_mesajlar (mesaj,baslik) VALUES('" + yenimesajj + "' , '" + yenimesajsablonadi + "' ) ") == true)
                {
                    Mesajlariyenile();
                }
                else
                {
                    MessageBox.Show(yenimesajsablonadi + " Failed to add template please check your database!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }

            }
            else
            {
                MessageBox.Show(yenimesajsablonadi + " Change the template name to the new template name to avoid confusion!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            yenimesajtextbox.ResetText();
            yenimesajsablonaditextBox.ResetText();
            yenimesajtextbox.Focus();
        }

        private void DataGridViewmesajlar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                string mesajsablonidsi = dataGridViewmesajlar.Rows[e.RowIndex].Cells[0].Value.ToString();
                string mesajsablonadi = dataGridViewmesajlar.Rows[e.RowIndex].Cells[2].Value.ToString();
                DialogResult dialogResult = MessageBox.Show(mesajsablonadi + " Deleting a message template means deleting reports that you discard.. \n \n Do you still want to delete? ", "Warning!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {

                    LDB.sqlcalistir("Delete tbl_log.* FROM(tbl_mesajlar INNER JOIN tbl_islemler ON tbl_mesajlar.id = tbl_islemler.mesajid) INNER JOIN tbl_log ON tbl_islemler.id = tbl_log.islemid WHERE tbl_mesajlar.id =" + mesajsablonidsi);
                    LDB.sqlcalistir("Delete FROM tbl_islemler where mesajid =" + mesajsablonidsi);
                    LDB.sqlcalistir("Delete from tbl_mesajlar where id=" + mesajsablonidsi);
                    Mesajlariyenile();
                }

            }
        }

        private void DataGridViewmesajlar_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridViewmesajlar.SelectedRows)
                {
                    mesajidasectikdevambtn.BackgroundImage = null;
                    selectmesajid = row.Cells[0].Value.ToString();
                    selectmesaj = row.Cells[3].Value.ToString();
                }
            }
            catch
            {
                selectmesajid = "0"; mesajidasectikdevambtn.BackgroundImage = WebWhatsappBotFree.Properties.Resources.devamedemiyorum;
            }
        }

        private void Mesajolusturyadasecpaneli_VisibleChanged(object sender, EventArgs e)
        {
            if (mesajolusturyadasecpaneli.Visible == true)
            {
                Mesajlariyenile();
            }
        }
        public void Mesajlariyenile()
        {
            mesajidasectikdevambtn.BackgroundImage = WebWhatsappBotFree.Properties.Resources.devamedemiyorum;
            dataGridViewmesajlar.Rows.Clear();
            mesajlarlistesi.Clear();

            DataTable cevapmesajlar = LDB.goruntule("select * from tbl_mesajlar");
            if (cevapmesajlar.Rows.Count >= 1)
            {
                int sira = 0;
                foreach (DataRow rowwws in cevapmesajlar.Rows)
                {
                    sira++;
                    dataGridViewmesajlar.Rows.Add(new object[] { Convert.ToString(rowwws["id"].ToString()), sira.ToString(), Convert.ToString(rowwws["baslik"].ToString()), Convert.ToString(rowwws["mesaj"].ToString()) });
                }
            }
        }

        private void Barkoduokutmadangerigelbtn_Click(object sender, EventArgs e)
        {
            Gizle();
            islemisonlandir = true;
            mesajolusturyadasecpaneli.Visible = true;
        }

        private void Anasayfagit_Click(object sender, EventArgs e)
        {
            islemisonlandir = true;
            Gizle();
            anasayfa.Visible = true;
        }

        private void Raporlardangeribtn_Click(object sender, EventArgs e)
        {
            Gizle();
            anasayfa.Visible = true;
        }

        private void Raporlardananasayfaya_Click(object sender, EventArgs e)
        {
            Gizle();
            anasayfa.Visible = true;
        }

        private void GeckoWebBrowser1_ShowContextMenu(object sender, GeckoContextMenuEventArgs e)
        {
            try
            {
                foreach (MenuItem i in e.ContextMenu.MenuItems)
                {
                    e.ContextMenu.MenuItems.Remove(i);
                }
            }
            catch { }
        }

        private void RAPORLAR_VisibleChanged(object sender, EventArgs e)
        {
            if (RAPORLAR.Visible == true)
            {
                raporgrid.Rows.Clear();
                while (DnsTest())
                {
                    Application.DoEvents();
                }
                DataTable cevap = LDB.goruntule("SELECT tbl_islemler.id as 'islemidisiii', tbl_gruplar.grupadi, tbl_mesajlar.baslik, tbl_islemler.basarili, tbl_islemler.basarisiz, tbl_islemler.siradaki, tbl_islemler.baslangictarihi FROM tbl_mesajlar INNER JOIN(tbl_gruplar INNER JOIN tbl_islemler ON tbl_gruplar.id = tbl_islemler.grupid) ON tbl_mesajlar.id = tbl_islemler.mesajid ORDER BY tbl_islemler.id DESC ");
                if (cevap.Rows.Count >= 1)
                {
                    foreach (DataRow rowwws in cevap.Rows)
                    {
                        raporgrid.Rows.Add(new object[] { Convert.ToString(rowwws["islemidisiii"].ToString()), Convert.ToString(rowwws["grupadi"].ToString()), Convert.ToString(rowwws["baslik"].ToString()), Convert.ToString(rowwws["basarili"].ToString()), Convert.ToString(rowwws["basarisiz"].ToString()), Convert.ToString(rowwws["siradaki"].ToString()), Convert.ToString(rowwws["baslangictarihi"].ToString()) });
                    }
                }
            }
        }

        private void Raporgrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7 && e.RowIndex >= 0)
            {
                int geceriliii = Convert.ToInt32(raporgrid.Rows[e.RowIndex].Cells[3].Value.ToString());

                if (geceriliii >= 1)
                {
                    DialogResult result = fld.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(this.fld.SelectedPath))
                    {
                        while (DnsTest())
                        {
                            Application.DoEvents();
                        }
                        string raporrridsi = raporgrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                        DTTOEXCEL.excelolustur(LDB.goruntule("SELECT tbl_numaralar.adi, tbl_numaralar.soyadi, tbl_numaralar.numara, tbl_mesajlar.mesaj, tbl_log.durum, tbl_log.tarih FROM tbl_gruplar INNER JOIN tbl_islemler ON tbl_islemler.grupid = tbl_gruplar.id INNER JOIN tbl_log ON tbl_islemler.id = tbl_log.islemid INNER JOIN tbl_mesajlar ON tbl_mesajlar.id = tbl_islemler.mesajid INNER JOIN tbl_numaralar ON tbl_log.numaraid = tbl_numaralar.id AND tbl_numaralar.grup_id = tbl_gruplar.id WHERE tbl_islemler.id = " + raporrridsi + " AND tbl_log.durum = 'başarılı'"), this.fld.SelectedPath.ToString() + "/basariligonderimler" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls", "Geçerli Gönderimler");
                        MessageBox.Show("was recorded");
                    }
                }
                else
                {
                    MessageBox.Show("No current messages!", "Attention!");
                }
            }
            else if(e.ColumnIndex == 8 && e.RowIndex >= 0)
            {
                int gecersizz = Convert.ToInt32(raporgrid.Rows[e.RowIndex].Cells[4].Value.ToString());

                if (gecersizz >= 1)
                {
                    DialogResult result = fld.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(this.fld.SelectedPath))
                    {
                        while (DnsTest())
                        {
                            Application.DoEvents();
                        }
                        string raporrridsi = raporgrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                        DTTOEXCEL.excelolustur(LDB.goruntule("SELECT tbl_numaralar.adi, tbl_numaralar.soyadi, tbl_numaralar.numara, tbl_log.durum, tbl_log.tarih FROM tbl_gruplar INNER JOIN tbl_islemler ON tbl_islemler.grupid = tbl_gruplar.id INNER JOIN tbl_log ON tbl_islemler.id = tbl_log.islemid INNER JOIN tbl_numaralar ON tbl_log.numaraid = tbl_numaralar.id AND tbl_numaralar.grup_id = tbl_gruplar.id WHERE tbl_islemler.id =" + raporrridsi + " AND tbl_log.durum = 'başarısız'"), this.fld.SelectedPath.ToString() + "/hataliveyawhatsappyok" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls", "Geçerli Gönderimler");
                        MessageBox.Show("was recorded");
                    }
                }
                else
                {
                    MessageBox.Show("No Invalid or Whatsapp Unused Number!", "Attention!");
                }
            }
            else if (e.ColumnIndex == 9 && e.RowIndex >= 0)
            {
                int siradaaa = Convert.ToInt32(raporgrid.Rows[e.RowIndex].Cells[5].Value.ToString());

                if (siradaaa >= 1)
                {
                    DialogResult result = fld.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(this.fld.SelectedPath))
                    {
                        while (DnsTest())
                        {
                            Application.DoEvents();
                        }
                        string raporrridsi = raporgrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                        DTTOEXCEL.excelolustur(LDB.goruntule("SELECT tbl_numaralar.numara, tbl_numaralar.soyadi, tbl_numaralar.adi FROM tbl_gruplar INNER JOIN tbl_islemler ON tbl_islemler.grupid = tbl_gruplar.id INNER JOIN tbl_numaralar ON tbl_numaralar.grup_id = tbl_gruplar.id WHERE tbl_islemler.id = "+ raporrridsi +" AND tbl_numaralar.kullan = 1 AND tbl_numaralar.id not IN(SELECT tbl_numaralar.id FROM tbl_gruplar INNER JOIN tbl_islemler ON tbl_islemler.grupid = tbl_gruplar.id INNER JOIN tbl_log ON tbl_islemler.id = tbl_log.islemid INNER JOIN tbl_numaralar ON tbl_log.numaraid = tbl_numaralar.id AND tbl_numaralar.grup_id = tbl_gruplar.id WHERE tbl_islemler.id = "+ raporrridsi ), this.fld.SelectedPath.ToString() + " /YarimkalanNumaralar" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls", "Geçerli Gönderimler");
                        MessageBox.Show("was recorded");
                    }
                }
                else
                {
                    MessageBox.Show("Unfinished Number!", "Attention!");
                }
            }
        }

        private void Instagram_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/esrefyildirici/");
        }

        private void Youtube_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/user/esrefyildirici");
        }

        private void Git_Click(object sender, EventArgs e)
        {
           System.Diagnostics.Process.Start("https://github.com/esrefyildirici");
        }

        private void YenimesajsablonaditextBox_Enter(object sender, EventArgs e)
        {
            if (yenimesajsablonaditextBox.Text == "Template name")
            {
                yenimesajsablonaditextBox.Text = "";
                yenimesajsablonaditextBox.ForeColor = Color.DimGray;
            }
        }

        private void YenimesajsablonaditextBox_Leave(object sender, EventArgs e)
        {
            if (yenimesajsablonaditextBox.Text == "")
            {
                yenimesajsablonaditextBox.Text = "Template name";
                yenimesajsablonaditextBox.ForeColor = Color.LightGray;
            }
        }

    }
}
