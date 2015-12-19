using System.Web.Optimization;

namespace Website
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			BundleTable.EnableOptimizations = false;
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/libs/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/countdown")
				.Include(
                    "~/Scripts/libs/countdown/jquery.plugin.js",
                    "~/Scripts/libs/countdown/jquery.countdown.js"));

			bundles.Add(new ScriptBundle("~/bundles/angular-nvd3")
				.Include(
                    "~/Scripts/libs/angular.js",
                    "~/Scripts/libs/angular-ng-grid.js",
                    "~/Scripts/libs/angular-resource.js",
                    "~/Scripts/libs/angular-route.js",
                    "~/Scripts/libs/angular-animate.js",
                    "~/Scripts/libs/ui-grid-stable.js",
                    "~/Scripts/libs/bindonce.js",
                    "~/Scripts/libs/ng-infinite-scroll.js",
                    "~/Scripts/libs/fsm-sticky-header.js",
                    "~/Scripts/libs/spin/spin.js",
                    "~/Scripts/libs/spin/jquery.spin.js",
                    "~/Scripts/libs/tooltip.js",
                    "~/Scripts/libs/popover.js",
                    "~/Scripts/jquery.signalR-2.2.0.js"));

			bundles.Add(new Bundle("~/bundles/app").IncludeDirectory(
			"~/Scripts/app",
			"*.js", true
			));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/libs/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/libs/bootstrap.js",
                      "~/Scripts/libs/respond.js"));

			bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
				"~/Content/bootstrap.css",
				"~/Content/bootstrap-responsive.css"
				));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/site.css"));
		}
	}
}
