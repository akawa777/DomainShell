using System.Web;
using System.Web.Optimization;

namespace DomainShell.Tests.Web
{
    public class BundleConfig
    {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=301862  を参照してください
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/Infrastructure/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Infrastructure/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/require").Include(
                        "~/Scripts/Infrastructure/require.js"));

            // 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備が
            // できたら、http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Infrastructure/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/Infrastructure/bootstrap.js",
                      "~/Scripts/Infrastructure/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Infrastructure/bootstrap.css",
                      "~/Content/Infrastructure/site.css"));
        }
    }
}
