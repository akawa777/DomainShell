using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;

namespace DomainShell.Tests.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _appStateId = DateTime.Now.ToString("yyyyMMddHHmmss");

            FileWatcher_Load();
        }

        private static string _appStateId;

        public static string AppStateId { get { return _appStateId;  } }
        private static FileSystemWatcher fsw = new FileSystemWatcher();
        private void FileWatcher_Load()
        {
            //監視対象のフォルダの設定
            fsw.Path = AppDomain.CurrentDomain.BaseDirectory;
            //監視する種類の設定
            fsw.NotifyFilter =
                (NotifyFilters.Attributes
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName);
            //サブディレクトリも監視
            fsw.IncludeSubdirectories = true;

            //すべてのファイルを監視しているならば            
            fsw.Filter = "";
            
            //イベント設定
            fsw.Created += fsw_Event;
            fsw.Changed += fsw_Event;
            fsw.Deleted += fsw_Event;
            fsw.Renamed += fsw_Event;
            //監視を開始
            fsw.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void fsw_Event(object sender, FileSystemEventArgs e)
        {
            _appStateId = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
