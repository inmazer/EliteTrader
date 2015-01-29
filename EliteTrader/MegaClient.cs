using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CG.Web.MegaApiClient;

namespace EliteTrader
{
    public static class MegaClient
    {
        public static void Upload(EliteTraderSettings settings, List<string> filePaths, Exception e, string version)
        {
            if (filePaths.Count == 0)
            {
                return;
            }

            const string folderName = "FailedScreenshots";

            MegaApiClient client = new MegaApiClient();

            MegaApiClient.AuthInfos auth = new MegaApiClient.AuthInfos(PasswordEncrypter.Decrypt("G3Kqan9mMhIM0C0Rj4AMomc1qEXke9PyhntMBB5AkUo="), "oqIBFpHBvyY", Convert.FromBase64String("x3tKeHpoSwpgmlc/KHBqYA=="));

            client.Login(auth);
            List<Node> nodes = client.GetNodes().ToList();

            Node root = nodes.Single(a => a.Type == NodeType.Root);
            Node screenshotsFolder = nodes.SingleOrDefault(n => string.Equals(n.Name, folderName, StringComparison.InvariantCultureIgnoreCase) && n.ParentId == root.Id);

            if (screenshotsFolder == null)
            {
                screenshotsFolder = client.CreateFolder(folderName, root);
                nodes.Add(screenshotsFolder);
            }

            Node versionFolder = nodes.SingleOrDefault(n => string.Equals(n.Name, version, StringComparison.InvariantCultureIgnoreCase) && n.ParentId == screenshotsFolder.Id);
            if (versionFolder == null)
            {
                versionFolder = client.CreateFolder(version, screenshotsFolder);
            }


            string guid = Guid.NewGuid().ToString();

            string settingsInfoFilename = string.Format("{0}.txt", guid);
            using (MemoryStream mes = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(mes))
            {
                sw.WriteLine("Version: {0}", version);
                sw.WriteLine(settings.Username);
                sw.WriteLine("Number of screenshots: {0}", filePaths.Count);
                sw.WriteLine();
                if (e != null)
                {
                    sw.WriteLine(e.ToString());
                }

                sw.Flush();
                mes.Position = 0;
                client.Upload(mes, settingsInfoFilename, versionFolder);
            }


            for (int i = 0; i < filePaths.Count; ++i)
            {
                string megaFilename = string.Format("{0}_{1}.png", guid, i);

                using (Bitmap bitmap = new Bitmap(filePaths[i]))
                using(MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    client.Upload(ms, megaFilename, versionFolder);
                }
            }

            client.Logout();
        }
    }
}
