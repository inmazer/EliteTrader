using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using EliteTrader.EliteOcr.Interfaces;
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

        public static string MajorityVoteString(string[] results)
        {
            if (results.Length == 0)
            {
                return null;
            }
            if (results.Length < 3)
            {
                return results[0];
            }

            Dictionary<string, int> votes = new Dictionary<string, int>();
            for (int i = 0; i < results.Length; ++i)
            {
                string result = results[i];
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

        public static string[] Process(TesseractEngine engine, Bitmap bitmap, string whitelist, string identifier, int attempts)
        {
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
            SetVariable(engine, "tessedit_char_whitelist", whitelist);
            using (MagickImage image = new MagickImage(bitmap))
            {
                string firstResult = null;
                string[] results = new string[attempts];
                for (int i = 0; i < attempts; ++i)
                {
                    image.Alpha(AlphaOption.Off);
                    image.Format = MagickFormat.Tif;
                    image.ChangeColorSpace(ColorSpace.GRAY);
                    using (Bitmap newBitmap = image.ToBitmap())
                    using (Page page = engine.Process(newBitmap))
                    {
                        string str = page.GetText().Trim();
                        results[i] = str;

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
                return results;
            }
        }

        private static readonly List<string> StaticWords = new List<string>
        {
            "CR",
            "MED",
            "LOW",
            "HIGH",
            "STATION",
            "STARPORT",
            "CORIOLIS",
            "OCELLUS",
            "O'NEILL",
            "ORBIS",
            "OUTPOST",
            "CIVILIAN",
            "COMMERCIAL",
            "MILITARY",
            "MINING",
            "SCIENTIFIC",
            "UNSANCTIONED",
            "ALLIANCE",
            "EMPIRE",
            "FEDERATION",
            "INDEPENDENT",
            "POOR",
            "RICH",
            "ANARCHY",
            "COLONY",
            "COMMUNISM",
            "CONFEDERACY",
            "COOPERATIVE",
            "CORPORATE",
            "DEMOCRACY",
            "DICTATORSHIP",
            "FEUDAL",
            "IMPERIAL",
            "NONE",
            "PATRONAGE",
            "PRISON",
            "COLONY",
            "THEOCRACY",
            "AGRICULTURAL",
            "EXTRACTION",
            "TECH",
            "INDUSTRIAL",
            "MILITARY",
            "REFINERY",
            "SERVICE",
            "TERRAFORMING",
            "TOURISM",
            "LARGE",
            "POPULATION",
            "AGRICULTURE",
            "ECONOMY",
            //"JAN",
            //"FEB",
            //"MAR",
            //"APR",
            //"MAY",
            //"JUN",
            //"JUL",
            //"AUG",
            //"SEP",
            //"OCT",
            //"NOV",
            //"DEC",
        };

        public static void UpdateDictionary(string dictionaryPath, ICommodityNameRepository commodityNameRepository)
        {
            List<string> commodityNames = commodityNameRepository.GetAllCommodityNames();

            using (FileStream fs = new FileStream(dictionaryPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
            {
                sw.NewLine = "\n";
                StaticWords.ForEach(a => sw.WriteLine(a));

                foreach (string commodityName in commodityNames)
                {
                    foreach (string word in commodityName.Trim().Split(' '))
                    {
                        string trimmedWord = word.Trim();
                        if (string.IsNullOrEmpty(trimmedWord))
                        {
                            continue;
                        }
                        int n;
                        if (int.TryParse(trimmedWord, out n))
                        {
                            continue;
                        }
                        sw.WriteLine(trimmedWord);
                    }
                }
            }
        }
    }
}
