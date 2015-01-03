using System;
using System.Drawing;
using System.IO;
using ImageMagick;
using Tesseract;

namespace EliteTrader.EliteOcr.Tesseract
{
    public static class TessTester
    {
        public static void Missing()
        {
            string configPath = Path.Combine(Environment.CurrentDirectory, @"tessdata\edl.config");

            //using (TesseractEngine engine = new TesseractEngine(null, "eng", EngineMode.CubeOnly, configPath))
            using (TesseractEngine engine = new TesseractEngine(null, "edl", EngineMode.TesseractOnly, configPath))
            {
                TessHelper.SetVariable(engine, "tessedit_char_whitelist", "0123456789");
                TessHelper.SetVariable(engine, "textord_debug_bugs", "1");

                engine.DefaultPageSegMode = PageSegMode.SingleLine;

                //engine.DefaultPageSegMode = PageSegMode.SingleLine;
                //TessHelper.SetVariable(engine, "tessedit_char_whitelist", "0123456789,");
                //Bitmap bitmap = new Bitmap(@"c:\tmp\screens\edl.elite_font.exp71 - Copy.tif");
                Bitmap bitmap = new Bitmap(@"c:\tmp\screens\edl.elite_font.exp71 - Copy.tif");

                using (MagickImage image = new MagickImage(bitmap))
                {
                    //image.Format = MagickFormat.Pbm;
                    image.Scale(100);
                    bitmap = image.ToBitmap();
                    bitmap.Save(@"c:\tmp\screens\tore3.tif");
                }

                //Bitmap bitmap2 = new Bitmap(bitmap.Width + 100, bitmap.Height);

                //BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                //byte[] bytes = new byte[bitmapData.Width*bitmapData.Height];
                //Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
                //bitmap.UnlockBits(bitmapData);

                //BitmapData bitmap2Data = bitmap2.LockBits(new Rectangle(50, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                //Marshal.Copy(bytes, 0, bitmap2Data.Scan0, bytes.Length);
                //bitmap2.UnlockBits(bitmap2Data);

                //bitmap2.Save(@"c:\tmp\screens\bigger.tif", ImageFormat.Tiff);

                //TiffSaver.Save(@"c:\tmp\screens\bigger.tif", bitmap2);

                using (Page page = engine.Process(bitmap))
                {
                    string str = page.GetText();
                    Console.WriteLine(str);
                }
            }
        }
    }
}
