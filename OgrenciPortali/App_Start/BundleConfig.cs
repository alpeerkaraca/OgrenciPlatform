using System.Web.Optimization;

namespace OgrenciPortali
{
    public class BundleConfig
    {
        // Paketleme hakkında daha fazla bilgi için lütfen https://go.microsoft.com/fwlink/?LinkId=301862 adresini ziyaret edin.
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Geliştirme yapmak ve öğrenme kaynağı olarak yararlanmak için Modernizr uygulamasının geliştirme sürümünü kullanın. Ardından
            // üretim için hazır. https://modernizr.com adresinde derleme aracını kullanarak yalnızca ihtiyacınız olan testleri seçin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/schedule").Include(
                "~/Scripts/util.js",
                "~/Scripts/main.js"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.min.js"));
            
            bundles.Add(new Bundle("~/bundles/datatables").Include(
                      "~/Scripts/datatables/datatables.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-lux.css",
                      "~/Content/css/style.css",
                      "~/Content/boxicons/boxicons.min.css",
                      "~/Content/boxicons/animations.min.css",
                      "~/Content/boxicons/transformations.min.css",
                      "~/Content/fontawesome/all.min.css",
                      "~/Content/datatables/datatables.min.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/schedule").Include(
                "~/Content/style.css"));
        }
    }
}
