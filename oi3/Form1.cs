using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace oi3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap image;
        Bitmap noise_result_image;
        Bitmap filter_result_image;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(dialog.FileName);
                    pictureBox1.Image = image;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Refresh();
                }
            }

        private void равномерныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noise_result_image = image.UniformNoise();
            pictureBox1.Image = noise_result_image;
            pictureBox1.Refresh();
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter_result_image = noise_result_image.MedianFilter(3);
            pictureBox1.Image = filter_result_image;
            pictureBox1.Refresh();
        }

        private void гаммаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noise_result_image = image.GammaNoise();
            pictureBox1.Image = noise_result_image;
            pictureBox1.Refresh();
        }

        private void билатериальныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter_result_image = noise_result_image.bilateralfilter();
            pictureBox1.Image = filter_result_image;
            pictureBox1.Refresh();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            PSNR filter = new PSNR();
            Cursor.Current = Cursors.WaitCursor;
            MessageBox.Show(PSNR.Execute((Bitmap)image, (Bitmap)filter_result_image).ToString());
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SSIM filter = new SSIM();
            Cursor.Current = Cursors.WaitCursor;
            MessageBox.Show(SSIM.Execute((Bitmap)image, (Bitmap)filter_result_image).ToString());
            Cursor.Current = Cursors.Default;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
           
        }
        }
    }
    

