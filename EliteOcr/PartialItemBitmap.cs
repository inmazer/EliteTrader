using System;
using System.Drawing;

namespace EliteTrader.EliteOcr
{
    public class PartialItemBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Bitmap BitmapOriginal { get; private set; }
        public Bitmap BitmapNoBackground { get; private set; }
        public Bitmap[] BitmapChars { get; private set; }

        public PartialItemBitmap(Bitmap bitmap, Bitmap bitmapOriginal, Bitmap bitmapNoBackground, Bitmap[] bitmapChars)
        {
            Bitmap = bitmap;
            BitmapOriginal = bitmapOriginal;
            BitmapNoBackground = bitmapNoBackground;
            BitmapChars = bitmapChars;
        }

        public void Save(string path)
        {
            if (Bitmap != null)
            {
                Bitmap.Save(path);
            }
        }

        public void Dispose()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }
            if (BitmapOriginal != null)
            {
                BitmapOriginal.Dispose();
                BitmapOriginal = null;
            }
            if (BitmapNoBackground != null)
            {
                BitmapNoBackground.Dispose();
                BitmapNoBackground = null;
            }
            if (BitmapChars != null)
            {
                for (int i = 0; i < BitmapChars.Length; ++i)
                {
                    BitmapChars[i].Dispose();
                }
                BitmapChars = null;
            }
        }
    }
}