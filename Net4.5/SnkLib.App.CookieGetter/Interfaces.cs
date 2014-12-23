﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SunokoLibrary.Application
{
    /// <summary>
    /// ブラウザからのCookieを取得する機能を定義します。
    /// </summary>
    public interface ICookieImporter
    {
        /// <summary>
        /// Cookieを取得するブラウザに関する情報を取得します。
        /// </summary>
        BrowserConfig Config { get; }
        /// <summary>
        /// Cookie保存の形態を取得します。
        /// </summary>
        PathType CookiePathType { get; }
        /// <summary>
        /// 利用可能かどうかを取得します。
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// 並べ替え時に用いられる数値を取得します。OSブラウザ: 0、有名ブラウザ: 1、派生ブラウザ: 2。
        /// </summary>
        int PrimaryLevel { get; }
        /// <summary>
        /// 指定されたURLとの通信に使えるCookieを返します。
        /// </summary>
        /// <param name="targetUrl">通信先のURL</param>
        /// <param name="container">取得Cookieを入れる対象</param>
        /// <returns>処理の成功不成功</returns>
        Task<ImportResult> GetCookiesAsync(Uri targetUrl, CookieContainer container);
        /// <summary>
        /// 自身と設定の異なるICookieImporterを生成します。
        /// </summary>
        ICookieImporter Generate(BrowserConfig config);
    }
    /// <summary>
    /// パス指定対象の種類を定義します。
    /// </summary>
    public enum PathType
    {
        /// <summary>ファイル</summary>
        File,
        /// <summary>フォルダ</summary>
        Directory,
    }
    /// <summary>
    /// Cookie取得の実行結果を定義します。
    /// </summary>
    public enum ImportResult
    {
        /// <summary>処理が正常終了状態にあります。</summary>
        Success,
        /// <summary>処理出来る状態下にありませんでした。</summary>
        Unavailable,
        /// <summary>データの参照に失敗。処理は中断されています。</summary>
        AccessError,
        /// <summary>データの解析に失敗。処理は中断されています。</summary>
        ConvertError,
        /// <summary>処理に失敗。想定されていないエラーが発生しています。</summary>
        UnknownError,
    }
    
    /// <summary>
    /// ブラウザに対して行える操作を定義します。
    /// </summary>
    public interface ICookieImporterFactory
    {
        /// <summary>
        /// 利用可能なすべてのICookieImporterを取得します。
        /// </summary>
        IEnumerable<ICookieImporter> GetCookieImporters();
    }
    /// <summary>
    /// BrowserConfigからICookieImporterを生成する操作を定義します。
    /// </summary>
    public interface ICookieImporterGenerator
    {
        /// <summary>
        /// 対応しているブラウザエンジンの識別子の配列を取得します。
        /// </summary>
        string[] EngineIds { get; }
        /// <summary>
        /// 指定されたブラウザ構成情報からICookieImporterを取得します。
        /// </summary>
        /// <param name="config">元となるブラウザ構成情報。</param>
        /// <returns>引数で指定されたブラウザを参照するインスタンス。</returns>
        ICookieImporter GetCookieImporter(BrowserConfig config);
    }
    /// <summary>
    /// 使用可能なICookieImporterの管理を行う機能を定義します。
    /// </summary>
    public interface ICookieImporterManager
    {
        /// <summary>
        /// 使用できるICookieImporterのリストを取得します。
        /// </summary>
        /// <param name="availableOnly">利用可能なものに絞る</param>
        Task<ICookieImporter[]> GetInstancesAsync(bool availableOnly);
        /// <summary>
        /// 設定値を指定したICookieImporterを取得します。アプリ終了時に直前まで使用していた
        /// ICookieImporterのConfigを設定として保存すれば、起動時にConfigをこのメソッドに
        /// 渡す事で適切なICookieImporterを再生成してくれる。
        /// </summary>
        /// <param name="targetConfig">再取得対象のブラウザの構成情報</param>
        /// <param name="allowDefault">取得不可の場合に既定のCookieImporterを返すかを指定できます。</param>
        Task<ICookieImporter> GetInstanceAsync(BrowserConfig targetConfig, bool allowDefault);
    }

    /// <summary>
    /// Cookie取得に関する例外。
    /// </summary>
    [Serializable]
    public class CookieImportException : Exception
    {
        /// <summary>例外を生成します。</summary>
        /// <param name="message">エラーの捕捉</param>
        /// <param name="result">エラーの種類</param>
        public CookieImportException(string message, ImportResult result)
            : base(message) { Result = result; }
        /// <summary>例外を再スローさせるための例外を生成します。</summary>
        /// <param name="message">エラーの捕捉</param>
        /// <param name="result">エラーの種類</param>
        /// <param name="inner">内部例外</param>
        public CookieImportException(string message, ImportResult result, Exception inner)
            : base(message, inner) { Result = result; }

        /// <summary>
        /// 例外要因の大まかな種類
        /// </summary>
        public ImportResult Result { get; private set; }
    }
}