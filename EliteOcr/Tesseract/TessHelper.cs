using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageMagick;
using Tesseract;

namespace EliteTrader.EliteOcr.Tesseract
{
    public static class TessHelper
    {
        public static void SetVariable(TesseractEngine engine, string variable, string value)
        {
            if (!engine.SetVariable(variable, value))
            {
                throw new Exception(string.Format("Failed to set tess variable {0} to value {1}", variable, value));
            }
        }

        public static void SetVariable(TesseractEngine engine, string variable, int value)
        {
            if (!engine.SetVariable(variable, value))
            {
                throw new Exception(string.Format("Failed to set tess variable {0} to value {1}", variable, value));
            }
        }

        public static void SetVariable(TesseractEngine engine, string variable, bool value)
        {
            if (!engine.SetVariable(variable, value))
            {
                throw new Exception(string.Format("Failed to set tess variable {0} to value {1}", variable, value));
            }
        }

        public static void SetVariable(TesseractEngine engine, string variable, long value)
        {
            if (!engine.SetVariable(variable, value))
            {
                throw new Exception(string.Format("Failed to set tess variable {0} to value {1}", variable, value));
            }
        }

        public static string MajorityVoteString(List<string> results)
        {
            if (results.Count == 0)
            {
                return null;
            }
            if (results.Count < 3)
            {
                return results[0];
            }

            Dictionary<string, int> votes = new Dictionary<string, int>();
            foreach (string result in results)
            {
                int value;
                if (votes.TryGetValue(result, out value))
                {
                    votes[result] = ++value;
                }
                else
                {
                    votes.Add(result, 1);
                }
            }

            return votes.OrderByDescending(a => a.Value).First().Key;
        }

        public static List<string> Process(TesseractEngine engine, Bitmap bitmap, string whitelist, string identifier, int samples, bool mustHaveValue)
        {
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
            SetVariable(engine, "tessedit_char_whitelist", whitelist);
            List<string> results = new List<string>();
            using (MagickImage image = new MagickImage(bitmap))
            {
                string firstResult = null;
                for (int i = 0; i < samples; ++i)
                {
                    image.Alpha(AlphaOption.Off);
                    image.Format = MagickFormat.Tif;
                    image.ChangeColorSpace(ColorSpace.GRAY);
                    using (Bitmap newBitmap = image.ToBitmap())
                    using (Page page = engine.Process(newBitmap))
                    {
                        string str = page.GetText().Trim();
                        if (string.IsNullOrEmpty(str))
                        {
                            image.Resize(150);
                            continue;
                        }
                        results.Add(str);

                        if (i == 0)
                        {
                            firstResult = str;
                        }
                        else if (i == 1)
                        {
                            if (firstResult == str)
                            {
                                return results;
                            }
                        }
                    }

                    image.Resize(150);
                }
            }
            if (mustHaveValue && results.Count == 0)
            {
                //Keep going until we get something or a ridiculous number of attempt is reached
                using (MagickImage image = new MagickImage(bitmap))
                {
                    for (int i = 0; i < 50; ++i)
                    {
                        image.Alpha(AlphaOption.Off);
                        image.Format = MagickFormat.Tif;
                        image.ChangeColorSpace(ColorSpace.GRAY);
                        using (Bitmap newBitmap = image.ToBitmap())
                        using (Page page = engine.Process(newBitmap))
                        {
                            string str = page.GetText().Trim();
                            if (string.IsNullOrEmpty(str))
                            {
                                image.Resize(105);
                                continue;
                            }
                            results.Add(str);

                            return results;
                        }
                    }
                }
                throw new Exception(string.Format("Got no valid results when doing ocr on ({0})", identifier));
            }
            return results;
        }
    }
}
