using Sitecore.Data;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Framework.Pipelines.Mvc.GetRenderer
{
    public class GetWindsorControllerRenderer : GetRendererProcessor
    {
        private static readonly ID ControllerRenderingId = new ID("2A3E91A0-7987-44B5-AB34-35C2D9DE83B9");

        public override void Process(GetRendererArgs args)
        {
            if (args.Result != null)
            {
                return;
            }

            if (args.RenderingTemplate == null || !args.RenderingTemplate.DescendsFromOrEquals(ControllerRenderingId))
            {
                return;
            }

            args.Result = GetRender(args.Rendering, args);
        }

        // ReSharper disable once UnusedParameter.Local
        private static Renderer GetRender(Rendering rendering, GetRendererArgs args)
        {
            var controllerInfo = new ControllerInfo(
                rendering.RenderingItem.InnerItem["Controller"],
                rendering.RenderingItem.InnerItem["Controller Action"],
                rendering.RenderingItem.InnerItem["Area"]);

            return new AreaControllerRenderer(controllerInfo, new AreaControllerRunner(controllerInfo));
        }
    }
}