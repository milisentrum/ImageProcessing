﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sw = Stopwatch.StartNew();

                menuStrip1.Enabled = trackBar1.Enabled = false;

                // Выбранные цвета
                foreach(var item in checkedLColorList.CheckedItems)
                {
                    if (item.ToString() == "White")
                        _checkedColors.Add(Color.White);

                    if (item.ToString() == "Black")
                        _checkedColors.Add(Color.Black);

                    if (item.ToString() == "Red")
                        _checkedColors.Add(Color.Red);

                    if (item.ToString() == "Green")
                        _checkedColors.Add(Color.Green);

                    if (item.ToString() == "Blue")
                        _checkedColors.Add(Color.Blue);

                }
                _checkedColors = checkedLColorList.

                pictureBox1.Image = null;
                _bitmaps.Clear();
                Bitmap bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run( () => { RunProcessing(bitmap); } );

                menuStrip1.Enabled = trackBar1.Enabled = true;

                sw.Stop();
                Text = $"Time processing: {sw.Elapsed.Seconds} sec";  
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

                // Изначально на битмапе часть пикселей белая, часть чёрная
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        bool isBlack = _rnd.Next(2) == 0;

                        if (isBlack)
                        {
                            currentBitmap.SetPixel(x, y, Color.Black);
                        }
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
    }
}
