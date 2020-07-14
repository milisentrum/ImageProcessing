using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _rnd = new Random();
        private List<Color> _checkedColors = new List<Color>();

        public Form1()
        {
            InitializeComponent();
            trackBar1.Enabled = saveBtn.Enabled = retryBtn.Enabled = stopBtn.Enabled = false;
            stopBtn.Visible = false;
        }

        /// <summary>
        /// устанавливаем выбранные цвета
        /// </summary>
        private void SetColors()
        {
            if (checkedLColorList.CheckedItems.Count == 0)
            {
                _checkedColors.Add(Color.White);
            }
            else 
            {
                foreach (var item in checkedLColorList.CheckedItems)
                {
                    if (item.ToString() == "Black")
                        _checkedColors.Add(Color.Black);

                    if (item.ToString() == "White")
                        _checkedColors.Add(Color.White);

                    if (item.ToString() == "Red")
                        _checkedColors.Add(Color.Red);

                    if (item.ToString() == "Green")
                        _checkedColors.Add(Color.Green);

                    if (item.ToString() == "Blue")
                        _checkedColors.Add(Color.Blue);

                }
            }
            
        }

        // Работа программы
        private async void Run()
        {
            var sw = Stopwatch.StartNew();

            menuStrip1.Enabled = trackBar1.Enabled = saveBtn.Enabled = checkedLColorList.Enabled = retryBtn.Enabled = false;

            stopBtn.Enabled = true;

            _checkedColors.Clear();

            SetColors();

            pictureBox1.Image = null;
            _bitmaps.Clear();
            Bitmap bitmap = new Bitmap(openFileDialog1.FileName);
            await Task.Run(() => { RunProcessing(bitmap); });

            menuStrip1.Enabled = trackBar1.Enabled = saveBtn.Enabled = checkedLColorList.Enabled = retryBtn.Enabled = true;

            sw.Stop();
            Text = $"Time processing: {sw.Elapsed.Seconds} sec";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Run();
            }
        }

        /// <summary>
        /// Заполням список _bitmaps битмапами с определённой частью пикселей, выкрашенных в белый.
        /// </summary>
        /// <param name="bitmap"></param>
        private void RunProcessing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentSetPixels = new List<Pixel>(pixels.Count - pixelsInStep);

            for (int i = 0; i < trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = _rnd.Next(pixels.Count);
                    currentSetPixels.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }

                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                // Изначально на битмапе часть пикселей разукрашена в выбранные цвета
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color randomColor = _checkedColors[_rnd.Next(_checkedColors.Count)];

                        currentBitmap.SetPixel(x, y, randomColor);  
                    }
                }
               
                foreach(var pixel in currentSetPixels)
                {
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }
                
                _bitmaps.Add(currentBitmap);

                this.Invoke(new Action( () => Text = $"{i}%"));
            }

            _bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point()
                        {
                            X = x,
                            Y = y
                        }
                    };

                    pixels.Add(pixel);
                }
            }

            return pixels;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count == 0)
                return;

            pictureBox1.Image = _bitmaps[trackBar1.Value];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count == 0)
                return;

            _bitmaps[trackBar1.Value].Save(@"C:\Users\Know How\savedImage.jpg");

             Clipboard.SetText(@"C:\Users\Know How\savedImage.jpg");

            if (File.Exists(@"C:\Users\Know How\savedImage.jpg"))
            {
                MessageBox.Show(@"Ваш в буфере обмена", "Изображение сохранено");
            }
            else
            {
                MessageBox.Show("Изобраение сохранено (нет) ");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Run();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {


            stopBtn.Enabled = false;
        }
    }
}
