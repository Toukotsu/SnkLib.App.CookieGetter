﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SunokoLibrary.Application.Browsers
{
    public class BlinkBrowserManager : BrowserManagerBase
    {
        public BlinkBrowserManager(
            string name, string dataFolder, int primaryLevel = 2, string cookieFileName = "Cookies",
            string defaultFolder = "Default", string profileFolderStarts = "Profile")
            : base(new[] { ENGINE_ID })
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("引数nameをnullや空文字にする事は出来ません。");

            _name = name;
            _dataFolder = dataFolder != null ? Utility.ReplacePathSymbols(dataFolder) : null;
            _primaryLevel = primaryLevel;
            _cookieFileName = cookieFileName;
            _defaultFolderName = defaultFolder;
            _profileFolderStarts = profileFolderStarts;
        }

        internal const string ENGINE_ID = "Blink";
        int _primaryLevel;
        string _name;
        string _dataFolder;
        string _cookieFileName;
        string _defaultFolderName;
        string _profileFolderStarts;

        public override IEnumerable<ICookieImporter> GetCookieImporters()
        { return GetDefaultProfiles().Concat(GetProfiles()); }
        public override ICookieImporter GetCookieImporter(BrowserConfig config)
        { return new BlinkCookieGetter(config, 2); }
        /// <summary>
        /// ユーザのデフォルト環境設定を用いたICookieImporter生成。
        /// </summary>
        /// <param name="getterGenerator">configを任意のimporterに変換する</param>
        /// <returns>長さ1の列挙子</returns>
        IEnumerable<ICookieImporter> GetDefaultProfiles()
        {
            string path = null;
            if (_dataFolder != null)
                path = Path.Combine(_dataFolder, _defaultFolderName, _cookieFileName);
            var conf = new BrowserConfig(_name, _defaultFolderName, path, ENGINE_ID, false);
            return new ICookieImporter[] { new BlinkCookieGetter(conf, _primaryLevel) };
        }
        /// <summary>
        /// ブラウザが持っているデフォルト以外の全ての環境設定からICookieImporterを生成する。
        /// </summary>
        /// <param name="getterGenerator">configを任意のimporterに変換する</param>
        /// <returns></returns>
        IEnumerable<ICookieImporter> GetProfiles()
        {
            var paths = Enumerable.Empty<ICookieImporter>();
            if (Directory.Exists(_dataFolder))
            {
                paths = Directory.EnumerateDirectories(_dataFolder)
                    .Where(path => Path.GetFileName(path).StartsWith(_profileFolderStarts, StringComparison.OrdinalIgnoreCase))
                    .Select(path => Path.Combine(path, _cookieFileName))
                    .Where(path => File.Exists(path))
                    .Select(path => new BlinkCookieGetter(new BrowserConfig(
                        _name, Path.GetFileName(Path.GetDirectoryName(path)), path, ENGINE_ID, false), _primaryLevel));
                return paths;
            }
            return paths;
        }
    }
}