﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SunokoLibrary.Application.Browsers
{
    public class LunascapeGeckoBrowserManager : ICookieImporterFactory
    {
        const string LUNASCAPE_PLUGIN_FOLDER5 = "%APPDATA%\\Lunascape\\Lunascape5\\ApplicationData\\gecko\\cookies.sqlite";
        const string LUNASCAPE_PLUGIN_FOLDER6 = "%APPDATA%\\Lunascape\\Lunascape6\\plugins";
        const string COOKIEPATH = "data\\cookies.sqlite";

        public ICookieImporter[] CreateCookieImporters()
        {
            var path = SearchCookieDirectory();
            var status = new BrowserConfig("Lunascape Gecko", "Default", path);
            return new[] { new GeckoCookieGetter(status) };
        }
        /// <summary>
        /// Lunascape6のプラグインフォルダからFirefoxのクッキーが保存されているパスを検索する
        /// </summary>
        /// <returns></returns>
        string SearchCookieDirectory()
        {
            var cookiePath = Utility.ReplacePathSymbols(LUNASCAPE_PLUGIN_FOLDER5);
            if (System.IO.File.Exists(cookiePath))
                return cookiePath;

            var pluginDir = Utility.ReplacePathSymbols(LUNASCAPE_PLUGIN_FOLDER6);
            cookiePath = null;
            if (Directory.Exists(pluginDir))
                cookiePath = Directory.EnumerateDirectories(pluginDir)
                    .Select(child => Path.Combine(child, COOKIEPATH))
                    .Where(child => File.Exists(child))
                    .FirstOrDefault();
            return cookiePath;
        }
    }
}