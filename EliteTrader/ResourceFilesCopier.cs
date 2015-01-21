using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EliteTrader.EliteOcr.Data;

namespace EliteTrader
{
    public static class ResourceFilesCopier
    {
        public static string AppDataPath;

        public static void SetupFiles()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(appData))
            {
                throw new Exception(string.Format("Unable to find existing app data folder ({0})", appData));
            }
            AppDataPath = Path.Combine(appData, "EliteTrader");
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
            string tessDataPath = Path.Combine(AppDataPath, "tessdata");
            if (!Directory.Exists(tessDataPath))
            {
                Directory.CreateDirectory(tessDataPath);
            }

            string stationsFilePath = Path.Combine(AppDataPath, "stations.txt");
            string extraWordsFilePath = Path.Combine(AppDataPath, "extrawords.txt");
            string rareItemsFilePath = Path.Combine(AppDataPath, "rareitems.txt");
            string dictionaryFilePath = Path.Combine(tessDataPath, "edl.user-words");

            if (!File.Exists(stationsFilePath))
            {
                WriteEmbeddedResourceFile("EliteTrader.Data.stations.txt", stationsFilePath);
            }
            if (!File.Exists(extraWordsFilePath))
            {
                WriteEmbeddedResourceFile("EliteTrader.Data.extrawords.txt", extraWordsFilePath);
            }
            if (!File.Exists(rareItemsFilePath))
            {
                WriteEmbeddedResourceFile("EliteTrader.Data.rareitems.txt", rareItemsFilePath);
            }

            if (ShouldUpdateDictionary(stationsFilePath, extraWordsFilePath, rareItemsFilePath, dictionaryFilePath))
            {
                HashSet<string> words = new HashSet<string>();
                //AddWordsFromFile(systemsFilePath, words);
                AddWordsFromFile(stationsFilePath, words);
                AddWordsFromFile(extraWordsFilePath, words);
                AddWordsFromFile(rareItemsFilePath, words);
                AddWords(words, DefaultCommodityItems.ItemNames.Keys);

                using (FileStream fs = new FileStream(dictionaryFilePath, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.NewLine = "\n";
                    foreach (string word in words.OrderBy(a => a))
                    {
                        sw.WriteLine(word);
                    }
                }
            }
            
            string edcTrainedDataDestinationPath = Path.Combine(tessDataPath, "edc.traineddata");
            string edlTrainedDataDestinationPath = Path.Combine(tessDataPath, "edl.traineddata");
            string edlConfigDestinationPath = Path.Combine(tessDataPath, "edl.config");

            string sourceTessPath = Path.Combine(Environment.CurrentDirectory, "tessdata");
            string edcTrainedDataSourcePath = Path.Combine(sourceTessPath, "edc.traineddata");
            string edlTrainedDataSourcePath = Path.Combine(sourceTessPath, "edl.traineddata");
            string edlConfigSourcePath = Path.Combine(sourceTessPath, "edl.config");

            if (ShouldCopyTessFile(edcTrainedDataSourcePath, edcTrainedDataDestinationPath))
            {
                CopyFile(edcTrainedDataSourcePath, edcTrainedDataDestinationPath);
            }
            if (ShouldCopyTessFile(edlTrainedDataSourcePath, edlTrainedDataDestinationPath))
            {
                CopyFile(edlTrainedDataSourcePath, edlTrainedDataDestinationPath);
            }
            if (ShouldCopyTessFile(edlConfigSourcePath, edlConfigDestinationPath))
            {
                CopyFile(edlConfigSourcePath, edlConfigDestinationPath);
            }
        }

        private static bool ShouldCopyTessFile(string sourcePath, string destinationPath)
        {
            if (!File.Exists(destinationPath))
            {
                return true;
            }
            FileInfo sourceInfo = new FileInfo(sourcePath);
            FileInfo destinationInfo = new FileInfo(destinationPath);
            if (sourceInfo.CreationTimeUtc > destinationInfo.CreationTimeUtc)
            {
                return true;
            }
            return false;
        }

        private static bool ShouldUpdateDictionary(string stationsFilePath, string extraWordsFilePath, string rareItemsFilePath, string dictionaryFilePath)
        {
            if (!File.Exists(dictionaryFilePath))
            {
                return true;
            }

            FileInfo stationsFileInfo = new FileInfo(stationsFilePath);
            FileInfo extraWordsFileInfo = new FileInfo(extraWordsFilePath);
            FileInfo rareItemsFileInfo = new FileInfo(rareItemsFilePath);
            
            FileInfo dictionaryFileInfo = new FileInfo(dictionaryFilePath);

            if (stationsFileInfo.LastWriteTimeUtc > dictionaryFileInfo.LastWriteTimeUtc ||
                extraWordsFileInfo.LastWriteTimeUtc > dictionaryFileInfo.LastWriteTimeUtc ||
                rareItemsFileInfo.LastWriteTimeUtc > dictionaryFileInfo.LastWriteTimeUtc)
            {
                return true;
            }

            return false;
        }

        private static void AddWordsFromFile(string path, HashSet<string> words)
        {
            List<string> lines = new List<string>();
            using (StreamReader file = new StreamReader(path))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                   lines.Add(line.ToUpper().Trim());
                }
            }

            AddWords(words, lines);
        }

        private static void CopyFile(string sourcePath, string destinationPath)
        {
            using (FileStream input = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream output = new FileStream(destinationPath, FileMode.Create))
            {
                input.CopyTo(output);
            }
        }

        private static void WriteEmbeddedResourceFile(string resourceName, string destinationPath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
                
            using (Stream input = assembly.GetManifestResourceStream(resourceName))
            using (FileStream output = new FileStream(destinationPath, FileMode.Create))
            {
                input.CopyTo(output);
            }
        }

        private static void AddWords(HashSet<string> uniqueWords, IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                foreach (string word in line.Trim().Split(' '))
                {
                    string trimmedWord = word.Trim();
                    if (string.IsNullOrEmpty(trimmedWord))
                    {
                        continue;
                    }
                    int n;
                    if (int.TryParse(trimmedWord.Replace("-", ""), out n))
                    {
                        continue;
                    }
                    if (trimmedWord.Length < 2)
                    {
                        continue;
                    }
                    uniqueWords.Add(trimmedWord.ToUpper());
                }
            }
        }
    }
}
