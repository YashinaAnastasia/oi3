using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;



namespace oi3
{
    static class Filters
    {
        static int Factorial(int n)
        {
            if (n == 1) return 1;

            return n * Factorial(n - 1);
        }
        static byte[] ComputeNoise(float[] uniform, int size)
        {
            Random random = new Random();
            int count = 0;
            var noise = new byte[size];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < (int)uniform[i]; j++)
                    noise[j + count] = (byte)i;

                count += (int)uniform[i];
            }

            for (int i = 0; i < size - count; i++)
                noise[count + i] = 0;

            noise = noise.OrderBy(x => random.Next()).ToArray();
            return noise;
        }
        public static Bitmap UniformNoise(this Bitmap image)
        {
            double a = 32;
            double b = 120;

            var uniform = new float[256];
            float sum = 0f;

            for (int i = 0; i < 256; i++)
            {
                float step = i;
                if (step >= a && step <= b)
                {
                    uniform[i] = (1 / (float)(b - a));
                }
                else
                {
                    uniform[i] = 0;
                }
                sum += uniform[i];
            }

            for (int i = 0; i < 256; i++)
            {
                uniform[i] /= sum;
                uniform[i] *= image.Width* image.Height;
                uniform[i] = (int)Math.Floor(uniform[i]);
            }
            int size = image.Width * image.Height;

            var noise = ComputeNoise(uniform, size);

            var resImage = new Bitmap(image);

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    var newValue = Clamp((byte)(.299 * color.R + .587 * color.G + .114 * color.B) +
                    noise[image.Width * y + x], 0, 255);

                    resImage.SetPixel(x, y, Color.FromArgb(newValue, newValue, newValue));
                }
            return resImage;

        }
        public static Bitmap GammaNoise(this Bitmap image)
        {

            var erlang = new float[256];
            double a = 2;
            double b = 10;
            Random rnd = new Random();
            float sum = 0;
            for (int i = 0; i < 256; i++)
            {
                double step = (double)i * 0.1;
                if (step >= 0)
                {
                    erlang[i] = (float)(Math.Exp(-a * step) * (Math.Pow(a, b) * Math.Pow(step, b - 1)) / Factorial((int)b - 1));
                }
                else
                {
                    erlang[i] = 0;
                }
                sum += erlang[i];
            }

            for (int i = 0; i < 256; i++)
            {
                erlang[i] /= sum;
                erlang[i] *= image.Width * image.Height;
                erlang[i] = (int)Math.Floor(erlang[i]);
            }

            int size = image.Width * image.Height;
            var noise = ComputeNoise(erlang, size);

            var resImage = new Bitmap(image);

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    var newValue = Clamp((byte)(.299 * color.R + .587 * color.G + .114 * color.B) +
                    noise[image.Width * y + x], 0, 255);

                    resImage.SetPixel(x, y, Color.FromArgb(newValue, newValue, newValue));
                }
            return resImage;
        }
        public static Bitmap MedianFilter(this Bitmap image, int matrixSize,
                                            int bias = 0, bool grayscale = false)
        {
            Bitmap resultimage = new Bitmap(image);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Color resultColor = image.GetPixel(x, y);
                    int radiusX = 3;
                    int radiusY = 3;
                    List<int> listR = new List<int>();
                    List<int> listG = new List<int>();
                    List<int> listB = new List<int>();

                    int idx;
                    int idy;
                    for (int l = -radiusX; l <= radiusX; ++l)
                        for (int k = -radiusY; k <= radiusY; ++k)
                        {
                            idx = Clamp(x + l, 0, image.Width - 1);
                            idy = Clamp(y + k, 0, image.Height - 1);
                            listR.Add(image.GetPixel(idx, idy).R);
                            listG.Add(image.GetPixel(idx, idy).G);
                            listB.Add(image.GetPixel(idx, idy).B);
                        }
                    listR.Sort();
                    listG.Sort();
                    listB.Sort();

                    int d1, d2, d3;
                    d1 = Clamp((int)listR[listR.Count() / 2], 0, 255);
                    d2 = Clamp((int)listG[listG.Count() / 2], 0, 255);
                    d3 = Clamp((int)listB[listB.Count() / 2], 0, 255);
                    resultColor = Color.FromArgb(d1, d2, d3);
                    resultimage.SetPixel(x, y, resultColor);
                }
            return resultimage;
        }
       
      
      public static Bitmap bilateralfilter(this Bitmap image)
        {
            Bitmap resultimage = new Bitmap(image);
            int radius = 1;
            int sigma = 2;
            int size = 2 * radius + 1;
            float[,] kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i) / 2 * (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Color Color = NewPixelColorBilaterial(image, kernel, x, y);
                    resultimage.SetPixel(x, y, Color);
                }
            return resultimage;
        }
       static public int Clamp(int value, int min, int max) { return value < min ? min : value > max ? max : value; }
        static public Color NewPixelColorBilaterial(Bitmap source, float[,] kernel, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float res = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, source.Width - 1);
                    int idY = Clamp(y + l, 0, source.Height - 1);
                    Color neighborColor = source.GetPixel(idX, idY);
                    res += neighborColor.R * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(Clamp((int)res, 0, 255),
                                 Clamp((int)res, 0, 255),
                                 Clamp((int)res, 0, 255));
        }
    }
}
