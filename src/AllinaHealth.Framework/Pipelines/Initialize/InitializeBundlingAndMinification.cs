using System.Web.Optimization;
using Sitecore.Pipelines;

namespace AllinaHealth.Framework.Pipelines.Initialize
{
    public class InitializeBundlingAndMinification
    {
        public void Process(PipelineArgs args)
        {
            //var cfg = (System.Web.Configuration.CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
            BundleTable.EnableOptimizations = false; // !cfg.Debug;
            RegisterBundles(BundleTable.Bundles);
        }

        private static void RegisterBundles(BundleCollection bundles)
        {
            // removed bootstrap.cs but need to keep the .js because we rely on the script for adding .active on aria-pressed
            bundles.Add(new ScriptBundle("~/assets/js/scripts").Include(
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/main.js"
            ));
        }
    }
}