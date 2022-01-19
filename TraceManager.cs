using System;
using System.IO;

namespace LGHBAcsEngine
{
    /// <summary>
    /// 保存ファイルはLog_20090101.logになります。
    /// 저장파일은 Log_200099191.log가 됩니다.
    /// </summary>
    public static class TraceManager
    {
        static bool _useSaveLog = true;
        static public string webRoot = @"C:\ACS\WEB\LOG\ACS_LOG_";

        /// <summary>
        /// 로그파일 저장여부 플래그
        /// </summary>
        public static bool UseSaveLog
        {
            get => _useSaveLog;
            set => _useSaveLog = value;
        }

        static int _traceMaxDay = 30;

        /// <summary>
        /// 보존할 최대일수
        /// </summary>
        public static int TraceMaxDay
        {
            get => _traceMaxDay;
            set 
            { 
                _traceMaxDay = value;
                if (_traceMaxDay <= 0) _traceMaxDay = 1;
            }
        }

        static string _logFolder = "Trace";

        /// <summary>
        /// 保存するログフォルダ
        /// 저장할 로그 폴더
        /// </summary>
        public static string LogFolder
        {
            get => _logFolder;
            set => _logFolder = value;
        }
        
        private static object writerLock = new object();  

        public static void AddLog(string log)
        {
            Console.ForegroundColor = ConsoleColor.White;            
            Console.WriteLine(log);
            if (!UseSaveLog) return;

            try
            {
                //読み込むファイルの名前
                //가저올 파일 이름
                //string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolder);
                //string folder = Path.Combine(webRoot, LogFolder, DateTime.Now.ToString("yyyyMMdd"));
                string folder = Path.Combine(webRoot, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = folder + "\\" + "Trace_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                DateTime dtm = DateTime.Now;
                string formatDateTime = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} [{6:000}] ",
                    dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, dtm.Second, dtm.Millisecond);
                log = formatDateTime + log;

                lock (writerLock)
                {
                    var sw = File.AppendText(fileName);
                    sw.WriteLine(log);
                    sw.Close();
                }

                CheckRemoveFiles(folder);
            }
            catch (Exception ex)
            {
                AddLogEvent("ApplicationLogAddError", "Log Append error :" + log + ex.Message + ex.StackTrace,9999);
            }
        }

        public static void AddLog(string log, string strpref)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
            if (!UseSaveLog) return;

            try
            {
                //読み込むファイルの名前
                //가저올 파일 이름
                //string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolder);
                //string folder = Path.Combine(webRoot, LogFolder, DateTime.Now.ToString("yyyyMMdd"));
                string folder = Path.Combine(webRoot, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string fileName = folder + "\\" + strpref + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                DateTime dtm = DateTime.Now;
                string formatDateTime = string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} [{6:000}] ",
                    dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, dtm.Second, dtm.Millisecond);
                log = formatDateTime + log;

                var sw = File.AppendText(fileName);
                sw.WriteLine(log);
                sw.Close();

                CheckRemoveFiles(folder);
            }
            catch (Exception ex)
            {
                AddLogEvent("ApplicationLogAddError", "Log Append error :" + log + ex.Message + ex.StackTrace, 9999);
            }
        }

        static void CheckRemoveFiles(string folder)
        {
            int maxHour = TraceMaxDay * 24; //

            string[] files = Directory.GetFiles(folder);
            for (int i = 0; i < files.Length; i++)
            {
                DateTime tm = File.GetLastAccessTime(files[i]);
                TimeSpan span = DateTime.Now - tm;
                if (span.TotalHours >= maxHour)
                {
                    File.Delete(files[i]);
                }
            }
        }
        
        /// <summary>
        /// エベントログにログを書く
        /// 이벤트 로그에 로그를 쓰다
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        public static void AddLogEvent(string source, string message,int eventId)
        {
            //ソース
            //string sourceName = Application.ProductName;//
            //ソースが存在していない時は、作成する
            //if (!System.Diagnostics.EventLog.SourceExists(source))
            //{
            //    //ログ名を空白にすると、"Application"となる
            //    System.Diagnostics.EventLog.CreateEventSource(source, "");
            //}

            //テスト用にイベントログエントリに付加するデータを適当に作る
            //byte[] myData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            //イベントログにエントリを書き込む
            //ここではエントリの種類をエラー、イベントIDを1、分類を1000とする
            // "イベントログに書き込む文字列",

            //System.Diagnostics.EventLog.WriteEntry(source, message, System.Diagnostics.EventLogEntryType.Error, eventID, 1000);//, 1, 1000, myData);

            //次のようにイベントソースとメッセージのみを指定して書き込むと、
            //Information("情報")エントリとして書き込まれる。
            //System.Diagnostics.EventLog.WriteEntry(
            //    "MySource", "イベントログに書き込む文字列");
        }
    }
}
