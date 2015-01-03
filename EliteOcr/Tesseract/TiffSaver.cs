using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ImageMagick;

namespace EliteTrader.EliteOcr.Tesseract
{
    public static class TiffSaver
    {
        private static int _counter = 0;
        private const string _pattern = "edl.elite_font.exp{0}.tif";

        private static readonly Guid TiffClsid = Guid.Parse("557cf405-1a04-11d3-9a73-0000f81ef32e");

        private static Bitmap _multipageTiff;

        public static int GetCurrentCounter()
        {
            return _counter;
        }

        public static void Save(string path, Bitmap[] bitmaps)
        {
            if (bitmaps == null)
            {
                return;
            }
            foreach (Bitmap bitmap in bitmaps)
            {
                Save(path, bitmap);
            }
        }

        //public static void Save(string path, Bitmap bitmap)
        //{
        //    return;

        //    if (bitmap == null)
        //    {
        //        return;
        //    }

        //    ImageCodecInfo tiffEnc = ImageCodecInfo.GetImageEncoders().Single(a => a.Clsid == TiffClsid);
        //    EncoderParameters encoderParameters = new EncoderParameters(1);
        //    encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionNone);
        //    bitmap.Save(string.Format(Path.Combine(path, string.Format(_pattern, _counter++))), tiffEnc, encoderParameters);
        //}

        public static void SaveScaled(string path, Bitmap[] bitmaps, int percentage)
        {
            if (bitmaps == null)
            {
                return;
            }
            foreach (Bitmap bitmap in bitmaps)
            {
                SaveScaled(path, bitmap, percentage);
            }
        }

        public static void SaveScaled(string path, Bitmap bitmap, int percentage)
        {
            if (bitmap == null)
            {
                return;
            }
            using (MagickImage image = new MagickImage(bitmap))
            {
                image.Resize(percentage);
                image.Alpha(AlphaOption.Off);
                image.ChangeColorSpace(ColorSpace.GRAY);
                Bitmap newBitmap = ImageParser.GetBitmapFromImage(image);
                Save(path, newBitmap);
            }
        }

        public static void Save(string path, Bitmap bitmap)
        {
            //return;

            if (bitmap == null)
            {
                return;
            }

            ImageCodecInfo tiffEnc = ImageCodecInfo.GetImageEncoders().Single(a => a.Clsid == TiffClsid);

            if (_multipageTiff == null)
            {
                _multipageTiff = bitmap;
                EncoderParameters encoderParameters = new EncoderParameters(2);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                encoderParameters.Param[1] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionNone);
                _multipageTiff.Save(Path.Combine(path, string.Format(_pattern, 0)), tiffEnc, encoderParameters);
            }
            else
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                _multipageTiff.SaveAdd(bitmap, encoderParameters);
            }
        }

        public static void FlushTiff()
        {
            //return;

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
            _multipageTiff.SaveAdd(encoderParameters);
        }


    }
}