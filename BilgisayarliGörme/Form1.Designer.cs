namespace BilgisayarliGörme
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.uygula = new System.Windows.Forms.Button();
            this.islemComboBox = new System.Windows.Forms.ComboBox();
            this.kmNumComboBox = new System.Windows.Forms.ComboBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.resimyukle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.girisFoto = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.cikisFoto = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.trackBarThreshold = new System.Windows.Forms.TrackBar();
            this.labelThreshold = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.girisFoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cikisFoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // uygula
            // 
            this.uygula.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.uygula.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uygula.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.uygula.ForeColor = System.Drawing.Color.White;
            this.uygula.Location = new System.Drawing.Point(827, 431);
            this.uygula.Name = "uygula";
            this.uygula.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.uygula.Size = new System.Drawing.Size(118, 50);
            this.uygula.TabIndex = 2;
            this.uygula.Text = "Uygula";
            this.uygula.UseVisualStyleBackColor = false;
            // 
            // islemComboBox
            // 
            this.islemComboBox.BackColor = System.Drawing.Color.White;
            this.islemComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.islemComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.islemComboBox.FormattingEnabled = true;
            this.islemComboBox.Location = new System.Drawing.Point(759, 335);
            this.islemComboBox.Name = "islemComboBox";
            this.islemComboBox.Size = new System.Drawing.Size(186, 31);
            this.islemComboBox.TabIndex = 3;
            // 
            // kmNumComboBox
            // 
            this.kmNumComboBox.BackColor = System.Drawing.Color.White;
            this.kmNumComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.kmNumComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.kmNumComboBox.FormattingEnabled = true;
            this.kmNumComboBox.Location = new System.Drawing.Point(759, 385);
            this.kmNumComboBox.Name = "kmNumComboBox";
            this.kmNumComboBox.Size = new System.Drawing.Size(186, 31);
            this.kmNumComboBox.TabIndex = 4;
            // 
            // chart1
            // 
            this.chart1.BorderlineColor = System.Drawing.Color.LightGray;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.BackColor = System.Drawing.Color.White;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(439, 20);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(637, 299);
            this.chart1.TabIndex = 5;
            this.chart1.Text = "chart1";
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.White;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(12, 389);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(330, 162);
            this.listBox1.TabIndex = 7;
            // 
            // resimyukle
            // 
            this.resimyukle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.resimyukle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resimyukle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resimyukle.ForeColor = System.Drawing.Color.White;
            this.resimyukle.Location = new System.Drawing.Point(626, 431);
            this.resimyukle.Name = "resimyukle";
            this.resimyukle.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.resimyukle.Size = new System.Drawing.Size(120, 50);
            this.resimyukle.TabIndex = 9;
            this.resimyukle.Text = "Resim Yükle";
            this.resimyukle.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(622, 343);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "İşlem";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(622, 393);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "KM Num.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(669, 621);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "İterasyon = ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(669, 579);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "T Değeri =";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(643, 666);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Toplam Pixel = ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(781, 666);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 16);
            this.label6.TabIndex = 17;
            this.label6.Text = "Toplam Pixel Sonuç";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(781, 621);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 16);
            this.label7.TabIndex = 16;
            this.label7.Text = "İterasyon Sonuç";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(781, 579);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 16);
            this.label8.TabIndex = 15;
            this.label8.Text = "T Değeri Sonuç";
            // 
            // girisFoto
            // 
            this.girisFoto.BackColor = System.Drawing.Color.White;
            this.girisFoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.girisFoto.Location = new System.Drawing.Point(12, 20);
            this.girisFoto.Name = "girisFoto";
            this.girisFoto.Size = new System.Drawing.Size(330, 299);
            this.girisFoto.TabIndex = 19;
            this.girisFoto.TabStop = false;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(12, 356);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(244, 27);
            this.label10.TabIndex = 20;
            this.label10.Text = "Piksel Sayısı =    T-R    T-G    T-B";
            // 
            // listBox2
            // 
            this.listBox2.BackColor = System.Drawing.Color.White;
            this.listBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 20;
            this.listBox2.Location = new System.Drawing.Point(1151, 389);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(327, 162);
            this.listBox2.TabIndex = 24;
            // 
            // cikisFoto
            // 
            this.cikisFoto.BackColor = System.Drawing.Color.White;
            this.cikisFoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cikisFoto.Location = new System.Drawing.Point(1151, 20);
            this.cikisFoto.Name = "cikisFoto";
            this.cikisFoto.Size = new System.Drawing.Size(330, 299);
            this.cikisFoto.TabIndex = 25;
            this.cikisFoto.TabStop = false;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Location = new System.Drawing.Point(1148, 356);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(244, 27);
            this.label13.TabIndex = 26;
            this.label13.Text = "Piksel Sayısı =    T-R    T-G    T-B";
            // 
            // trackBarThreshold
            // 
            this.trackBarThreshold.Location = new System.Drawing.Point(625, 520);
            this.trackBarThreshold.Maximum = 255;
            this.trackBarThreshold.Name = "trackBarThreshold";
            this.trackBarThreshold.Size = new System.Drawing.Size(300, 56);
            this.trackBarThreshold.TabIndex = 27;
            this.trackBarThreshold.TickFrequency = 10;
            this.trackBarThreshold.Value = 50;
            this.trackBarThreshold.Visible = false;
            this.trackBarThreshold.ValueChanged += new System.EventHandler(this.trackBarThreshold_ValueChanged);
            // 
            // labelThreshold
            // 
            this.labelThreshold.AutoSize = true;
            this.labelThreshold.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelThreshold.Location = new System.Drawing.Point(625, 500);
            this.labelThreshold.Name = "labelThreshold";
            this.labelThreshold.Size = new System.Drawing.Size(146, 20);
            this.labelThreshold.TabIndex = 28;
            this.labelThreshold.Text = "Threshold Değeri: 50";
            this.labelThreshold.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1528, 713);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cikisFoto);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.girisFoto);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.resimyukle);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.kmNumComboBox);
            this.Controls.Add(this.islemComboBox);
            this.Controls.Add(this.uygula);
            this.Controls.Add(this.trackBarThreshold);
            this.Controls.Add(this.labelThreshold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bilgisayarlı Görme";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.girisFoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cikisFoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button uygula;
        private System.Windows.Forms.ComboBox islemComboBox;
        private System.Windows.Forms.ComboBox kmNumComboBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button resimyukle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox girisFoto;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.PictureBox cikisFoto;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TrackBar trackBarThreshold;
        private System.Windows.Forms.Label labelThreshold;
    }
}

