using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BilgisayarliGörme
{
    /*
     * GÖRÜNTÜ İŞLEME UYGULAMASI
     * Bu uygulama, temel görüntü işleme ve kümeleme algoritmalarını içeren bir Windows Forms uygulamasıdır.
     * 
     * TEMEL ÖZELLİKLER:
     * 1. Resim Yükleme: Kullanıcı bilgisayarından bir resim dosyası seçebilir.
     * 2. Gri Tonlama: Renkli resmi gri tonlamalı hale çevirir.
     * 3. Y (Luminance) Dönüşümü: İnsan gözünün hassasiyetini dikkate alarak gri tonlamaya çevirir.
     * 4. Histogram Eşitleme: Görüntünün kontrastını iyileştirir.
     * 5. K-Means Kümeleme: Farklı mesafe ölçütleri kullanarak görüntüyü kümelere ayırır:
     *    - Intensity (Yoğunluk) bazlı kümeleme
     *    - RGB renk uzayında Öklid mesafesi bazlı kümeleme
     *    - Mahalanobis mesafesi bazlı kümeleme
     *    - Normalize edilmiş Mahalanobis mesafesi bazlı kümeleme
     * 6. Kenar Bulma: Sobel operatörü kullanarak görüntüdeki kenarları tespit eder.
     * 
     * KULLANIM:
     * 1. "Resim Yükle" butonu ile bir resim seçin
     * 2. İşlem ComboBox'ından yapmak istediğiniz işlemi seçin
     * 3. K-Means işlemleri için küme sayısını (k) seçin
     * 4. "Uygula" butonuna basarak seçilen işlemi gerçekleştirin
     * 
     * ÇIKTILAR:
     * - İşlenmiş görüntü sağ tarafta gösterilir
     * - Histogram grafiği alt kısımda gösterilir
     * - K-Means işlemleri için küme merkezleri ve piksel sayıları listelenir
     * - İterasyon sayısı ve toplam piksel sayısı gösterilir
     */

    public partial class Form1 : Form
    {
        public Form1()
        {
            // İşlem ComboBox'ını temizle ve yeni işlemleri ekle
            InitializeComponent();
            islemComboBox.Items.Clear();
            islemComboBox.Items.AddRange(new string[] {
                "Gri Yap",            // RGB → Gri dönüşümü
                "Y Yap",              // RGB →  Y (Luminance) dönüşümü
                "Histogram",          // Histogram eşitleme
                "KM Intensity",       // K-Means yoğunluk kümeleme
                "KM Öklid RGB",       // K-Means RGB Öklid mesafeli kümeleme
                "KM Mahlanobis",      // K-Means Mahalanobis mesafeli kümeleme
                "KM Mahlanobis ND",   // K-Means normalize edilmiş Mahalanobis
                "Kenar Bulma"         // Kenar tespit algoritması
            });

            // K değeri ComboBox'ını temizle ve küme sayılarını ekle (2-20 ve 50)
            kmNumComboBox.Items.Clear();
            kmNumComboBox.Items.AddRange(new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 50 });

            // Buton ve ComboBox olaylarını tanımla
            resimyukle.Click += new EventHandler(resimyukle_Click);
            uygula.Click += new EventHandler(uygula_Click);
            islemComboBox.SelectedIndexChanged += new EventHandler(islemComboBox_SelectedIndexChanged);
        }

        // Form yüklendiğinde çalışacak metod
        private void Form1_Load(object sender, EventArgs e)
        {
            // Histogram grafiğinin eksen ayarlarını yap
            AyarlaChart1();
        }

        // Resim yükleme butonu click olayı
        private void resimyukle_Click(object sender, EventArgs e)
        {
            // Dosya seçme dialogu oluştur
            OpenFileDialog ofd = new OpenFileDialog();
            // Sadece resim dosyalarını göster
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            ofd.Title = "Resim Seçiniz";

            // Kullanıcı bir dosya seçip OK'e bastıysa
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Seçilen resmi PictureBox'a yükle
                    girisFoto.Image = Image.FromFile(ofd.FileName);
                    // Resmi pencereye sığacak şekilde ayarla
                    girisFoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    // Hata durumunda kullanıcıyı bilgilendir
                    MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message, "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        //--------------------------------------------------------------------------------------------------------


        /*
         * GRİ TONLAMA İŞLEMİ
         * Bu metod, renkli bir görüntüyü gri tonlamalı hale çevirir.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Her piksel için RGB (Kırmızı, Yeşil, Mavi) değerleri alınır
         * 2. Bu değerlerin aritmetik ortalaması hesaplanır: (R + G + B) / 3
         * 3. Bulunan ortalama değer, yeni pikselin R, G ve B değerlerine eşit olarak atanır
         * 
         * FORMÜL:
         * Gri Değer = (Kırmızı + Yeşil + Mavi) / 3
         * 
         * NOT: Bu basit bir gri tonlama yöntemidir. İnsan gözünün renk hassasiyetini
         * dikkate almaz. Daha hassas sonuçlar için Y (Luminance) dönüşümü tercih edilebilir.
         */
        private void GriYap()
        {
            //Giriş Resminin olup olmadığını kontrol ediyoruz
            if (girisFoto.Image == null) return;

            // Giriş resminden yeni bir Bitmap oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);

            //Çıkış için aynı boyutlarda boş bir Bitmap oluşturulur
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            //Her pixel için döngü oluşturarak işlem yapıyoruz
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    //Her pikselin RGB değerleri alınır
                    Color piksel = girisResim.GetPixel(x, y);
                    
                    //RGB değerlerinin aritmetik ortalaması alınarak gri değer hesaplanır
                    int griDeger = (piksel.R + piksel.G + piksel.B) / 3;

                    //Yeni piksel rengi, hesaplanan gri değer kullanılarak oluşturulur
                    Color yeniRenk = Color.FromArgb(griDeger, griDeger, griDeger);

                    //Bu değer R, G ve B kanallarına eşit olarak atanır.
                    cikisResim.SetPixel(x, y, yeniRenk);
                }
            }

            //Sonuç Göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;
        }




        //-----------------------------------------------------------------------------------------------------------


        /*
         * Y (LUMINANCE) DÖNÜŞÜMÜ
         * Bu metod, renkli bir görüntüyü insan gözünün renk hassasiyetini dikkate alarak
         * gri tonlamalı hale çevirir.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Her piksel için RGB değerleri alınır
         * 2. İnsan gözünün renk hassasiyetine göre ağırlıklandırılmış formül uygulanır:
         *    - Kırmızı için 0.299 (İnsan gözü kırmızıya %29.9 hassas)
         *    - Yeşil için 0.587 (İnsan gözü yeşile %58.7 hassas)
         *    - Mavi için 0.114 (İnsan gözü maviye %11.4 hassas)
         * 
         * FORMÜL:
         * Y = 0.299R + 0.587G + 0.114B
         * 
         * NOT: Bu yöntem, basit gri tonlamaya göre insan gözünün algısına daha yakın
         * sonuçlar verir. Özellikle görüntü işleme uygulamalarında tercih edilir.
         */
        private void YYap()
        {
            //Giriş Resminin olup olmadığını kontrol ediyoruz
            if (girisFoto.Image == null) return;

            // Giriş resminden yeni bir Bitmap oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);

            //Çıkış için aynı boyutlarda boş bir Bitmap oluşturulur
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            //Her pixel için döngü oluşturarak işlem yapıyoruz
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    //Her pikselin RGB değerleri alınır
                    Color piksel = girisResim.GetPixel(x, y);

                    //Y değeri hesaplanır : Y = 0.299R + 0.587G + 0.114B
                    int yDeger = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);

                    //Y değerinin 0-255 aralığında olması sağlandı
                    yDeger = Math.Max(0, Math.Min(255, yDeger));

                    //Yeni piksel rengi, hesaplanan y değeri kullanılarak oluşturulur
                    Color yeniRenk = Color.FromArgb(yDeger, yDeger, yDeger);

                    //Bu değer R, G ve B kanallarına eşit olarak atanır.
                    cikisResim.SetPixel(x, y, yeniRenk);
                }
            }

            //Sonuç Göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;
        }





        //-----------------------------------------------------------------------------------------------------------


        // Slider değeri değiştiğinde çağrılacak olay
        private void trackBarThreshold_ValueChanged(object sender, EventArgs e)
        {
            labelThreshold.Text = $"Threshold Değeri: {trackBarThreshold.Value}";
            // Eğer kenar bulma seçili ise, değer değiştiğinde otomatik olarak uygula
            if (islemComboBox.SelectedItem?.ToString() == "Kenar Bulma")
            {
                KenarBulma();
            }
        }

        private void islemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kenar bulma seçildiğinde slider'ı göster, diğer durumlarda gizle
            if (islemComboBox.SelectedItem?.ToString() == "Kenar Bulma")
            {
                trackBarThreshold.Visible = true;
                labelThreshold.Visible = true;
            }
            else
            {
                trackBarThreshold.Visible = false;
                labelThreshold.Visible = false;
            }
        }

        /*
         * KENAR BULMA İŞLEMİ (SOBEL OPERATÖRÜ)
         * Bu metod, görüntüdeki kenarları tespit etmek için Sobel operatörünü kullanır.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Görüntü önce gri tonlamaya çevrilir: Y = 0.299R + 0.587G + 0.114B
         * 2. Her piksel için 3x3'lük komşuluk üzerinde Sobel operatörü uygulanır:
         *    - Yatay Kenarlar (Gx): [[-1 0 1],  [-2 0 2],  [-1 0 1]]
         *    - Dikey Kenarlar (Gy): [[ 1 2 1],  [ 0 0 0],  [-1 -2 -1]]
         * 3. X ve Y yönündeki değerler ayrı matrislerde saklanır:
         *    - kenarX[x,y] = |Gx|  (X yönündeki değerin mutlak değeri)
         *    - kenarY[x,y] = |Gy|  (Y yönündeki değerin mutlak değeri)
         * 4. Her piksel için X ve Y değerleri toplanır:
         *    kenarDegeri = |Gx| + |Gy|
         * 5. Hesaplanan değer belirlenen eşik değeri ile karşılaştırılır:
         *    - Eğer kenarDegeri < threshold ise piksel = 0 (siyah)
         *    - Eğer kenarDegeri ≥ threshold ise piksel = 255 (beyaz)
         * 
         * PARAMETRELER:
         * - Threshold (Eşik) Değeri: Kullanıcı tarafından ayarlanabilir
         *   Düşük değerler daha fazla kenar tespit eder, yüksek değerler daha az
         * 
         * NOT: Bu yöntem, X ve Y yönlerindeki değerleri ayrı ayrı hesaplayıp
         * toplar. Gradyan büyüklüğü hesaplaması yerine mutlak değerlerin
         * toplamı kullanılır.
         */
        private void KenarBulma()
        {
            //Giriş Resminin olup olmadığını kontrol ediyoruz
            if (girisFoto.Image == null) return;

            //Giriş, gri tonlama ve çıkış resimleri için Bitmap'ler oluşturulur
            Bitmap girisResim = new Bitmap(girisFoto.Image);
            Bitmap griResim = new Bitmap(girisResim.Width, girisResim.Height);
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            //X ve Y Yönündeki gradyanlar için Sobel Matrisi oluşturduk.
            int[,] sobelX = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } }; //Dikey Kenar Tespiti
            int[,] sobelY = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } }; //Yatay Kenar Tespiti

            // Görüntüyü gri tonlamaya çevir
            // Bu adım kenar tespitini kolaylaştırır çünkü tek bir kanal üzerinde işlem yapılır
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Mevcut pikselin rengini al
                    Color piksel = girisResim.GetPixel(x, y);
                    int griDeger = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);
                    // Hesaplanan gri değeri ile yeni piksel oluştur
                    griResim.SetPixel(x, y, Color.FromArgb(griDeger, griDeger, griDeger));
                }
            }

            // X ve Y yönündeki kenar değerlerini tutacak matrisler
            int[,] kenarX = new int[girisResim.Width, girisResim.Height];
            int[,] kenarY = new int[girisResim.Width, girisResim.Height];

            // Her pixel için Kenar tespiti (Kenar Pixeller hariç)
            for (int x = 1; x < girisResim.Width - 1; x++)
            {
                for (int y = 1; y < girisResim.Height - 1; y++)
                {
                    // X ve Y yönündeki değerleri tutacak değişkenler
                    int gx = 0, gy = 0;

                    // 3x3'lük komşuluk üzerinde Sobel operatörlerini uygula
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            // Komşu pikselin gri değerini al
                            Color piksel = griResim.GetPixel(x + i, y + j);
                            int griDeger = piksel.R;   // Gri resimde R=G=B olduğu için herhangi birini alabiliriz

                            // X ve Y yönündeki değerleri hesapla
                            gx += griDeger * sobelX[i + 1, j + 1];
                            gy += griDeger * sobelY[i + 1, j + 1];
                        }
                    }

                    // X ve Y yönündeki değerleri ayrı matrislere kaydet
                    kenarX[x, y] = Math.Abs(gx);
                    kenarY[x, y] = Math.Abs(gy);
                }
            }

            // Slider'dan threshold değerini al
            int threshold = trackBarThreshold.Value;

            // X ve Y matrislerindeki değerleri topla ve eşik değeri ile karşılaştır
            for (int x = 1; x < girisResim.Width - 1; x++)
            {
                for (int y = 1; y < girisResim.Height - 1; y++)
                {
                    // X ve Y yönündeki değerleri topla
                    int kenarDegeri = kenarX[x, y] + kenarY[x, y];

                    // Eşik değeri ile karşılaştır
                    kenarDegeri = kenarDegeri < threshold ? 0 : 255;

                    // Sonuç pikselini çıkış görüntüsüne yaz
                    cikisResim.SetPixel(x, y, Color.FromArgb(kenarDegeri, kenarDegeri, kenarDegeri));
                }
            }

            //Sonuç Göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;
        }



        //-----------------------------------------------------------------------------------------------------------



        /*
         * K-MEANS YOĞUNLUK (INTENSITY) KÜMELEME
         * Bu metod, görüntüyü piksellerin parlaklık değerlerine göre k adet kümeye ayırır.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Her piksel için Y (Luminance) değeri hesaplanır: Y = 0.299R + 0.587G + 0.114B
         * 2. k adet rastgele merkez seçilir
         * 3. Her piksel, kendisine en yakın merkeze atanır (mutlak fark kullanılır)
         * 4. Her kümenin yeni merkezi, o kümedeki piksellerin ortalaması olarak güncellenir
         * 5. Merkezler değişmeyene kadar 3. ve 4. adımlar tekrarlanır
         * 
         * PARAMETRELER:
         * - k: Küme sayısı (kullanıcı tarafından seçilir)
         * 
         * ÇIKTILAR:
         * - İşlenmiş görüntü: Her piksel, ait olduğu kümenin merkez değerini alır
         * - Histogram: Orijinal görüntünün histogramı ve küme merkezleri
         * - İterasyon sayısı: Algoritmanın kaç adımda sonlandığı
         * - Küme bilgileri: Her kümenin merkez değeri ve piksel sayısı
         * 
         * NOT: Bu yöntem, görüntüyü sadece parlaklık değerlerine göre segmente eder.
         * Renk bilgisi dikkate alınmaz.
         */
        private void KMIntensity()
        {
            //Giriş Resminin olup olmadığını kontrol ediyoruz
            if (girisFoto.Image == null) return;

            // Küme sayısının seçilip seçilmediğini kontrol et
            if (kmNumComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen küme sayısını seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen küme sayısını al (k değeri)
            int k = (int)kmNumComboBox.SelectedItem;

            // Giriş görüntüsünden yeni bir Bitmap oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);

            // Çıkış için aynı boyutlarda boş bir Bitmap oluştur
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            // Tüm piksellerin gri değerlerini tutacak dizi
            // Dizinin boyutu = görüntünün toplam piksel sayısı
            int[] pikselDegerleri = new int[girisResim.Width * girisResim.Height];
            int index = 0;

            // Görüntüyü gri tonlamaya çevir ve değerleri diziye kaydet
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Her piksel için Y (Luminance) değerini hesapla
                    Color piksel = girisResim.GetPixel(x, y);

                    // Y = 0.299R + 0.587G + 0.114B formülü ile gri değer hesaplanır
                    int griDeger = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);
                    pikselDegerleri[index++] = griDeger;
                }
            }

            // K-Means için başlangıç merkezlerini rastgele seç
            Random rnd = new Random();
            int[] merkezler = new int[k];
            for (int i = 0; i < k; i++)
            {
                // Mevcut piksellerden rastgele birini merkez olarak seç
                merkezler[i] = pikselDegerleri[rnd.Next(pikselDegerleri.Length)];
            }

            // K-Means iterasyonları için maksimum sayı
            int maxIterasyon = 100;
            int iterasyonSayisi = 0;

            // Her pikselin hangi kümeye ait olduğunu tutan dizi
            int[] kümeler = new int[pikselDegerleri.Length];

            // K-Means algoritması ana döngüsü
            for (int iter = 0; iter < maxIterasyon; iter++)
            {
                iterasyonSayisi++;
                // Kümelerde değişim olup olmadığını kontrol etmek için flag
                bool değişimVar = false;

                // 1. ADIM: Her pikseli en yakın kümeye ata
                for (int i = 0; i < pikselDegerleri.Length; i++)
                {
                    int enYakınKüme = 0;
                    int enKüçükUzaklık = int.MaxValue;

                    // Her merkez için uzaklık hesapla
                    for (int j = 0; j < k; j++)
                    {
                        // Intensity değerleri arasındaki mutlak fark = uzaklık
                        int uzaklık = Math.Abs(pikselDegerleri[i] - merkezler[j]);
                        if (uzaklık < enKüçükUzaklık)
                        {
                            enKüçükUzaklık = uzaklık;
                            enYakınKüme = j;
                        }
                    }

                    // Eğer piksel farklı bir kümeye atandıysa değişim var demektir
                    if (kümeler[i] != enYakınKüme)
                    {
                        değişimVar = true;
                        kümeler[i] = enYakınKüme;
                    }
                }

                // 2. ADIM: Küme merkezlerini güncelle
                // Her küme için toplam ve eleman sayısını tut
                int[] toplamlar = new int[k];
                int[] sayılar = new int[k];

                // Her kümenin toplam değerini ve eleman sayısını hesapla
                for (int i = 0; i < pikselDegerleri.Length; i++)
                {
                    toplamlar[kümeler[i]] += pikselDegerleri[i];
                    sayılar[kümeler[i]]++;
                }

                // Yeni merkez = kümedeki değerlerin ortalaması
                for (int i = 0; i < k; i++)
                {
                    if (sayılar[i] > 0)
                    {
                        merkezler[i] = toplamlar[i] / sayılar[i];
                    }
                }

                // Eğer hiç değişim yoksa algoritma yakınsadı demektir, döngüyü bitir
                if (!değişimVar) break;
            }

            // İterasyon sayısını label7'ye yazdır
            label7.Text = iterasyonSayisi.ToString();

            // Sonuç görüntüsünü oluştur
            index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Her piksel için ait olduğu kümenin merkez değerini kullan
                    int kümeIndex = kümeler[index];
                    int yeniDeger = merkezler[kümeIndex];

                    // Gri tonlamalı piksel oluştur (R=G=B=yeniDeger)
                    cikisResim.SetPixel(x, y, Color.FromArgb(yeniDeger, yeniDeger, yeniDeger));
                    index++;
                }
            }

            // Sonuç görüntüsünü göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;

            // Önceki serileri temizle
            chart1.Series.Clear();
            chart1.Titles.Clear();

            // Grafiğin başlığını ayarla
            chart1.Titles.Add($"KM Intensity (k={k}) Histogram");

            // Histogram serisi
            var histogramSeries = chart1.Series.Add("Histogram");
            histogramSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            // Histogram dizisi
            int[] histogram = new int[256];
            foreach (int deger in pikselDegerleri)
            {
                histogram[deger]++;
            }

            // Histogram değerlerini grafiğe ekle
            for (int i = 0; i < 256; i++)
            {
                histogramSeries.Points.AddXY(i, histogram[i]);
            }

            // Merkez noktaları için yeni bir seri
            var merkezSeries = chart1.Series.Add("Merkezler");
            merkezSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            merkezSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
            merkezSeries.MarkerSize = 10;
            merkezSeries.Color = Color.Red;

            // Merkez noktalarını ekle ve ListBox'a yazdır
            listBox1.Items.Clear();
            List<int> siraliMerkezler = new List<int>(merkezler);
            siraliMerkezler.Sort();

            for (int i = 0; i < k; i++)
            {
                merkezSeries.Points.AddXY(merkezler[i], histogram[merkezler[i]]);
                listBox1.Items.Add($"T{i + 1}: {siraliMerkezler[i]}");
            }

            // ListBox2'ye merkez değerlerini yazdır
            listBox2.Items.Clear();
            for (int i = 0; i < k; i++)
            {
                // Her kümenin piksel sayısını hesapla
                int kumePikselSayisi = 0;
                for (int j = 0; j < pikselDegerleri.Length; j++)
                {
                    if (kümeler[j] == i)
                    {
                        kumePikselSayisi++;
                    }
                }
                // Intensity değerlerinde R=G=B olduğu için aynı değeri kullanıyoruz
                listBox2.Items.Add($"T{i+1}: {kumePikselSayisi,6}px     (R={merkezler[i]}, G={merkezler[i]}, B={merkezler[i]})");
            }

            // T değerlerinin sayısını Label8'e yazdır
            label8.Text = k.ToString();
        }



        //-----------------------------------------------------------------------------------------------------------





        /*
         * K-MEANS ÖKLİD RGB KÜMELEME
         * Bu metod, görüntüyü RGB renk uzayında Öklid mesafesini kullanarak k adet kümeye ayırır.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Her piksel 3 boyutlu renk uzayında (R,G,B) bir nokta olarak ele alınır
         * 2. k adet rastgele merkez seçilir
         * 3. Her piksel için tüm merkezlere olan Öklid mesafesi hesaplanır:
         *    d = √[(Rp-Rk)² + (Gp-Gk)² + (Bp-Bk)²]
         *    Rp,Gp,Bp: Piksel renk değerleri
         *    Rk,Gk,Bk: Merkez renk değerleri
         * 4. Her piksel en yakın olduğu merkeze atanır
         * 5. Her kümenin yeni merkezi, kümedeki piksellerin RGB değerlerinin ortalaması olarak güncellenir
         * 6. Merkezler değişmeyene kadar 3-5 adımları tekrarlanır
         * 
         * PARAMETRELER:
         * - k: Küme sayısı (kullanıcı tarafından seçilir)
         * 
         * ÇIKTILAR:
         * - İşlenmiş görüntü: Her piksel, ait olduğu kümenin merkez rengini alır
         * - Histogram: Orijinal görüntünün histogramı ve küme merkezleri
         * - İterasyon sayısı: Algoritmanın kaç adımda sonlandığı
         * - Küme bilgileri: Her kümenin RGB değerleri ve piksel sayısı
         * 
         * NOT: Bu yöntem, renkleri 3 boyutlu uzayda gruplandırır ve
         * KMIntensity'ye göre renk bilgisini daha iyi korur.
         */
        private void KMOklidRGB()
        {
            // Giriş görüntüsünün varlığını kontrol et
            if (girisFoto.Image == null) return;

            // Küme sayısının seçilip seçilmediğini kontrol et
            if (kmNumComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen küme sayısını seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen küme sayısını al (k değeri)
            int k = (int)kmNumComboBox.SelectedItem;

            // Giriş ve çıkış için Bitmap'ler oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            // ADIM 1: Tüm piksellerin RGB renklerini diziye kaydet
            Color[] pikselRenkleri = new Color[girisResim.Width * girisResim.Height];
            int index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    pikselRenkleri[index++] = girisResim.GetPixel(x, y);
                }
            }

            // ADIM 2: K-Means için başlangıç merkezlerini rastgele seç
            Random rnd = new Random();
            Color[] merkezler = new Color[k];
            for (int i = 0; i < k; i++)
            {
                // Mevcut piksellerden rastgele birini merkez olarak seç
                merkezler[i] = pikselRenkleri[rnd.Next(pikselRenkleri.Length)];
            }

            // K-Means iterasyonları için maksimum sayı
            int maxIterasyon = 100;
            int iterasyonSayisi = 0;

            // Her pikselin hangi kümeye ait olduğunu tutan dizi
            int[] kümeler = new int[pikselRenkleri.Length];

            // K-Means algoritması ana döngüsü
            bool merkezlerDegisti = true;
            while (merkezlerDegisti && iterasyonSayisi < maxIterasyon)
            {
                iterasyonSayisi++;
                merkezlerDegisti = false;

                // ADIM 3 ve 4: Her piksel için Öklid mesafesini hesapla ve en yakın kümeye ata
                for (int i = 0; i < pikselRenkleri.Length; i++)
                {
                    int enYakınKüme = 0;
                    double enKüçükUzaklık = double.MaxValue;

                    // Her merkez için Öklid uzaklığını hesapla
                    for (int j = 0; j < k; j++)
                    {
                        // 3 boyutlu Öklid mesafesi hesaplama
                        // d = √[(Rp-Rk)² + (Gp-Gk)² + (Bp-Bk)²]
                        double uzaklık = Math.Sqrt(
                            Math.Pow(pikselRenkleri[i].R - merkezler[j].R, 2) +
                            Math.Pow(pikselRenkleri[i].G - merkezler[j].G, 2) +
                            Math.Pow(pikselRenkleri[i].B - merkezler[j].B, 2)
                        );

                        // En küçük mesafeyi ve küme indeksini güncelle
                        if (uzaklık < enKüçükUzaklık)
                        {
                            enKüçükUzaklık = uzaklık;
                            enYakınKüme = j;
                        }
                    }

                    // Eğer piksel farklı bir kümeye atandıysa değişim var demektir
                    if (kümeler[i] != enYakınKüme)
                    {
                        merkezlerDegisti = true;
                        kümeler[i] = enYakınKüme;
                    }
                }

                // ADIM 5: Küme merkezlerini güncelle (ağırlık merkezine kaydır)
                if (merkezlerDegisti)
                {
                    // Her renk kanalı için ayrı toplam tut
                    long[] toplamR = new long[k];
                    long[] toplamG = new long[k];
                    long[] toplamB = new long[k];
                    int[] sayılar = new int[k];

                    // Her kümenin toplam R,G,B değerlerini ve eleman sayısını hesapla
                    for (int i = 0; i < pikselRenkleri.Length; i++)
                    {
                        toplamR[kümeler[i]] += pikselRenkleri[i].R;
                        toplamG[kümeler[i]] += pikselRenkleri[i].G;
                        toplamB[kümeler[i]] += pikselRenkleri[i].B;
                        sayılar[kümeler[i]]++;
                    }

                    // Yeni merkez = kümedeki R,G,B değerlerinin ortalaması
                    for (int i = 0; i < k; i++)
                    {
                        if (sayılar[i] > 0)
                        {
                            int yeniR = (int)(toplamR[i] / sayılar[i]);
                            int yeniG = (int)(toplamG[i] / sayılar[i]);
                            int yeniB = (int)(toplamB[i] / sayılar[i]);

                            // Yeni merkez rengini oluştur
                            Color yeniMerkez = Color.FromArgb(yeniR, yeniG, yeniB);
                            
                            // Eğer merkez değiştiyse, değişim bayrağını güncelle
                            if (yeniMerkez != merkezler[i])
                            {
                                merkezler[i] = yeniMerkez;
                                merkezlerDegisti = true;
                            }
                        }
                    }
                }
            }

            // İterasyon sayısını label7'ye yazdır
            label7.Text = iterasyonSayisi.ToString();

            // ADIM 7: Sonuç görüntüsünü oluştur (etiketlere göre renklendirme)
            index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Her piksel için ait olduğu kümenin merkez rengini kullan
                    int kümeIndex = kümeler[index];
                    cikisResim.SetPixel(x, y, merkezler[kümeIndex]);
                    index++;
                }
            }

            // Sonuç görüntüsünü göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;

            // Histogram grafiğini çiz (merkez noktalarıyla)
            CizHistogram(pikselRenkleri, $"KM Öklid RGB (k={k}) Histogram", merkezler, k, kümeler);
        }



        //-----------------------------------------------------------------------------------------------------------




        // Kovaryans matrisini hesaplayan yardımcı fonksiyon (KMMahalanobis'de kullanıldı)
        private double[,] HesaplaKovaryansMatrisi(Color[] pikselRenkleri)
        {
            // Önce ortalama R,G,B değerlerini hesapla
            double ortR = 0, ortG = 0, ortB = 0;
            foreach (Color c in pikselRenkleri)
            {
                ortR += c.R;
                ortG += c.G;
                ortB += c.B;
            }
            // Ortalamaları piksel sayısına böl
            ortR /= pikselRenkleri.Length;
            ortG /= pikselRenkleri.Length;
            ortB /= pikselRenkleri.Length;

            // 3x3 kovaryans matrisini hesapla
            double[,] kovaryans = new double[3, 3];
            foreach (Color c in pikselRenkleri)
            {
                // Her piksel için ortalamadan sapmaları hesapla
                double diffR = c.R - ortR;
                double diffG = c.G - ortG;
                double diffB = c.B - ortB;

                // Kovaryans matrisinin elemanlarını güncelle
                kovaryans[0, 0] += diffR * diffR; // Var(R)
                kovaryans[0, 1] += diffR * diffG; // Var(R,G)
                kovaryans[0, 2] += diffR * diffB; // Var(R,B)
                kovaryans[1, 1] += diffG * diffG; // Var(G)
                kovaryans[1, 2] += diffG * diffB; // Var(G,B)
                kovaryans[2, 2] += diffB * diffB; // Var(B)
            }

            // Simetrik matrisin alt üçgenini üst üçgene kopyala
            kovaryans[1, 0] = kovaryans[0, 1];
            kovaryans[2, 0] = kovaryans[0, 2];
            kovaryans[2, 1] = kovaryans[1, 2];

            // Kovaryans değerlerini normalize et
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    kovaryans[i, j] /= (pikselRenkleri.Length - 1);

            return kovaryans;
        }
        // Mahalanobis mesafesini hesaplayan yardımcı fonksiyon (KMMahalanobis'de kullanıldı)
        private double HesaplaMahalanobisMesafesi(Color p1, Color p2, double[,] kovaryansInv)
        {
            // İki renk arasındaki farkı hesapla
            double[] diff = new double[3] {
                p1.R - p2.R,  // R kanalındaki fark
                p1.G - p2.G,  // G kanalındaki fark
                p1.B - p2.B   // B kanalındaki fark
            };

            // Mahalanobis mesafesi hesaplama
            // d = √[(x-μ)ᵀ Σ⁻¹ (x-μ)]
            double mesafe = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    mesafe += diff[i] * kovaryansInv[i, j] * diff[j];

            return Math.Sqrt(mesafe);
        }




        //3x3 matris tersini alan metod (KMMahalanobis ve KMMahalanobisND de kullanıldı.)
        private double[,] MatrisInverse3x3(double[,] matrix)
        {
            // 1. ADIM: Determinantı hesapla
            // Sarrus kuralı ile 3x3 matrisin determinantı:
            // |A| = a11(a22a33 - a23a32) - a12(a21a33 - a23a31) + a13(a21a32 - a22a31)
            double det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                        - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                        + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

            // Determinantın tersini al (sonraki işlemlerde çarpan olarak kullanılacak)
            double invDet = 1.0 / det;

            // 2. ADIM: Kofaktör matrisinin transpozunu hesapla ve determinantın tersi ile çarp
            double[,] inverse = new double[3, 3];

            // Her bir elemanın tersini hesapla
            // inverse[i,j] = ((-1)^(i+j) * minör) / determinant
            // Minör: İlgili eleman silindiğinde kalan 2x2 matrisin determinantı
            inverse[0, 0] = (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) * invDet;
            inverse[0, 1] = (matrix[0, 2] * matrix[2, 1] - matrix[0, 1] * matrix[2, 2]) * invDet;
            inverse[0, 2] = (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) * invDet;
            inverse[1, 0] = (matrix[1, 2] * matrix[2, 0] - matrix[1, 0] * matrix[2, 2]) * invDet;
            inverse[1, 1] = (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) * invDet;
            inverse[1, 2] = (matrix[0, 2] * matrix[1, 0] - matrix[0, 0] * matrix[1, 2]) * invDet;
            inverse[2, 0] = (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]) * invDet;
            inverse[2, 1] = (matrix[0, 1] * matrix[2, 0] - matrix[0, 0] * matrix[2, 1]) * invDet;
            inverse[2, 2] = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) * invDet;

            return inverse;
        }


        /*
         * K-MEANS MAHALANOBIS KÜMELEME
         * Bu metod, görüntüyü Mahalanobis mesafesini kullanarak k adet kümeye ayırır.
         * Mahalanobis mesafesi, veri noktaları arasındaki ilişkileri dikkate alan
         * istatistiksel bir uzaklık ölçüsüdür.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Görüntü önce gri tonlamaya çevrilir (Y = 0.299R + 0.587G + 0.114B)
         * 2. k adet rastgele merkez seçilir
         * 3. Her küme için varyans hesaplanır
         * 4. Her piksel için Mahalanobis mesafesi hesaplanır:
         *    d = |x - μ| / √σ²
         *    x: Piksel değeri
         *    μ: Küme merkezi
         *    σ²: Küme varyansı
         * 5. Her piksel en yakın kümeye atanır
         * 6. Küme merkezleri ve varyansları güncellenir
         * 7. Merkezler değişmeyene kadar 4-6 adımları tekrarlanır
         * 
         * PARAMETRELER:
         * - k: Küme sayısı (kullanıcı tarafından seçilir)
         * 
         * ÇIKTILAR:
         * - İşlenmiş görüntü: Her piksel, ait olduğu kümenin rengini alır
         * - Histogram: Orijinal görüntünün histogramı ve küme merkezleri
         * - İterasyon sayısı: Algoritmanın kaç adımda sonlandığı
         * - Küme bilgileri: Her kümenin merkez değerleri ve piksel sayısı
         * 
         * NOT: Bu yöntem, veri dağılımını dikkate aldığı için
         * bazı durumlarda Öklid mesafesine göre daha iyi sonuç verir.
         */
        private void KMMahalanobis()
        {
            // Giriş görüntüsünün varlığını kontrol et
            if (girisFoto.Image == null) return;

            // Küme sayısının seçilip seçilmediğini kontrol et
            if (kmNumComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen küme sayısını seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen küme sayısını al (k değeri)
            int k = (int)kmNumComboBox.SelectedItem;

            // Giriş ve çıkış için Bitmap'ler oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            // Tüm piksellerin gri değerlerini tutacak dizi
            int[] pikselDegerleri = new int[girisResim.Width * girisResim.Height];
            Color[] pikselRenkleri = new Color[girisResim.Width * girisResim.Height];
            int index = 0;

            // Görüntüdeki tüm piksellerin gri değerlerini diziye kaydet
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    Color piksel = girisResim.GetPixel(x, y);
                    pikselRenkleri[index] = piksel;
                    // Y (Luminance) formülü ile gri değeri hesapla
                    pikselDegerleri[index] = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);
                    index++;
                }
            }

            // K-Means için başlangıç merkezlerini rastgele seç
            Random rnd = new Random();
            int[] merkezler = new int[k];
            for (int i = 0; i < k; i++)
            {
                int rastgeleIndex = rnd.Next(pikselDegerleri.Length);
                merkezler[i] = pikselDegerleri[rastgeleIndex];
            }

            // K-Means iterasyonları için maksimum sayı
            int maxIterasyon = 100;
            int iterasyonSayisi = 0;

            // Her pikselin hangi kümeye ait olduğunu tutan dizi
            int[] kümeler = new int[pikselDegerleri.Length];
            // Her kümenin varyansını tutan dizi
            double[] varyanslar = new double[k];

            // K-Means algoritması ana döngüsü
            bool değişimVar = true;
            while (değişimVar && iterasyonSayisi < maxIterasyon)
            {
                iterasyonSayisi++;
                değişimVar = false;

                // Her kümenin varyansını hesapla
                for (int i = 0; i < k; i++)
                {
                    double toplamKareFark = 0;
                    int kümePikselSayısı = 0;

                    for (int j = 0; j < pikselDegerleri.Length; j++)
                    {
                        if (kümeler[j] == i)
                        {
                            toplamKareFark += Math.Pow(pikselDegerleri[j] - merkezler[i], 2);
                            kümePikselSayısı++;
                        }
                    }

                    // İlk iterasyonda veya küme boşsa varsayılan varyans kullan
                    varyanslar[i] = kümePikselSayısı > 1 ? 
                        toplamKareFark / (kümePikselSayısı - 1) : 1000;
                }

                // Her piksel için en yakın kümeyi bul (Mahalanobis uzaklığı ile)
                for (int i = 0; i < pikselDegerleri.Length; i++)
                {
                    int enYakınKüme = 0;
                    double enKüçükUzaklık = double.MaxValue;

                    // Her merkez için Mahalanobis uzaklığını hesapla
                    for (int j = 0; j < k; j++)
                    {
                        // Mahalanobis uzaklığı = |x - merkez| / √varyans
                        double uzaklık = Math.Abs(pikselDegerleri[i] - merkezler[j]) / 
                            Math.Sqrt(varyanslar[j]);

                        if (uzaklık < enKüçükUzaklık)
                        {
                            enKüçükUzaklık = uzaklık;
                            enYakınKüme = j;
                        }
                    }

                    // Eğer piksel farklı bir kümeye atandıysa değişim var demektir
                    if (kümeler[i] != enYakınKüme)
                    {
                        değişimVar = true;
                        kümeler[i] = enYakınKüme;
                    }
                }

                // Küme merkezlerini güncelle
                if (değişimVar)
                {
                    long[] toplamlar = new long[k];
                    int[] sayılar = new int[k];

                    // Her kümenin toplam değerlerini ve eleman sayısını hesapla
                    for (int i = 0; i < pikselDegerleri.Length; i++)
                    {
                        toplamlar[kümeler[i]] += pikselDegerleri[i];
                        sayılar[kümeler[i]]++;
                    }

                    // Yeni merkez = kümedeki değerlerin ortalaması
                    for (int i = 0; i < k; i++)
                    {
                        if (sayılar[i] > 0)
                        {
                            merkezler[i] = (int)(toplamlar[i] / sayılar[i]);
                        }
                    }
                }
            }

            // İterasyon sayısını label7'ye yazdır
            label7.Text = iterasyonSayisi.ToString();

            // Sonuç görüntüsünü oluştur
            index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Her piksel için ait olduğu kümenin merkez değerini kullan
                    int kümeIndex = kümeler[index];
                    int griDeger = merkezler[kümeIndex];
                    // Gri değeri R=G=B olacak şekilde ata
                    cikisResim.SetPixel(x, y, Color.FromArgb(griDeger, griDeger, griDeger));
                    index++;
                }
            }

            // Sonuç görüntüsünü göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;

            // Histogram grafiğini çiz
            // Orijinal görüntünün gri değerlerini kullan
            Color[] girisPikselRenkleri = pikselRenkleri;  // Orijinal pikselleri kullan
            Color[] merkez_renkler = new Color[k];
            
            // Merkez renklerini gri tonlamalı olarak hazırla
            for (int i = 0; i < k; i++)
            {
                merkez_renkler[i] = Color.FromArgb(merkezler[i], merkezler[i], merkezler[i]);
            }

            CizHistogram(girisPikselRenkleri, $"KM Mahalanobis (k={k}) Histogram", merkez_renkler, k, kümeler);
        }



        //-----------------------------------------------------------------------------------------------------------



        // Renk normalizasyonu için yardımcı fonksiyon (KMMahalanobisND'de kullanıldı)
        private Color NormalizeColor(Color renk, double ortR, double ortG, double ortB, double stdR, double stdG, double stdB)
        {
            // Z-score normalizasyonu yapıp 0-255 aralığına ölçekle
            int normR = (int)((renk.R - ortR) / stdR * 50 + 128);
            int normG = (int)((renk.G - ortG) / stdG * 50 + 128);
            int normB = (int)((renk.B - ortB) / stdB * 50 + 128);

            // Değerleri 0-255 aralığına sınırla
            normR = Math.Max(0, Math.Min(255, normR));
            normG = Math.Max(0, Math.Min(255, normG));
            normB = Math.Max(0, Math.Min(255, normB));

            return Color.FromArgb(normR, normG, normB);
        }



        // Giriş görüntüsünün varlığını kontrol et
        private void KMMahalanobisND()
        {
            // Giriş görüntüsünün varlığını kontrol et
            if (girisFoto.Image == null) return;

            // Küme sayısının seçilip seçilmediğini kontrol et
            if (kmNumComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen küme sayısını seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen küme sayısını al (k değeri)
            int k = (int)kmNumComboBox.SelectedItem;

            // Giriş ve çıkış için Bitmap'ler oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            // ADIM 1: Resimdeki her pikselin RGB değerlerini diziye al
            Color[] pikselRenkleri = new Color[girisResim.Width * girisResim.Height];
            int index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    pikselRenkleri[index++] = girisResim.GetPixel(x, y);
                }
            }

            // ADIM 2: Cluster sayısı kadar RGB merkezi rastgele seç
            Random rnd = new Random();
            Color[] merkezler = new Color[k];
            for (int i = 0; i < k; i++)
            {
                merkezler[i] = pikselRenkleri[rnd.Next(pikselRenkleri.Length)];
            }

            // Her pikselin hangi kümeye ait olduğunu tutan dizi
            int[] kümeler = new int[pikselRenkleri.Length];
            int iterasyonSayisi = 0;
            int maxIterasyon = 100;
            bool değişimVar = true;

            while (değişimVar && iterasyonSayisi < maxIterasyon)
            {
                iterasyonSayisi++;
                değişimVar = false;

                // ADIM 4: Kovaryans matrisini hesapla
                double[,] kovaryans = new double[3, 3];
                double ortR = 0, ortG = 0, ortB = 0;

                // Ortalama R,G,B değerlerini hesapla
                foreach (Color c in pikselRenkleri)
                {
                    ortR += c.R;
                    ortG += c.G;
                    ortB += c.B;
                }
                ortR /= pikselRenkleri.Length;
                ortG /= pikselRenkleri.Length;
                ortB /= pikselRenkleri.Length;

                // Kovaryans matrisini hesapla
                foreach (Color c in pikselRenkleri)
                {
                    double diffR = c.R - ortR;
                    double diffG = c.G - ortG;
                    double diffB = c.B - ortB;

                    kovaryans[0, 0] += diffR * diffR;  // Var(R)
                    kovaryans[0, 1] += diffR * diffG;  // Cov(R,G)
                    kovaryans[0, 2] += diffR * diffB;  // Cov(R,B)
                    kovaryans[1, 1] += diffG * diffG;  // Var(G)
                    kovaryans[1, 2] += diffG * diffB;  // Cov(G,B)
                    kovaryans[2, 2] += diffB * diffB;  // Var(B)
                }

                // Kovaryans matrisini normalize et
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        kovaryans[i, j] /= (pikselRenkleri.Length - 1);

                // Simetrik matrisin alt üçgenini üst üçgene kopyala
                kovaryans[1, 0] = kovaryans[0, 1];
                kovaryans[2, 0] = kovaryans[0, 2];
                kovaryans[2, 1] = kovaryans[1, 2];

                // ADIM 5: Her küme için kovaryans değerlerini hesapla
                double[] varyansR = new double[k];
                double[] varyansG = new double[k];
                double[] varyansB = new double[k];

                for (int i = 0; i < k; i++)
                {
                    double toplamR = 0, toplamG = 0, toplamB = 0;
                    int sayı = 0;

                    for (int j = 0; j < pikselRenkleri.Length; j++)
                    {
                        if (kümeler[j] == i)
                        {
                            toplamR += Math.Pow(pikselRenkleri[j].R - merkezler[i].R, 2);
                            toplamG += Math.Pow(pikselRenkleri[j].G - merkezler[i].G, 2);
                            toplamB += Math.Pow(pikselRenkleri[j].B - merkezler[i].B, 2);
                            sayı++;
                        }
                    }

                    varyansR[i] = sayı > 1 ? toplamR / (sayı - 1) : 1000;
                    varyansG[i] = sayı > 1 ? toplamG / (sayı - 1) : 1000;
                    varyansB[i] = sayı > 1 ? toplamB / (sayı - 1) : 1000;
                }

                // ADIM 6: Kovaryans matrisinin tersini al
                double[,] kovaryansInv = MatrisInverse3x3(kovaryans);

                // ADIM 7: Her noktanın kovaryans matrisine olan uzaklıklarını hesapla
                for (int i = 0; i < pikselRenkleri.Length; i++)
                {
                    int enYakınKüme = 0;
                    double enKüçükUzaklık = double.MaxValue;

                    for (int j = 0; j < k; j++)
                    {
                        // Mahalanobis uzaklığını hesapla
                        double[] diff = new double[] {
                            pikselRenkleri[i].R - merkezler[j].R,
                            pikselRenkleri[i].G - merkezler[j].G,
                            pikselRenkleri[i].B - merkezler[j].B
                        };

                        double uzaklık = 0;
                        for (int r = 0; r < 3; r++)
                            for (int c = 0; c < 3; c++)
                                uzaklık += diff[r] * kovaryansInv[r, c] * diff[c];

                        uzaklık = Math.Sqrt(uzaklık);

                        if (uzaklık < enKüçükUzaklık)
                        {
                            enKüçükUzaklık = uzaklık;
                            enYakınKüme = j;
                        }
                    }

                    // ADIM 8: Etiketleme
                    if (kümeler[i] != enYakınKüme)
                    {
                        değişimVar = true;
                        kümeler[i] = enYakınKüme;
                    }
                }

                // Küme merkezlerini güncelle
                if (değişimVar)
                {
                    long[] toplamR = new long[k];
                    long[] toplamG = new long[k];
                    long[] toplamB = new long[k];
                    int[] sayılar = new int[k];

                    for (int i = 0; i < pikselRenkleri.Length; i++)
                    {
                        toplamR[kümeler[i]] += pikselRenkleri[i].R;
                        toplamG[kümeler[i]] += pikselRenkleri[i].G;
                        toplamB[kümeler[i]] += pikselRenkleri[i].B;
                        sayılar[kümeler[i]]++;
                    }

                    for (int i = 0; i < k; i++)
                    {
                        if (sayılar[i] > 0)
                        {
                            int yeniR = (int)(toplamR[i] / sayılar[i]);
                            int yeniG = (int)(toplamG[i] / sayılar[i]);
                            int yeniB = (int)(toplamB[i] / sayılar[i]);
                            merkezler[i] = Color.FromArgb(yeniR, yeniG, yeniB);
                        }
                    }
                }
            }

            // İterasyon sayısını label7'ye yazdır
            label7.Text = iterasyonSayisi.ToString();

            // Sonuç görüntüsünü oluştur
            index = 0;
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    int kümeIndex = kümeler[index];
                    cikisResim.SetPixel(x, y, merkezler[kümeIndex]);
                    index++;
                }
            }

            // Sonuç görüntüsünü göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;

            // Histogram grafiğini çiz
            CizHistogram(pikselRenkleri, $"KM Mahalanobis ND (k={k}) Histogram", merkezler, k, kümeler);
        }




        //-----------------------------------------------------------------------------------------------------------



        /*
         * HİSTOGRAM EŞİTLEME
         * Bu metod, görüntünün kontrastını iyileştirmek için histogram eşitleme
         * tekniğini kullanır. Görüntüdeki gri seviyelerin dağılımını dengeler.
         * 
         * ÇALIŞMA PRENSİBİ:
         * 1. Görüntü gri tonlamaya çevrilir (Y = 0.299R + 0.587G + 0.114B)
         * 2. Histogram hesaplanır (her gri seviyenin görülme sıklığı)
         * 3. Kümülatif Dağılım Fonksiyonu (CDF) hesaplanır:
         *    CDF(i) = Σ(histogram(j)) , j=0'dan i'ye kadar
         * 4. Normalizasyon yapılır:
         *    h(v) = round((CDF(v) - CDFmin) / (M*N - CDFmin) * 255)
         *    M*N: Toplam piksel sayısı
         *    CDFmin: CDF'nin sıfır olmayan minimum değeri
         * 5. Her piksel için yeni değer atanır
         * 
         * ÇIKTILAR:
         * - İşlenmiş görüntü: Kontrastı iyileştirilmiş gri tonlamalı görüntü
         * - Histogram: Orijinal görüntünün histogramı
         * 
         * NOT: Bu yöntem özellikle düşük kontrastlı görüntülerde detayları
         * ortaya çıkarmak için kullanılır. Ancak bazı durumlarda aşırı
         * kontrast artışına neden olabilir.
         */
        private void HistogramEsitlemeGri()
        {
            // Giriş görüntüsünün varlığını kontrol et
            if (girisFoto.Image == null) return;
            // Giriş, gri ve çıkış görüntüsünün iin  bir Bitmap oluştur
            Bitmap girisResim = new Bitmap(girisFoto.Image);
            Bitmap griResim = new Bitmap(girisResim.Width, girisResim.Height);
            Bitmap cikisResim = new Bitmap(girisResim.Width, girisResim.Height);

            // 1. ADIM: Renkli görüntüyü gri tonlamaya çevir
            for (int x = 0; x < girisResim.Width; x++)
            {
                for (int y = 0; y < girisResim.Height; y++)
                {
                    // Y (Luminance) formülü ile gri değeri hesapla
                    Color piksel = girisResim.GetPixel(x, y);
                    int griDeger = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);
                    // Gri değeri ile yeni piksel oluştur (R=G=B=griDeger)
                    griResim.SetPixel(x, y, Color.FromArgb(griDeger, griDeger, griDeger));
                }
            }

            // 2. ADIM: Histogram dizisini oluştur (0-255 arası değerler için)
            int[] histogram = new int[256];
            // Her gri seviyenin görülme sıklığını hesapla
            for (int x = 0; x < griResim.Width; x++)
            {
                for (int y = 0; y < griResim.Height; y++)
                {
                    // Gri resimde R=G=B olduğu için herhangi birini alabiliriz
                    int griDeger = griResim.GetPixel(x, y).R;  
                    histogram[griDeger]++;   // İlgili gri seviyenin sayacını artır
                }
            }

            // 3. ADIM: Kümülatif Dağılım Fonksiyonu (CDF) hesapla
            int[] cdf = new int[256];
            cdf[0] = histogram[0];  // İlk değer doğrudan histogramdan alınır
            // Diğer değerler önceki değerlerin toplamı olarak hesaplanır
            for (int i = 1; i < 256; i++)
            {
                cdf[i] = cdf[i - 1] + histogram[i];
            }


            // 4. ADIM: CDF'nin sıfır olmayan minimum değerini bul
            // Bu değer histogram eşitleme formülünde kullanılacak
            int cdfMin = cdf.FirstOrDefault(v => v > 0);


            // 5. ADIM: Histogram eşitleme formülünü uygula
            // Toplam piksel sayısını hesapla
            int toplamPiksel = griResim.Width * griResim.Height;

            // Her gri seviye için yeni değeri hesapla
            int[] yeniDegerler = new int[256];
            for (int i = 0; i < 256; i++)
            {
                // Histogram eşitleme formülü:
                // h(v) = round( (cdf(v) - cdfMin) / (M*N - cdfMin) * (L-1) )
                // M*N = toplam piksel sayısı
                // L = gri seviye sayısı (256)
                yeniDegerler[i] = (int)(((double)(cdf[i] - cdfMin) / (toplamPiksel - cdfMin)) * 255);
            }

            // 6. ADIM: Eşitlenmiş histogramı görüntüye uygula
            for (int x = 0; x < griResim.Width; x++)
            {
                for (int y = 0; y < griResim.Height; y++)
                {
                    // Mevcut pikselin gri değerini al
                    int griDeger = griResim.GetPixel(x, y).R;
                    // Bu gri değer için hesaplanmış yeni değeri kullan
                    int yeniDeger = yeniDegerler[griDeger];
                    // Yeni değer ile piksel oluştur
                    cikisResim.SetPixel(x, y, Color.FromArgb(yeniDeger, yeniDeger, yeniDeger));
                }
            }
            // 7. ADIM: Sonuç görüntüsünü göster
            cikisFoto.Image = cikisResim;
            cikisFoto.SizeMode = PictureBoxSizeMode.StretchImage;

            // 8. ADIM: Histogram grafiğini oluştur ve göster
            // Önceki serileri temizle (varsa)
            chart1.Series.Clear();
            chart1.Titles.Clear();

            // Grafiğin başlığını ayarla
            chart1.Titles.Add("Histogram Eşitleme");

            // Yeni bir seri oluştur ve "Histogram" adını ver
            var series = chart1.Series.Add("Histogram");

            // Grafik tipini sütun (Column) olarak ayarla
            // Bu, her gri seviye için dikey bir çubuk oluşturur
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            // Histogramdaki en yüksek değeri bul
            int maxHistogramDegeri = 0;
            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > maxHistogramDegeri)
                    maxHistogramDegeri = histogram[i];
            }

            // Her gri seviye (0-255) için histogram değerlerini grafiğe ekle
            for (int i = 0; i < 256; i++)
            {
                series.Points.AddXY(i, histogram[i]);
            }

            // Y ekseni ayarlarını güncelle
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = maxHistogramDegeri * 1.1; // En yüksek değerin %10 üstüne kadar göster
            chart1.ChartAreas[0].AxisY.Interval = maxHistogramDegeri / 10; // 10 aralık olacak şekilde ayarla
            chart1.ChartAreas[0].AxisY.MajorGrid.Interval = maxHistogramDegeri / 10;
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0";
            chart1.ChartAreas[0].AxisY.Title = "Piksel Sayısı";
            chart1.ChartAreas[0].AxisX.Title = "Gri Seviye";
        }

        //Histogram Grafiğinin Eksen Ayarları 
        private void AyarlaChart1()
        {
            // X Ekseni (Gri Seviye) Ayarları
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 255;
            chart1.ChartAreas[0].AxisX.Interval = 50;

            // Y Ekseni (Piksel Sayısı) Ayarları - Başlangıçta minimum 0 olarak ayarla
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            
            // Y ekseni maksimum ve aralık değerleri, veri eklendiğinde otomatik ayarlanacak
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart1.ChartAreas[0].AxisY.Maximum = double.NaN; // Otomatik ölçeklendirme için
            chart1.ChartAreas[0].AxisY.Interval = double.NaN; // Otomatik aralık için
        }



        //-----------------------------------------------------------------------------------------------------------



        //Uygula Buton Tıklama
        private void uygula_Click(object sender, EventArgs e)
        {
            // Grafiği sıfırla
            chart1.Series.Clear();
            chart1.Titles.Clear();
            AyarlaChart1();

            // 1. KONTROL: Resim yüklenmiş mi?
            if (girisFoto.Image == null)
            {
                MessageBox.Show("Lütfen önce bir resim yükleyin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2. KONTROL: İşlem seçilmiş mi?
            if (islemComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir işlem seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Her işlem başlangıcında ListBox'ları temizle
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            // K-Means olmayan işlemler için label7'yi temizle
            label7.Text = "";

            // Resmin toplam piksel sayısını hesapla ve label6'ya yazdır
            if (girisFoto.Image != null)
            {
                Bitmap girisResim = new Bitmap(girisFoto.Image);
                int toplamPiksel = girisResim.Width * girisResim.Height;
                label6.Text = toplamPiksel.ToString();
            }
            else
            {
                label6.Text = "";
            }

            // 3. SEÇİLEN İŞLEME GÖRE YÖNLENDIRME
            switch (islemComboBox.SelectedItem.ToString())
            {
                case "Gri Yap":
                    GriYap();
                    break;
                case "Y Yap":
                    YYap();
                    break;
                case "Kenar Bulma":
                    KenarBulma();
                    break;
                case "KM Intensity":
                    KMIntensity();
                    break;
                case "KM Öklid RGB":
                    KMOklidRGB();
                    break;
                case "KM Mahlanobis":
                    KMMahalanobis();
                    break;
                case "KM Mahlanobis ND":
                    KMMahalanobisND();
                    break;
                case "Histogram":
                    HistogramEsitlemeGri();
                    break;
                // Tanımlanmamış işlem
                default:
                    MessageBox.Show("Seçilen işlem henüz uygulanmamış!", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        // KM işlemleri için histogram çizme yardımcı fonksiyonu
        private void CizHistogram(Color[] pikselRenkleri, string baslik, Color[] merkezler = null, int k = 0, int[] kümeler = null)
        {
            // Gri tonlama histogramı için dizi
            int[] histogram = new int[256];

            // Her piksel için gri değeri hesapla ve histograma ekle
            foreach (Color piksel in pikselRenkleri)
            {
                int griDeger = (int)(0.299 * piksel.R + 0.587 * piksel.G + 0.114 * piksel.B);
                histogram[griDeger]++;
            }

            // Önceki serileri ve başlıkları temizle
            chart1.Series.Clear();
            chart1.Titles.Clear();
            listBox1.Items.Clear(); // ListBox1'i temizle
            listBox2.Items.Clear(); // ListBox2'yi temizle

            // Grafiğin başlığını ayarla
            chart1.Titles.Add(baslik);

            // Histogram serisi
            var histogramSeries = chart1.Series.Add("Histogram");
            histogramSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            // Histogram değerlerini grafiğe ekle
            for (int i = 0; i < 256; i++)
            {
                histogramSeries.Points.AddXY(i, histogram[i]);
            }

            // Eğer merkez noktaları verilmişse, onları da ekle
            if (merkezler != null && k > 0)
            {
                var merkezSeries = chart1.Series.Add("Merkezler");
                merkezSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                merkezSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
                merkezSeries.MarkerSize = 10;
                merkezSeries.Color = Color.Red;

                // Her küme için benzersiz pikselleri saklamak için dictionary
                Dictionary<int, HashSet<Color>> kumeOrijinalRenkler = new Dictionary<int, HashSet<Color>>();
                for (int i = 0; i < k; i++)
                {
                    kumeOrijinalRenkler[i] = new HashSet<Color>();
                }

                // Giriş resmindeki benzersiz renkleri topla
                for (int j = 0; j < pikselRenkleri.Length; j++)
                {
                    int kumeIndex = kümeler[j];
                    kumeOrijinalRenkler[kumeIndex].Add(pikselRenkleri[j]);
                }

                // ListBox1'e değerleri ekle (algoritmalara göre farklı gösterim)
                for (int i = 0; i < k; i++)
                {
                    var benzersizRenkler = kumeOrijinalRenkler[i];
                    if (benzersizRenkler.Count > 0)
                    {
                        // Kümedeki renklerin ortalamasını hesapla
                        int toplamR = 0, toplamG = 0, toplamB = 0;
                        foreach (var renk in benzersizRenkler)
                        {
                            toplamR += renk.R;
                            toplamG += renk.G;
                            toplamB += renk.B;
                        }
                        int ortR = toplamR / benzersizRenkler.Count;
                        int ortG = toplamG / benzersizRenkler.Count;
                        int ortB = toplamB / benzersizRenkler.Count;

                        // Başlığa göre farklı gösterim yap
                        if (baslik.Contains("Intensity") || baslik.Contains("Mahalanobis (k="))
                        {
                            // Intensity ve KM Mahalanobis için sadece gri değer
                            double griDeger = 0.299 * ortR + 0.587 * ortG + 0.114 * ortB;
                            listBox1.Items.Add($"T{i + 1}: {(int)griDeger}");
                        }
                        else
                        {
                            // KM Öklid RGB ve KM Mahalanobis ND için RGB değerleri
                            listBox1.Items.Add($"T{i + 1}: (R={ortR}, G={ortG}, B={ortB})");
                        }
                    }
                }

                // ListBox2'ye çıkış resminin RGB değerlerini ekle (merkez değerleri)
                for (int i = 0; i < k; i++)
                {
                    int kumePikselSayisi = 0;
                    for (int j = 0; j < pikselRenkleri.Length; j++)
                    {
                        if (kümeler[j] == i)
                        {
                            kumePikselSayisi++;
                        }
                    }
                    listBox2.Items.Add($"T{i+1}: {kumePikselSayisi,6}px     (R={merkezler[i].R}, G={merkezler[i].G}, B={merkezler[i].B})");
                }

                // Merkez noktalarını grafiğe ekle
                for (int i = 0; i < k; i++)
                {
                    int griDeger = (int)(0.299 * merkezler[i].R + 0.587 * merkezler[i].G + 0.114 * merkezler[i].B);
                    merkezSeries.Points.AddXY(griDeger, histogram[griDeger]);
                }

                // T değerlerinin sayısını Label8'e yazdır
                label8.Text = k.ToString();
            }
            else
            {
                // Merkez noktası yoksa Label8'i temizle
                label8.Text = "0";
            }
        }
    }
}
