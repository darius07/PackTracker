﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;

namespace PackTracker.Update {
  class Updater {

    public bool? NewVersionAvailable(out Release LatestRelease) {
      LatestRelease = null as Release;
      try {
        LatestRelease = GetLatestRelease();
      }
      catch(WebException) {
        return null;
      }

      Version LatestVersion = new Version(Regex.Match(LatestRelease.tag_name, @"\d+(\.\d+)*").ToString());

      return Plugin.CurrentVersion.CompareTo(LatestVersion) < 0;

    }

    public bool Update() {
      Release LatestRelease = GetLatestRelease();
      Asset Asset = LatestRelease.assets.Single(x => x.name == "PackTracker.zip");

      try {
        using(WebClient client = new WebClient()) {
          using(Stream download = client.OpenRead(Asset.browser_download_url)) {
            string path = Path.Combine(Config.AppDataPath, "Plugins");
            string tempPath = Path.Combine(Path.GetTempPath(), "PackTracker");

            if(Directory.Exists(tempPath)) {
              Directory.Delete(tempPath, true);
            }

            ZipArchive Zipper = new ZipArchive(download);
            Zipper.ExtractToDirectory(tempPath);

            foreach(string file in Directory.GetFiles(tempPath)) {
              string target = Path.Combine(path, Path.GetFileName(file));
              File.Copy(file, target, true);
              File.SetLastWriteTime(target, DateTime.Now);
            }

            Directory.Delete(tempPath, true);

            return true;
          }
        }
      }
      catch(WebException) {
        return false;
      }
    }

    public Release GetLatestRelease() {
      HttpWebRequest request = WebRequest.CreateHttp(@"https://api.github.com/repos/mgk82/PackTracker/releases/latest");
      request.Proxy = null;
      request.UserAgent = "PackTracker";

      Release Release = new Release();
      using(Stream response = request.GetResponse().GetResponseStream()) {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(Release.GetType());
        Release = (Release)ser.ReadObject(response);
      }

      return Release;
    }
  }
}
